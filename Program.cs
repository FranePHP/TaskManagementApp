using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManagementApp
{
    class Program
    {
        static List<Task> tasks = new List<Task>();        
        static readonly string filePath = "tasks.json";
        static void Main()
        {
            LoadTasks(); // Load Tasks from the JSON file at the start of the application

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            bool running = true;

            while (running)
            {
                Console.WriteLine("\nTask Management Application");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. View Tasks");
                Console.WriteLine("3. Edit Task");
                Console.WriteLine("4. Delete Task");
                Console.WriteLine("5. Exit");

                Console.WriteLine("Choose an option: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        ViewTasks();
                        break;
                    case "3":
                        EditTask();
                        break;
                    case "4":
                        DeleteTask();
                        break;
                    case "5":                        
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

            }
            SaveTasks(); // Save Tasks to the JSON file before exiting
        }

        static void OnProcessExit(object? sender, EventArgs e)
        {
            SaveTasks(); // Save Tasks when the application is exiting
        }


        // Save Tasks to a JSON file
        static void SaveTasks()
        {
            try
            {
                string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access to the path '{filePath}' is denied. {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: An I/O error occured while writing the file. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while saving tasks: {ex.Message}");
            }
        }

        // Load Tasks from a JSON file
        static void LoadTasks()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    List<Task>? loadedTasks = JsonSerializer.Deserialize<List<Task>>(json);
                    if (loadedTasks != null)
                    {
                        tasks = loadedTasks;
                    }
                    else
                    {
                        tasks = new List<Task>();
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Error: The file '{filePath}' was not found. {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Error: Access to the path '{filePath}' is denied. {ex.Message}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error: The file '{filePath}' contains invalid JSON. {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error: An I/O error occured while reading the file. {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occured while loading tasks: {ex.Message}");
                }
            }
        }
        static void AddTask()
        {
            string? title;
            while (true)
            {
                Console.Write("Enter task title: ");
                title = Console.ReadLine();
                if (!string.IsNullOrEmpty(title))
                {
                    break;
                }
                Console.WriteLine("Title cannot be empty.");
            }

            string? description;
            while (true)
            {
                Console.Write("Enter task description: ");
                description = Console.ReadLine();
                if (!string.IsNullOrEmpty(description))
                {
                    break;
                }
                Console.WriteLine("Description cannot be empty.");
            }

            DateTime dueDate;
            while (true)
            {
                Console.Write("Enter task due date (yyyy-mm-dd): ");
                string? dueDateInput = Console.ReadLine();
                if (DateTime.TryParse(dueDateInput, out dueDate))
                {
                    break;
                }
                else
                {
                    Console.Write("Invalid date format. Please enter a valid date (yyyy-mm-dd).");
                }
            }

            string? priority;
            while (true)
            {
                Console.Write("Enter task priority (Low, Medium, High): ");
                priority = Console.ReadLine();
                if (priority == "Low" || priority == "Medium" || priority == "High")
                {
                    break;
                }
                Console.WriteLine("Priority must be 'Low', 'Medium' or 'High'.");
            }

            Task newTask = new Task(title, description, dueDate, priority);
            tasks.Add(newTask);

            Console.WriteLine("Task added successfully.");
        }

        static void ViewTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available.");
                return;
            }

            Console.WriteLine("\nTasks:");
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i]}");
            }
        }

        static void EditTask()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to edit.");
                return;
            }

            int taskNumber;
            while (true)
            {
                Console.Write("Enter task number to edit: ");
                string? input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out taskNumber) && taskNumber >= 1 && taskNumber <= tasks.Count)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please enter a valid task number.");
            }

            Task task = tasks[taskNumber - 1];              

            Console.Write("Enter new title (leave blank to keep current): ");
            string? newTitle = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTitle))
            {
                task.Title = newTitle;
            }

            Console.Write("Enter new description (leave blank to keep current):");
            string? newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription))
            {
                task.Description = newDescription;
            }

            while (true)
            {
                Console.Write("Enter new due date (leave blank to keep current):");
                string? newDueDate = Console.ReadLine();
                if (string.IsNullOrEmpty(newDueDate))
                {
                    break;
                }

                if (DateTime.TryParse(newDueDate, out DateTime parsedDate))
                {
                    task.DueDate = parsedDate;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please enter a valid date (yyyy-mm-dd).");
                }
            }

            while (true)
            {
                Console.Write("Enter new priority (leave blank to keep current):");
                string? newPriority = Console.ReadLine();
                if (string.IsNullOrEmpty(newPriority))
                {
                    break;
                }

                if (newPriority == "Low" || newPriority == "Medium" || newPriority == "High")
                {
                    task.Priority = newPriority;
                    break;
                }
                else
                {
                    Console.WriteLine("Priority must be 'Low', 'Medium' or 'High'.");
                }
            }

            Console.WriteLine("Task updated successfully.");
        }
        static void DeleteTask()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to delete.");
                return;
            }

            int taskNumber;
            while (true)
            {
                Console.WriteLine("Enter task number to delete: ");
                string? input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out taskNumber) && taskNumber >= 1 && taskNumber <= tasks.Count)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please enter a valid task number.");
            }
            
            tasks.RemoveAt(taskNumber - 1);
            Console.WriteLine("Task deleted successfully.");
        }
    }       

    class Task
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }

        public Task(string title, string description, DateTime dueDate, string priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
        }

        public override string ToString()
        {
            return $"Title: {Title}, Description: {Description}, Due Date: {DueDate.ToShortDateString()}, Priority: {Priority}";
        }
    }
}
