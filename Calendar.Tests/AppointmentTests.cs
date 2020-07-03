using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Tests
{
    [TestFixture]
    class AppointmentTests
    {
        #region Fields
        private string[] start, end;
        private User user;
        private UsersList participants;
        private Appointment appointment;
        private int day, month, year;
        #endregion

        #region Methods
        [Test]
        public void Test_Event_Is_This_Month_Returns_True()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
            Assert.IsTrue(appointment.isThisMonth(month, year));
        }

        [Test]
        public void Test_Event_Is_This_Day_Returns_True()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);
            day = DateTime.Now.Day;
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
            Assert.IsTrue(appointment.isThisDay(day, month, year));
        }

        [Test]
        public void Test_Edit_Event_Edits_Name_Correctly()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);
            appointment.Edit("evento2", "descripcion2", appointment.Date, start, end);
            Appointment expected = new Appointment("evento2", "descripcion2", appointment.Date, start, end, user, participants);

            Assert.AreEqual(expected.Name, appointment.Name);
        }

        [Test]
        public void Test_Edit_Event_Edits_Description_Correctly()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);
            appointment.Edit("evento2", "descripcion2", appointment.Date, start, end);
            Appointment expected = new Appointment("evento2", "descripcion2", appointment.Date, start, end, user, participants);

            Assert.AreEqual(expected.Description, appointment.Description);
        }

        [Test]
        public void Test_Month_View_Text_Returns_Correct_String()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);

            string expected = "evento(10:10 - 20:20)";

            Assert.AreEqual(expected, appointment.MonthViewAppointmentText());
        }

        [Test]
        public void Test_Week_View_Event_Text_Returns_Correct_String()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            user = new User("user");
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);

            string expected = "evento - Desde 10:10";
            Assert.AreEqual(expected, appointment.WeekViewAppointmentText(10));
        }
        #endregion
    }
}
