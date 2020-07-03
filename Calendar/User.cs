using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class User
    {
        #region Fields
        private string name;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Methods
        public User(string name)
        {
            this.name = name;
        }

        public bool hasSameNameAs(string otherUserName)
        {
            if (otherUserName == null)
            {
                throw new ArgumentNullException(nameof(otherUserName));
            }
            if (name == otherUserName)
            {
                return true;
            }
            return false;
        }

        public bool participatesInEvent(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            if (appointment.Owner.hasSameNameAs(name))
            {
                return true;
            }

            if (appointment.Participants != null)
            {
                foreach (User participant in appointment.Participants.Users)
                {
                    if (participant.hasSameNameAs(name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
