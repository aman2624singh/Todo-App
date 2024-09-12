using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class Reminder:ObservableObject
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsDailyReminder { get; set; }
    }
}
