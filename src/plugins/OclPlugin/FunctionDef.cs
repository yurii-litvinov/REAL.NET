using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
  class FunctionDef
  {
    public string name;
    public List<string> param;
    public HelloParser.ExpressionContext context;
  }
}
