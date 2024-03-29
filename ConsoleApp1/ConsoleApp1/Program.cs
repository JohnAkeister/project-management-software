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
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ConsoleApp1
{
    class Program
    {
        private static Validation validation = new Validation();
        private static Display display = new Display();
        private static Login login = new Login();
        private static Admin admin = new Admin();
        private static Project project = new Project();
        private static User user = new User();
        private static Logs logs = new Logs();
        private static Notifications notifs = new Notifications();
        private static string usertype;
        private static string username;
        private static int maxusermenu = 4;
        private static int maxadminmenu = 4;
        static void Main()
        {
            BeginProgram();                    
        }
        public static void BeginProgram()
        {      
            usertype = login.BeginLogin(ref username, validation);
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
                        project.ViewProjects(usertype,username,validation, user);
                        break;
                    case 2:
                        if (usertype == "user")
                        {
                            user.ViewUsersProjects(username, validation); // From this allow a user to edit a project if they are the owner of the project
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
                                    admin.DeleteUser(validation);
                                    break;
                                case 3:
                                    admin.EditUser(validation);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 3:
                        notifs.ViewNotifications(validation,username);
                        break;
                    case 4:
                        exit = true;
                        break;
                    
                        
                }
            }
        }
        private static void DisplayUserMenu()
        {
            Console.WriteLine("User Menu for "+username+": ");
            Console.WriteLine("\n1) View all Projects\n2) View your Projects\n3) View your notifications\n4) Exit");
        }
        private static void DisplayAdminMenu()
        {
            Console.WriteLine("Admin Menu for " + username + ": ");
            Console.WriteLine("\n1) View all Projects\n2) View all Members\n3) View your notifications\n4) Exit");
        }


    }
    class Display
    {
        Validation validation = new Validation();       
        public int DisplayUsers() // displays all users to the admin
        {
            using (SqlConnection conn = new SqlConnection()) 
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getuserscommand = new SqlCommand("SELECT UserID, Username, IsAdmin FROM Logins WHERE UserID > @0", conn);
                getuserscommand.Parameters.Add(new SqlParameter("0", "0"));
                SqlDataReader reader = getuserscommand.ExecuteReader();
                Console.WriteLine("UserID\tUsername\tIsAdmin?");
                while (reader.Read())
                {
                    
                    Console.WriteLine(String.Format("{0}\t | {1}\t | {2}",reader[0], reader[1], reader[2]));
                }
            }
            Console.WriteLine("\nMenu: \n1) Add a new user\n2) Delete a user\n3) Edit a user\n4) Return to main menu");
            int ans = validation.CheckIntString("\nPlease enter your choice:", 1, 4);
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

    public class Validation
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
    public class Login
    {
        
        private User user = new User();
        public string BeginLogin(ref string username, Validation validation)
        {
            string usertype;
            int choice = validation.CheckIntString("Would you like to login to an user or admin account: \n1) User\n2) Admin\n", 1, 2);
            switch (choice)
            {
                case 1:
                    usertype = "user";
                    UserLogin(ref username, validation, usertype);
                    return "user";
                    break;
                case 2:
                    usertype = "admin";
                    UserLogin(ref username,validation,usertype);
                    return "admin";
                    break;
                default:
                    return "error";
                    break;
            }
            
        }
        private void UserLogin(ref string username,Validation validation,string usertype)
        {
            bool valid = false;
            
            while (!valid)
            {
                username = validation.readString("\nPlease enter your UserName: ");
                string password = validation.readString("\nPlease enter your Password: ");            
                valid = user.SetUpUser(ref username, password,usertype);
            }
            
        }
        
    }
    public class User
    {        
        private Project project = new Project();
        private Logs logs = new Logs();
        private string UserName;    
        private string UserPassword;
        private bool onProject;
        public User()
        {
            
            this.UserName = "";           
            this.UserPassword = "";
            this.onProject = false;
        }
        public bool SetUpUser(ref string name, string password, string usertype)
        {
            this.UserName = name;
            this.UserPassword = password;           
            using (SqlConnection conn = new SqlConnection())
            {
              
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                try
                {
                    SqlCommand loginscommand = new SqlCommand("SELECT * FROM Logins WHERE Username = @0", conn); // extracts from DB when the username is valid
                    loginscommand.Parameters.Add(new SqlParameter("0", UserName));
                    using (SqlDataReader reader = loginscommand.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            if ((char.Parse(reader[3].ToString()) == 'Y' && (usertype == "admin" || usertype =="user")) ||(char.Parse(reader[3].ToString()) == 'N' && usertype == "user"))
                            {
                                if (reader[2].ToString() == UserPassword)
                                {
                                    Console.WriteLine("Login Successful");
                                    Console.WriteLine("UserID\tUsername\tPassword\tIsAdmin?");
                                    Console.WriteLine(String.Format("{0}\t | {1}\t | {2}\t | {3}", reader[0], reader[1], reader[2], reader[3]));
                                    name = this.UserName;
                                    return true;
                                }
                                else
                                {
                                    Console.WriteLine("Login Unsuccesful. Please re-enter username and password");
                                    return false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid you cannot access this account type with this user. Please enter a correct account type or close program.");
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
        public void ViewUsersProjects(string username, Validation validation) // does pass test to display the valid users projects
        {
            List<int> projectids = new List<int>();
            List<int> projectidsnodupes = new List<int>();
            int projectID = 0;
            using (SqlConnection conn = new SqlConnection())
            {
                try
                {

                
                    conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                    conn.Open();
                    SqlCommand getuserprojects = new SqlCommand("SELECT ProjectID FROM Tasks WHERE AssignedMember = @0 ",conn);
                    getuserprojects.Parameters.Add(new SqlParameter("0", username));
                    using (SqlDataReader reader = getuserprojects.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projectids.Add(Convert.ToInt32(reader[0].ToString()));
                        }
                        projectidsnodupes = projectids.Distinct().ToList();

                    }
                
                    SqlCommand getprojectinfo = new SqlCommand("SELECT * FROM Projects WHERE ProjectID = @0 AND ProjectID > @1",conn);
                    int[] projectidarry = new int[projectidsnodupes.Count];
                    projectidsnodupes.Sort();
                    Console.WriteLine("ProjectID|ProjectName|Project Status|Number of Tasks|Number of Members|Percentage Complete|Project Owner");
                
                    for (int i = 0; i < projectidsnodupes.Count; i++)
                    {
                        projectID = projectidsnodupes[i];
                        getprojectinfo.Parameters.Add(new SqlParameter("0", projectID));
                        getprojectinfo.Parameters.Add(new SqlParameter("1", "0"));
                        using (SqlDataReader reader = getprojectinfo.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3} \t | {4} \t | {5} \t | {6}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6]));
                            }
                        }
                        getprojectinfo.Parameters.Clear();
                    }
                }
                catch (SqlException er)
                {
                    Console.WriteLine(er.Message);
                }
            }
            int ans = 0;
            while (ans !=3)
            {
                Console.WriteLine("What would you like to do?\n1) View a projects tasks\n2) Edit a project you are the owner of\n3) View a project's timeline\n4) Return to Main Menu");
                ans = validation.CheckIntString("Enter your choice (1-4): ", 1, 4);
                switch (ans)
                {
                    case 1:
                        bool valid = false;
                        while (!valid)
                        {
                            int ans2 = validation.CheckIntString("\nPlease enter the project ID you wish to view the tasks of: ", 1, projectidsnodupes.Last());
                            for (int i = 0; i < projectidsnodupes.Count; i++) // stops user from accessing a project they know exists but cant see
                            {
                                if (ans2 == projectidsnodupes[i])
                                {
                                    ViewProjectTasks(ans2);
                                    valid = true;
                                }
                            }
                            if (!valid)
                            {
                                Console.WriteLine("You do not have access to this project please enter a different ID");
                            }
                        }
                        
                        
                        break;
                    case 2:
                        int ans3 = validation.CheckIntString("\nPlease enter the project ID you wish to edit that you are the owner of: ", 1, projectidsnodupes.Last());
                        EditProject(ans3,username, validation);
                        break;
                    case 3:
                        bool valid2 = false;
                        
                            int ans4 = validation.CheckIntString("\nPlease enter the project ID you wish to view the tasks of: ", 1, projectidsnodupes.Last());
                            for (int i = 0; i < projectidsnodupes.Count; i++) // stops user from accessing a project they know exists but cant see
                            {
                                if (ans4 == projectidsnodupes[i])
                                {
                                    logs.ViewLogs(username, ans4);
                                    valid2 = true;
                                }
                            }
                            if (!valid2)
                            {
                                Console.WriteLine("You do not have access to view the timeline of this project");
                            }
                        
                        break;
                    default:
                        break;
                }
            }            
        }
        public void ViewProjectTasks(int projectID)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand gettasks = new SqlCommand("SELECT * FROM Tasks WHERE ProjectID = @0 AND TaskID > @1 AND ProjectID > @1", conn);
                gettasks.Parameters.Add(new SqlParameter("0", projectID));
                gettasks.Parameters.Add(new SqlParameter("1", "0"));
                using (SqlDataReader reader = gettasks.ExecuteReader())
                {
                    Console.WriteLine("Task ID|Task Name|Project ID|Assigned Team Member|Status|Description");
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}\t | {1}\t | {2}\t | {3}\t | {4}\t | {5}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]));
                    }
                }                
            }
        }
        private void EditProject(int projectID,string username,Validation validation)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getproject = new SqlCommand("SELECT ProjectOwner FROM Projects WHERE ProjectID = @0", conn);
                getproject.Parameters.Add(new SqlParameter("0", projectID));
                string ProjectOwner = getproject.ExecuteScalar().ToString();
                if (username == ProjectOwner)
                {
                    ViewProjectTasks(projectID);
                    EditTask(projectID,username,validation);
                }
                else
                {
                    Console.WriteLine("You do not own this project and cannot edit it");
                }
            }
        }
        public void EditTask(int projectID, string username, Validation validation)
        {
            int edittaskid;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getmaxtask = new SqlCommand("SELECT Max(TaskID) FROM Tasks WHERE ProjectID = @0", conn);
                SqlCommand getmintask = new SqlCommand("SELECT Min(TaskID) FROM Tasks WHERE ProjectID = @0", conn);
                getmintask.Parameters.Add(new SqlParameter("0", projectID));
                getmaxtask.Parameters.Add(new SqlParameter("0", projectID));
                int maxtaskid = Int32.Parse(getmaxtask.ExecuteScalar().ToString());
                int mintaskid = Int32.Parse(getmintask.ExecuteScalar().ToString());
                edittaskid = validation.CheckIntString("Which task would you like to edit?: ", mintaskid, maxtaskid);
                SqlCommand gettask = new SqlCommand("SELECT * FROM Tasks WHERE TaskID = @0", conn);
                gettask.Parameters.Add(new SqlParameter("0", edittaskid));
                Console.WriteLine("Which Parameter would you like to update: \n1) Task Name\n2) Assigned Member\n3) Status\n4) Description");
                int ans = validation.CheckIntString("Please enter your choice (1-4): ", 1, 4);
                switch (ans)
                {
                    case 1:
                        string newname = validation.readString("Please enter the new name: ");
                        changename(newname,conn,edittaskid);
                        break;
                    case 2:
                        string newuser = validation.readString("Please enter the name of the new assigned member: ");
                        changemember(newuser,conn,edittaskid,validation);
                        break;
                    case 3:
                        changestatus(conn,edittaskid, validation);
                        project.changeprojectpercent(projectID,conn);
                        break;
                    case 4:
                        string newdesc = validation.readString("Please enter the new description: ");
                        changedesc(newdesc,conn,edittaskid);
                        break;
                }
            }
            Logs log = new Logs();
            log.AddLog(edittaskid,username, validation);
        }
        private void changename(string newname, SqlConnection conn, int taskID)
        {
            SqlCommand updatename = new SqlCommand("UPDATE Tasks SET TaskName = @0 WHERE TaskID = @1 ", conn);
            updatename.Parameters.Add(new SqlParameter("0", newname));
            updatename.Parameters.Add(new SqlParameter("1", taskID));
            updatename.ExecuteNonQuery();
        }
        private void changemember(string newuser, SqlConnection conn, int taskID,Validation validation)
        {
            List<string> usernames = new List<string>();
            SqlCommand readnames = new SqlCommand("SELECT Username FROM Logins",conn);
            using (SqlDataReader reader = readnames.ExecuteReader())
            {
                Console.WriteLine("Current List of users: ");
                while (reader.Read())
                {
                    usernames.Add(reader[0].ToString());
                    Console.WriteLine(reader[0].ToString());
                }
            }
            bool valid = false;
            while (!valid)
            {
                for (int i = 0; i < usernames.Count; i++)
                {
                    if (newuser == usernames[i])
                    {
                        valid = true;
                    }
                }
                if (!valid)
                {
                    Console.WriteLine("User not found in list of users please enter a different name");
                    newuser = validation.readString("Please enter a user to assign to the task");
                }
            }
            SqlCommand changemember = new SqlCommand("UPDATE Tasks SET AssignedMember = @0 WHERE TaskID = @1", conn);
            changemember.Parameters.Add(new SqlParameter("0", newuser));
            changemember.Parameters.Add(new SqlParameter("1", taskID));
            changemember.ExecuteNonQuery();
            Console.WriteLine("Assigned changed to " + newuser + " for Task ID: " + taskID);
        }
        private void changestatus(SqlConnection conn, int taskID,Validation validation)
        {
            string newstatus = "";
            SqlCommand getcurrentstatus = new SqlCommand("SELECT Status FROM Tasks WHERE TaskID = @0",conn);
            getcurrentstatus.Parameters.Add(new SqlParameter("0", taskID));
            string currentstatus = getcurrentstatus.ExecuteScalar().ToString();
            if (currentstatus.Contains("Not Started"))
            {
                Console.WriteLine("Current status of project is: " + currentstatus);
                Console.WriteLine("Available statuses for this project are: \n1) In-Progress\n2) Completed");
                int ans = validation.CheckIntString("Please enter your choice (1-2): ", 1, 2);
                switch (ans)
                {
                    case 1:
                        newstatus = "In-Progress";
                        break;
                    case 2:
                        newstatus = "Completed";
                        break;
                    default:
                        newstatus = "error";
                        break;
                }
            }
            else if (currentstatus.Contains("In-Progress"))
            {
                Console.WriteLine("Current status of project is: " + currentstatus);
                Console.WriteLine("Available statuses for this project are: \n1) Not Started\n2) Completed");
                int ans = validation.CheckIntString("Please enter your choice (1-2): ", 1, 2);
                switch (ans)
                {
                    case 1:
                        newstatus = "Not Started";
                        break;
                    case 2:
                        newstatus = "Completed";
                        break;
                    default:
                        newstatus = "error";
                        break;
                }
            }
            else if (currentstatus.Contains("Completed"))
            {
                Console.WriteLine("Current status of project is: " + currentstatus);
                Console.WriteLine("Available statuses for this project are: \n1) Not Started\n2) In-Progress");
                int ans = validation.CheckIntString("Please enter your choice (1-2): ", 1, 2);
                switch (ans)
                {
                    case 1:
                        newstatus = "Not Started";
                        break;
                    case 2:
                        newstatus = "In-Progress";
                        break;
                    default:
                        newstatus = "error";
                        break;
                }
            }
            SqlCommand updatestatus = new SqlCommand("UPDATE Tasks SET Status = @0 WHERE TaskID = @1", conn);
            updatestatus.Parameters.Add(new SqlParameter("0", newstatus));
            updatestatus.Parameters.Add(new SqlParameter("1", taskID));
            updatestatus.ExecuteNonQuery();

        }
        private void changedesc(string newdesc, SqlConnection conn, int taskID)
        {
            SqlCommand updatedesc = new SqlCommand("UPDATE Tasks SET Description = @0 WHERE TaskID = @1 ", conn);
            updatedesc.Parameters.Add(new SqlParameter("0", newdesc));
            updatedesc.Parameters.Add(new SqlParameter("1", taskID));
            updatedesc.ExecuteNonQuery();
        }
    }
    class Admin
    {
        
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
                    conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
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
        public void DeleteUser(Validation validation)
        {
            Console.WriteLine("Please enter the ID of the user you wish to remove: ");
            int ans = validation.ReadInt(Console.ReadLine());
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
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
        public void EditUser(Validation validation)
        {
            
                Console.WriteLine("Please enter the ID of the user you wish to edit: ");
                string reply = Console.ReadLine();
                Console.WriteLine("Which category would you like to edit: \n1) Username\n2) Password\n3) IsAdmin");
                int ans = validation.CheckIntString("Please choose between 1 and 4",1,4);
                switch (ans)
                {
                    case 1:
                    UpdateName(reply,validation);
                        break;
                    case 2:
                    UpdatePassword(reply, validation);
                        break;
                    case 3:
                    UpdateAdmin(reply);
                        break;
                }

            
            
        }
        private void UpdateName(string UserID,Validation validation)
        {
            string newname = validation.readString("Please enter the new username: ");
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                SqlCommand updateloginscommand = new SqlCommand("Update Logins SET Username = @0 WHERE UserID = @1 ", conn);
                updateloginscommand.Parameters.Add(new SqlParameter("0", newname));
                updateloginscommand.Parameters.Add(new SqlParameter("1", UserID));
                conn.Open();
                updateloginscommand.ExecuteNonQuery();
                Console.WriteLine("Username updated for UserID: " + UserID);
            }
        }
        private void UpdatePassword(string UserID,Validation validation)
        {
            string newpass = validation.readString("Please enter the new password: ");
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                SqlCommand updatepasscommand = new SqlCommand("Update Logins SET Password = @0 WHERE UserID = @1 ",conn);
                updatepasscommand.Parameters.Add(new SqlParameter("0", newpass));
                updatepasscommand.Parameters.Add(new SqlParameter("1", UserID));
                conn.Open();
                updatepasscommand.ExecuteNonQuery();
                Console.WriteLine("Password updated for UserID: " + UserID);
            }
        }
        private void UpdateAdmin(string UserID)
        {           
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand updateadmincommand = new SqlCommand("Update Logins SET IsAdmin = @0 WHERE UserID = @1 ",conn);
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
                updateadmincommand.Parameters.Add(new SqlParameter("0", readin));
                updateadmincommand.Parameters.Add(new SqlParameter("1", UserID));
                
                updateadmincommand.ExecuteNonQuery();
                Console.WriteLine("Admin status updated for UserID: " + UserID);
            }
        }
    }
    /*using (SqlConnection conn = new SqlConnection()) code for sql query start
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
            }*/
    /*SqlCommand command1 = new SqlCommand("INSERT INTO Logins (UserID,Username,Password) VALUES(@0,@1,@2)", conn);
    command1.Parameters.Add(new SqlParameter("0", UserID));
                command1.Parameters.Add(new SqlParameter("1", UserName)); code to create new user, will be useful for admin class
                command1.Parameters.Add(new SqlParameter("2", UserPassword));*/ 
    public class Logs
    {
        
        public void AddLog(int taskID, string username,Validation validation)
        {
            Notifications notif = new Notifications();
            int maxlogid;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand addlog = new SqlCommand("INSERT INTO Logs (LogID,LogTime,Logtext,TaskID,Username) VALUES (@0,@1,@2,@3,@4)", conn);
                SqlCommand readlogid = new SqlCommand("SELECT Max(LogID) FROM Logs",conn);
                maxlogid = Int32.Parse(readlogid.ExecuteScalar().ToString());
                maxlogid++;
                DateTime currentdt = DateTime.Now;
                addlog.Parameters.Add(new SqlParameter("0", maxlogid));
                addlog.Parameters.Add(new SqlParameter("1", currentdt));
                string logtext = validation.readString("Please enter the update log for this task: ");
                addlog.Parameters.Add(new SqlParameter("2", logtext));
                addlog.Parameters.Add(new SqlParameter("3", taskID));
                addlog.Parameters.Add(new SqlParameter("4", username));
                addlog.ExecuteNonQuery();
                Console.WriteLine("Log ID: " + maxlogid + " added for task ID: " + taskID);
            }
            notif.NewNotification(maxlogid,validation);
        }
        public void ViewLogs(string username,int projectID)
        {
            List<int> ListOfIds = new List<int>();
            List<int> NoDupesList = new List<int>();
            int count = 0;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand gettaskids = new SqlCommand("SELECT TaskID FROM Tasks WHERE ProjectID = @0", conn);
                gettaskids.Parameters.Add(new SqlParameter("0", projectID));
                using (SqlDataReader reader = gettaskids.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListOfIds.Add(int.Parse(reader[0].ToString()));
                    }
                }
                SqlCommand getlogs = new SqlCommand("SELECT * FROM Logs WHERE Username = @0 AND TaskID = @1 AND LogID > @2", conn);
                
                for (int i = 0; i < ListOfIds.Count; i++)
                {
                    getlogs.Parameters.Add(new SqlParameter("0", username));
                    getlogs.Parameters.Add(new SqlParameter("1", ListOfIds[i]));
                    getlogs.Parameters.Add(new SqlParameter("2", "0"));
                    using (SqlDataReader reader = getlogs.ExecuteReader())
                    {
                        Console.WriteLine("LogID|Log Date and Time|Made By|For Task|Log text");
                        while (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0}|{1}|{2}\t|{3}\t|{4}", reader[0], reader[1], reader[4], reader[3], reader[2]));
                            count++;
                        }
                    }
                    getlogs.Parameters.Clear();         
                }
                if (count ==0)
                {
                    Console.WriteLine("No Update logs for this project");
                }
                
            }
        }
    }
    class Notifications
    {
        public void NewNotification(int logID, Validation validation)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand gettaskid = new SqlCommand("SELECT TaskID FROM Logs WHERE LogID = @0", conn);
                gettaskid.Parameters.Add(new SqlParameter("0", logID));
                int taskID = int.Parse(gettaskid.ExecuteScalar().ToString());
                SqlCommand getassignedmember = new SqlCommand("SELECT AssignedMember FROM Tasks WHERE TaskID = @0", conn);
                getassignedmember.Parameters.Add(new SqlParameter("0", taskID));
                string assignedmember = getassignedmember.ExecuteScalar().ToString();
                SqlCommand newnotif = new SqlCommand("INSERT INTO Notifications (NotifID,NotifText,LogID,Member) VALUES (@0,@1,@2,@3)",conn);
                SqlCommand getmaxid = new SqlCommand("SELECT Max(NotifID) FROM Notifications", conn);
                int maxid = int.Parse(getmaxid.ExecuteScalar().ToString());
                maxid++;
                newnotif.Parameters.Add(new SqlParameter("0", maxid));
                string notiftext = "New notification for user " + assignedmember + ". There has been an update to task " + taskID + ". Please check your projects and the project timelines.";
                newnotif.Parameters.Add(new SqlParameter("1", notiftext));
                newnotif.Parameters.Add(new SqlParameter("2", logID));
                newnotif.Parameters.Add(new SqlParameter("3", assignedmember));
                newnotif.ExecuteNonQuery();
            }
        }
        public void ViewNotifications(Validation validation, string username)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getnotifs = new SqlCommand("SELECT NotifID,NotifText FROM Notifications WHERE Member = @0", conn);
                getnotifs.Parameters.Add(new SqlParameter("0", username));
                using (SqlDataReader reader = getnotifs.ExecuteReader())
                {
                    Console.WriteLine("Notification ID | Notification");
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}\t|{1}", reader[0], reader[1]));
                    }
                }
            }
        }
    }
    class Project
    {        
             
        private string ProjectName;        
        private int PercentComplete;
        private int NumofTasks;
        private int NumofMembers;
        private string ProjectOwner;
        private const int zero = 0;

        public void ViewProjects(string usertype,string username, Validation validation, User user)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand readprojects = new SqlCommand("SELECT ProjectID,ProjectName,Status,NumberOfTasks,ProjectOwner, PercentComplete FROM Projects WHERE ProjectID > @0",conn);
                readprojects.Parameters.Add(new SqlParameter("0", zero));
                using (SqlDataReader reader = readprojects.ExecuteReader())
                {
                    Console.WriteLine("ProjectID|Project Name\t|Project Status\t|Number of Tasks|Project Owner| Percent Complete");
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0} \t | {1} \t | {2} \t | {3}\t | {4}\t | {5}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5]));
                    }
                }
            }
            if (usertype == "admin")
            {
                Console.WriteLine("Please choose an option: \n1) Add new Project\n2) Delete a Project\n3) Edit a Project\n4) View members working on a project\n5) Exit");
                int ans = validation.CheckIntString("Please Enter your choice: ", 1, 5);
                switch (ans)
                {
                    case 1:
                        AddProject(validation,username);
                        break;
                    case 2:
                        DeleteProject(validation);
                        break;
                    case 3:
                        EditProject(username,validation,user);
                        break;                   
                    case 4:
                        ViewProjectMembers(validation);
                        break;
                    default:
                        break;
                }
            }
            
        }
        public void AddProject(Validation validation, string username)
        {            
            ProjectName = validation.readString("What would you like the Project to be called?: ");
            NumofTasks = validation.CheckIntString("How many tasks will this project have?(Max 50): ",1,50);
            NumofMembers = validation.CheckIntString("How many team members will be assigned to this project?(Max 10): ", 1, 10);
            ProjectOwner = validation.readString("What is the name of the Project Owner?: ");
            PercentComplete = 0;
            int maxid = 0;
            using (SqlConnection conn = new SqlConnection())
            {
                try
                {
                    conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                    conn.Open();
                    SqlCommand addproject = new SqlCommand("INSERT INTO Projects (ProjectID,ProjectName,Status,NumberOfTasks,NumberOfMembers,PercentComplete,ProjectOwner) VALUES (@0,@1,@2,@3,@4,@5,@6)", conn);
                    SqlCommand readvalue = new SqlCommand("SELECT Max(ProjectID) FROM Projects",conn);
                    maxid = Int32.Parse(readvalue.ExecuteScalar().ToString());
                    maxid++;
                    addproject.Parameters.Add(new SqlParameter("0", maxid));
                    addproject.Parameters.Add(new SqlParameter("1", ProjectName));
                    addproject.Parameters.Add(new SqlParameter("2", "Not Started"));
                    addproject.Parameters.Add(new SqlParameter("3", NumofTasks));
                    addproject.Parameters.Add(new SqlParameter("4", NumofMembers));
                    addproject.Parameters.Add(new SqlParameter("5", PercentComplete));
                    addproject.Parameters.Add(new SqlParameter("6", ProjectOwner));
                    Console.WriteLine("Project " + ProjectName + " has been created. Total rows affected: " + addproject.ExecuteNonQuery());
                }
                catch (SqlException er)
                {

                    Console.WriteLine("Error occured. Sql error " + er.Message);
                }
            }
            AddTask(maxid, validation,username);
            
            
        }
        public void DeleteProject(Validation validation)
        {
            int deleteID;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getmaxid = new SqlCommand("SELECT Max(ProjectID) FROM Projects", conn);
                int maxid = Int32.Parse(getmaxid.ExecuteScalar().ToString());
                deleteID = validation.CheckIntString("Please enter the ID of the project you wish to delete: ",1,maxid);
                SqlCommand deletetasks = new SqlCommand("DELETE FROM Tasks WHERE ProjectID = @0",conn);
                deletetasks.Parameters.Add(new SqlParameter("0", deleteID));
                SqlCommand deleteproject = new SqlCommand("DELETE FROM Projects WHERE ProjectID = @0",conn);
                deleteproject.Parameters.Add(new SqlParameter("0", deleteID));
                Console.WriteLine("Are you sure you want to delete project ID: " + deleteID + "?");
                string reply = validation.readString("Y/N: ");
                if (reply.ToLower() == "y")
                {
                    deletetasks.ExecuteNonQuery();
                    deleteproject.ExecuteNonQuery();
                    Console.WriteLine("Project Deleted, returning to main menu");
                }
                else
                {
                    Console.WriteLine("Project has not been deleted, returning to main menu");
                }
            }
        }
        public void EditProject(string username, Validation validation, User user)
        {
            int projectID;
            Console.WriteLine();
            projectID = validation.CheckIntString("Please enter the ID of the project you wish to edit: ", 1, 5);
            bool valid = false;
            while (!valid)
            {
                int ans = validation.CheckIntString("Please choose from the following; \n1) Edit Project name\n2) Edit Project Owner\n3) Edit Project Status\n4) View a Project's Tasks\n5) Return to Main Menu",1,5);
                switch (ans)
                {
                    case 1:
                        EditProjectName(projectID,validation);
                        break;
                    case 2:
                        EditProjectOwner(projectID, validation);
                        break;
                    case 3:
                        EditProjectStatus(projectID, validation);
                        break;
                    case 4:
                        EditProjectTasks(projectID,username,validation,user);
                        break;
                    default:
                        valid = true;
                        break;
                }
            }
            
        } // view project tasks needs to be done
        public void ViewProjectMembers(Validation validation)
        {
            List<string> ListOfMembers = new List<string>();
            List<string> ListNoDupes = new List<string>();
            Console.WriteLine();
            int projectID = validation.CheckIntString("Please enter the ID of the project you wish to view the members of: ", 1, 5);
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getmembers = new SqlCommand("SELECT AssignedMember FROM Tasks WHERE ProjectID = @0", conn);
                getmembers.Parameters.Add(new SqlParameter("0",projectID));
                using (SqlDataReader reader = getmembers.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ListOfMembers.Add(reader[0].ToString());
                    }
                    ListNoDupes = ListOfMembers.Distinct().ToList();
                    Console.WriteLine("List of current members assigned to Project ID: " + projectID);
                    for (int i = 0; i < ListNoDupes.Count; i++)
                    {
                        Console.WriteLine(ListNoDupes[i]);
                    }
                }
            }
        }
        private void EditProjectName(int projectID,Validation validation)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getprojname = new SqlCommand("SELECT ProjectName FROM Projects WHERE ProjectID = @0",conn);
                getprojname.Parameters.Add(new SqlParameter("0", projectID));
                Console.WriteLine("Current Project name is: " + getprojname.ExecuteScalar().ToString());
                string newname = validation.readString("Please enter the new project name: ");
                SqlCommand updatename = new SqlCommand("UPDATE Projects SET ProjectName = @0 WHERE ProjectID = @1", conn);
                updatename.Parameters.Add(new SqlParameter("0", newname));
                updatename.Parameters.Add(new SqlParameter("1",projectID));
                updatename.ExecuteNonQuery();
                Console.WriteLine("Project " + projectID + " name updated to " + newname);
            }
        }
        private void EditProjectOwner(int projectID,Validation validation)
        {
            List<string> ListofMembers = new List<string>();
            string newname = "";
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getprojowner = new SqlCommand("SELECT ProjectOwner FROM Projects WHERE ProjectID = @0", conn);
                getprojowner.Parameters.Add(new SqlParameter("0", projectID));
                Console.WriteLine("Current Project Owner is: " + getprojowner.ExecuteScalar().ToString());               
                SqlCommand updatename = new SqlCommand("UPDATE Projects SET ProjectOwner = @0 WHERE ProjectID = @1", conn);                
                SqlCommand readin = new SqlCommand("SELECT Username FROM Logins", conn);
                using (SqlDataReader reader = readin.ExecuteReader())
                {
                    Console.WriteLine("List of Members: ");
                    while (reader.Read())
                    {
                        ListofMembers.Add(reader[0].ToString());
                        Console.WriteLine(reader[0].ToString());


                    }
                }
                bool exit = false;
                while (!exit)
                {
                    newname = validation.readString("\nEnter the Name of the user you would like to assign to this task: ");
                    for (int j = 0; j < ListofMembers.Count; j++)
                    {
                        if (newname == ListofMembers[j])
                        {
                            exit = true;
                        }
                    }
                    if (!exit)
                    {
                        Console.WriteLine("\nError User not found in user list. Please re-enter when prompted");
                    }
                }
                updatename.Parameters.Add(new SqlParameter("0", newname));
                updatename.Parameters.Add(new SqlParameter("1", projectID));
                updatename.ExecuteNonQuery();
                Console.WriteLine("Project " + projectID + " owner updated to " + newname);
            }
        }
        private void EditProjectStatus(int projectID, Validation validation)
        {
            string currentstatus = "";
            string newstatus = "Not Started";
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                conn.Open();
                SqlCommand getprojstatus = new SqlCommand("SELECT Status FROM Projects Where ProjectID = @0", conn);
                getprojstatus.Parameters.Add(new SqlParameter("0", projectID));
                currentstatus = getprojstatus.ExecuteScalar().ToString();
                Console.WriteLine("Current status is: " + currentstatus);
                if (currentstatus == "Not Started")
                {
                    Console.WriteLine("Available statuses are: \n1) In-Progress\n2) Complete");
                    int ans = validation.CheckIntString("Enter (1-2): ", 1, 2);
                    switch (ans)
                    {
                        case 1:
                            newstatus = "In-Progress";
                            break;
                        case 2:
                            newstatus = "Complete";
                            break;

                    }
                }
                else if (currentstatus == "In-Progress")
                {
                    Console.WriteLine("Available statuses are: \n1) Not Started\n2) Complete");
                    int ans = validation.CheckIntString("Enter (1-2): ", 1, 2);
                    switch (ans)
                    {
                        case 1:
                            newstatus = "Not Started";
                            break;
                        case 2:
                            newstatus = "Complete";
                            break;

                    }
                }
                else
                {
                    Console.WriteLine("Available statuses are: \n1) Not Started\n2) In-Progress");
                    int ans = validation.CheckIntString("Enter (1-2): ", 1, 2);
                    switch (ans)
                    {
                        case 1:
                            newstatus = "Not Started";
                            break;
                        case 2:
                            newstatus = "In-Progress";
                            break;

                    }
                }
                SqlCommand updatestatus = new SqlCommand("UPDATE Projects SET Status = @0 WHERE ProjectID = @1", conn);
                updatestatus.Parameters.Add(new SqlParameter("0", newstatus));
                updatestatus.Parameters.Add(new SqlParameter("1", projectID));
                updatestatus.ExecuteNonQuery();
                Console.WriteLine("Project " + projectID + " ID status has been changed from " + currentstatus + " to " + newstatus);
            }
        }
        private void EditProjectTasks(int projectID,string username,Validation validation, User user)
        {
            user.ViewProjectTasks(projectID);
            Console.WriteLine("Which task would you like to edit?: ");
            user.EditTask(projectID,username,validation);
        }
        
        private void AddTask(int projectID,Validation validation, string username)
        {
            List<string> ListofMembers = new List<string>();
            string taskname;
            string taskdesc;
            string assignedmember = "";     
            for (int i = 0; i < NumofTasks; i++)
            {
                try
                {


                    taskname = validation.readString("What is the name of taks number " + (i+1) + "?: ");
                    taskdesc = validation.readString("Please enter the description of the task: ");
                    using (SqlConnection conn = new SqlConnection())
                    {
                        conn.ConnectionString = "Server=localhost\\SQLEXPRESS02 ;Database=SQLDB ; Trusted_Connection=true";
                        conn.Open();
                        SqlCommand readin = new SqlCommand("SELECT Username FROM Logins", conn);
                        using (SqlDataReader reader = readin.ExecuteReader())
                        {
                            Console.WriteLine("List of Members: ");
                            while (reader.Read())
                            {
                                ListofMembers.Add(reader[0].ToString());
                                Console.WriteLine(reader[0].ToString());
                                
                            
                            }
                        }                       
                        bool exit = false;
                        while (!exit)
                        {
                            assignedmember = validation.readString("\nEnter the Name of the user you would like to assign to this task: ");
                            for (int j = 0; j < ListofMembers.Count; j++)
                            {
                                if (assignedmember == ListofMembers[j])
                                {
                                    exit = true;
                                }
                            }
                            if (!exit)
                            {
                                Console.WriteLine("\nError User not found in user list. Please re-enter when prompted");
                            }
                        }
                        SqlCommand addtask = new SqlCommand("INSERT INTO Tasks (TaskID,TaskName,ProjectID,AssignedMember,Description) VALUES (@0,@1,@2,@3,@4)", conn);
                        SqlCommand readtasknum = new SqlCommand("SELECT Max(TaskID) FROM Tasks", conn);
                        int taskid = Int32.Parse(readtasknum.ExecuteScalar().ToString());
                        taskid++;
                        addtask.Parameters.Add(new SqlParameter("0", taskid));
                        addtask.Parameters.Add(new SqlParameter("1", taskname));
                        addtask.Parameters.Add(new SqlParameter("2", projectID));
                        addtask.Parameters.Add(new SqlParameter("3", assignedmember));
                        addtask.Parameters.Add(new SqlParameter("4", taskdesc));
                        addtask.ExecuteNonQuery();
                        Console.WriteLine("Task '" + taskname + "' added to projectID " + projectID);
                        Logs log = new Logs();
                        log.AddLog(taskid,username,validation);
                    }
                }
                catch (SqlException er)
                {

                    Console.WriteLine("Error occured with : " + er.Message);
                }
            
            }
        } //create a function to add members that can be called by the project leader        
        public void changeprojectpercent(int projectID, SqlConnection conn)
        {
            List<string> ListOfStatus = new List<string>();
            int countofcomplete = 0;
            SqlCommand getstatuses = new SqlCommand("SELECT Status FROM Tasks WHERE ProjectID = @0", conn);
            SqlCommand getpercent = new SqlCommand("SELECT PercentComplete FROM Projects WHERE ProjectID = @0", conn);
            getpercent.Parameters.Add(new SqlParameter("0", projectID));
            getstatuses.Parameters.Add(new SqlParameter("0", projectID));
            using (SqlDataReader reader = getstatuses.ExecuteReader())
            {
                while (reader.Read())
                {
                    ListOfStatus.Add(reader[0].ToString());
                }
            }
            foreach (string item in ListOfStatus)
            {
                if (item == "Complete")
                {
                    countofcomplete++;
                }
            }
            SqlCommand getnumoftasks = new SqlCommand("SELECT NumberOfTasks FROM Projects WHERE ProjectID = @0", conn);
            getnumoftasks.Parameters.Add(new SqlParameter("0", projectID));
            int numoftasks = int.Parse(getnumoftasks.ExecuteScalar().ToString());
            int percentcomplete = (countofcomplete / numoftasks)*100;
            SqlCommand updatepercent = new SqlCommand("UPDATE Projects SET PercentComplete = @0 WHERE ProjectID = @1", conn);
            updatepercent.Parameters.Add(new SqlParameter("0", percentcomplete));
            updatepercent.Parameters.Add(new SqlParameter("1", projectID));
            updatepercent.ExecuteNonQuery();
        }
        
        
        
    }
}
