using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
namespace WPF_Editor.Models.Interfaces
{
    interface IScene
    {
        ISceneMediator Scene_mediator { get; }
        void HandleLeftSingleClick(object click_info);
    }
}
