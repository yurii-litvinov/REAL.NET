using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.ProgramRunner.Interpeter
{
    public class LogoInterpeter
    {
        LogoInterpeter(IModel model)
        {
            Model = model;
        }

        public IModel Model { get; }
    }
}
