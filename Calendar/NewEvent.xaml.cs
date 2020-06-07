using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    /// Lógica de interacción para NewEvent.xaml
    /// </summary>
    public partial class NewEvent : Window
    {
        #region Constants
        private const string timeWarning = "Pon una hora de término mayor a la de inicio";
        private const string userWarning = "Uno de los usuarios está ocupado a esa hora";
        private const int minutesInAnHour = 60;
        private const int hourIndex = 0;
        private const int minuteIndex = 0;
        #endregion

        #region Fields
        private EventsList calendar = new EventsList();
        private User user;
        private UsersList allUsers;
        private UsersList selectedUsers = new UsersList();
        #endregion

        #region Methods
        private void SetListBoxItems()
        {
            listBoxAllUsers.ItemsSource = allUsers.Users;
        }

        private bool DatesCollide(Event oldEvent, DateTime newEventDate, string[] newEventStart, string[] newEventEnd)
        {
            bool areDifferentDates = oldEvent.Date != newEventDate;
            int oldEventStartTime = Int32.Parse(oldEvent.Start[hourIndex]) + Int32.Parse(oldEvent.Start[minuteIndex]) * minutesInAnHour;
            int oldEventEndTIme = Int32.Parse(oldEvent.End[hourIndex]) + Int32.Parse(oldEvent.End[minuteIndex]) * minutesInAnHour;
            int newEventStartTime = Int32.Parse(newEventStart[hourIndex]) + Int32.Parse(newEventStart[minuteIndex]) * minutesInAnHour;
            int newEventEndTime = Int32.Parse(newEventEnd[hourIndex]) + Int32.Parse(newEventEnd[minuteIndex]) * minutesInAnHour;
            bool areAtDifferentHours = oldEventStartTime >= newEventEndTime || newEventStartTime >= oldEventEndTIme;
            if (areDifferentDates || areAtDifferentHours)
            {
                return false;
            }
            return true;
        }

        private bool UsersAreAvailable(DateTime date, string[] start, string[] end)
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

        private void saveSelectedUsers()
        {
            foreach (User item in listBoxAllUsers.SelectedItems)
            {
                selectedUsers.Users.Add(item);
            }
        }

        private void CancelBtn_Click(Object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(Object sender, EventArgs e)
        {
            saveSelectedUsers();
            string name = TextBoxName.Text;
            string description = TextBoxDescription.Text;
            DateTime date = DatePickerEventDate.SelectedDate.Value.Date;
            string startHour = ComboBoxStartTimeHour.Text.ToString();
            string startMinute = ComboBoxStartTimeMinute.Text.ToString();
            string endHour = ComboBoxFinishTimeHour.Text;
            string endMinute = ComboBoxFinishTimeMinute.Text;
            string[] start = { startHour, startMinute };
            string[] end = { endHour, endMinute };

            bool isNotValid = int.Parse(endHour) < int.Parse(startHour) || (int.Parse(startHour) == int.Parse(endHour) && int.Parse(endMinute) <= int.Parse(startMinute));
            if (isNotValid)
            {
                TextBlockWarning.Text = timeWarning;
                TextBlockWarning.Visibility = Visibility.Visible;
            }
            else if (UsersAreAvailable(date, start, end) == false)
            {
                TextBlockWarning.Text = userWarning;
                TextBlockWarning.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockWarning.Visibility = Visibility.Collapsed;
                Event newEvent = new Event(name, description, date, start, end, user, selectedUsers);
                calendar.Events.Add(newEvent);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("Events.txt", FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, calendar);
                stream.Close();
                this.Close();
            }
        }

        public NewEvent(User passedUser)
        {
            Utils util = new Utils();
            calendar = util.ReadEventsSerialFile();
            allUsers = util.ReadUsersSerialFile();
            user = passedUser;

            InitializeComponent();
            SetListBoxItems();
            DatePickerEventDate.SelectedDate = DateTime.Today;

            ButtonCancel.Click += new RoutedEventHandler(CancelBtn_Click);
            ButtonSave.Click += new RoutedEventHandler(SaveBtn_Click);
        }
        #endregion

    }
}
