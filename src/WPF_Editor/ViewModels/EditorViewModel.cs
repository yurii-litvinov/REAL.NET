using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Repo;
using WPF_Editor.Models;
using WPF_Editor.Models.Console;

namespace WPF_Editor.ViewModels
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Mediator Mediator { get; }

        //Try to use expression body methods.
        public EditorViewModel()
        {
            Mediator = Mediator.CreateMediator();
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}