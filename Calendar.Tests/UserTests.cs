using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Tests
{
    [TestFixture]
    class UserTests
    {
        #region Fields
        private User user = new User("usuario");
        private Appointment appointment;
        private string[] start, end;
        private UsersList participants;
        #endregion

        #region Methods
        [Test]
        public void Test_Has_Same_Name_When_True()
        {
            Assert.IsTrue(user.HasSameNameAs("usuario"));
        }

        [Test]
        public void Test_Participates_In_Event_Returns_True_When_Is_Owner()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            participants = new UsersList();
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);

            Assert.IsTrue(user.ParticipatesInEvent(appointment));
        }

        [Test]
        public void Test_Participates_In_Event_Returns_True_When_Is_Participant()
        {
            start = new string[] { "10", "10" };
            end = new string[] { "20", "20" };
            User user2 = new User("user2");
            participants = new UsersList();
            participants.AddUser(user);
            appointment = new Appointment("evento", "descripcion", DateTime.Today, start, end, user2, participants);

            Assert.IsTrue(user.ParticipatesInEvent(appointment));
        }
        #endregion
    }
}
