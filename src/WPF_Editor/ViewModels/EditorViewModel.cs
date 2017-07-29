namespace WPF_Editor.ViewModels
{
    public class EditorViewModel
    {
        public EditorViewModel()
        {
            Mediator = MediatorViewModel.CreateMediator();
        }

        public MediatorViewModel Mediator { get; }
    }
}