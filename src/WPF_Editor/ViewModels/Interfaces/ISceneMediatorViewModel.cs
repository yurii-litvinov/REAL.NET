using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using WPF_Editor.ViewModels.Helpers;

namespace WPF_Editor.ViewModels.Interfaces
{
    public interface ISceneMediatorViewModel
    {
        ModelElement GetInstanceOfSelectedType();
    }
}
