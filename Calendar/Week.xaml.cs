using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
    public partial class Week : Window
    {
        int month;
        int year;
        int day;
        const int positionUnit = 1;
        const int firstDayOfMonth = 1;
        const int sundayFirstPosition = 0;
        const int sundayLastPosition = 7;
        const int january = 1;
        const int december = 12;
        const int oneMonth = 1;
        const int oneDay = 1;
        const int hourIndex = 0;
        const int minuteIndex = 1;
        const int weekLength = 7;
        const int twoWeeks = 14;
        const int oneHour = 1;
        const int minuteZero = 0;
        EventsList calendar;
        List<ItemsControl> eventsList;
        TextBlock[] daysOfWeekTextBlocks;
        string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        string[] daysOfWeek = new string[] { "Lunes ", "Martes ", "Miércoles ", "Jueves ", "Viernes ", "Sábado ", "Domingo " };

        void GetDayEvents(int year, int month, int day, int weekDay)
        {
            if (calendar != null)
            {
                EventsList dayEvents = new EventsList();
                foreach (Event appoinment in calendar.events)
                {
                    int appointmentYear = appoinment.date.Year;
                    int appointmentMonth = appoinment.date.Month;
                    int appointmentDay = appoinment.date.Day;

                    if (year == appointmentYear && month == appointmentMonth && day == appointmentDay)
                    {
                        dayEvents.events.Add(appoinment);
                    }
                }

                SetDayEvents(dayEvents, weekDay + positionUnit);
            }
        }

        void SetDayEvents(EventsList events, int weekDay)
        {
            for (int hour = 0; hour < 24; hour++)
            {
                ItemsControl itemsControlEvents = new ItemsControl();
                eventsList.Add(itemsControlEvents);
                GridHours.Children.Add(itemsControlEvents);
                Grid.SetColumn(itemsControlEvents, weekDay);
                Grid.SetRow(itemsControlEvents, hour + positionUnit);

                foreach (Event appointment in events.events)
                {
                    int startHour = int.Parse(appointment.start[hourIndex]);
                    int startMinute = int.Parse(appointment.start[minuteIndex]);
                    int endHour = int.Parse(appointment.end[hourIndex]);
                    int endMinute = int.Parse(appointment.end[minuteIndex]);
                    TextBlock textBlockEvent = new TextBlock();
                    if (startHour == hour)
                    {
                        textBlockEvent.Text = appointment.name + " - Desde " + appointment.start[hourIndex] + ":" + appointment.start[minuteIndex];
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                    else if (endHour == hour && endMinute > minuteZero)
                    {
                        textBlockEvent.Text = appointment.name + " - Hasta " + appointment.end[hourIndex] + ":" + appointment.end[minuteIndex];
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                    else if (endHour == hour + oneHour && endMinute == minuteZero)
                    {
                        textBlockEvent.Text = appointment.name + " - Hasta " + appointment.end[hourIndex] + ":" + appointment.end[minuteIndex];
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                    else if (startHour < hour && endHour > hour)
                    {
                        textBlockEvent.Text = appointment.name;
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                }
            }
        }

        void ClearEvents()
        {
            foreach (ItemsControl itemsControl in eventsList) { itemsControl.Items.Clear(); }
            eventsList.Clear();
        }

        void SetCalendarDays(int firstWeekdayOfMonth, int lastDayOfLastMonth, int lastMonthYear, int lastMonth)
        {
            for (int weekDay = 0; weekDay < daysOfWeekTextBlocks.Length; weekDay++)
            {
                if (firstWeekdayOfMonth <= weekDay + positionUnit)
                {
                    daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + day.ToString();

                    GetDayEvents(year, month, day, weekDay);

                    day++;
                }
                else
                {
                    int lastMonthDay = lastDayOfLastMonth - (firstWeekdayOfMonth - oneDay) + weekDay + positionUnit;
                    daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + lastMonthDay.ToString();

                    GetDayEvents(lastMonthYear, lastMonth, lastMonthDay, weekDay);
                }
            }
        }

        void SetFirstWeek()
        {
            ClearEvents();

            DateTime firstDayDate = new DateTime(year, month, firstDayOfMonth);

            int lastMonth = month - oneMonth;
            int lastMonthYear = year;
            if (lastMonth < january)
            {
                lastMonth = december;
                lastMonthYear--;
            }
            int lastDayOfLastMonth = DateTime.DaysInMonth(lastMonthYear, lastMonth);

            int firstWeekdayOfMonth = (int)firstDayDate.DayOfWeek;
            if (firstWeekdayOfMonth == sundayFirstPosition) { firstWeekdayOfMonth = sundayLastPosition; }

            day = firstDayOfMonth;

            SetCalendarDays(firstWeekdayOfMonth, lastDayOfLastMonth, lastMonthYear, lastMonth);

            if (lastMonthYear != year)
            {
                TextBlockYear.Text = lastMonthYear.ToString() + " / " + year.ToString();
            }
            else
            {
                TextBlockYear.Text = year.ToString();
            }
            if (firstWeekdayOfMonth > 1)
            {
                TextBlockMonth.Text = months[lastMonth - positionUnit] + " / " + months[month - positionUnit];
            }
            else
            {
                TextBlockMonth.Text = months[month - positionUnit];
            }
        }

        // set Month and Year when user clicks Next
        void SetNextWeekMonthAndYear(bool changeOfMonth)
        {
            if (changeOfMonth)
            {
                int nextMonth = month + oneMonth;
                int nextMonthYear = year;
                if (nextMonth > december)
                {
                    nextMonth = january;
                    nextMonthYear++;
                }
                TextBlockMonth.Text = months[month - positionUnit] + " / " + months[nextMonth - positionUnit];
                if (day >= weekLength)
                {
                    TextBlockMonth.Text = months[nextMonth - positionUnit];
                }
                if (year != nextMonthYear)
                {
                    TextBlockYear.Text = year.ToString() + " / " + nextMonthYear.ToString();
                    if (day >= weekLength)
                    {
                        TextBlockYear.Text = nextMonthYear.ToString();
                    }
                }
                else
                {
                    TextBlockYear.Text = year.ToString();
                }
                year = nextMonthYear;
                month = nextMonth;
            }
            else
            {
                TextBlockMonth.Text = months[month - positionUnit];
                TextBlockYear.Text = year.ToString();
            }
        }

        void SetNextWeek_Click(Object sender, EventArgs e)
        {
            ClearEvents();
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            bool changeOfMonth = false;
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                if (day > lastDayOfMonth)
                {
                    day = firstDayOfMonth;
                    changeOfMonth = true;
                    GetDayEvents(year, month + oneMonth, day, weekDay);
                }
                else
                {
                    GetDayEvents(year, month, day, weekDay);
                }
                daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + day.ToString();
                day++;
            }
            SetNextWeekMonthAndYear(changeOfMonth);
        }

        // set last week of last month
        void SetLastMonth()
        {
            month--;
            if (month < january)
            {
                month = december;
                year--;
            }
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            day = lastDayOfMonth + (day - twoWeeks);
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + day.ToString();

                GetDayEvents(year, month, day, weekDay);

                day++;
            }
            TextBlockMonth.Text = months[month - positionUnit];
            TextBlockYear.Text = year.ToString();
        }

        // set last week
        void SetLastWeek()
        {
            day = day - twoWeeks;
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + day.ToString();
                GetDayEvents(year, month, day, weekDay);
                day++;
            }
            TextBlockMonth.Text = months[month - positionUnit];
            TextBlockYear.Text = year.ToString();
        }

        void SetLastWeek_Click(Object sender, EventArgs e)
        {
            ClearEvents();
            if (day <= weekLength + oneDay)
            {
                SetLastMonth();
            }
            else if (day > weekLength + oneDay && day <= twoWeeks)
            {
                SetFirstWeek();
            }
            else
            {
                SetLastWeek();
            }
           
        }

        void MonthlyViewBtn_Click(Object sender, EventArgs e)
        {
            MainWindow monthlyView = new MainWindow(month, year);
            monthlyView.Show();
            this.Close();
        }

        void ReadSerialFile()
        {
            if (File.Exists("Events.txt"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("Events.txt", FileMode.Open, FileAccess.Read);
                calendar = (EventsList)formatter.Deserialize(stream);
            }
        }

        void NewEventBtn_Click(Object sender, EventArgs e)
        {
            NewEvent newEvent = new NewEvent();
            newEvent.ShowDialog();
            ReadSerialFile();
            Week newWeekView = new Week(month, year);
            newWeekView.Show();
            this.Close();
        }

        public Week(int passed_month, int passed_year)
        {
            month = passed_month;
            year = passed_year;
            ReadSerialFile();

            InitializeComponent();

            daysOfWeekTextBlocks = new TextBlock[] { TextBlockMonday, TextBlockTuesday, TextBlockWednesday, TextBlockThursday, TextBlockFriday, TextBlockSaturday, TextBlockSunday };
            ButtonMonthlyView.Click += new RoutedEventHandler(MonthlyViewBtn_Click);
            ButtonNextWeek.Click += new RoutedEventHandler(SetNextWeek_Click);
            ButtonLastWeek.Click += new RoutedEventHandler(SetLastWeek_Click);
            ButtonNewEvent.Click += new RoutedEventHandler(NewEventBtn_Click);

            eventsList = new List<ItemsControl>();
            SetFirstWeek();
        }
    }
}
