using EditorPrototype.Models.Console;
using EditorPrototype.Models.Palette;
using EditorPrototype.Models.Scene;
using EditorPrototype.Models.Toolbar;

namespace EditorPrototype.Models.ControlFactory
{
    public interface IControlFactory
    {
        IToolbar CreateToolbar();

        IConsole CreateConsole();

        IPalette CreatePalette();

        IScene CreateScene();

        IScene CreateNewScene();
    }
}
