using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Data
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        public static SQLiteAsyncConnection Database
        {
            get
            {
                if (_database == null)
                {
                    string folderPath;
                    if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TodoApp");
                    }
                    else
                    {
                        folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    }

                    // Ensure the folder exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Combine the folder path with the database file name
                    string dbPath = Path.Combine(folderPath, "TodoApp.db");

                    // Initialize the database
                    _database = new SQLiteAsyncConnection(dbPath);

                    // Create tables
                    _database.CreateTableAsync<User>().Wait();
                    _database.CreateTableAsync<TaskItem>().Wait();
                    _database.CreateTableAsync<TaskEvent>().Wait();
                    _database.CreateTableAsync<Photo>().Wait();
                }
                return _database;
            }
        }

    }
}
