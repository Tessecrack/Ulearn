using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<TValue> : IEnumerable<TValue>
        where TValue : IComparable
    {
        public BinaryTree() { }
        public class Node
        {
            public Node LeftSide { get; set; }
            public Node RightSide { get; set; }
            public TValue Value { get; set; }
            public Node(TValue value)
            {
                Value = value;
                LeftSide = null;
                RightSide = null;
            }
        }

        public Node Root { get; set; }
        public Node Left => Root.LeftSide;
        public Node Right => Root.RightSide;
        public TValue Value { get => Root.Value; set { } }
        public int Count { get; set; }

        private bool IsNull(Node node) => node == null;
        public void Add(TValue value)
        {
            if (Root == null) Root = new Node(value);
            else
            {
                Node currentNode = Root;
                while (true)
                {
                    int comparator = currentNode.Value.CompareTo(value);
                    if (comparator >= 0)
                    {
                        if (IsNull(currentNode.LeftSide))
                        { currentNode.LeftSide = new Node(value); return; }
                        else currentNode = currentNode.LeftSide;
                    }
                    else 
                    {
                        if (IsNull(currentNode.RightSide))
                        { currentNode.RightSide = new Node(value); return; }
                        else currentNode = currentNode.RightSide;
                    }
                }
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            if (Root != null)
            {
                var currentNode = Root;
                var nodes = new Stack<Node>();
                nodes.Push(currentNode);
                bool flag = true;
                while (nodes.Count > 0)
                {
                    if (flag)
                    {
                        while (currentNode.LeftSide != null)
                        {
                            nodes.Push(currentNode);
                            currentNode = currentNode.LeftSide;
                        }
                    }
                    yield return currentNode.Value;
                    flag = currentNode.RightSide != null;
                    currentNode = flag ? currentNode.RightSide : nodes.Pop();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class BinaryTree
    {
        public static BinaryTree<TValue> Create<TValue>(params TValue[] elements)
            where TValue : IComparable
        {
            var wood = new BinaryTree<TValue>();
            foreach (var element in elements)
                wood.Add(element);
            return wood;
        }
    }
}
