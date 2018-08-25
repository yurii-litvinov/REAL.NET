namespace WpfControlsLib.Controls.Toolbar
{
    using System;
    using System.ComponentModel;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;

    public class ButtonWrapper : IButton, INotifyPropertyChanged
    {
        private IButton wrappedButton;

        public ButtonWrapper(IButton button)
        {
            this.wrappedButton = button;
            this.wrappedButton.ButtonEnabledChanged += this.ThrowIsEnabledPropertyChanged;
            this.WinInputCommand = new CommandXAMLAdapter(this.Command);
        }

        public event EventHandler ButtonEnabledChanged;

        /// <summary>
        /// Throws when binded property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand Command => this.wrappedButton.Command;

        public string Description => this.wrappedButton.Description;

        public string Image => this.wrappedButton.Image;

        public bool IsEnabled => this.wrappedButton.IsEnabled;

        /// <summary>
        /// Gets System.Windows.Input.Command which is necessary for correct working with XAML
        /// NOTICE: Added for compatibility with XAML
        /// </summary>
        public System.Windows.Input.ICommand WinInputCommand { get; }

        public void DoAction() => this.Command.Execute();

        public void SetDisabled()
        {
            this.wrappedButton.SetDisabled();
            this.ThrowButtonEnabledChanged();
        }

        public void SetEnabled()
        {
            this.wrappedButton.SetEnabled();
            this.ThrowButtonEnabledChanged();
        }

        private void ThrowButtonEnabledChanged() => this.ButtonEnabledChanged?.Invoke(this, EventArgs.Empty);

        private void ThrowIsEnabledPropertyChanged(object sender, EventArgs args) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
    }
}
