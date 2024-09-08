using Notification.Wpf;
using Notification.Wpf.Classes;
using Notifications.Wpf.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Calendar_app
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;

            string connectionString = "Data Source=EventsinList.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = "CREATE TABLE IF NOT EXISTS EventsTable (Name TEXT, Date TEXT)";
                using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
                string selectQuery = "SELECT * FROM EventsTable";
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Name: {reader["Name"]}");
                        }
                    }
                }
                string selectQueryTwo = "SELECT * FROM EventsTable";
                using (SQLiteCommand command = new SQLiteCommand(selectQueryTwo, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Name: {reader["Name"]}");
                            events.Items.Add($"{reader["Name"]}");
                        }
                    }
                }
            }

            var dataText = "Data: ";
            DateTime today = DateTime.Today;
            string correctDate = today.ToString("d");
            data.Text = dataText + correctDate;

            var monthText = "Month: ";
            int monthNumber = DateTime.Today.Month;
            string monthName = DateTime.Today.ToString("MMMM");
            month.Text = monthText + monthName + "" + "(" + monthNumber + ")";

            var yearText = "Year: ";
            int yearNum = DateTime.Today.Year;
            year.Text = yearText + yearNum + "(г)";

            var dayText = "WeekDay: ";
            //int day = DateTime.Today.Day;
            string dayName = DateTime.Today.ToString("dddd");
            weekday.Text = dayText  + "(" + dayName + ")";

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_updater;
            timer.Start();

            var noneDate = "Event data: select date!";
            eventDescription.Content = noneDate;

            int daysOfCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            DateTime currentDay = DateTime.Now;
            string dayNow = currentDay.ToString("dd");
            int todayDay = Convert.ToInt32(dayNow);
            monthDaysCounter.Content = dayNow + "/" + daysOfCurrentMonth;
            progressDays.Minimum = 0;
            progressDays.Maximum = daysOfCurrentMonth;
            progressDays.Value = todayDay;

            var timeDay = DateTime.Now;
            string message = "";
            var messageColor = Colors.White;

            if (timeDay.Hour < 12)
            {
                dayTime.Content = "Morning";
                message = "Hello, Good Morning!";
                messageColor = Colors.Yellow;
            }
            else if (timeDay.Hour < 17)
            {
                dayTime.Content = "Afternoon";
                message = "Hello, Good Afternoon!";
                messageColor = Colors.Green;
            }
            else if (timeDay.Hour < 21)
            {
                dayTime.Content = "Evening";
                message = "Hello, Good Evening!";
                messageColor = Colors.Orange;
            }
            else
            {
                dayTime.Content = "Night";
                message = "Hello, Good Night!";
                messageColor = Colors.Blue;
            }

            string eventMessage = "\nCheck today's events so you don't forget anything.";

            var helloNotificationContent = new NotificationContent
            {
                Title = "Calendar app",
                Message = message + eventMessage,
                Type = NotificationType.Information,
                TrimType = NotificationTextTrimType.NoTrim,
                Background = new SolidColorBrush(messageColor),
                Foreground = new SolidColorBrush(Colors.White),
            };
            
            var notificationManager = new NotificationManager();
            notificationManager.Show(helloNotificationContent);

            var gridPanel = new Grid();
            var Background = new SolidColorBrush(Colors.Orange);
            var text_name = new TextBlock { Text = "Calendar app", Margin = new Thickness(0, 10, 0, 0), HorizontalAlignment = HorizontalAlignment.Center };
            var text_block = new TextBlock { Text = "Chek your events.", Margin = new Thickness(0, 10, 0, 0), HorizontalAlignment = HorizontalAlignment.Center };
            var panelBtn = new StackPanel { Height = 100, Margin = new Thickness(0, 40, 0, 0) };
            var btn1 = new Button { Width = 200, Height = 40, Content = "Chek", Background = Background };
            btn1.Click += create_second_window;
            var text = new TextBlock { Foreground = Brushes.White, Text = "Click button to check your events.", Margin = new Thickness(0, 10, 0, 0), HorizontalAlignment = HorizontalAlignment.Center };
            panelBtn.VerticalAlignment = VerticalAlignment.Bottom;
            panelBtn.Children.Add(btn1);
            var row1 = new RowDefinition();
            var row2 = new RowDefinition();
            var row3 = new RowDefinition();
            gridPanel.RowDefinitions.Add(new RowDefinition());
            gridPanel.RowDefinitions.Add(new RowDefinition());
            gridPanel.RowDefinitions.Add(new RowDefinition());
            gridPanel.RowDefinitions.Add(new RowDefinition());
            gridPanel.HorizontalAlignment = HorizontalAlignment.Center;
            gridPanel.Children.Add(text_block);
            gridPanel.Children.Add(text);
            gridPanel.Children.Add(panelBtn);
            gridPanel.Children.Add(text_name);
            Grid.SetRow(panelBtn, 3);
            Grid.SetRow(text_block, 1);
            Grid.SetRow(text_name, 0);
            Grid.SetRow(text, 2);

            object content = gridPanel;
            
            notificationManager.Show(content, null, TimeSpan.MaxValue);
        }

        public void timer_updater(object sender, EventArgs e)
        {
            var timeText = "Time: ";
            time.Text = timeText + DateTime.Now.ToString("HH:mm:ss");
        }

        private void event_date_picker(object sender, EventArgs e)
        {
            var dateText = "Event data: ";
            var isNull = "none data";
            DateTime? selectedDate = eventData.SelectedDate;
            
            if (selectedDate.HasValue)
            {
                string formatedDate = selectedDate.Value.ToString("d");
                eventDescription.Content = dateText + formatedDate;
            } else if (selectedDate == null) 
            {  
                eventDescription.Content = dateText + isNull;
            }
        }

        private void add_event(object sender, EventArgs e)
        {
            try
            {
                DateTime? selectedDate = eventData.SelectedDate;
                string dateInfo = selectedDate.Value.ToString("d");
                DateTime? datePick = eventData.SelectedDate;
                string pickDate = datePick.Value.ToString("d");
                string eventValue = eventText.Text;
                events.Items.Add("Event" + "(" + dateInfo + ")" + eventValue);
                //string valueEvent = "Event" + "(" + dateInfo + ")" + eventValue;
                string connectionString = "Data Source=EventsinList.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO EventsTable (Name, Date) VALUES (@name, @date)";
                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        var value = events.Items;
                        foreach (var item in value)
                        {
                            string valueTxt = item.ToString();
                            command.Parameters.AddWithValue("@name", valueTxt);
                            command.Parameters.AddWithValue("@date", pickDate);
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch 
            {
                Console.WriteLine("Error of add event, please try again later.");
                //other actions...
            }
        }

        private void clear_event_text(object sender, EventArgs e)
        {
            eventText.Text = "";
        }

        public void create_second_window(object sender, EventArgs e)
        {
            SecondWindow window = new SecondWindow();
            window.Show();            
        }

    }
}
