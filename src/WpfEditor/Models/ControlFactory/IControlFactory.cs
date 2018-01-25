using EditorPluginInterfaces;
using EditorPluginInterfaces.Toolbar;

namespace WpfEditor.Models.ControlFactory
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
