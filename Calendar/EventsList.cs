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
        public List<Event> events;

        public EventsList()
        {
            events = new List<Event>();
        }
    }
}
