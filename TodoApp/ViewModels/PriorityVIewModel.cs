using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.ViewModels
{
    public partial class PriorityVIewModel:ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TaskItem> pinnedItems;



        public PriorityVIewModel()
        {
            PinnedItems = new ObservableCollection<TaskItem>();
        }

        [RelayCommand]
        private void OnClosePopup()
        {
            MopupService.Instance.PopAsync();
        }
    }
}
