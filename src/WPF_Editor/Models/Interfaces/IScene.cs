using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using System.Windows.Input;
namespace WPF_Editor.Models.Interfaces
{
    public interface IScene
    {
        void HandleSingleLeftClick(object sender, MouseButtonEventArgs e);
    }
}
