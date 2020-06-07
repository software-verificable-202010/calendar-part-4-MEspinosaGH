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
        #region Fields
        private Utils util = new Utils();
        private UsersList users = new UsersList();
        private User user;
        #endregion

        #region Methods
        private void AcceptBtn_Click(Object sender, EventArgs e)
        {
            bool isInList = false;
            foreach(User oldUser in users.Users)
            {
                Console.WriteLine(oldUser.Name);
                if (oldUser.Name == textBoxUserName.Text)
                {
                    isInList = true;
                    user = oldUser;
                }
            }
            if (isInList == false)
            {
                user = new User(textBoxUserName.Text);
                users.Users.Add(user);
            }
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Users.txt", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, users);
            stream.Close();

            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            MainWindow window = new MainWindow(month, year, user);
            window.Show();

            this.Close();
        }

        public Login()
        {
            InitializeComponent();
            buttonAccept.Click += new RoutedEventHandler(AcceptBtn_Click);
            users = util.ReadUsersSerialFile();
        }
        #endregion
    }
}
