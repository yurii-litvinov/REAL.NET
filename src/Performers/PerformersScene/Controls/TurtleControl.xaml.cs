using System;
using System.Windows.Controls;
using PerformersScene.ViewModels.Animation;

namespace PerformersScene.Controls
{
    /// <summary>
    /// Логика взаимодействия для TurtleControl.xaml
    /// </summary>
    public partial class TurtleControl : UserControl
    {

        public TurtleControl()
        {
            InitializeComponent();
        }

        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            var animationCompleted = DataContext as IAnimationCompletedHandler;
            animationCompleted.AnimationCompletedCommand.Execute(null);
        }
    }
}
