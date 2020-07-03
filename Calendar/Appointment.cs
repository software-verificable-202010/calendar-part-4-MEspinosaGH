using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class Appointment
    {
        #region Constants
        private const int hourIndex = 0;
        private const int minuteIndex = 1;
        #endregion

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
        public Appointment(string name, string description, DateTime date, string[] start, string[] end, User owner, UsersList participants)
        {
            this.name = name;
            this.description = description;
            this.date = date;
            this.start = start;
            this.end = end;
            this.owner = owner;
            this.participants = participants;
        }

        public string[] GetStart()
        { return start; }

        public void SetStart(string[] value)
        { start = value; }

        public string[] GetEnd()
        { return end; }

        public void SetEnd(string[] value)
        { end = value; }

        public bool isThisMonth(int month, int year)
        {
            return date.Month == month && date.Year == year;
        }

        public bool isThisDay(int day, int month, int year)
        {
            return date.Day == day && date.Month == month && date.Year == year;
        }

        public void Edit(string newName, string newDescription, DateTime newDate, string[] newStart, string[] newEnd)
        {
            name = newName;
            description = newDescription;
            date = newDate;
            start = newStart;
            end = newEnd;
        }

        public string MonthViewAppointmentText()
        {
            string startText = String.Format(CultureInfo.InvariantCulture, format: "{0}:{1}", start[hourIndex], start[minuteIndex]);
            string endText = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", end[hourIndex], end[minuteIndex]);
            return String.Format(CultureInfo.InvariantCulture, "{0}({1} - {2})", name, startText, endText);
        }

        public string WeekViewAppointmentText(int hour)
        {
            int startHour = int.Parse(start[hourIndex], NumberFormatInfo.InvariantInfo);
            int endHour = int.Parse(end[hourIndex], NumberFormatInfo.InvariantInfo);
            int endMinute = int.Parse(end[minuteIndex], NumberFormatInfo.InvariantInfo);
            bool isStartHour = startHour == hour;
            bool isEndHour = endHour == hour && endMinute > 0;
            bool isLastHour = endHour == hour + 1 && endMinute == 0;
            if (isStartHour)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0} - Desde {1}:{2}", name, start[hourIndex], start[minuteIndex]);
            }
            else if (isEndHour || isLastHour)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0} - Hasta {1}:{2}", name, end[hourIndex], end[minuteIndex]);
            }
            return name;
            
        }
        #endregion
    }
}
