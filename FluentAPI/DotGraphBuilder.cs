using NUnit.Framework;
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

    public class DotGraphBuilder
    {
	public static DotGraph DirectedGraph(string graphName) => new DotGraph(graphName, "digraph");
        public static DotGraph NondirectedGraph(string graphName) => new DotGraph(graphName, "graph");
    }

    public class DotGraph
    {
        protected Graph dotGraph;
        private readonly bool isDirectGraph = false;
        public DotGraph(Graph graph) => dotGraph = graph;
        public DotGraph(string graphName, string typeGraph)
        {
            if (typeGraph == "digraph")
                isDirectGraph = true;
            dotGraph = new Graph(graphName, isDirectGraph, false);
        }

        public string Build() => dotGraph.ToDotFormat();

        public DotNode AddNode(string node)
        {
            dotGraph.AddNode(node);
            return new DotNode(graph: dotGraph, node);
        }

        public DotEdge AddEdge(string from, string to)
        {
            dotGraph.AddEdge(from, to);
            return new DotEdge(graph: dotGraph, from, to);
        }
    }

    public class DotNode : DotGraph
    {
        private readonly string node;
        public DotNode(Graph graph, string node) : base (graph) => this.node = node;
        public DotGraph With(Action<NodeBuilder> action)
        {
            foreach(var element in dotGraph.Nodes)
                if (element.Name == node)
                {
                    action(new NodeBuilder(element));
                    break;
                }
            return this;
        }
    }

    public class DotEdge : DotGraph
    {
        private readonly (string from, string to) edge;
        public DotEdge(Graph graph, string from, string to) : base(graph) => edge = (from, to);
        public DotGraph With(Action<EdgeBuilder> action)
        {
            foreach(var element in dotGraph.Edges)
                if (element.SourceNode == edge.from && element.DestinationNode == edge.to)
                {
                    action(new EdgeBuilder(element));
                    break;
                }
            return this;
        }
    }

    public class NodeBuilder : Attributess<NodeBuilder>
    {
        public NodeBuilder(GraphNode node) => Attributes = node.Attributes;
        public NodeBuilder Shape(NodeShape shape)
        {
            Attributes["shape"] = shape.ToString().ToLower();
            return this;
        }
    }

    public class EdgeBuilder : Attributess<EdgeBuilder>
    {
        public EdgeBuilder(GraphEdge edge) => Attributes = edge.Attributes;
        public EdgeBuilder Weight(int weight)
        {
            Attributes["weight"] = weight.ToString();
            return this;
        }
    }

    public class Attributess<T> where T : class
    {
        protected Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public T Color(string clr)
        {
            Attributes["color"] = clr;
            return this as T;
        }
		
        public T Label(string lbl)
        {
            Attributes["label"] = lbl;
            return this as T;
        }

        public T FontSize(int fSize)
        {
            Attributes["fontsize"] = fSize.ToString();
            return this as T;
        }
    }
}
