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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int month;
        int year;
        string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        const int positionUnit = 1;
        const int firstDayOfMonth = 1;
        const int sundayFirstPosition = 0;
        const int sundayLastPosition = 7;
        const int firstMonth = 1;
        const int lastMonth = 12;
        const int hour = 0;
        const int minute = 1;
        EventsList calendar;
        EventsList monthEvents;
        List<ItemsControl> eventsList;

        TextBlock[] daysOfMonth;

        void ReadSerialFile()
        {
            if (File.Exists("Events.txt"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("Events.txt", FileMode.Open, FileAccess.Read);
                calendar = (EventsList)formatter.Deserialize(stream);
            }
        }

        void GetMonthEvents()
        {
            if (calendar != null)
            {
                monthEvents = new EventsList();
                foreach (Event appointment in calendar.events)
                {
                    if (appointment.date.Year == year && appointment.date.Month == month)
                    {
                        monthEvents.events.Add(appointment);
                    }
                }
            }
        }

        void ClearEvents()
        {
            foreach (ItemsControl itemsControl in eventsList)
            {
                itemsControl.Items.Clear();
            }
            eventsList.Clear();
        }

        void SetEventsText(Event appointment, int day, ItemsControl itemsControlEvents)
        {
            if (appointment.date.Day == day)
            {
                TextBlock textBlockEvent = new TextBlock();
                string start = appointment.start[hour].ToString() + ":" + appointment.start[minute].ToString();
                string end = appointment.end[hour].ToString() + ":" + appointment.end[minute].ToString();
                string name = appointment.name;
                textBlockEvent.Text = name + "(" + start + " - " + end + ")";
                itemsControlEvents.Items.Add(textBlockEvent);
            }
        }

        void SetDayEvents(int day, int dayIndex)
        {
            if (monthEvents != null)
            {
                // ItemsControlEvents: Controller to list for TextBlock events
                ItemsControl itemsControlEvents = new ItemsControl();
                eventsList.Add(itemsControlEvents);
                gridMonth.Children.Add(itemsControlEvents);
                Grid.SetColumn(itemsControlEvents, Grid.GetColumn(daysOfMonth[dayIndex]));
                Grid.SetRow(itemsControlEvents, Grid.GetRow(daysOfMonth[dayIndex]));
                itemsControlEvents.HorizontalAlignment = HorizontalAlignment.Left;
                itemsControlEvents.VerticalAlignment = VerticalAlignment.Bottom;
                itemsControlEvents.Margin = new Thickness(5, 5, 5, 5);
                foreach (Event appointment in monthEvents.events)
                {
                    SetEventsText(appointment, day, itemsControlEvents);
                }
            }

        }

        void SetDaysOfMonth()
        {
            if (month.Equals(null) || year.Equals(null))
            {
                month = DateTime.Now.Month;
                year = DateTime.Now.Year;
            }

            ClearEvents();
            GetMonthEvents();
            
            textBlockMonth.Text = months[month - positionUnit];
            textBlockYear.Text = year.ToString();

            DateTime firstDayOfMonthDate = new DateTime(year, month, firstDayOfMonth);

            int firstWeekDay = (int)firstDayOfMonthDate.DayOfWeek;
            if (firstWeekDay == sundayFirstPosition) { firstWeekDay = sundayLastPosition; }

            int lastDay = DateTime.DaysInMonth(year, month);
            int day = firstDayOfMonth;

            for (int dayIndex = 0; dayIndex < daysOfMonth.Length; dayIndex++)
            {
                if (dayIndex < firstWeekDay - positionUnit || day > lastDay)
                {
                    daysOfMonth[dayIndex].Text = "";
                }
                else
                {
                    daysOfMonth[dayIndex].Text = day.ToString();
                    SetDayEvents(day, dayIndex);
                    day++;
                }
            }
        }

        void LastMonthBtn_Click(Object sender, EventArgs e)
        {
            month--;
            if (month < firstMonth)
            {
                year--;
                month = lastMonth;
            }

            SetDaysOfMonth();
        }

        void NextMonthBtn_Click(Object sender, EventArgs e)
        {
            month++;
            if (month > lastMonth)
            {
                year++;
                month = firstMonth;
            }

            SetDaysOfMonth();
        }

        void WeeklyViewBtn_Click(Object sender, EventArgs e)
        {
            Week weekWindow = new Week(month, year);
            weekWindow.Show();
            this.Close();
        }

        void NewEventBtn_Click(Object sender, EventArgs e)
        {
            NewEvent newEvent = new NewEvent();
            newEvent.ShowDialog();
            ReadSerialFile();
            SetDaysOfMonth();
        }

        public MainWindow(int passed_month, int passed_year)
        {
            ReadSerialFile();
            InitializeComponent();
            year = passed_year;
            month = passed_month;
            buttonLastMonth.Click += new RoutedEventHandler(LastMonthBtn_Click);
            buttonNextMonth.Click += new RoutedEventHandler(NextMonthBtn_Click);
            buttonWeeklyView.Click += new RoutedEventHandler(WeeklyViewBtn_Click);
            buttonNewEvent.Click += new RoutedEventHandler(NewEventBtn_Click);

            daysOfMonth = new TextBlock[] {this.TextBlockDay1, this.TextBlockDay2, this.TextBlockDay3, this.TextBlockDay4, this.TextBlockDay5, this.TextBlockDay6, this.TextBlockDay7,
                    this.TextBlockDay8, this.TextBlockDay9, this.TextBlockDay10, this.TextBlockDay11, this.TextBlockDay12, this.TextBlockDay13, this.TextBlockDay14,
                    this.TextBlockDay15, this.TextBlockDay16, this.TextBlockDay17, this.TextBlockDay18, this.TextBlockDay19, this.TextBlockDay20, this.TextBlockDay21,
                    this.TextBlockDay22, this.TextBlockDay23, this.TextBlockDay24, this.TextBlockDay25, this.TextBlockDay26, this.TextBlockDay27, this.TextBlockDay28,
                    this.TextBlockDay29, this.TextBlockDay30, this.TextBlockDay31, this.TextBlockDay32, this.TextBlockDay33, this.TextBlockDay34, this.TextBlockDay35,
                    this.TextBlockDay36, this.TextBlockDay37, this.TextBlockDay38, this.TextBlockDay39, this.TextBlockDay40, this.TextBlockDay41, this.TextBlockDay42};

            eventsList = new List<ItemsControl>();

            SetDaysOfMonth();
        }
    }
}
