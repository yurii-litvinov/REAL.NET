namespace EditorPrototype.Models.Toolbar
{
    public interface IButton
    {
        ICommand GetAction();

        void DoAction();

        string Description { get; }

        string Image { get; }
    }
}