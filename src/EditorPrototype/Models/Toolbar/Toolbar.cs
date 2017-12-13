using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorPrototype.Models.Toolbar
{
    public class Toolbar : IToolbar
    {
        public IList<IButton> Buttons => throw new NotImplementedException();

        public void AddButton(ICommand command, string desription, string image)
        {
            throw new NotImplementedException();
        }

        public void AddMenu(IMenu menu)
        {
            throw new NotImplementedException();
        }
    }
}
