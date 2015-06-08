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

namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BanList = new List<Users>();
        }

        IConnections Connection;
        string constr = "Data Source=PC;Initial Catalog=Programowanie;Integrated Security=True";
        List<Users> BanList;


        public void Check(object sender=null, RoutedEventArgs e=null)
        {
            if ((bool)AdoButton.IsChecked)
                Connection = new Ado(constr);
            else if ((bool)LinQButton.IsChecked)
                Connection = new LinQ();
            else
                Connection = new Entity();

            Connection.Connect();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var box = (sender as TextBox);

            if ( (box.Text == "User") || (box.Text == "Password") )
                box.Text = null;

        }


        private void User_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (sender as TextBox);

            if (box.Text == "")
                box.Text = "User";           
        }

        private void Passwd_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (sender as TextBox);

            if (box.Text == "")
                box.Text = "Password";  
        }

        private void ListUsers(object sender, RoutedEventArgs e)
        {
            Check();

            OutputList.Items.Clear();
            foreach (var user in Connection.ListUsers())
            {
                var cb = new CheckBox()
                {
                    Content = (user.User + " " + user.Password),
                    IsChecked = user.IsBanned,                  
                    Foreground = new SolidColorBrush(Color.FromRgb(207, 212, 207))
                };
                cb.Click += cb_Click;
                cb.MouseRightButtonDown += cb_Click;
                cb.Checked += cb_Checked;
                cb.Unchecked += cb_Checked;

                OutputList.Items.Add(cb);

                BanList.Clear();
            }
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (sender as CheckBox);
            BanList.Add(new Users() { User = cb.Content.ToString().Split(' ')[0], IsBanned = (bool)cb.IsChecked });
            if ((bool)Autoban.IsChecked)
            {
                Connection.ApplyBans(BanList);
                BanList.Clear();
            }
        }

        void cb_Click(object sender, RoutedEventArgs e)
        {
            var cb = (sender as CheckBox);
            var tmp = cb.Content.ToString().Split(' ');
            UserBox.Text = tmp[0];
            PasswdBox.Text = tmp[1];
        }

        private void AddUserButton(object sender, RoutedEventArgs e)
        {
            Connection.AddUser(UserBox.Text, PasswdBox.Text);
            ListUsers(sender, null);
        }

        private void DeleteUserButton(object sender, RoutedEventArgs e)
        {
            Connection.DelUser(UserBox.Text);
            ListUsers(sender, null);
        }

        private void ApplyBansButton(object sender, RoutedEventArgs e)
        {
            Connection.ApplyBans(BanList);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            Application.Current.MainWindow.DragMove();
                
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateTableButton(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection = new Ado(constr);
                Connection.Connect();
                (Connection as Ado).CreateTable();
            }
            catch(Exception)
            {
                MessageBox.Show("Baza nie mogła zostać utowrzona.");
            }
        }

    


    }
}
