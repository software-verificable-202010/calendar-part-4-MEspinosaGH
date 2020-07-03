using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class UsersList
    {
        #region Fields
        private List<User> users;
        #endregion

        #region Properties
        public List<User> Users
        {
            get { return users; }
        }
        #endregion

        #region Methods
        public UsersList()
        {
            users = new List<User>();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public void ClearUsers()
        {
            Users.Clear();
        }

        public bool AreAvailable(DateTime date, string[] start, string[] end, AppointmentsList calendar)
        {
            if (calendar == null)
            {
                return true;
            }
            foreach (Appointment appointment in calendar.Appointments)
            {
                if (Utils.DatesCollide(appointment, date, start, end))
                {
                    foreach (User selectedUser in users)
                    {
                        if (appointment.Owner.HasSameNameAs(selectedUser.Name))
                        {
                            return false;
                        }
                        if (appointment.Participants != null)
                        {
                            foreach (User participant in appointment.Participants.Users)
                            {
                                if (participant.HasSameNameAs(selectedUser.Name))
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
        #endregion
    }
}
