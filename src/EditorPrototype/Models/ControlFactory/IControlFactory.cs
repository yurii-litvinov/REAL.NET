namespace EditorPrototype.Models.ControlFactory
{
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;

    public interface IControlFactory
    {
        IToolbar CreateToolbar();

        IConsole CreateConsole();

        IPalette CreatePalette();

        IScene CreateScene();

        IScene CreateNewScene();
    }
}
