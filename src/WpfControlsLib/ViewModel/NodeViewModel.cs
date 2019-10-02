using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using GraphX.PCL.Common.Models;
using Repo;

namespace WpfControlsLib.ViewModel
{
    /// <summary>
    /// NodeViewModel is the view model for visual model nodes. It is used to bind to GraphX vertex controls.
    /// </summary>
    public class NodeViewModel : VertexBase, INotifyPropertyChanged
    {
        private Brush color = Brushes.Green;
        private IList<AttributeViewModel> attributes = new List<AttributeViewModel>();
        private Brush borderColor = Brushes.White;
        private bool isAllowed = true;
        private string picture = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeViewModel"/> class.
        /// </summary>
        public NodeViewModel(string text = "")
        {
            this.Name = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets name of the node.
        /// </summary>
        public string Name { get; set; }

        public INode Node { get; set; }

        public Brush Color
        {
            get => this.color;

            set
            {
                this.color = value;
                this.OnPropertyChanged();
            }
        }

        public Brush BorderColor
        {
            get => this.borderColor;
            set
            {
                this.borderColor = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsAllowed
        {
            get => this.isAllowed;

            set
            {
                this.BorderColor = value ? Brushes.White : Brushes.Red;
                this.isAllowed = value;
            }
        }

        public IList<AttributeViewModel> Attributes
        {
            get => this.attributes;

            set
            {
                this.attributes = value;
                this.OnPropertyChanged();
            }
        }

        public string Picture
        {
            get => "pack://application:,,,/" + this.picture;

            set
            {
                this.picture = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsVirtual { get; set; } = false;

        public override string ToString()
        {
            return this.Name;
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
