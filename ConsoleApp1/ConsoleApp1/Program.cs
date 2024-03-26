using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
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
            Console.WriteLine("Project: "+ projectname + "\nList of tasks: ");
            for (int i = 0; i < tasks.Length; i++)
            {
                Console.WriteLine((i + 1) +") " + tasks[i]);

            }
            Console.ReadLine();
        }
    }
}
