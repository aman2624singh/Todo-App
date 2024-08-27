using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class TaskItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool Done { get; set; }
        public bool IsSelected { get; set; }
        public string Notes { get; set; }
        public string Priority { get; set; }
        public Byte[] Attachment { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsPinned { get; set; }
        public string Category { get; set; }
        public int UserId { get; set; } // Foreign key to User table
    }
}
