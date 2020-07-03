using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Calendar;
using NUnit.Framework;

namespace Calendar.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        #region Fields
        private AppointmentsList eventList;
        private UsersList users;
        private Appointment oldEvent;
        private string[] oldStart, oldEnd;
        private User user;
        private UsersList participants;
        private DateTime newEventDate;
        private string[] newEventStart, newEventEnd;
        private AppointmentsList calendar;
        private AppointmentsList monthEvents;
        private int day, month, year;
        #endregion

        #region Methods
        [Test]
        public void Read_Events_Serial_File_Returns_EventList()
        {
            eventList = new AppointmentsList();
            Assert.AreEqual(eventList.GetType(), Utils.ReadEventsSerialFile().GetType());
        }

        [Test]
        public void Write_Events_Serial_File_Returns_True()
        {
            calendar = new AppointmentsList();
            oldEvent = new Appointment("evento", "descripcion", DateTime.Today, oldStart, oldEnd, user, participants);
            calendar.Appointments.Add(oldEvent);
            Assert.IsTrue(Utils.WriteEventsSerialFile(calendar, "TestEventFile.txt"));
        }

        [Test]
        public void Read_Users_Serial_File_Returns_EventList()
        {
            users = new UsersList();
            Assert.AreEqual(users.GetType(), Utils.ReadUsersSerialFile().GetType());
        }

        [Test]
        public void Write_Users_Serial_File_Returns_True()
        {
            user = new User("user");
            users = new UsersList();
            users.Users.Add(user);
            Assert.IsTrue(Utils.WriteUsersSerialFile(users, "TestEventFile.txt"));
        }

        [Test]
        public void Test_Dates_Collide_When_Dates_Collide_Resturns_True()
        {
            oldStart = new string[] { "10", "10" };
            oldEnd = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            oldEvent = new Appointment("evento", "descripcion", DateTime.Today, oldStart, oldEnd, user, participants);
            newEventDate = DateTime.Today;
            newEventStart = new string[] { "11", "11" };
            newEventEnd = new string[] { "22", "22" };

            Assert.IsTrue(Utils.DatesCollide(oldEvent, newEventDate, newEventStart, newEventEnd));
        }

        [Test]
        public void Test_Get_Month_Events_Returns_Correct_Events()
        {
            calendar = new AppointmentsList();
            oldStart = new string[] { "10", "10" };
            oldEnd = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            oldEvent = new Appointment("evento", "descripcion", DateTime.Today, oldStart, oldEnd, user, participants);
            calendar.AddAppointment(oldEvent);
            month = DateTime.Today.Month;
            year = DateTime.Today.Year;

            monthEvents = Utils.GetMonthEvents(calendar, month, year, user);

            Assert.AreEqual(calendar.Appointments, monthEvents.Appointments);
        }

        [Test]
        public void Test_Get_Day_Events_Returns_Events()
        {
            calendar = new AppointmentsList();
            oldStart = new string[] { "10", "10" };
            oldEnd = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            oldEvent = new Appointment("evento", "descripcion", DateTime.Today, oldStart, oldEnd, user, participants);
            calendar.AddAppointment(oldEvent);
            day = DateTime.Today.Day;
            month = DateTime.Today.Month;
            year = DateTime.Today.Year;

            monthEvents = Utils.GetDayEvents(calendar, day, month, year, user);

            Assert.AreEqual(calendar.Appointments, monthEvents.Appointments);
        }

        [Test]
        public void Test_Is_Last_Year_Returns_True()
        {
            Assert.IsTrue(Utils.IsLastYear(0));
        }

        [Test]
        public void Test_Is_Next_Year_Returns_True()
        {
            Assert.IsTrue(Utils.IsNextYear(13));
        }

        [Test]
        public void Test_Set_First_Day_Of_Week_Returns_Correct_Value()
        {
            Assert.AreEqual(7, Utils.SetFirstWeekDay(0));
        }

        [Test]
        public void Test_Is_Not_Day_Of_Month_Returns_True()
        {
            Assert.IsTrue(Utils.IsNotMonthDay(0, 0, 33, 31));
        }

        [Test]
        public void Test_Year_Text_Returns_Correct_String()
        {
            Assert.AreEqual("2019 / 2020", Utils.YearText(2019, 2020));
        }
        #endregion
    }
}
