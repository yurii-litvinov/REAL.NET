using System;
using System.Collections.Generic;

namespace REAL.NET.Models
{
    public class Console
    {
        private ConsoleWindow messageConsole;
        private ConsoleWindow errorConsole;

        public Console(List<ConsoleWindow> list)
        {
            messageConsole = new ConsoleWindow();
            errorConsole = new ConsoleWindow();
        }
    }
}
