using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class EditorViewModel
    {

        public MediatorViewModel Mediator { get; }

        public EditorViewModel()
        {
            Mediator = MediatorViewModel.CreateMediator();
        }

    }
}