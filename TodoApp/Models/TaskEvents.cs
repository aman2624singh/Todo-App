using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class TaskEvent:ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _eventName;

        [Required(ErrorMessage = "Event name is required.")]
        public string EventName
        {
            get => _eventName;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 10)
                {
                    throw new ArgumentException("Event name cannot exceed 10 words.");
                }
                _eventName = value;
            }
        }
        public DateTime EventDate { get; set; }
        public int TaskItemId { get; set; }

        private bool _isDone;
        public bool IsDone
        {
            get => _isDone;
            set => SetProperty(ref _isDone, value);
        }

    }
}
