namespace EditorPrototype.Models.Toolbar
{
    public interface ICommand
    {
        bool IsCanBeUndone { get; }

        void Execute();

        void Undo();
    }
}