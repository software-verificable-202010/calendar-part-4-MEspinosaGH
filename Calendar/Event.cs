using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    class Event
    {
        #region Fields
        private string name;
        private string description;
        private DateTime date;
        private string[] start;
        private string[] end;
        private User owner;
        private UsersList participants;
        #endregion

        #region Properties  
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string[] Start
        {
            get { return start; }
            set { start = value; }
        }

        public string[] End
        {
            get { return end; }
            set { end = value; }
        }

        public User Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public UsersList Participants
        {
            get { return participants; }
            set { participants = value; }
        }
        #endregion

        #region Methods
        public Event(string name, string description, DateTime date, string[] start, string[] end, User owner, UsersList participants)
        {
            this.name = name;
            this.description = description;
            this.date = date;
            this.start = start;
            this.end = end;
            this.owner = owner;
            this.participants = participants;
        }
        #endregion
    }
}
