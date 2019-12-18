using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.Models
{
    public interface ITurtle
    {
        double Speed { get; }

        double X { get; }

        double Y { get; }

        double Angle { get; }
    }
}
