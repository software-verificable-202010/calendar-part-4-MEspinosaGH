using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    class UsersList
    {
        #region Fields
        private List<User> users;
        #endregion

        #region Properties
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }
        #endregion

        #region Methods
        public UsersList()
        {
            users = new List<User>();
        }
        #endregion
    }
}
