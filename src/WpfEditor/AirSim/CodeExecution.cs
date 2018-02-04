using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Security.Permissions;
using Repo.DataLayer;
using WpfEditor.Model;
using WpfEditor.ViewModel;

namespace WpfEditor.AirSim
{
    class CodeExecution
    {
        public void Execute(object pair)
        {
            var tuple = pair as Tuple<Graph, Action<string>>;
            var graph = tuple.Item1;
            Action<string> writeToMessageBox = tuple.Item2;
            
            var edges = graph.DataGraph.Edges.ToList();
            NodeViewModel initNode = null;
            int cnt = 0;
            foreach (var edge in edges)
            {
                if (edge.Source.Name == "aInitialNode")
                {
                    initNode = edge.Source;
                    ++cnt;
                }
            }
            if (cnt != 1)
            {
                writeToMessageBox("Error: ");
                if (initNode == null)
                {
                    writeToMessageBox("There is no initial nodes");
                }
                else
                {
                    writeToMessageBox("There is more than one initial nodes");
                }
                writeToMessageBox("\n");
                return;
            }

            NodeViewModel curNode = initNode;
            MultirotorClient client = null;
            while (curNode.Name != "aFinalNode")
            {
                var name = curNode.Name;
                bool condition = false;
                switch (curNode.Name)
                {
                    case "aInitialNode":
                        client = new MultirotorClient();
                        client.CreateClient();
                        client.ConfirmConnection();
                        client.EnableApiControl();
                        break;
                    case "aTakeoff":
                        client.Takeoff(float.Parse(curNode.Attributes[0].Value));
                        break;
                    case "aMove":
                        client.MoveByVelocityZ(float.Parse(curNode.Attributes[0].Value));
                        break;
                    case "aLand":
                        client.Land();
                        break;
                    case "aTimer":
                        client.Sleep(float.Parse(curNode.Attributes[0].Value));
                        break;
                    case "aHover":
                        client.Hover();
                        client.EnableApiControl();
                        break;
                    case "aIfNode":
                        condition = CheckCondition(client, curNode.Attributes[0].Value);
                        break;
                }

                if (curNode.Name != "aIfNode")
                {
                    if (graph.DataGraph.OutEdges(curNode).Count() > 1)
                    {
                        writeToMessageBox("Error: Node " + curNode.Name + " has more than the 1 out edge \n");
                        client.Dispose();
                        return;
                    }
                    if (!graph.DataGraph.OutEdges(curNode).Any())
                    {
                        writeToMessageBox("Error: Node " + curNode.Name +
                                           " has no out edges and it is not the final node \n");
                        client.Dispose();
                        return;
                    }
                    curNode = graph.DataGraph.OutEdges(curNode).ToList()[0].Target;
                }
                else
                {
                    if (graph.DataGraph.OutEdges(curNode).Count() != 2)
                    {
                        writeToMessageBox("Error: ifNode out edges count is not equal 2 \n");
                        client.Dispose();
                        return;
                    }
                    if (condition)
                    {
                        EdgeViewModel edge = graph.DataGraph.OutEdges(curNode).ToList()[0];
                        curNode = edge.Attributes[0].Value == "true"
                            ? edge.Target
                            : graph.DataGraph.OutEdges(curNode).ToList()[1].Target;
                    }
                    else
                    {
                        EdgeViewModel edge = graph.DataGraph.OutEdges(curNode).ToList()[0];
                        curNode = edge.Attributes[0].Value == "false"
                            ? edge.Target
                            : graph.DataGraph.OutEdges(curNode).ToList()[1].Target;
                    }
                }
                writeToMessageBox($"Node {name} done\n");
            }
            client.Land();
            client.Dispose();
            writeToMessageBox("Program done\n");
        }

        private bool CheckCondition(MultirotorClient client, string condition)
        {
            string sourceCode =
                @"using System; 
                using System.IO;
                using EditorPrototype;
                    public class Code
                    { 
                        public bool Exe(MultirotorClient client)
                        {
                            return " + condition + @";
                        }
                    }";
            return bool.Parse(EvalCode("Code", "Exe", sourceCode, new object[] { client }));
        }

        private string EvalCode(string typeName, string methodName, string sourceCode, object[] args)
        {
            string output;
            var compiler = CodeDomProvider.CreateProvider("CSharp");
            var parameters = new CompilerParameters
            {
                CompilerOptions = "/t:library",
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                ReferencedAssemblies = { "bin/Debug/EditorPrototype.exe" }
            };

            var results = compiler.CompileAssemblyFromSource(parameters, sourceCode);

            if (!results.Errors.HasErrors)
            {
                var assembly = results.CompiledAssembly;
                var evaluatorType = assembly.GetType(typeName);
                var evaluator = Activator.CreateInstance(evaluatorType);

                output = this.InvokeMethod(evaluatorType, methodName, evaluator, args).ToString();
                return output;
            }

            output = "\r\nHouston, we have a problem at compile time!";
            return results.Errors.Cast<CompilerError>().Aggregate(output, (current, ce) => current +
                                                                                           $"\r\nline {ce.Line}: {ce.ErrorText}");
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        private object InvokeMethod(Type evaluatorType, string methodName, object evaluator, object[] args)
        {
            return evaluatorType.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, evaluator, args);
        }
    }
}
