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

namespace AirSim.AirSimLib
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using Repo;

    /// <summary>
    /// Class for execution of visual program on AirSim
    /// </summary>
    internal class CodeExecution
    {
        private static int recursionLevel = 0;
        private static Stack<INode> recursionNodes = new Stack<INode>();

        /// <summary>
        /// Main execution method
        /// </summary>
        /// <param name="programGraph"> Visual program to execute </param>
        /// <param name="writeToConsole"> Method to writing to console </param>
        public void Execute(IModel programGraph, Action<string> writeToConsole)
        {
            var curNode = this.GetInitNode(programGraph, writeToConsole);
            if (curNode == null)
            {
                return;
            }

            writeToConsole("Running your code");
            var client = new MultirotorClient();
            while (recursionLevel >= 0)
            {
                Execution.Exec.ExecuteNode(ref curNode, client, programGraph, writeToConsole);
                curNode = Execution.Exec.GetNextNode(curNode, client, programGraph, writeToConsole);
                if (curNode == null)
                {
                    return;
                }

                writeToConsole($"Node {curNode.Name} done");
            }

            client.Land();
            client.Dispose();
            writeToConsole("Program done");
        }

        private INode GetInitNode(IModel graph, Action<string> writeToMessageBox)
        {
            INode initNode = null;
            var nodes = graph.Nodes.ToList();
            int cnt = 0;
            foreach (var node in nodes)
            {
                if (node.Name == "aInitialNode")
                {
                    initNode = node;
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

        #region NodeFunctions

        private abstract class NodeExecution
        {
            public abstract void ExecuteNode(INode node, MultirotorClient client);

            public virtual INode GetNextNode(
                                                INode node,
                                                MultirotorClient client,
                                                IModel graph,
                                                Action<string> writeToMessageBox)
            {
                var outEdges = graph.Edges.Where(x => x.From == node).ToList();
                if (outEdges.Count > 1)
                {
                    writeToMessageBox("Error: Node " + node.Name + " has more than the 1 out edge ");
                    client.Dispose();
                    return null;
                }

                if (outEdges.Count == 0)
                {
                    if (recursionNodes.Count > 0)
                    {
                        return recursionNodes.Pop();
                    }

                    writeToMessageBox("Error: Node " + node.Name +
                                      " has no out edges and it is not the final node ");
                    client.Dispose();
                    return null;
                }

                return (INode)outEdges[0].To;
            }
        }

        private class InitNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
            {
                if (recursionLevel == 0)
                {
                    client.CreateClient();
                    client.ConfirmConnection();
                    client.EnableApiControl();
                }
            }
        }

        private class FinalNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
            {
                --recursionLevel;
            }
        }

        private class TakeoffNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
                => client.Takeoff(float.Parse(node.Attributes.ToList()[0].StringDefaultValue));
        }

        private class MoveNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
            {
                var t = float.Parse(node.Attributes.ToList()[0].StringDefaultValue);
                client.EnableApiControl();
                client.MoveByVelocityZ(float.Parse(node.Attributes.ToList()[0].StringDefaultValue));
            }
        }

        private class LandNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
                => client.Land();
        }

        private class TimerNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
                => client.Sleep(float.Parse(node.Attributes.ToList()[0].StringDefaultValue));
        }

        private class HoverNode : NodeExecution
        {
            public override void ExecuteNode(INode node, MultirotorClient client)
            {
                client.Hover();
                client.EnableApiControl();
            }
        }

        private class IfNode : NodeExecution
        {
            private bool condition;

            public override void ExecuteNode(INode node, MultirotorClient client)
                => this.condition = this.CheckCondition(client, node.Attributes.ToList()[0].StringDefaultValue);

            private bool CheckCondition(MultirotorClient client, string conditionString)
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
                bool ans = false;
                if (!bool.TryParse(this.EvalCode("Code", "Exe", sourceCode, new object[] { client }), out ans))
                {
                    return true;
                }

                return ans;
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
                return results.Errors.Cast<CompilerError>().Aggregate(output, (current, ce) => current + $"\rline {ce.Line}: {ce.ErrorText}");
            }

            [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
            private object InvokeMethod(Type evaluatorType, string methodName, object evaluator, object[] args)
            {
                return evaluatorType.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, evaluator, args);
            }

            public override INode GetNextNode(INode node, MultirotorClient client, IModel graph, Action<string> writeToMessageBox)
            {
                var outEdges = graph.Edges.Where(x => x.From == node).ToList();
                if (outEdges.Count != 2)
                {
                    writeToMessageBox("Error: ifNode out edges count is not equal 2 ");
                    client.Dispose();
                    return null;
                }

                IEdge edge = outEdges[0];
                if (this.condition)
                {
                    return (INode)(edge.Attributes.ToList()[0].StringDefaultValue == "true"
                        ? edge.To
                        : outEdges[1].To);
                }
                else
                {
                    return (INode)(edge.Attributes.ToList()[0].StringDefaultValue == "false"
                        ? edge.To
                        : outEdges[1].To);
                }
            }
        }

        private class Execution
        {
            private static readonly Dictionary<string, NodeExecution> strategies =
                new Dictionary<string, NodeExecution>();

            private static Execution instance;

            private Execution()
            {
                strategies.Add("aInitialNode", new InitNode());
                strategies.Add("aFinalNode", new FinalNode());
                strategies.Add("aTakeoff", new TakeoffNode());
                strategies.Add("aMove", new MoveNode());
                strategies.Add("aTimer", new TimerNode());
                strategies.Add("aHover", new HoverNode());
                strategies.Add("aIfNode", new IfNode());
                strategies.Add("aLand", new LandNode());

                strategies.Add("InitialNode", new InitNode());
                strategies.Add("FinalNode", new FinalNode());
                strategies.Add("Takeoff", new TakeoffNode());
                strategies.Add("Move", new MoveNode());
                strategies.Add("Timer", new TimerNode());
                strategies.Add("Hover", new HoverNode());
                strategies.Add("IfNode", new IfNode());
                strategies.Add("Land", new LandNode());
            }

            public static Execution Exec => instance ?? (instance = new Execution());

            public void ExecuteNode(ref INode node, MultirotorClient client, IModel graph, Action<string> writeToMessageBox)
            {
                if (!strategies.ContainsKey(node.Name))
                {
                    // TODO: Functions were removed from repo, so this code does not work now.
                    recursionNodes.Push(strategies["aInitialNode"].GetNextNode(node, client, graph, writeToMessageBox));
                    ++recursionLevel;
                    this.ExecuteNode(ref node, client, graph, writeToMessageBox);
                    return;
                }

                strategies[node.Name].ExecuteNode(node, client);
            }

            public INode GetNextNode(
                                        INode node,
                                        MultirotorClient client,
                                        IModel graph,
                                        Action<string> writeToMessageBox)
            {
                if (!strategies.ContainsKey(node.Name))
                {
                    return strategies["aInitialNode"].GetNextNode(node, client, graph, writeToMessageBox);
                }

                return strategies[node.Name].GetNextNode(node, client, graph, writeToMessageBox);
            }
        }

        #endregion
    }
}