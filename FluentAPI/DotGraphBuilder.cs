using System;
using System.Linq;
using System.Collections.Generic;

namespace FluentApi.Graph
{
    public enum NodeShape
    {
        Box,
        Ellipse
    }
    public class AddNodeBuilder : DotGraphBuilder
    {
        private Graph graph;
        private string nodeName;

        public AddNodeBuilder(Graph myGraph, string myNodeName) : base(myGraph)
        {
            graph = myGraph;
            nodeName = myNodeName;
        }

        public DotGraphBuilder With(Action<BuilderNode> action)
        {
            foreach (var node in graph.Nodes)
                if (node.Name == nodeName)
                {
                    action(new BuilderNode(node));
                    break;
                }
            return this;
        }
    }

    public class AddEdgeBuilder : DotGraphBuilder
    {
        private Graph graph;
        private string fromName;
        private string toName;

        public AddEdgeBuilder(Graph myGraph, string nodeFromName, string nodeToName) : base(myGraph)
        {
            graph = myGraph;
            fromName = nodeFromName;
            toName = nodeToName;
        }

        public DotGraphBuilder With(Action<BuilderEdge> action)
        {
            foreach (var edge in graph.Edges)
                if (edge.SourceNode == fromName && edge.DestinationNode == toName)
                {
                    action(new BuilderEdge(edge));
                    break;
                }
            return this;
        }
    }

    public class DotGraphBuilder
    {
        private Graph dotGraph;

        protected DotGraphBuilder(Graph graph) => dotGraph = graph;

        public static DotGraphBuilder DirectedGraph(string graphName) => new DotGraphBuilder(new Graph(graphName, true, false));

        public static DotGraphBuilder NondirectedGraph(string graphName) => new DotGraphBuilder(new Graph(graphName, false, false));

        public AddNodeBuilder AddNode(string nodeName)
        {
            var nodeBuilder = new AddNodeBuilder(dotGraph, nodeName);
            dotGraph.AddNode(nodeName);
            return nodeBuilder;
        }

        public AddEdgeBuilder AddEdge(string nodeFromName, string nodeToName)
        {
            var edgeBuilder = new AddEdgeBuilder(dotGraph, nodeFromName, nodeToName);
            dotGraph.AddEdge(nodeFromName, nodeToName);
            return edgeBuilder;
        }

        public string Build() => dotGraph.ToDotFormat();
    }


    public class BuilderNode : AttributesBuilder<BuilderNode>
    {
        public BuilderNode(GraphNode node) => Attributes = node.Attributes;
        public BuilderNode Shape(NodeShape shape)
        {
            Attributes["shape"] = shape.ToString().ToLower();
            return this;
        }
    }

    public class BuilderEdge : AttributesBuilder<BuilderEdge>
    {
        public BuilderEdge(GraphEdge edge) => Attributes = edge.Attributes;
        public BuilderEdge Weight(int weight)
        {
            Attributes["weight"] = weight.ToString();
            return this;
        }
    }
    public class AttributesBuilder<TypeAttribute>
        where TypeAttribute : class
    {
        protected Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public TypeAttribute Label(string label)
        {
            Attributes["label"] = label;
            return this as TypeAttribute;
        }

        public TypeAttribute FontSize(int fontSize)
        {
            Attributes["fontsize"] = fontSize.ToString();
            return this as TypeAttribute;
        }

        public TypeAttribute Color(string color)
        {
            Attributes["color"] = color;
            return this as TypeAttribute;
        }

    }
}