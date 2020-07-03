using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Tests
{
    [TestFixture]
    class AppointmentsListTests
    {
        #region Fields
        private Appointment appointment;
        private AppointmentsList events;
        private List<Appointment> expected;
        private string[] start;
        private string[] end;
        private User user;
        private UsersList participants;
        #endregion

        #region Methods
        [Test]
        public void Test_Add_Event_Works()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "11", "11" };
            participants = new UsersList();
            user = new User("usuario");
            events = new AppointmentsList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);

            events.AddAppointment(appointment);
            expected = new List<Appointment> { appointment };
            Assert.AreEqual(events.Appointments, expected);
        }

        [Test]
        public void Test_Remove_Event_Works()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "11", "11" };
            participants = new UsersList();
            user = new User("usuario");
            events = new AppointmentsList();
            Appointment appointment1 = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);
            Appointment appointment2 = new Appointment("evento2", "descripcion2", DateTime.Today, start, end, user, participants);

            events.AddAppointment(appointment1);
            events.AddAppointment(appointment2);
            events.RemoveAppointment(appointment1);

            expected = new List<Appointment> { appointment2 };

            Assert.AreEqual(events.Appointments, expected);
        }
        #endregion
    }
}
