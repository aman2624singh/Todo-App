using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public interface IPhotoService
    {
        Task<int> AddPhotoAsync(Photo photo);
        Task<int> DeletePhotoAsync(Photo photo);
        Task<List<Photo>> GetPhotosByTaskAsync(int taskId);
        Task<Photo> GetPhotoAsync(int photoId);

        Task<List<Photo>> GetPhotosByTaskIdAsync(int taskId);
    }
}
