using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    class Users
    {
        public string User { get; set; }
        public string Password { get; set; }
        public bool IsBanned { get; set; }
    }
    interface IConnections
    {
        void Connect();
        void AddUser(string user, string password);
        void DelUser(string user);
        List<Users> ListUsers();
        void ApplyBans(List<Users> users);
    }

    class Ado : IConnections
    {

        private string connectionString { get; set; }

        SqlConnection con;
        string cmd;
        SqlCommand com;

        public Ado(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }
        public void Connect()
        {
            con = new SqlConnection(connectionString);
            con.Open();
        }

        public void AddUser(string user, string password)
        {
            cmd = "INSERT INTO Users([user], [password]) VALUES (@user, @pass)";
            com = new SqlCommand(cmd, con);
            com.Parameters.AddWithValue("@user", user);
            com.Parameters.AddWithValue("@pass", password);

            com.ExecuteNonQuery();
        }

        public void DelUser(string user)
        {
            cmd = "DELETE FROM Users WHERE [user] = @user";
            com = new SqlCommand(cmd, con);
            com.Parameters.AddWithValue("@user", user);

            com.ExecuteNonQuery();
        }


        public List<Users> ListUsers()
        {    
            cmd = "SELECT [User], [password], isBanned FROM Users";
            com = new SqlCommand(cmd, con);

            List<Users> tmp = new List<Users>();

            using (SqlDataReader sr = com.ExecuteReader())
            {
                while (sr.Read())
                {
                    tmp.Add(new Users() { User = (string)sr[0], Password = (string)sr[1], IsBanned = (bool)sr[2] });                  
                }
            }

            return tmp;            
            
        }

        public void ApplyBans(List<Users> users)
        {
            foreach (var user in users)
            {
                cmd = "UPDATE Users SET isBanned=@ban WHERE [User]=@user";
                com = new SqlCommand(cmd, con);
                com.Parameters.AddWithValue("@ban", user.IsBanned);
                com.Parameters.AddWithValue("@user", user.User);

                com.ExecuteNonQuery();
            } 	        
        }
}


    class LinQ : IConnections
    {
        LinQDataDataContext con;

        public void Connect()
        {
            con = new LinQDataDataContext();
        }

        public void AddUser(string user, string password)
        {
            UsersTab T = new UsersTab()
            {
                user1 = user,
                password = password,
                isBanned = false
            };

            con.UsersTabs.InsertOnSubmit(T);

            con.SubmitChanges();
        }

        public void DelUser(string user)
        {
            var todelete = from x in con.UsersTabs where x.user1 == user select x;
            foreach (var _user in todelete)
            {
                con.UsersTabs.DeleteOnSubmit(_user);
            }

            con.SubmitChanges();
        }

        public List<Users> ListUsers()
        {
            var users = new List<Users>();
            foreach (var user in con.UsersTabs)
            {
                users.Add(new Users() { User = user.user1, Password = user.password, IsBanned = (bool)user.isBanned });
            }
            return users;
        }

        public void ApplyBans(List<Users> users)
        {
            foreach (var user in users)
            {
                var changes = from x in con.UsersTabs where x.user1 == user.User select x;
                foreach (var item in changes)
                {
                    item.isBanned = user.IsBanned;
                }

            }

            con.SubmitChanges();
        }
    }

    class Entity : IConnections
    {
        ProgramowanieEntities con;
        public void Connect()
        {
            con = new ProgramowanieEntities();
        }

        public void AddUser(string user, string password)
        {
            UsersEntityTab t = new UsersEntityTab()
            {
                user = user,
                password = password,
                isBanned = false
            };

            con.Users.Add(t);
            con.SaveChanges();
        }

        public void DelUser(string user)
        {
            var todelete = from x in con.Users where x.user == user select x;
            foreach (var _user in todelete)
            {
                con.Users.Remove(_user);
            }

            con.SaveChanges();

        }

        public List<Users> ListUsers()
        {
            var users = new List<Users>();
            foreach (var user in con.Users)
            {
                users.Add(new Users() { User = user.user, Password = user.password, IsBanned = (bool)user.isBanned });
            }
            return users;
        }

        public void ApplyBans(List<Users> users)
        {
            foreach (var user in users)
            {
                var changes = from x in con.Users where x.user == user.User select x;
                foreach (var item in changes)
                {
                    item.isBanned = user.IsBanned;
                }

                con.SaveChanges();
            }
        }
    }

   
}
