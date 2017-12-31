namespace EditorPrototype.Models.ControlFactory
{
    using PluginLibrary.MainInterfaces;

    public interface IControlFactory
    {
        IToolbar CreateToolbar();

        IConsole CreateConsole();

        IPalette CreatePalette();

        IScene CreateScene();

        IScene CreateNewScene();
    }
}
