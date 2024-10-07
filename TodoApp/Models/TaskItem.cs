using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class TaskItem:ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public DateTime DueDate { get; set; }
        public bool Done { get; set; }
        public string Notes { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsPriority
        {
            get => _isPriority;
            set => SetProperty(ref _isPriority, value);
        }
        public int UserId { get; set; } 

        private bool _isSelected;
        private bool _isPriority;
        private string _title;
    }
}
