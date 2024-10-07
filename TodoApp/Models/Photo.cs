using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class Photo:ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public byte[] PhotoData { get; set; }
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }
        

        public string    FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }
       
        public int TaskItemId { get; set; } 

        private string _filePath;
        private string _fileName;
    }
}
