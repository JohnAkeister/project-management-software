using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;

namespace ConsoleApp1
{
    class Program
    {
        static Validation validation = new Validation();
        static Display display = new Display();
        static Login login = new Login();
        static Admin admin = new Admin();
        static string usertype;
        static int maxusermenu = 5;
        static int maxadminmenu = 5;
        static void Main()
        {
            BeginProgram();                    
        }
        public static void BeginProgram()
        {      
            usertype = login.BeginLogin();
            int ans = 0;
            bool exit = false;
            int adminmenu = 0;
            while (exit == false)
            {
                if (usertype.ToLower() == "user")
                {
                    DisplayUserMenu();
                    ans = validation.CheckIntString("Enter your choice between 1 and " + maxusermenu + ": ", 1, maxusermenu);
                }
                else if (usertype.ToLower() == "admin")
                {
                    DisplayAdminMenu();
                    ans = validation.CheckIntString("Enter your choice between 1 and " + maxusermenu + ": ", 1, maxadminmenu);
                }
                else
                {
                    Console.WriteLine("Error occured. Closing the program");
                }
                switch (ans)
                {
                    case 1:
                        break;
                    case 2:
                        if (usertype == "user")
                        {
                            
                            
                        }
                        else if(usertype == "admin")
                        {                            
                            adminmenu = display.DisplayUsers(); // CHANGE
                            switch (adminmenu)
                            {
                                case 1:
                                    admin.AddUser();
                                    break;
                                case 2:
                                    admin.DeleteUser();
                                    break;
                                case 3:
                                    admin.EditUser();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        if (usertype == "user")
                        {
                            exit = true;
                        }
                        else
                        {
                            exit = true;
                        }
                        break;
                }
            }
        }
        static void DisplayUserMenu()
        {
            Console.WriteLine("User Menu: ");
            Console.WriteLine("\n1) View all Projects\n2) View your Project\n3) View your notifications\n4) Change your password\n5) Exit");
        }
        static void DisplayAdminMenu()
        {
            Console.WriteLine("Admin Menu: ");
            Console.WriteLine("\n1) View all Projects\n2) View all Members\n3) View your notifications\n4) Change your password\n5) Exit");
        }


    }
    class Display
    {
        Validation validation = new Validation();
        public void DisplayNotification()
        {

        }
        public int DisplayUsers() // displays all users to the admin
        {
            using (SqlConnection conn = new SqlConnection()) 
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand command1 = new SqlCommand("SELECT UserID, Username, IsAdmin FROM Logins", conn);
                SqlDataReader reader = command1.ExecuteReader();
                Console.WriteLine("UserID\tUsername\tIsAdmin?");
                while (reader.Read())
                {
                    
                    Console.WriteLine(String.Format("{0}\t | {1}\t | {2}",reader[0], reader[1], reader[2]));
                }
            }
            Console.WriteLine("\nMenu: \n1) Add a new user\n2) Delete a user\n3) Edit a user\n4) Return to main menu");
            int ans = validation.CheckIntString("\nPlease enter your choice:", 1, 3);
            switch (ans)
            {
                case 1:
                    return 1;
                    break;
                case 2:
                    return 2;
                    break;
                case 3:
                    return 3;
                    break;
                default:
                    return 0;
                    break;
            }
        }
    }

