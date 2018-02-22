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

namespace WpfEditor.AirSim
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using WpfControlsLib.Model;
    using WpfControlsLib.ViewModel;

    /// <summary>
    /// Class for execution of visual program on AirSim
    /// </summary>
    internal class CodeExecution
    {
        private static Action<string> writeToMessageBox;
        private static Graph graph;
        private static bool condition;

        /// <summary>
        /// Main execution method
        /// </summary>
        /// <param name="programGraph"> Visual program to execute </param>
        /// <param name="writeToConsole"> Method to writing to console </param>
        public void Execute(Graph programGraph, Action<string> writeToConsole)
        {
            writeToMessageBox = writeToConsole;
            graph = programGraph;

            NodeViewModel curNode = this.GetInitNode();
            if (curNode == null)
                return;

            writeToMessageBox("Running your code");
            var client = new MultirotorClient();
            while (curNode.Name != "aFinalNode")
            {
                Execution.ExecuteNode(curNode, client);
                curNode = Execution.GetNextNode(curNode, client);
                if (curNode == null)
                    return;
                writeToMessageBox($"Node {curNode.Name} done");
            }
            client.Land();
            client.Dispose();
            writeToMessageBox("Program done");
        }

        private NodeViewModel GetInitNode()
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

        private static bool CheckCondition(MultirotorClient client, string conditionString)
        {
            string sourceCode =
                @"using System; 
                using System.IO;
                using WpfEditor.AirSim;
                public class Code
                { 
                    public bool Exe(MultirotorClient client)
                    {
                        return " + conditionString + @";
                    }
                }";
            return bool.Parse(EvalCode("Code", "Exe", sourceCode, new object[] { client }));
        }

        private static string EvalCode(string typeName, string methodName, string sourceCode, object[] args)
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

                output = InvokeMethod(evaluatorType, methodName, evaluator, args).ToString();
                return output;
            }

            output = "\rHouston, we have a problem at compile time!";
            return results.Errors.Cast<CompilerError>().Aggregate(output, (current, ce) => current +
                                                                                           $"\rline {ce.Line}: {ce.ErrorText}");
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        private static object InvokeMethod(Type evaluatorType, string methodName, object evaluator, object[] args)
        {
            return evaluatorType.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, evaluator, args);
        }

        #region NodeFunctions

        private abstract class NodeExecution
        {
            public abstract void ExecuteNode(NodeViewModel node, MultirotorClient client);

            public virtual NodeViewModel GetNextNode(NodeViewModel node, MultirotorClient client)
            {
                if (graph.DataGraph.OutEdges(node).Count() > 1)
                {
                    writeToMessageBox("Error: Node " + node.Name + " has more than the 1 out edge ");
                    client.Dispose();
                    return null;
                }
                if (!graph.DataGraph.OutEdges(node).Any())
                {
                    writeToMessageBox("Error: Node " + node.Name +
                                      " has no out edges and it is not the final node ");
                    client.Dispose();
                    return null;
                }
                return graph.DataGraph.OutEdges(node).ToList()[0].Target;
            }
        }

        private class InitNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
            {
                client.CreateClient();
                client.ConfirmConnection();
                client.EnableApiControl();
            }
        }

        private class TakeoffNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
                => client.Takeoff(float.Parse(node.Attributes[0].Value));
        }

        private class MoveNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
                => client.MoveByVelocityZ(float.Parse(node.Attributes[0].Value));
        }

        private class LandNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
                => client.Land();
        }

        private class TimerNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
                => client.Sleep(float.Parse(node.Attributes[0].Value));
        }

        private class HoverNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
            {
                client.Hover();
                client.EnableApiControl();
            }
        }

        private class IfNode : NodeExecution
        {
            public override void ExecuteNode(NodeViewModel node, MultirotorClient client)
                => condition = CheckCondition(client, node.Attributes[0].Value);

            public override NodeViewModel GetNextNode(NodeViewModel node, MultirotorClient client)
            {
                if (graph.DataGraph.OutEdges(node).Count() != 2)
                {
                    writeToMessageBox("Error: ifNode out edges count is not equal 2 ");
                    client.Dispose();
                    return null;
                }
                if (condition)
                {
                    EdgeViewModel edge = graph.DataGraph.OutEdges(node).ToList()[0];
                    return edge.Attributes[0].Value == "true"
                        ? edge.Target
                        : graph.DataGraph.OutEdges(node).ToList()[1].Target;
                }
                else
                {
                    EdgeViewModel edge = graph.DataGraph.OutEdges(node).ToList()[0];
                    return edge.Attributes[0].Value == "false"
                        ? edge.Target
                        : graph.DataGraph.OutEdges(node).ToList()[1].Target;
                }
            }
        }

        private static class Execution
        {
            private static readonly Dictionary<string, NodeExecution> strategies =
                new Dictionary<string, NodeExecution>();

            static Execution()
            {
                strategies.Add("aInitialNode", new InitNode());
                strategies.Add("aTakeoff", new TakeoffNode());
                strategies.Add("aMove", new MoveNode());
                strategies.Add("aTimer", new TimerNode());
                strategies.Add("aHover", new HoverNode());
                strategies.Add("aIfNode", new IfNode());
                strategies.Add("aLand", new LandNode());
            }

            public static void ExecuteNode(NodeViewModel node, MultirotorClient client)
                => strategies[node.Name].ExecuteNode(node, client);

            public static NodeViewModel GetNextNode(NodeViewModel node, MultirotorClient client)
                => strategies[node.Name].GetNextNode(node, client);
        }

        #endregion
    }
}
