using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Models
{
    public class Attachment
    {
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
