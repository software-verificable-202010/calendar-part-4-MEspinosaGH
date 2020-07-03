using System;
using System.Globalization;
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
        private const string eventsFile = "Events.txt";
        #endregion

        #region Fields
        private AppointmentsList calendar = new AppointmentsList();
        private User user;
        private UsersList allUsers;
        private UsersList selectedUsers = new UsersList();
        #endregion

        #region Methods
        public NewEvent(User passedUser)
        {
            calendar = Utils.ReadEventsSerialFile();
            allUsers = Utils.ReadUsersSerialFile();
            user = passedUser;

            InitializeComponent();
            SetListBoxItems();
            DatePickerEventDate.SelectedDate = DateTime.Today;

            ButtonCancel.Click += new RoutedEventHandler(CancelBtn_Click);
            ButtonSave.Click += new RoutedEventHandler(SaveBtn_Click);
        }

        private void SetListBoxItems()
        {
            listBoxAllUsers.ItemsSource = allUsers.Users;
        }

        private void CancelBtn_Click(Object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(Object sender, EventArgs e)
        {
            SaveSelectedUsers();
            string name = TextBoxName.Text;
            string description = TextBoxDescription.Text;
            DateTime date = DatePickerEventDate.SelectedDate.Value.Date;
            string startHour = ComboBoxStartTimeHour.Text.ToString();
            string startMinute = ComboBoxStartTimeMinute.Text.ToString();
            string endHour = ComboBoxFinishTimeHour.Text;
            string endMinute = ComboBoxFinishTimeMinute.Text;
            string[] start = { startHour, startMinute };
            string[] end = { endHour, endMinute };

            bool isNotValid = int.Parse(endHour,
                NumberFormatInfo.InvariantInfo) < int.Parse(startHour, NumberFormatInfo.InvariantInfo) || 
                (int.Parse(startHour, NumberFormatInfo.InvariantInfo) == int.Parse(endHour, NumberFormatInfo.InvariantInfo) 
                && int.Parse(endMinute, NumberFormatInfo.InvariantInfo) <= int.Parse(startMinute, NumberFormatInfo.InvariantInfo));
            if (isNotValid)
            {
                MessageBox.Show(timeWarning);
            }
            else if (selectedUsers.AreAvailable(date, start, end, calendar) == false)
            {
                MessageBox.Show(userWarning);
            }
            else
            {
                Appointment newAppointment = new Appointment(name, description, date, start, end, user, selectedUsers);
                calendar.AddAppointment(newAppointment);
                if (Utils.WriteEventsSerialFile(calendar, eventsFile))
                {
                    this.Close();
                }

            }
        }
        private void SaveSelectedUsers()
        {
            foreach (User item in listBoxAllUsers.SelectedItems)
            {
                selectedUsers.AddUser(item);
            }
        }
        #endregion

    }
}
