using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAL.NET.Models.FakeRepo
{
    using Repo;
    using System.ComponentModel;

    struct NotifyingNode
    {

        private NodeInfo node;

        public string Name { get => node.name; }
        public string ID { get => node.id; }
        public NodeType NodeType { get => node.nodeType; }
    }
}
