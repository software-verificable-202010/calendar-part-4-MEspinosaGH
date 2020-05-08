using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    /// Lógica de interacción para NewEvent.xaml
    /// </summary>
    public partial class NewEvent : Window
    {
        EventsList calendar = new EventsList();

        void ReadSerialFile()
        {
            if (File.Exists("Events.txt"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("Events.txt", FileMode.Open, FileAccess.Read);
                calendar = (EventsList)formatter.Deserialize(stream);
            }
        }

        void CancelBtn_Click(Object sender, EventArgs e)
        {
            this.Close();
        }

        void SaveBtn_Click(Object sender, EventArgs e)
        {
            string name = TextBoxName.Text;
            string description = TextBoxDescription.Text;
            DateTime date = DatePickerEventDate.SelectedDate.Value.Date;
            string startHour = ComboBoxStartTimeHour.Text.ToString();
            string startMinute = ComboBoxStartTimeMinute.Text.ToString();
            string endHour = ComboBoxFinishTimeHour.Text;
            string endMinute = ComboBoxFinishTimeMinute.Text;
            string[] start = { startHour, startMinute };
            string[] end = { endHour, endMinute };

            if (int.Parse(endHour) < int.Parse(startHour) || (int.Parse(startHour) == int.Parse(endHour) && int.Parse(endMinute) <= int.Parse(startMinute)))
            {
                TextBlockWarning.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockWarning.Visibility = Visibility.Collapsed;
                Event newEvent = new Event(name, description, date, start, end);
                calendar.events.Add(newEvent);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("Events.txt", FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, calendar);
                stream.Close();
                this.Close();
            }
        }

        public NewEvent()
        {
            ReadSerialFile();

            InitializeComponent();
            DatePickerEventDate.SelectedDate = DateTime.Today;

            ButtonCancel.Click += new RoutedEventHandler(CancelBtn_Click);
            ButtonSave.Click += new RoutedEventHandler(SaveBtn_Click);
        }
    }
}
