using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Editor.Models.Interfaces
{
    /// <summary>
    /// This interface is required for connection with Scene
    /// </summary>
    public interface ISceneMediator
    {
        IScene Scene { get; }
    }
}
