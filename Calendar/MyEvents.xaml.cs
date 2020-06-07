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
        private const int minutesInAnHour = 60;
        #endregion

        #region Fields
        private Utils util = new Utils();
        private EventsList calendar;
        private EventsList myEvents = new EventsList();
        private User user;
        private Event selectedEvent;
        private UsersList allUsers;
        #endregion

        #region Methods
        private void FilterCalendar()
        {
            myEvents.Events.Clear();
            foreach(Event appointment in calendar.Events)
            {
                if (appointment.Owner.Name == user.Name)
                {
                    myEvents.Events.Add(appointment);
                }
            }
        }

        private void AddEventsToListBox()
        {
            listBoxEvents.ItemsSource = myEvents.Events;
        }

        private void SetEventValues()
        {
            TextBoxName.Text = selectedEvent.Name;
            TextBoxDescription.Text = selectedEvent.Description;
            DatePickerEventDate.SelectedDate = selectedEvent.Date;
            ComboBoxStartTimeHour.SelectedItem = ComboBoxStartTimeHour.Items[Int32.Parse(selectedEvent.Start[hourIndex], NumberFormatInfo.InvariantInfo)];
            ComboBoxStartTimeMinute.SelectedItem = ComboBoxStartTimeMinute.Items[Int32.Parse(selectedEvent.Start[minuteIndex], NumberFormatInfo.InvariantInfo)];
            ComboBoxFinishTimeHour.SelectedItem = ComboBoxFinishTimeHour.Items[Int32.Parse(selectedEvent.End[hourIndex], NumberFormatInfo.InvariantInfo)];
            ComboBoxFinishTimeMinute.SelectedItem = ComboBoxFinishTimeMinute.Items[Int32.Parse(selectedEvent.End[minuteIndex], NumberFormatInfo.InvariantInfo)];
        }

        private void CloseWindow()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Events.txt", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, calendar);
            stream.Close();
            this.Close();
        }

        private bool DatesCollide(Event oldEvent, DateTime newEventDate, string[] newEventStart, string[] newEventEnd)
        {
            bool areDifferentDates = oldEvent.Date != newEventDate;
            int oldEventStartTime = Int32.Parse(oldEvent.Start[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(oldEvent.Start[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            int oldEventEndTIme = Int32.Parse(oldEvent.End[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(oldEvent.End[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            int newEventStartTime = Int32.Parse(newEventStart[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(newEventStart[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            int newEventEndTime = Int32.Parse(newEventEnd[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(newEventEnd[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            bool areAtDifferentHours = oldEventStartTime >= newEventEndTime || newEventStartTime >= oldEventEndTIme;
            if (areDifferentDates || areAtDifferentHours)
            {
                return false;
            }
            return true;
        }

        private bool UsersAreAvailable(DateTime date, string[] start, string[] end, UsersList selectedUsers)
        {
            if (listBoxAllUsers.SelectedItems == null)
            {
                return true;
            }
            foreach (Event appointment in calendar.Events)
            {
                if (DatesCollide(appointment, date, start, end))
                {
                    foreach (User selectedUser in selectedUsers.Users)
                    {
                        if (appointment.Owner.Equals(selectedUser))
                        {
                            return false;
                        }
                        if (appointment.Participants != null)
                        {
                            foreach (User participant in appointment.Participants.Users)
                            {
                                if (participant.Name == selectedUser.Name)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void listBoxEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = listBoxEvents.SelectedIndex;
            selectedEvent = listBoxEvents.Items[selectedIndex] as Event;
            ButtonDelete.IsEnabled = true;
            SetEventValues();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Event eventToRemove = calendar.Events.Single(appointment => appointment.Equals(selectedEvent));
            calendar.Events.Remove(eventToRemove);
            CloseWindow();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Event eventToEdit = calendar.Events.Single(appointment => appointment.Equals(selectedEvent));
            string startHour = ComboBoxStartTimeHour.Text.ToString();
            string startMinute = ComboBoxStartTimeMinute.Text.ToString();
            string endHour = ComboBoxFinishTimeHour.Text;
            string endMinute = ComboBoxFinishTimeMinute.Text;
            string[] start = { startHour, startMinute };
            string[] end = { endHour, endMinute };
            if (eventToEdit.Participants != null)
            {
                eventToEdit.Participants.Users.Clear();
            }
            foreach (User item in listBoxAllUsers.SelectedItems)
            {
                eventToEdit.Participants.Users.Add(item);
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
            else if (UsersAreAvailable(date, start, end, eventToEdit.Participants) == false)
            {
                MessageBox.Show(userWarning);
            }
            else
            {
                eventToEdit.Name = TextBoxName.Text;
                eventToEdit.Description = TextBoxDescription.Text;
                eventToEdit.Date = date;
                eventToEdit.Start = start;
                eventToEdit.End = end;
                CloseWindow();
            }
        }

        public MyEvents(User passedUser)
        {
            InitializeComponent();
            ButtonDelete.IsEnabled = false;
            calendar = util.ReadEventsSerialFile();
            allUsers = util.ReadUsersSerialFile();
            user = passedUser;
            FilterCalendar();
            AddEventsToListBox();
            listBoxAllUsers.ItemsSource = allUsers.Users;
        }
        #endregion
    }
}
