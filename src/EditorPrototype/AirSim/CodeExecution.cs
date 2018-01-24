using System;
using System.Linq;

namespace EditorPrototype.AirSim
{
    class CodeExecution
    {
        public static void Execute(object pair)
        {
            var tuple = pair as Tuple<Graph, Action<string>>;
            var graph = tuple.Item1;
            Action<string> writeToMessageBox = tuple.Item2;

            var edges = graph.DataGraph.Edges.ToList();
            DataVertex initNode = null;
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

            DataVertex curNode = initNode;
            MutirotorClient client = null;
            while (curNode.Name != "aFinalNode")
            {
                bool condition = false;
                switch (curNode.Name)
                {
                    case "aInitialNode":
                        client = new MutirotorClient();
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
                        condition = true;
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
                        DataEdge edge = graph.DataGraph.OutEdges(curNode).ToList()[0];
                        curNode = edge.Attributes[0].Value == "true"
                            ? edge.Target
                            : graph.DataGraph.OutEdges(curNode).ToList()[1].Target;
                    }
                    else
                    {
                        DataEdge edge = graph.DataGraph.OutEdges(curNode).ToList()[0];
                        curNode = edge.Attributes[0].Value == "false"
                            ? edge.Target
                            : graph.DataGraph.OutEdges(curNode).ToList()[1].Target;
                    }
                }

            }
        }
    }
}
