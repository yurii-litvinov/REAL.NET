using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfEditor.Controls.ModelExplorer
{
    public class ModelExplorerElement: INotifyPropertyChanged
    {
        public ModelExplorerElement(Repo.IElement element)
        {
            this.Element = element;
        }

        public Repo.IElement Element { get; }

        public Repo.Metatype Metatype => this.Element.Metatype;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ModelExplorerNode : ModelExplorerElement
    {
        public ModelExplorerNode(Repo.IElement element)
        : base(element)
        {
        }

        public string Name => this.Element.Name;
        public string Image => "pack://application:,,,/" + this.Element.Shape;
    }

    public class ModelExplorerEdge : ModelExplorerElement
    {
        public ModelExplorerEdge(Repo.IElement element)
            : base(element)
        {
        }

        public string Source => (this.Element as Repo.IEdge)?.From.Name;
        public string Target => (this.Element as Repo.IEdge)?.To.Name;
    }
}
