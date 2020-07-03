using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Tests
{
    [TestFixture]
    class UsersListTests
    {
        #region Fields
        private UsersList users;
        private User user, user2;
        private AppointmentsList events;
        private string[] start;
        private string[] end;
        private UsersList participants;
        #endregion

        #region Methods
        [Test]
        public void Test_Add_User_Works()
        {
            user = new User("usuario");
            users = new UsersList();
            users.AddUser(user);
            List<User> userList = new List<User> { user };
            Assert.AreEqual(userList, users.Users);
        }

        [Test]
        public void Test_Clear_Users_Works()
        {
            user = new User("usuario");
            users = new UsersList();
            users.AddUser(user);
            users.ClearUsers();
            List<User> userList = new List<User> {};
            Assert.AreEqual(userList, users.Users);
        }

        [Test]
        public void Test_Users_Are_Available_Returns_True()
        {
            user = new User("usuario");
            user2 = new User("usuario2");
            users = new UsersList();
            users.AddUser(user);
            users.AddUser(user2);

            start = new string[] { "10", "10" };
            end = new string[] { "11", "11" };
            participants = new UsersList();
            events = new AppointmentsList();
            Appointment appointment1 = new Appointment("evento", "descripcion", DateTime.Today, start, end, user, participants);
            Appointment appointment2 = new Appointment("evento2", "descripcion2", DateTime.Today, start, end, user, participants);

            events.AddAppointment(appointment1);
            events.AddAppointment(appointment2);

            Assert.IsFalse(users.AreAvailable(DateTime.Today, start, end, events));
        }
        #endregion
    }
}
