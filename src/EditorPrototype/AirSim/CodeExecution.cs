using System.Linq;

namespace EditorPrototype.AirSim
{
    class CodeExecution
    {
        public static void Execute(object ograph)
        {
            var graph = ograph as Graph;
            var edges = graph.DataGraph.Edges.ToList();
            DataVertex initNode = null;
            foreach (var edge in edges)
            {
                if (edge.Source.Name == "aInitialNode")
                {
                    initNode = edge.Source;
                }
            }

            DataVertex curNode = initNode;
            MutirotorClient client = null;
            while (curNode.Name != "aFinalNode")
            {
                switch (curNode.Name)
                {
                    case "aInitialNode":
                        client = new MutirotorClient();
                        client.CreateClient();
                        client.ConfirmConnection();
                        client.EnableApiControl();
                        break;
                    case "aTakeoff":
                        client.Takeoff(float.Parse(curNode.Attributes[0].Value)); // TODO: make float
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
                }

                var target = graph.DataGraph.OutEdges(curNode).ToList()[0].Target;
                curNode = target;
            }

        }
    }
}
