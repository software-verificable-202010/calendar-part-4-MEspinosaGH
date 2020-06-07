using System;
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
        #endregion

        #region Fields
        private Utils util = new Utils();
        private EventsList calendar;
        private EventsList myEvents = new EventsList();
        private User user;
        private Event selectedEvent;
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
            ComboBoxStartTimeHour.SelectedItem = ComboBoxStartTimeHour.Items[Int32.Parse(selectedEvent.Start[hourIndex])];
            ComboBoxStartTimeMinute.SelectedItem = ComboBoxStartTimeMinute.Items[Int32.Parse(selectedEvent.Start[minuteIndex])];
            ComboBoxFinishTimeHour.SelectedItem = ComboBoxFinishTimeHour.Items[Int32.Parse(selectedEvent.End[hourIndex])];
            ComboBoxFinishTimeMinute.SelectedItem = ComboBoxFinishTimeMinute.Items[Int32.Parse(selectedEvent.End[minuteIndex])];
        }

        private void CloseWindow()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Events.txt", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, calendar);
            stream.Close();
            this.Close();
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
            eventToEdit.Name = TextBoxName.Text;
            eventToEdit.Description = TextBoxDescription.Text;
            eventToEdit.Date = DatePickerEventDate.SelectedDate.Value.Date;
            string startHour = ComboBoxStartTimeHour.Text.ToString();
            string startMinute = ComboBoxStartTimeMinute.Text.ToString();
            string endHour = ComboBoxFinishTimeHour.Text;
            string endMinute = ComboBoxFinishTimeMinute.Text;
            string[] start = { startHour, startMinute };
            string[] end = { endHour, endMinute };
            eventToEdit.Start = start;
            eventToEdit.End = end;
            CloseWindow();
        }

        public MyEvents(User passedUser)
        {
            InitializeComponent();
            ButtonDelete.IsEnabled = false;
            calendar = util.ReadEventsSerialFile();
            user = passedUser;
            FilterCalendar();
            AddEventsToListBox();
        }
        #endregion
    }
}
