using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    class Event
    {

        public string name;
        public string description;
        public DateTime date;
        public string[] start;
        public string[] end;

        public Event(string name, string description, DateTime date, string[] start, string[] end)
        {
            this.name = name;
            this.description = description;
            this.date = date;
            this.start = start;
            this.end = end;
        }
    }
}
