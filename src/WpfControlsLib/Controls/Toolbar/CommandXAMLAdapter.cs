﻿namespace WpfControlsLib.Controls.Toolbar
{
    using EditorPluginInterfaces;
    using System;

    public class CommandXAMLAdapter : System.Windows.Input.ICommand
    {
        private ICommand command;

        public CommandXAMLAdapter(ICommand command)
        {
            this.command = command;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => this.command.Execute();
    }
}
