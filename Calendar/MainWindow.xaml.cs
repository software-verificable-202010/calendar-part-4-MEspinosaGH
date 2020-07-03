using System;
using System.Globalization;
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
        #region Constants
        private const int positionUnit = 1;
        private const int firstDayOfMonth = 1;
        private const int firstMonth = 1;
        private const int lastMonth = 12;
        private const string defaultUserName = "User";
        #endregion

        #region Fields
        private string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        private int month;
        private int year;
        private User user;
        private AppointmentsList calendar;
        private AppointmentsList monthEvents;
        private List<ItemsControl> listItemsControlEvents;
        private TextBlock[] textBlocksDaysOfMonth;
        #endregion

        #region Methods

        public MainWindow(int passedMonth, int passedYear, User passedUser)
        {
            calendar = Utils.ReadEventsSerialFile();
            InitializeComponent();
            year = passedYear;
            month = passedMonth;
            if (passedUser == null)
            {
                passedUser = new User(defaultUserName);
            }
            user = passedUser;
            buttonLastMonth.Click += new RoutedEventHandler(LastMonthBtn_Click);
            buttonNextMonth.Click += new RoutedEventHandler(NextMonthBtn_Click);
            buttonWeeklyView.Click += new RoutedEventHandler(WeeklyViewBtn_Click);
            buttonNewEvent.Click += new RoutedEventHandler(NewEventBtn_Click);

            textBlockUserName.Text = string.Format(CultureInfo.InvariantCulture, "Hola, {0}", user.Name);

            textBlocksDaysOfMonth = new TextBlock[] {this.TextBlockDay1, this.TextBlockDay2, this.TextBlockDay3, this.TextBlockDay4, this.TextBlockDay5, this.TextBlockDay6, this.TextBlockDay7,
                    this.TextBlockDay8, this.TextBlockDay9, this.TextBlockDay10, this.TextBlockDay11, this.TextBlockDay12, this.TextBlockDay13, this.TextBlockDay14,
                    this.TextBlockDay15, this.TextBlockDay16, this.TextBlockDay17, this.TextBlockDay18, this.TextBlockDay19, this.TextBlockDay20, this.TextBlockDay21,
                    this.TextBlockDay22, this.TextBlockDay23, this.TextBlockDay24, this.TextBlockDay25, this.TextBlockDay26, this.TextBlockDay27, this.TextBlockDay28,
                    this.TextBlockDay29, this.TextBlockDay30, this.TextBlockDay31, this.TextBlockDay32, this.TextBlockDay33, this.TextBlockDay34, this.TextBlockDay35,
                    this.TextBlockDay36, this.TextBlockDay37, this.TextBlockDay38, this.TextBlockDay39, this.TextBlockDay40, this.TextBlockDay41, this.TextBlockDay42};

            listItemsControlEvents = new List<ItemsControl>();

            SetDaysOfMonth();
        }
        private void LastMonthBtn_Click(Object sender, EventArgs e)
        {
            month--;
            if (Utils.IsLastYear(month))
            {
                year--;
                month = lastMonth;
            }
            SetDaysOfMonth();
        }

        private void NextMonthBtn_Click(Object sender, EventArgs e)
        {
            month++;
            if (Utils.IsNextYear(month))
            {
                year++;
                month = firstMonth;
            }

            SetDaysOfMonth();
        }

        private void WeeklyViewBtn_Click(Object sender, EventArgs e)
        {
            Week weekWindow = new Week(month, year, user);
            weekWindow.Show();
            this.Close();
        }

        private void NewEventBtn_Click(Object sender, EventArgs e)
        {
            NewEvent newEvent = new NewEvent(user);
            newEvent.ShowDialog();
            calendar = Utils.ReadEventsSerialFile();
            SetDaysOfMonth();
        }

        private void ButtonEvents_Click(object sender, RoutedEventArgs e)
        {
            MyEvents myEvents = new MyEvents(user);
            myEvents.ShowDialog();
            calendar = Utils.ReadEventsSerialFile();
            SetDaysOfMonth();
        }

        private void SetDaysOfMonth()
        {
            if (month.Equals(null) || year.Equals(null))
            {
                month = DateTime.Now.Month;
                year = DateTime.Now.Year;
            }

            ClearEvents();
            monthEvents = Utils.GetMonthEvents(calendar, month, year, user);

            textBlockMonth.Text = months[month - positionUnit];
            textBlockYear.Text = year.ToString(CultureInfo.InvariantCulture);

            DateTime firstDayOfMonthDate = new DateTime(year, month, firstDayOfMonth);

            int firstWeekDay = Utils.SetFirstWeekDay((int)firstDayOfMonthDate.DayOfWeek);

            int lastDay = DateTime.DaysInMonth(year, month);
            int day = firstDayOfMonth;

            for (int dayIndex = 0; dayIndex < textBlocksDaysOfMonth.Length; dayIndex++)
            {
                if (Utils.IsNotMonthDay(dayIndex, firstWeekDay, day, lastDay))
                {
                    textBlocksDaysOfMonth[dayIndex].Text = "";
                }
                else
                {
                    textBlocksDaysOfMonth[dayIndex].Text = day.ToString(CultureInfo.InvariantCulture);
                    SetDayEvents(day, dayIndex);
                    day++;
                }
            }
        }

        private void ClearEvents()
        {
            foreach (ItemsControl itemsControl in listItemsControlEvents)
            {
                itemsControl.Items.Clear();
            }
            listItemsControlEvents.Clear();
        }

        private void SetDayEvents(int day, int dayIndex)
        {
            if (monthEvents != null)
            {
                ItemsControl itemsControlEvents = new ItemsControl();
                listItemsControlEvents.Add(itemsControlEvents);
                gridMonth.Children.Add(itemsControlEvents);
                Grid.SetColumn(itemsControlEvents, Grid.GetColumn(textBlocksDaysOfMonth[dayIndex]));
                Grid.SetRow(itemsControlEvents, Grid.GetRow(textBlocksDaysOfMonth[dayIndex]));
                itemsControlEvents.HorizontalAlignment = HorizontalAlignment.Left;
                itemsControlEvents.VerticalAlignment = VerticalAlignment.Bottom;
                itemsControlEvents.Margin = new Thickness(5, 5, 5, 5);
                foreach (Appointment appointment in monthEvents.Appointments)
                {
                    SetEventsText(appointment, day, itemsControlEvents);
                }
            }

        }

        private static void SetEventsText(Appointment appointment, int day, ItemsControl itemsControlEvents)
        {
            if (appointment.Date.Day == day)
            {
                TextBlock textBlockEvent = new TextBlock();
                textBlockEvent.Text = appointment.MonthViewAppointmentText();   
                itemsControlEvents.Items.Add(textBlockEvent);
            }
        }

        #endregion
    }
}
