using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Editor.ViewModels
{ 
    /*
    public class ConsoleViewModel : INotifyPropertyChanged
    {

        private readonly IAppConsole console = AppConsole.CreateConsole();

        public bool ConsoleVisibility => console.IsVisible;

        public ConsoleWindow MessageConsole => console.GetConsoleWindowByName("MessageConsole");

        public ConsoleWindow ErrorConsole => console.GetConsoleWindowByName("ErrorConsole");

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void ChangeConsoleVisibility()
        {
            if (console.IsVisible)
            {
                console.HideConsole();
            }
            else
            {
                console.ShowConsole();
            }
            OnPropertyChanged("ConsoleVisibility");
        }

        public ConsoleViewModel()
        {
            console.GetConsoleWindowByName("MessageConsole").NewMessage("message");
            console.GetConsoleWindowByName("ErrorConsole").NewMessage("error");
        }
    }
    */
}
