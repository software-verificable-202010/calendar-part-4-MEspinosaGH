using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    class Utils
    {
        public EventsList ReadEventsSerialFile()
        {
            if (File.Exists("Events.txt"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("Events.txt", FileMode.Open, FileAccess.Read);
                EventsList events = (EventsList)formatter.Deserialize(stream);
                stream.Close();
                return events;
            } else
            {
                return new EventsList();
            }
        }

        public UsersList ReadUsersSerialFile()
        {
            if (File.Exists("Users.txt"))
            {
                IFormatter userFormatter = new BinaryFormatter();
                Stream userStream = new FileStream("Users.txt", FileMode.Open, FileAccess.Read);
                UsersList users = (UsersList)userFormatter.Deserialize(userStream);
                userStream.Close();
                return users;
            }
            else
            {
                return new UsersList();
            }
        }
    }
}
