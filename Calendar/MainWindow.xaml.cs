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
        #region Constants
        private string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        private const int positionUnit = 1;
        private const int firstDayOfMonth = 1;
        private const int sundayFirstPosition = 0;
        private const int sundayLastPosition = 7;
        private const int firstMonth = 1;
        private const int lastMonth = 12;
        private const int hour = 0;
        private const int minute = 1;
        private Utils util = new Utils();
        #endregion

        #region Fields
        private int month;
        private int year;
        private User user;
        private EventsList calendar;
        private EventsList monthEvents;
        private List<ItemsControl> listItemsControlEvents;
        private TextBlock[] textBlocksDaysOfMonth;
        #endregion

        #region Methods
        private void GetMonthEvents()
        {
            if (calendar != null)
            {
                monthEvents = new EventsList();
                foreach (Event appointment in calendar.Events)
                {
                    bool appointmentIsThisMonth = appointment.Date.Month == month && appointment.Date.Year == year;
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
                    if (appointmentIsThisMonth && isThisUserAppointment)
                    {
                        monthEvents.Events.Add(appointment);
                    }
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

        private void SetEventsText(Event appointment, int day, ItemsControl itemsControlEvents)
        {
            if (appointment.Date.Day == day)
            {
                TextBlock textBlockEvent = new TextBlock();
                string start = String.Format(format: "{0}:{1}", arg0: appointment.Start[hour], arg1: appointment.Start[minute]);
                string end = String.Format("{0}:{1}", appointment.End[hour], appointment.End[minute]);
                string name = appointment.Name;
                textBlockEvent.Text = String.Format("{0}({1} - {2})", name, start, end);   
                itemsControlEvents.Items.Add(textBlockEvent);
            }
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
                foreach (Event appointment in monthEvents.Events)
                {
                    SetEventsText(appointment, day, itemsControlEvents);
                }
            }

        }

        private void SetDaysOfMonth()
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
            if (firstWeekDay == sundayFirstPosition)
            {
                firstWeekDay = sundayLastPosition;
            }

            int lastDay = DateTime.DaysInMonth(year, month);
            int day = firstDayOfMonth;

            for (int dayIndex = 0; dayIndex < textBlocksDaysOfMonth.Length; dayIndex++)
            {
                bool monthHasNotStarted = dayIndex < firstWeekDay - positionUnit;
                bool monthEnded = day > lastDay;
                if ( monthHasNotStarted || monthEnded)
                {
                    textBlocksDaysOfMonth[dayIndex].Text = "";
                }
                else
                {
                    textBlocksDaysOfMonth[dayIndex].Text = day.ToString();
                    SetDayEvents(day, dayIndex);
                    day++;
                }
            }
        }

        private void LastMonthBtn_Click(Object sender, EventArgs e)
        {
            month--;
            if (month < firstMonth)
            {
                year--;
                month = lastMonth;
            }

            SetDaysOfMonth();
        }

        private void NextMonthBtn_Click(Object sender, EventArgs e)
        {
            month++;
            if (month > lastMonth)
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
            calendar = util.ReadEventsSerialFile();
            SetDaysOfMonth();
        }

        public void MyEventsBtn_Click(Object sender, EventArgs e)
        {
            MyEvents myEvents = new MyEvents(user);
            myEvents.ShowDialog();
            calendar = util.ReadEventsSerialFile();
            SetDaysOfMonth();
        }

        public MainWindow(int passedMonth, int passedYear, User passedUser)
        {
            calendar = util.ReadEventsSerialFile();
            InitializeComponent();
            year = passedYear;
            month = passedMonth;
            user = passedUser;
            buttonLastMonth.Click += new RoutedEventHandler(LastMonthBtn_Click);
            buttonNextMonth.Click += new RoutedEventHandler(NextMonthBtn_Click);
            buttonWeeklyView.Click += new RoutedEventHandler(WeeklyViewBtn_Click);
            buttonNewEvent.Click += new RoutedEventHandler(NewEventBtn_Click);
            buttonEvents.Click += new RoutedEventHandler(MyEventsBtn_Click);
            
            textBlockUserName.Text = string.Format("Hola, {0}", user.Name);

            textBlocksDaysOfMonth = new TextBlock[] {this.TextBlockDay1, this.TextBlockDay2, this.TextBlockDay3, this.TextBlockDay4, this.TextBlockDay5, this.TextBlockDay6, this.TextBlockDay7,
                    this.TextBlockDay8, this.TextBlockDay9, this.TextBlockDay10, this.TextBlockDay11, this.TextBlockDay12, this.TextBlockDay13, this.TextBlockDay14,
                    this.TextBlockDay15, this.TextBlockDay16, this.TextBlockDay17, this.TextBlockDay18, this.TextBlockDay19, this.TextBlockDay20, this.TextBlockDay21,
                    this.TextBlockDay22, this.TextBlockDay23, this.TextBlockDay24, this.TextBlockDay25, this.TextBlockDay26, this.TextBlockDay27, this.TextBlockDay28,
                    this.TextBlockDay29, this.TextBlockDay30, this.TextBlockDay31, this.TextBlockDay32, this.TextBlockDay33, this.TextBlockDay34, this.TextBlockDay35,
                    this.TextBlockDay36, this.TextBlockDay37, this.TextBlockDay38, this.TextBlockDay39, this.TextBlockDay40, this.TextBlockDay41, this.TextBlockDay42};

            listItemsControlEvents = new List<ItemsControl>();

            SetDaysOfMonth();
        }
        #endregion
    }
}
