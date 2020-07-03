using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para MyEvents.xaml
    /// </summary>
    public partial class MyEvents : Window
    {
        #region Constants
        private const int hourIndex = 0;
        private const int minuteIndex = 1;
        private const string timeWarning = "Pon una hora de término mayor a la de inicio";
        private const string userWarning = "Uno de los usuarios está ocupado a esa hora";
        private const string eventsFile = "Events.txt";
        #endregion

        #region Fields
        private AppointmentsList calendar;
        private AppointmentsList myEvents = new AppointmentsList();
        private User user;
        private Appointment selectedEvent;
        private UsersList allUsers;
        #endregion

        #region Methods
        public MyEvents(User passedUser)
        {
            InitializeComponent();
            ButtonDelete.IsEnabled = false;
            calendar = Utils.ReadEventsSerialFile();
            allUsers = Utils.ReadUsersSerialFile();
            user = passedUser;
            FilterCalendar();
            AddEventsToListBox();
            listBoxAllUsers.ItemsSource = allUsers.Users;
        }

        private void FilterCalendar()
        {
            myEvents.Appointments.Clear();
            foreach(Appointment appointment in calendar.Appointments)
            {
                if (appointment.Owner.HasSameNameAs(user.Name))
                {
                    myEvents.AddAppointment(appointment);
                }
            }
        }

        private void AddEventsToListBox()
        {
            listBoxEvents.ItemsSource = myEvents.Appointments;
        }

        private void ListBoxEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = listBoxEvents.SelectedIndex;
            selectedEvent = listBoxEvents.Items[selectedIndex] as Appointment;
            ButtonDelete.IsEnabled = true;
            SetEventValues();
        }
        private void SetEventValues()
        {
            TextBoxName.Text = selectedEvent.Name;
            TextBoxDescription.Text = selectedEvent.Description;
            DatePickerEventDate.SelectedDate = selectedEvent.Date;
            ComboBoxStartTimeHour.SelectedItem = ComboBoxStartTimeHour.Items[Int32.Parse(selectedEvent.GetStart()[hourIndex], NumberFormatInfo.InvariantInfo)];
            ComboBoxStartTimeMinute.SelectedItem = ComboBoxStartTimeMinute.Items[Int32.Parse(selectedEvent.GetStart()[minuteIndex], NumberFormatInfo.InvariantInfo)];
            ComboBoxFinishTimeHour.SelectedItem = ComboBoxFinishTimeHour.Items[Int32.Parse(selectedEvent.GetEnd()[hourIndex], NumberFormatInfo.InvariantInfo)];
            ComboBoxFinishTimeMinute.SelectedItem = ComboBoxFinishTimeMinute.Items[Int32.Parse(selectedEvent.GetEnd()[minuteIndex], NumberFormatInfo.InvariantInfo)];
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Appointment eventToRemove = calendar.Appointments.Single(appointment => appointment.Equals(selectedEvent));
            calendar.RemoveAppointment(eventToRemove);
            CloseWindow();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Appointment eventToEdit = calendar.Appointments.Single(appointment => appointment.Equals(selectedEvent));
            string startHour = ComboBoxStartTimeHour.Text.ToString();
            string startMinute = ComboBoxStartTimeMinute.Text.ToString();
            string endHour = ComboBoxFinishTimeHour.Text;
            string endMinute = ComboBoxFinishTimeMinute.Text;
            string[] start = { startHour, startMinute };
            string[] end = { endHour, endMinute };
            if (eventToEdit.Participants != null)
            {
                eventToEdit.Participants.ClearUsers();
            }
            foreach (User item in listBoxAllUsers.SelectedItems)
            {
                eventToEdit.Participants.AddUser(item);
            }
            DateTime date = DatePickerEventDate.SelectedDate.Value.Date;
            bool isNotValid = int.Parse(endHour,
                NumberFormatInfo.InvariantInfo) < int.Parse(startHour, NumberFormatInfo.InvariantInfo) ||
                (int.Parse(startHour, NumberFormatInfo.InvariantInfo) == int.Parse(endHour, NumberFormatInfo.InvariantInfo)
                && int.Parse(endMinute, NumberFormatInfo.InvariantInfo) <= int.Parse(startMinute, NumberFormatInfo.InvariantInfo));
            if (isNotValid)
            {
                MessageBox.Show(timeWarning);
            }
            else if (eventToEdit.Participants.AreAvailable(date, start, end, calendar) == false)
            {
                MessageBox.Show(userWarning);
            }
            else
            {
                eventToEdit.Edit(TextBoxName.Text, TextBoxDescription.Text, date, start, end);
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            if (Utils.WriteEventsSerialFile(calendar, eventsFile))
            {
                this.Close();
            }
        }
        #endregion
    }
}
