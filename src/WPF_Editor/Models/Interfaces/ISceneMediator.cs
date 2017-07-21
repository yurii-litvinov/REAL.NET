using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using WPF_Editor.ViewModels;
namespace WPF_Editor.Models.Interfaces
{
    public interface ISceneMediator
    {

        IScene Scene { get; }
        Element GetSelectedPaletteItem();
    }
}
