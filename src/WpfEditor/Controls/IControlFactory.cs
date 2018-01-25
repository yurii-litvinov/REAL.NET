using EditorPluginInterfaces;
using EditorPluginInterfaces.Toolbar;

namespace WpfEditor.Controls
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
