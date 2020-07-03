using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    public static class Utils
    {
        #region Constants
        private const int hourIndex = 0;
        private const int minuteIndex = 1;
        private const int minutesInAnHour = 60;
        private const int january = 1;
        private const int december = 12;
        private const int sundayInFirstPosition = 0;
        private const int sundayInLastPosition = 7;
        private const int unitPosition = 1;
        private const string eventsFile = "Events.txt";
        private const string usersFile = "Users.txt";
        #endregion

        #region Methods
        public static AppointmentsList ReadEventsSerialFile()
        {
            if (File.Exists(eventsFile))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(eventsFile, FileMode.Open, FileAccess.Read);
                AppointmentsList events = (AppointmentsList)formatter.Deserialize(stream);
                stream.Close();
                return events;
            } else
            {
                return new AppointmentsList();
            }
        }

        public static bool WriteEventsSerialFile(AppointmentsList calendar, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, calendar);
            stream.Close();
            return true;
        }
        public static bool WriteUsersSerialFile(UsersList users, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, users);
            stream.Close();
            return true;
        }

        public static UsersList ReadUsersSerialFile()
        {
            if (File.Exists(usersFile))
            {
                IFormatter userFormatter = new BinaryFormatter();
                Stream userStream = new FileStream(usersFile, FileMode.Open, FileAccess.Read);
                UsersList users = (UsersList)userFormatter.Deserialize(userStream);
                userStream.Close();
                return users;
            }
            else
            {
                return new UsersList();
            }
        }

        public static bool DatesCollide(Appointment oldEvent, DateTime newEventDate, string[] newEventStart, string[] newEventEnd)
        {
            if (oldEvent == null)
            {
                throw new ArgumentNullException(nameof(oldEvent));
            }
            if (newEventStart == null)
            {
                throw new ArgumentNullException(nameof(newEventStart));
            }
            if (newEventEnd == null)
            {
                throw new ArgumentNullException(nameof(newEventEnd));
            }
            bool areDifferentDates = oldEvent.Date != newEventDate;
            int oldEventStartTime = Int32.Parse(oldEvent.GetStart()[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(oldEvent.GetStart()[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            int oldEventEndTIme = Int32.Parse(oldEvent.GetEnd()[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(oldEvent.GetEnd()[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            int newEventStartTime = Int32.Parse(newEventStart[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(newEventStart[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            int newEventEndTime = Int32.Parse(newEventEnd[hourIndex], NumberFormatInfo.InvariantInfo) + Int32.Parse(newEventEnd[minuteIndex], NumberFormatInfo.InvariantInfo) * minutesInAnHour;
            bool areAtDifferentHours = oldEventStartTime >= newEventEndTime || newEventStartTime >= oldEventEndTIme;
            if (areDifferentDates || areAtDifferentHours)
            {
                return false;
            }
            return true;
        }

        public static AppointmentsList GetMonthEvents(AppointmentsList calendar, int month, int year, User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            AppointmentsList monthEvents = new AppointmentsList();
            if (calendar == null)
            {
                return monthEvents;
            }
            foreach (Appointment appointment in calendar.Appointments)
            {
                if (appointment.isThisMonth(month, year) && user.participatesInEvent(appointment))
                {
                    monthEvents.AddAppointment(appointment);
                }
            }
            return monthEvents;
        }

        public static AppointmentsList GetDayEvents(AppointmentsList calendar, int day, int month, int year, User user)
        {
            AppointmentsList dayEvents = new AppointmentsList();

            if (calendar == null)
            {
                return dayEvents;
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            foreach (Appointment appointment in calendar.Appointments)
            {
                if (appointment.isThisDay(day, month, year) && user.participatesInEvent(appointment))
                {
                    dayEvents.AddAppointment(appointment);
                }
            }
            return dayEvents;
        }

        public static bool IsLastYear(int month)
        {
            return month < january;
        }

        public static bool IsNextYear(int month)
        {
            return month > december;
        }

        public static int SetFirstWeekDay(int day)
        {
            if (day == sundayInFirstPosition)
            {
                return sundayInLastPosition;
            }
            return day;
        }

        public static bool IsNotMonthDay(int dayIndex, int firstWeekDay, int day, int lastDay)
        {
            bool monthHasNotStarted = dayIndex < firstWeekDay - unitPosition;
            bool monthEnded = day > lastDay;
            return monthHasNotStarted || monthEnded;
        }

        public static string YearText(int lastMonthYear, int year)
        {
            if (lastMonthYear != year)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0} / {1}", lastMonthYear, year);
            }
            return year.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
