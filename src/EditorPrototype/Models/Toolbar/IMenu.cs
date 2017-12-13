using System.Collections.Generic;

namespace EditorPrototype.Models.Toolbar
{
    public interface IMenu
    {
        bool IsMenuType { get; }

        IList<IMenu> GetChildren();

        void DoAction();

        ICommand GetAction();
    }
}