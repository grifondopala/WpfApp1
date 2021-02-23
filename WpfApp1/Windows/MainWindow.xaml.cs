using Microsoft.Win32;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfApp1.DataModel;

namespace WpfApp1.Windows
{

    public class WrittenUser
    {
        public int UserID { set; get; }    
        public string Login { set; get; }
        public Message LastMessage { set; get; }
    }
	
    class SendingTimeComparer : IComparer<WrittenUser>
    {
        public int Compare(WrittenUser user1, WrittenUser user2)
        {
            if (user1.LastMessage.SendingTime > user2.LastMessage.SendingTime)
            {
                return 1;
            }
            else if (user1.LastMessage.SendingTime < user2.LastMessage.SendingTime)
            {
                return -1;
            }
            return 0;
        }
    }

    public partial class MainWindow : Window
    {
        UserContext db = new UserContext();
        string connectionString = @"Data source=DESKTOP-6EQ0CT6;Initial Catalog=userstore;Integrated Security=True";

        private int ID;

        private List<WrittenUser> WrittenUsers;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey WpfName = Registry.CurrentUser.OpenSubKey(@"WpfName");
            ID = Int32.Parse(WpfName.GetValue("ID").ToString());

            WrittenUsers = CreateWrittenList();
            int index = 0;
            foreach(WrittenUser user in WrittenUsers)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(60, GridUnitType.Pixel);
                Messages.RowDefinitions.Add(rd);
                Button LastMessage = CreateLastMessage(user, index);
                Messages.Children.Add(LastMessage);
                index++;
            }
        }

        private List<WrittenUser> CreateWrittenList()
        {
            List<WrittenUser> List = new List<WrittenUser> { };
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = $"SELECT * FROM sys.sysobjects WHERE name LIKE '%{ID}%' and name LIKE '%dialogue%'";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string dialogueName = reader.GetValue(0).ToString();
                        int id = Int32.Parse(dialogueName.Replace("dialogue", "").Replace("_", "").Replace($"{ID}", ""));
                        List<Message> LastMessage = db.Database.SqlQuery<Message>($"SELECT TOP 1 * FROM {dialogueName} ORDER BY ID DESC").ToList();
                        string login = db.Users.FirstOrDefault(u => u.ID == id).Login;
                        WrittenUser user = new WrittenUser { UserID = id, Login = login, LastMessage = LastMessage[0]};
                        List.Add(user);
                    }
                }
                reader.Close();
            }
            SendingTimeComparer stc = new SendingTimeComparer();
            List.Sort(stc);
            return List;
        }

        private Button CreateLastMessage(WrittenUser user, int index)
        {
            Button button = new Button();
            Grid.SetRow(button, index);
            Grid.SetColumn(button, 0);
            button.Style = (Style)this.FindResource("LastMessage");
            //Создание buttonGrid

            Grid buttonGrid = new Grid();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            buttonGrid.RowDefinitions.Add(row1);
            buttonGrid.RowDefinitions.Add(row2);

            //Создание логина отправителя

            TextBlock login = new TextBlock();
            login.Text = user.Login;
            Grid.SetRow(login, 0);
            Grid.SetColumn(login, 0);
            login.Style = (Style)this.FindResource("TextStyle");
            buttonGrid.Children.Add(login);

            //Создание последнего сообщения

            TextBlock lastMessage = new TextBlock();
            lastMessage.Text = user.LastMessage.IDSender == ID ? "You: " + user.LastMessage.TextMessage : user.LastMessage.TextMessage; ;
            Grid.SetRow(lastMessage, 1);
            Grid.SetColumn(lastMessage, 0);
            lastMessage.Style = (Style)this.FindResource("TextStyle");
            buttonGrid.Children.Add(lastMessage);
            //

            button.Content = buttonGrid;
            button.Tag = user.UserID;
            return button;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string searchText = SearchBox.Text;
            }
        }
    }
}
