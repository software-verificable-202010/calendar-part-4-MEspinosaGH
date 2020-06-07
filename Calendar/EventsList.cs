using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    class EventsList
    {
        #region Fields
        private List<Event> events;
        #endregion

        #region Properties
        public List<Event> Events
        {
            get { return events; }
            set { events = value; }
        }
        #endregion

        #region Methods
        public EventsList()
        {
            events = new List<Event>();
        }
        #endregion
    }
}
