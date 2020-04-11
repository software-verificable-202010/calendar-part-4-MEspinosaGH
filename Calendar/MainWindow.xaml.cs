using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        int month = DateTime.Now.Month;
        int year = DateTime.Now.Year;
        string[] months = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        TextBlock[] daysOfMonth; 

        public MainWindow()
        {
            void SetDaysOfMonth()
            {
                TextBlockMonth.Text = months[month - 1];
                TextBlockYear.Text = year.ToString();

                DateTime firstDayDate = new DateTime(year, month, 1);

                int firstWeekDay = (int)firstDayDate.DayOfWeek;

                int lastDay = DateTime.DaysInMonth(year, month);

                int day = 1;

                for (int i = 0; i< daysOfMonth.Length; i++) 
                {
                    if (i < firstWeekDay - 1 || day > lastDay)
                    {
                        daysOfMonth[i].Text = "";
                    }
                    else
                    {
                        daysOfMonth[i].Text = day.ToString();
                        day++;
                    }
                }

            }

            void LastMonthBtn_Click(Object sender, EventArgs e)
            {
                month--;
                if (month < 1)
                {
                    year--;
                    month = 12;
                }

                SetDaysOfMonth();
            }

            void NextMonthBtn_Click(Object sender, EventArgs e)
            {
                month++;
                if (month > 12)
                {
                    year++;
                    month = 1;
                }

                SetDaysOfMonth();
            }

            InitializeComponent();

            ButtonLastMonth.Click += new RoutedEventHandler(LastMonthBtn_Click);
            ButtonNextMonth.Click += new RoutedEventHandler(NextMonthBtn_Click);

            daysOfMonth = new TextBlock[] {this.TextBlockDay1, this.TextBlockDay2, this.TextBlockDay3, this.TextBlockDay4, this.TextBlockDay5, this.TextBlockDay6, this.TextBlockDay7,
                    this.TextBlockDay8, this.TextBlockDay9, this.TextBlockDay10, this.TextBlockDay11, this.TextBlockDay12, this.TextBlockDay13, this.TextBlockDay14,
                    this.TextBlockDay15, this.TextBlockDay16, this.TextBlockDay17, this.TextBlockDay18, this.TextBlockDay19, this.TextBlockDay20, this.TextBlockDay21,
                    this.TextBlockDay22, this.TextBlockDay23, this.TextBlockDay24, this.TextBlockDay25, this.TextBlockDay26, this.TextBlockDay27, this.TextBlockDay28,
                    this.TextBlockDay29, this.TextBlockDay30, this.TextBlockDay31, this.TextBlockDay32, this.TextBlockDay33, this.TextBlockDay34, this.TextBlockDay35,
                    this.TextBlockDay36, this.TextBlockDay37, this.TextBlockDay38, this.TextBlockDay39, this.TextBlockDay40, this.TextBlockDay41, this.TextBlockDay42};


            var currentYear = DateTime.Now.Year.ToString();
            var currentMonth = DateTime.Now.ToString("MMMM");
;

            SetDaysOfMonth();
        }
    }
}
