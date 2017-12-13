using System.Collections.Generic;

namespace EditorPrototype.Models.Toolbar
{
    public interface IToolbar
    {
        IList<IButton> Buttons { get; }

        void AddButton(ICommand command, string desription, string image);

        void AddMenu(IMenu menu);
    }
}
