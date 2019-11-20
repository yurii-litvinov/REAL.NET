using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.ViewModels
{
    public class DrawingSceneViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double SpeedRatio
        {
            get { return speedRatio; }
            set 
            {
                speedRatio = value;
                OnPropertyChanged();
            }
        }

        private double speedRatio = 0.3;

        private void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
