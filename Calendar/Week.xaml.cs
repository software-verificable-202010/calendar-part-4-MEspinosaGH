using System;
using System.Globalization;
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
        #region Constants
        private const int positionUnit = 1;
        private const int firstDayOfMonth = 1;
        private const int january = 1;
        private const int december = 12;
        private const int oneMonth = 1;
        private const int oneDay = 1;
        private const int weekLength = 7;
        private const int twoWeeks = 14;
        private const string defaultUserName = "User";
        private readonly string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        private readonly string[] daysOfWeek = new string[] { "Lunes ", "Martes ", "Miércoles ", "Jueves ", "Viernes ", "Sábado ", "Domingo " };
        #endregion

        #region Fields
        private AppointmentsList calendar;
        private int month;
        private int year;
        private int day;
        private List<ItemsControl> itemControlListEvents;
        private TextBlock[] daysOfWeekTextBlocks;
        private User user;
        private AppointmentsList dayEvents;
        #endregion

        #region Methods
        public Week(int passedMonth, int passedYear, User passedUser)
        {
            month = passedMonth;
            year = passedYear;
            if (passedUser == null)
            {
                passedUser = new User(defaultUserName);
            }
            user = passedUser;

            calendar = Utils.ReadEventsSerialFile();

            InitializeComponent();

            daysOfWeekTextBlocks = new TextBlock[] { TextBlockMonday, TextBlockTuesday, TextBlockWednesday, TextBlockThursday, TextBlockFriday, TextBlockSaturday, TextBlockSunday };
            ButtonMonthlyView.Click += new RoutedEventHandler(MonthlyViewBtn_Click);
            ButtonNextWeek.Click += new RoutedEventHandler(SetNextWeek_Click);
            ButtonLastWeek.Click += new RoutedEventHandler(SetLastWeek_Click);
            ButtonNewEvent.Click += new RoutedEventHandler(NewEventBtn_Click);
            textBlockUserName.Text = string.Format(CultureInfo.InvariantCulture, "Hola, {0}", user.Name);

            itemControlListEvents = new List<ItemsControl>();
            SetFirstWeek();
        }

        private void MonthlyViewBtn_Click(Object sender, EventArgs e)
        {
            MainWindow monthlyView = new MainWindow(month, year, user);
            monthlyView.Show();
            this.Close();
        }

        private void NewEventBtn_Click(Object sender, EventArgs e)
        {
            NewEvent newEvent = new NewEvent(user);
            newEvent.ShowDialog();
            calendar = Utils.ReadEventsSerialFile();
            Week newWeekView = new Week(month, year, user);
            newWeekView.Show();
            this.Close();
        }

        private void ButtonEvents_Click(object sender, RoutedEventArgs e)
        {
            MyEvents myEvents = new MyEvents(user);
            myEvents.ShowDialog();
            calendar = Utils.ReadEventsSerialFile();
            Week newWeekView = new Week(month, year, user);
            newWeekView.Show();
            this.Close();
        }

        private void SetNextWeek_Click(Object sender, EventArgs e)
        {
            ClearEvents();
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            bool changeOfMonth = false;
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                bool monthEnded = day > lastDayOfMonth;
                if (monthEnded)
                {
                    day = firstDayOfMonth;
                    changeOfMonth = true;
                    dayEvents = Utils.GetDayEvents(calendar, day, month + oneMonth, year, user);
                }
                else
                {
                    dayEvents = Utils.GetDayEvents(calendar, day, month, year, user);
                }
                SetDayEvents(dayEvents, weekDay + positionUnit);
                daysOfWeekTextBlocks[weekDay].Text = String.Format(CultureInfo.InvariantCulture, "{0}{1}", daysOfWeek[weekDay], day);
                day++;
            }
            SetNextWeekMonthAndYear(changeOfMonth);
        }

        private void SetNextWeekMonthAndYear(bool changeOfMonth)
        {
            if (changeOfMonth)
            {
                int nextMonth = month + oneMonth;
                int nextMonthYear = year;

                bool isNextMonth = day >= weekLength;
                if (Utils.IsNextYear(nextMonth))
                {
                    nextMonth = january;
                    nextMonthYear++;
                }
                TextBlockYear.Text = Utils.YearText(year, nextMonthYear);
                TextBlockMonth.Text = String.Format(CultureInfo.InvariantCulture, "{0} / {1}", months[month - positionUnit], months[nextMonth - positionUnit]);
                if (isNextMonth)
                {
                    TextBlockMonth.Text = months[nextMonth - positionUnit];
                    TextBlockYear.Text = nextMonthYear.ToString(CultureInfo.InvariantCulture);
                }

                year = nextMonthYear;
                month = nextMonth;
            }
            else
            {
                TextBlockMonth.Text = months[month - positionUnit];
                TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void SetLastWeek_Click(Object sender, EventArgs e)
        {
            ClearEvents();
            bool isLastMonth = day <= weekLength + oneDay;
            bool isFirstWeek = day > weekLength + oneDay && day <= twoWeeks;
            if (isLastMonth)
            {
                SetLastMonth();
            }
            else if (isFirstWeek)
            {
                SetFirstWeek();
            }
            else
            {
                SetLastWeek();
            }
        }

        private void SetLastMonth()
        {
            month--;
            if (Utils.IsLastYear(month))
            {
                month = december;
                year--;
            }
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            day = lastDayOfMonth + (day - twoWeeks);
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                daysOfWeekTextBlocks[weekDay].Text = String.Format(CultureInfo.InvariantCulture, "{0}{1}", daysOfWeek[weekDay], day);
                dayEvents = Utils.GetDayEvents(calendar, day, month, year, user);
                SetDayEvents(dayEvents, weekDay + positionUnit);
                day++;
            }
            TextBlockMonth.Text = months[month - positionUnit];
            TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
        }

        private void SetFirstWeek()
        {
            ClearEvents();

            DateTime firstDayDate = new DateTime(year, month, firstDayOfMonth);

            int lastMonth = month - oneMonth;
            int lastMonthYear = year;
            if (Utils.IsLastYear(lastMonth))
            {
                lastMonth = december;
                lastMonthYear--;
            }
            int lastDayOfLastMonth = DateTime.DaysInMonth(lastMonthYear, lastMonth);

            int firstWeekdayOfMonth = Utils.SetFirstWeekDay((int)firstDayDate.DayOfWeek);

            day = firstDayOfMonth;

            SetCalendarDays(firstWeekdayOfMonth, lastDayOfLastMonth, lastMonthYear, lastMonth);

            TextBlockYear.Text = Utils.YearText(lastMonthYear, year);

            bool hasDifferentMonths = firstWeekdayOfMonth > 1;
            if (hasDifferentMonths)
            {
                TextBlockMonth.Text = String.Format(CultureInfo.InvariantCulture, "{0} / {1}", months[lastMonth - positionUnit], months[month - positionUnit]);
            }
            else
            {
                TextBlockMonth.Text = months[month - positionUnit];
            }
        }

        private void SetCalendarDays(int firstWeekdayOfMonth, int lastDayOfLastMonth, int lastMonthYear, int lastMonth)
        {
            for (int weekDay = 0; weekDay < daysOfWeekTextBlocks.Length; weekDay++)
            {
                bool isThisMonth = firstWeekdayOfMonth <= weekDay + positionUnit;
                string text;
                if (isThisMonth)
                {
                    text = String.Format(CultureInfo.InvariantCulture, "{0} {1}", daysOfWeek[weekDay], day);
                    dayEvents = Utils.GetDayEvents(calendar, day, month, year, user);
                    day++;
                }
                else
                {
                    int lastMonthDay = lastDayOfLastMonth - (firstWeekdayOfMonth - oneDay) + weekDay + positionUnit;
                    text = String.Format(CultureInfo.InvariantCulture, "{0} {1}", daysOfWeek[weekDay], lastMonthDay);
                    dayEvents = Utils.GetDayEvents(calendar, lastMonthDay, lastMonth, lastMonthYear, user);
                }
                daysOfWeekTextBlocks[weekDay].Text = text;
                SetDayEvents(dayEvents, weekDay + positionUnit);
            }
        }



        private void ClearEvents()
        {
            foreach (ItemsControl itemsControl in itemControlListEvents) { itemsControl.Items.Clear(); }
            itemControlListEvents.Clear();
        }

        private void SetLastWeek()
        {
            day = day - twoWeeks;
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + day.ToString(CultureInfo.InvariantCulture);
                dayEvents = Utils.GetDayEvents(calendar, day, month, year, user);
                SetDayEvents(dayEvents, weekDay + positionUnit);
                day++;
            }
            TextBlockMonth.Text = months[month - positionUnit];
            TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
        }

        private void SetDayEvents(AppointmentsList events, int weekDay)
        {
            for (int hour = 0; hour < 24; hour++)
            {
                ItemsControl itemsControlEvents = new ItemsControl();
                itemControlListEvents.Add(itemsControlEvents);
                GridHours.Children.Add(itemsControlEvents);
                Grid.SetColumn(itemsControlEvents, weekDay);
                Grid.SetRow(itemsControlEvents, hour + positionUnit);

                foreach (Appointment appointment in events.Appointments)
                {
                    TextBlock textBlockEvent = new TextBlock();
                    textBlockEvent.Text = appointment.WeekViewAppointmentText(hour);
                    itemsControlEvents.Items.Add(textBlockEvent);
                }
            }
        }
        #endregion
    }
}
