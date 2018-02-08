/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Security.Permissions;
using WpfEditor.Model;
using WpfEditor.ViewModel;

namespace WpfEditor.AirSim
{
    /// <summary>
    /// Class for execution of visual program on AirSim
    /// </summary>
    internal class CodeExecution
    {
        private Action<string> writeToMessageBox;

        /// <summary>
        /// Main execution method
        /// </summary>
        /// <param name="graph"> Visual program to execute </param>
        /// <param name="writeToConsole"> Method to writing to console </param>
        public void Execute(Graph graph, Action<string> writeToConsole)
        {
            writeToMessageBox = writeToConsole;

            NodeViewModel curNode = GetInitNode(graph);
            if (curNode == null)
                return;

            writeToMessageBox("Running your code");
            var client = new MultirotorClient();
            while (curNode.Name != "aFinalNode")
            {
                var name = curNode.Name;
                bool condition = false;
                switch (curNode.Name)
                {
                    case "aInitialNode":
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
                        writeToMessageBox("Error: Node " + curNode.Name + " has more than the 1 out edge ");
                        client.Dispose();
                        return;
                    }
                    if (!graph.DataGraph.OutEdges(curNode).Any())
                    {
                        writeToMessageBox("Error: Node " + curNode.Name +
                                           " has no out edges and it is not the final node ");
                        client.Dispose();
                        return;
                    }
                    curNode = graph.DataGraph.OutEdges(curNode).ToList()[0].Target;
                }
                else
                {
                    if (graph.DataGraph.OutEdges(curNode).Count() != 2)
                    {
                        writeToMessageBox("Error: ifNode out edges count is not equal 2 ");
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
                writeToMessageBox($"Node {name} done");
            }
            client.Land();
            client.Dispose();
            writeToMessageBox("Program done");
        }

        private NodeViewModel GetInitNode(Graph graph)
        {
            NodeViewModel initNode = null;
            var edges = graph.DataGraph.Edges.ToList();
            int cnt = 0;
            foreach (var edge in edges)
            {
                if (edge.Source.Name == "aInitialNode")
                {
                    initNode = edge.Source;
                    ++cnt;
                }
            }
            if (initNode == null || cnt > 1)
            {
                writeToMessageBox("Error: ");
                writeToMessageBox(initNode == null
                    ? "There is no initial nodes"
                    : "There is more than one initial nodes");
                writeToMessageBox("");
                return null;
            }
            return initNode;
        }

        private bool CheckCondition(MultirotorClient client, string condition)
        {
            string sourceCode =
                @"using System; 
                using System.IO;
                using WpfEditor.AirSim;
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
                ReferencedAssemblies = { "WpfEditor.exe" }
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

            output = "\rHouston, we have a problem at compile time!";
            return results.Errors.Cast<CompilerError>().Aggregate(output, (current, ce) => current +
                                                                                           $"\rline {ce.Line}: {ce.ErrorText}");
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        private object InvokeMethod(Type evaluatorType, string methodName, object evaluator, object[] args)
        {
            return evaluatorType.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, evaluator, args);
        }
    }
}
