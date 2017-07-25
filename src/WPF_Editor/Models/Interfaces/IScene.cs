using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using System.Windows.Input;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Models.Interfaces
{
    public interface IScene
    {
        void CreateNode();
        IElement LastCreatedElement { get;}
        
    }
}
