using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static Validation validation = new Validation();
        static Login login = new Login();
        static string usertype;
        static int maxusermenu = 5;
        int maxadminmenu = 5;
        static void Main()
        {
            BeginProgram();
            
            if (usertype.ToLower() == "user")
            {
                DisplayUserMenu();
                int ans = validation.CheckIntString("Enter your choice between 1 and " + maxusermenu + ": ", 1, maxusermenu);
            }
            else if (usertype.ToLower() == "admin")
            {

            }

        }
        public static void BeginProgram()
        {      
            usertype = login.BeginLogin();

        }
        static void DisplayUserMenu()
        {
            Console.WriteLine("User Menu: ");
            Console.WriteLine("\n1) View all Projects\n2) View your Project\n3) View your notifications\n4) Change your password\n5) Exit");
        }
        static void DisplayAdminMenu()
        {
            Console.WriteLine("Admin Menu: ");
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
        Validation validation = new Validation();
        public string BeginLogin()
        {
            int choice = validation.CheckIntString("Would you like to login to an user or admin account: \n1) User\n2) Admin\n", 1, 2);
            switch (choice)
            {
                case 1:
                    UserLogin();
                    break;
                case 2:               
                    break;
            }

            return "user";
        }
        private void UserLogin()
        {
            string username = validation.readString("\nPlease enter your UserName: ");
            string password = validation.readString("\nPlease enter your Password: ");
            User user = new User();
            user.SetUpUser(username, password, false);
        }
    }
    class User
    {
        private string UserName;
        private int UserID;
        private string UserPassword;
        private bool onProject;
        public User()
        {
            this.UserName = "";
            this.UserID = 0;
            this.UserPassword = "";
            this.onProject = false;
        }
        public void SetUpUser(string name, string password, bool project)
        {
            this.UserName = name;
            this.UserPassword = password;
            this.onProject= project;
        }
    }
    
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
