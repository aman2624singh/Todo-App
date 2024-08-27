using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class Photo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int TaskItemId { get; set; } // Foreign key to TaskItem table
    }
}
