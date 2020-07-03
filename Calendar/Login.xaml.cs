using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        #region Constants
        private const string usersFile = "Users.txt";
        #endregion

        #region Fields
        private readonly UsersList users = new UsersList();
        private User user;
        #endregion

        #region Methods
        public Login()
        {
            InitializeComponent();
            buttonAccept.Click += new RoutedEventHandler(AcceptBtn_Click);
            users = Utils.ReadUsersSerialFile();
        }
        private void AcceptBtn_Click(Object sender, EventArgs e)
        {
            bool isInList = false;
            foreach(User oldUser in users.Users)
            {
                if (oldUser.HasSameNameAs(textBoxUserName.Text))
                {
                    isInList = true;
                    user = oldUser;
                }
            }
            if (isInList == false)
            {
                user = new User(textBoxUserName.Text);
                users.AddUser(user);
            }
            if (Utils.WriteUsersSerialFile(users, usersFile))
            {
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                MainWindow window = new MainWindow(month, year, user);
                window.Show();

                this.Close();
            }

        }
        #endregion
    }
}
