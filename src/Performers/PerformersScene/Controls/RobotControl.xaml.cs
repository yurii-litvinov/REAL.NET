using System;
using System.Windows.Controls;
using PerformersScene.ViewModels.Animation;

namespace PerformersScene.Controls
{
    public partial class RobotControl : UserControl
    {
        public RobotControl()
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