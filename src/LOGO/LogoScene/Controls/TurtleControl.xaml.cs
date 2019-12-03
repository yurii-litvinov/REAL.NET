﻿using System;
using System.Windows.Controls;
using LogoScene.ViewModels.Animation;

namespace LogoScene.Controls
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
            var viewModel = DataContext as IAnimationCompletedHandler;
            viewModel.AnimationCompletedCommand.Execute(null);
        }
    }
}