    class Validation
    {
        public string readString(string prompt)//checks for null entry
        {
            string result;
            do
            {
                Console.Write(prompt);
                result = Console.ReadLine();
            } while (result == "");
            return result;
        }
        public int ReadInt(string enteredstring) // checks for an integer
        {
            do
            {
                int result;
                bool valid = int.TryParse(enteredstring, out result);
                if (valid)
                {
                    return result;
                }
                else
                {
                    enteredstring = readString("Invalid input! Please enter an integer: ");
                }
            } while (true);
        }
        public int CheckIntString(string prompt, int low, int high) // checks that the entered value is a non-null integer between two specified values
        {
            int result;
            do
            {
                string intString = readString(prompt);
                result = ReadInt(intString);
                if (result < low || result > high)
                {
                    Console.WriteLine("Please enter an integer between " + low + " and " + high);
                }
            } while (result < low || result > high);
            return result;
        }
    }
    class Login
    {
        private Validation validation = new Validation();
        private User user = new User();
        public string BeginLogin()
        {
            int choice = validation.CheckIntString("Would you like to login to an user or admin account: \n1) User\n2) Admin\n", 1, 2);
            switch (choice)
            {
                case 1:
                    UserLogin();
                    return "user";
                    break;
                case 2:
                    UserLogin();
                    return "admin";
                    break;
                default:
                    return "error";
                    break;
            }
            
        }
        private void UserLogin()
        {
            bool valid = false;
            while (!valid)
            {
                string username = validation.readString("\nPlease enter your UserName: ");
                string password = validation.readString("\nPlease enter your Password: ");            
                valid = user.SetUpUser(username, password);
            }
            
        }
        private void AdminLogin()
        { /* Potentially not necessary
            bool valid = false;
            while (!valid)
            {
                string username = validation.readString("\nPlease enter your UserName: ");
                string password = validation.readString("\nPlease enter your Password: ");
                
            }*/
        }
    }
    class User
    {
        private string UserName;    
        private string UserPassword;
        private bool onProject;
        public User()
        {
            this.UserName = "";           
            this.UserPassword = "";
            this.onProject = false;
        }
        public bool SetUpUser(string name, string password)
        {
            this.UserName = name;
            this.UserPassword = password;           
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                try
                {
                    SqlCommand command = new SqlCommand("SELECT * FROM Logins WHERE Username = @0", conn); // extracts from DB when the username is valid
                    command.Parameters.Add(new SqlParameter("0", UserName));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            if (reader[2].ToString() == UserPassword)
                            {
                                Console.WriteLine("Login Successful");
                                Console.WriteLine("UserID\tUsername\tPassword\tIsAdmin?");
                                Console.WriteLine(String.Format("{0}\t | {1}\t | {2}\t | {3}", reader[0], reader[1], reader[2], reader[3]));
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Login Unsuccesful. Please re-enter username and password");
                                return false;
                            }
                        }
                    }
                }
                catch (SqlException er)
                {

                    Console.WriteLine("Error in SQL Server, " + er.Message);
                }
                Console.WriteLine("\nInvalid UserName");
                return false;

            }
        }
    }
    class Admin
    {
        Validation validation = new Validation();
        public void AddUser()
        {
            string result = "";
            int userID;
            Console.WriteLine("Please enter the new user's username");
            string UserName = Console.ReadLine();
            Console.WriteLine("Please enter the new user's password");
            string UserPassword = Console.ReadLine();
            Console.WriteLine("Is the User an admin?(y/n)");
            string IsAdmin = Console.ReadLine();
            IsAdmin = IsAdmin[0].ToString();
            IsAdmin = IsAdmin.ToUpper();
            using (SqlConnection conn = new SqlConnection())
            {
                try
                {
                    conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                    conn.Open();
                    SqlCommand command1 = new SqlCommand("INSERT INTO Logins (UserID,Username,Password,IsAdmin) VALUES(@0,@1,@2,@3)", conn);
                    SqlCommand command2 = new SqlCommand("SELECT Max(UserID) From Logins", conn);
                    userID = Int32.Parse(command2.ExecuteScalar().ToString());
                    userID++;
                    command1.Parameters.Add(new SqlParameter("0", userID));
                    command1.Parameters.Add(new SqlParameter("1", UserName));
                    command1.Parameters.Add(new SqlParameter("2", UserPassword));
                    command1.Parameters.Add(new SqlParameter("3", IsAdmin));
                    Console.WriteLine("New user created, Total rows affected " + command1.ExecuteNonQuery());
                }
                catch (SqlException er)
                {
                    Console.WriteLine("Error with: " + er.Message);
                    
                }
                               
            }
        }
        public void DeleteUser()
        {
            Console.WriteLine("Please enter the ID of the user you wish to remove: ");
            int ans = validation.ReadInt(Console.ReadLine());
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand deletecommand = new SqlCommand("DELETE FROM Logins WHERE UserID = @1",conn);
                SqlCommand readvalue = new SqlCommand("SELECT * FROM Logins WHERE UserID = @1",conn);
                readvalue.Parameters.Add(new SqlParameter("1", ans));
                deletecommand.Parameters.Add(new SqlParameter("1", ans));
                using (SqlDataReader reader = readvalue.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Are you sure you want to delete " + reader[1].ToString() + " from the system?");
                    }                   
                }
                string reply = validation.readString("Y/N: ");
                if (reply.ToLower() == "y")
                {
                    deletecommand.ExecuteNonQuery();
                    Console.WriteLine("User Deleted, returning to main menu");
                }
                else
                {
                    Console.WriteLine("User has not been deleted, returning to main menu");
                }
            }
        }
        public void EditUser()
        {
            
                Console.WriteLine("Please enter the ID of the user you wish to edit: ");
                string reply = Console.ReadLine();
                Console.WriteLine("Which category would you like to edit: \n1) Username\n2) Password\n3) IsAdmin");
                int ans = validation.CheckIntString("Please choose between 1 and 4",1,4);
                switch (ans)
                {
                    case 1:
                    UpdateName(reply);
                        break;
                    case 2:
                    UpdatePassword(reply);
                        break;
                    case 3:
                    UpdateAdmin(reply);
                        break;
                }

            
            
        }
        private void UpdateName(string UserID)
        {
            string newname = validation.readString("Please enter the new username: ");
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                SqlCommand command = new SqlCommand("Update Logins SET Username = @0 WHERE UserID = @1 ", conn);
                command.Parameters.Add(new SqlParameter("0", newname));
                command.Parameters.Add(new SqlParameter("1", UserID));
                conn.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("Username updated for UserID: " + UserID);
            }
        }
        private void UpdatePassword(string UserID)
        {
            string newpass = validation.readString("Please enter the new password: ");
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                SqlCommand command = new SqlCommand("Update Logins SET Password = @0 WHERE UserID = @1 ",conn);
                command.Parameters.Add(new SqlParameter("0", newpass));
                command.Parameters.Add(new SqlParameter("1", UserID));
                conn.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("Password updated for UserID: " + UserID);
            }
        }
        private void UpdateAdmin(string UserID)
        {           
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand command = new SqlCommand("Update Logins SET IsAdmin = @0 WHERE UserID = @1 ",conn);
                SqlCommand adminget = new SqlCommand("SELECT IsAdmin FROM Logins WHERE UserID = @1",conn);
                adminget.Parameters.Add(new SqlParameter("1", UserID));
                char readin = char.Parse(adminget.ExecuteScalar().ToString());
                
                if (readin == 'N')
                {
                    readin = 'Y';
                }
                else
                {
                    readin = 'N';
                }
                command.Parameters.Add(new SqlParameter("0", readin));
                command.Parameters.Add(new SqlParameter("1", UserID));
                
                command.ExecuteNonQuery();
                Console.WriteLine("Admin status updated for UserID: " + UserID);
            }
        }
    }
    /*using (SqlConnection conn = new SqlConnection()) code for sql query start
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
            }*/
    /*SqlCommand command1 = new SqlCommand("INSERT INTO Logins (UserID,Username,Password) VALUES(@0,@1,@2)", conn);
    command1.Parameters.Add(new SqlParameter("0", UserID));
                command1.Parameters.Add(new SqlParameter("1", UserName)); code to create new user, will be useful for admin class
                command1.Parameters.Add(new SqlParameter("2", UserPassword));*/
    class Task
    {
        private string TaskName;
        private string TaskDescription;
        private int TaskID;
    }
    class Project
    {
        private List<Task> ListOfTasks;
        private string Status;
        private string ProjectName;
        private int ProjectID;
        private decimal PercentComplete;
        private int NumofTasks;
        private int NumofMembers;

        public string getName() { return this.ProjectName; }
        public void setName(string name) { this.ProjectName = name; }
        public string getStatus() { return this.Status; }
        public void setStatus(string status) { this.Status = status; }
        public int getID() { return this.ProjectID; }
        public void setID(int ID) { this.ProjectID = ID; }
        public decimal getpercent() { return this.PercentComplete; }
        public void setPercent(decimal percent) { this.PercentComplete = percent; }
        public int getnumoftasks() { return this.NumofTasks; }
        public void setnumoftasks(int tasks) { this.NumofTasks = tasks; }
        public int getnumofmem() { return this.NumofMembers; }
        public void setNumOfMembers(int members) { this.NumofMembers = members; }
        public void Placeholder()
        {
            string[] tasks;
            Console.WriteLine("Please enter a Project name");
            string projectname = Console.ReadLine();
            Console.WriteLine("How many team members will be assigned?");
            int numofusers = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("How many tasks will be part of this project?");
            int numoftasks = Convert.ToInt32(Console.ReadLine());
            tasks = new string[numoftasks];
            for (int i = 0; i < numoftasks; i++)
            {

                Console.WriteLine("Please enter the task name");
                tasks[i] = Console.ReadLine();
            }
            Console.WriteLine("Project: " + projectname + "\nList of tasks: ");
            for (int i = 0; i < tasks.Length; i++)
            {
                Console.WriteLine((i + 1) + ") " + tasks[i]);

            }
            Console.ReadLine();
        }
        
    }
}
