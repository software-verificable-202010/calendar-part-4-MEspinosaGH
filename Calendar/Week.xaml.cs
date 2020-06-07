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
        private const int sundayFirstPosition = 0;
        private const int sundayLastPosition = 7;
        private const int january = 1;
        private const int december = 12;
        private const int oneMonth = 1;
        private const int oneDay = 1;
        private const int hourIndex = 0;
        private const int minuteIndex = 1;
        private const int weekLength = 7;
        private const int twoWeeks = 14;
        private const int oneHour = 1;
        private const int minuteZero = 0;
        private const string defaultUserName = "User";
        Utils util = new Utils();
        private string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        private string[] daysOfWeek = new string[] { "Lunes ", "Martes ", "Miércoles ", "Jueves ", "Viernes ", "Sábado ", "Domingo " };
        #endregion

        #region Fields
        private EventsList calendar;
        private int month;
        private int year;
        private int day;
        private List<ItemsControl> itemControlListEvents;
        private TextBlock[] daysOfWeekTextBlocks;
        private User user;
        #endregion

        #region Methods
        private void GetDayEvents(int year, int month, int day, int weekDay)
        {
            if (calendar != null)
            {
                EventsList dayEvents = new EventsList();
                foreach (Event appointment in calendar.Events)
                {
                    int appointmentYear = appointment.Date.Year;
                    int appointmentMonth = appointment.Date.Month;
                    int appointmentDay = appointment.Date.Day;

                    bool appoinmentIsOnActualDay = day == appointmentDay && month == appointmentMonth && year == appointmentYear;
                    bool isThisUserAppointment = appointment.Owner.Name == user.Name;
                    if (appointment.Participants != null)
                    {
                        foreach (User participant in appointment.Participants.Users)
                        {
                            Console.WriteLine(participant.Name);
                            if (participant.Name == user.Name)
                            {
                                isThisUserAppointment = true;
                            }
                        }
                    }
                    if (appoinmentIsOnActualDay && isThisUserAppointment)
                    {
                        dayEvents.Events.Add(appointment);
                    }
                }
                SetDayEvents(dayEvents, weekDay + positionUnit);
            }
        }

        private void SetDayEvents(EventsList events, int weekDay)
        {
            for (int hour = 0; hour < 24; hour++)
            {
                ItemsControl itemsControlEvents = new ItemsControl();
                itemControlListEvents.Add(itemsControlEvents);
                GridHours.Children.Add(itemsControlEvents);
                Grid.SetColumn(itemsControlEvents, weekDay);
                Grid.SetRow(itemsControlEvents, hour + positionUnit);

                foreach (Event appointment in events.Events)
                {
                    int startHour = int.Parse(appointment.Start[hourIndex], NumberFormatInfo.InvariantInfo);
                    int startMinute = int.Parse(appointment.Start[minuteIndex], NumberFormatInfo.InvariantInfo);
                    int endHour = int.Parse(appointment.End[hourIndex], NumberFormatInfo.InvariantInfo);
                    int endMinute = int.Parse(appointment.End[minuteIndex], NumberFormatInfo.InvariantInfo);
                    TextBlock textBlockEvent = new TextBlock();
                    bool isStartHour = startHour == hour;
                    bool isEndHour = endHour == hour && endMinute > minuteZero;
                    bool isLastHour = endHour == hour + oneHour && endMinute == minuteZero;
                    bool isMiddleHour = startHour < hour && endHour > hour;
                    if (isStartHour)
                    {
                        textBlockEvent.Text = String.Format(CultureInfo.InvariantCulture, "{0} - Desde {1}:{2}", appointment.Name, appointment.Start[hourIndex], appointment.Start[minuteIndex]);
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                    else if (isEndHour)
                    {
                        textBlockEvent.Text = String.Format(CultureInfo.InvariantCulture, "{0} - Hasta {1}:{2}", appointment.Name, appointment.End[hourIndex], appointment.End[minuteIndex]);
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                    else if (isLastHour)
                    {
                        textBlockEvent.Text = String.Format(CultureInfo.InvariantCulture, "{0} - Hasta {1}:{2}", appointment.Name, appointment.End[hourIndex], appointment.End[minuteIndex]);
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                    else if (isMiddleHour)
                    {
                        textBlockEvent.Text = appointment.Name;
                        itemsControlEvents.Items.Add(textBlockEvent);
                    }
                }
            }
        }

        private void ClearEvents()
        {
            foreach (ItemsControl itemsControl in itemControlListEvents) { itemsControl.Items.Clear(); }
            itemControlListEvents.Clear();
        }

        private void SetCalendarDays(int firstWeekdayOfMonth, int lastDayOfLastMonth, int lastMonthYear, int lastMonth)
        {
            for (int weekDay = 0; weekDay < daysOfWeekTextBlocks.Length; weekDay++)
            {
                bool isThisMonth = firstWeekdayOfMonth <= weekDay + positionUnit;
                if (isThisMonth)
                {
                    daysOfWeekTextBlocks[weekDay].Text = String.Format(CultureInfo.InvariantCulture, "{0} {1}", daysOfWeek[weekDay], day);

                    GetDayEvents(year, month, day, weekDay);

                    day++;
                }
                else
                {
                    int lastMonthDay = lastDayOfLastMonth - (firstWeekdayOfMonth - oneDay) + weekDay + positionUnit;
                    daysOfWeekTextBlocks[weekDay].Text = String.Format(CultureInfo.InvariantCulture, "{0}{1}", daysOfWeek[weekDay], lastMonthDay);

                    GetDayEvents(lastMonthYear, lastMonth, lastMonthDay, weekDay);
                }
            }
        }

        private void SetFirstWeek()
        {
            ClearEvents();

            DateTime firstDayDate = new DateTime(year, month, firstDayOfMonth);

            int lastMonth = month - oneMonth;
            int lastMonthYear = year;
            bool isLastYear = lastMonth < january;
            if (isLastYear)
            {
                lastMonth = december;
                lastMonthYear--;
            }
            int lastDayOfLastMonth = DateTime.DaysInMonth(lastMonthYear, lastMonth);

            int firstWeekdayOfMonth = (int)firstDayDate.DayOfWeek;

            bool startsOnSunday = firstWeekdayOfMonth == sundayFirstPosition;
            if (startsOnSunday)
            {
                firstWeekdayOfMonth = sundayLastPosition;
            }

            day = firstDayOfMonth;

            SetCalendarDays(firstWeekdayOfMonth, lastDayOfLastMonth, lastMonthYear, lastMonth);

            bool hasDifferentYears = lastMonthYear != year;
            bool hasDifferentMonths = firstWeekdayOfMonth > 1;
            if (hasDifferentYears)
            {
                TextBlockYear.Text = String.Format(CultureInfo.InvariantCulture, "{0} / {1}", lastMonthYear, year);
            }
            else
            {
                TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
            }
            if (hasDifferentMonths)
            {
                TextBlockMonth.Text = String.Format(CultureInfo.InvariantCulture, "{0} / {1}", months[lastMonth - positionUnit], months[month - positionUnit]);
            }
            else
            {
                TextBlockMonth.Text = months[month - positionUnit];
            }
        }

        private void SetNextWeekMonthAndYear(bool changeOfMonth)
        {
            if (changeOfMonth)
            {
                int nextMonth = month + oneMonth;
                int nextMonthYear = year;

                bool hasToChangeYear = nextMonth > december;
                bool isNextMonth = day >= weekLength;
                bool isNextYear = year != nextMonthYear;
                if (hasToChangeYear)
                {
                    nextMonth = january;
                    nextMonthYear++;
                }
                TextBlockMonth.Text = String.Format(CultureInfo.InvariantCulture, "{0} / {1}", months[month - positionUnit], months[nextMonth - positionUnit]);
                if (isNextMonth)
                {
                    TextBlockMonth.Text = months[nextMonth - positionUnit];
                }
                if (isNextYear)
                {
                    TextBlockYear.Text = String.Format(CultureInfo.InvariantCulture, "{0} / {1}", year.ToString(CultureInfo.InvariantCulture), nextMonthYear.ToString(CultureInfo.InvariantCulture));
                    if (isNextMonth)
                    {
                        TextBlockYear.Text = nextMonthYear.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
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
                    GetDayEvents(year, month + oneMonth, day, weekDay);
                }
                else
                {
                    GetDayEvents(year, month, day, weekDay);
                }
                daysOfWeekTextBlocks[weekDay].Text = String.Format(CultureInfo.InvariantCulture, "{0}{1}", daysOfWeek[weekDay], day);
                day++;
            }
            SetNextWeekMonthAndYear(changeOfMonth);
        }

        private void SetLastMonth()
        {
            month--;
            bool isLastYear = month < january;
            if (isLastYear)
            {
                month = december;
                year--;
            }
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            day = lastDayOfMonth + (day - twoWeeks);
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                daysOfWeekTextBlocks[weekDay].Text = String.Format(CultureInfo.InvariantCulture, "{0}{1}", daysOfWeek[weekDay], day);
                GetDayEvents(year, month, day, weekDay);
                day++;
            }
            TextBlockMonth.Text = months[month - positionUnit];
            TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
        }

        private void SetLastWeek()
        {
            day = day - twoWeeks;
            for (int weekDay = 0; weekDay < weekLength; weekDay++)
            {
                daysOfWeekTextBlocks[weekDay].Text = daysOfWeek[weekDay] + day.ToString(CultureInfo.InvariantCulture);
                GetDayEvents(year, month, day, weekDay);
                day++;
            }
            TextBlockMonth.Text = months[month - positionUnit];
            TextBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);
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
            calendar = util.ReadEventsSerialFile();
            Week newWeekView = new Week(month, year, user);
            newWeekView.Show();
            this.Close();
        }

        private void buttonEvents_Click(object sender, RoutedEventArgs e)
        {
            MyEvents myEvents = new MyEvents(user);
            myEvents.ShowDialog();
            calendar = util.ReadEventsSerialFile();
            Week newWeekView = new Week(month, year, user);
            newWeekView.Show();
            this.Close();
        }

        public Week(int passedMonth, int passedYear, User passedUser)
        {
            month = passedMonth;
            year = passedYear;
            if (passedUser == null)
            {
                passedUser = new User(defaultUserName);
            }
            user = passedUser;

            calendar = util.ReadEventsSerialFile();

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
        #endregion
    }
}
