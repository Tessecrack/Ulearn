using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public static class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] values) where T : IComparable
        {
            var tree = new BinaryTree<T>();
            foreach (var value in values)
                tree.Add(value);
            return tree;
        }
    }

    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable
    {
        private SortedSet<T> sortedSet = new SortedSet<T>();
        public class Node
        {
            internal T Value;
            internal Node Left;
            internal Node Right;
            public Node(T value)
            {
                this.Value = value;
                Left = null;
                Right = null;
            }
        }

        private Node Root { get; set; }
        public T Value => Root.Value;
        public Node Left => Root.Left;
        public Node Right => Root.Right;
        public void Add(T element)
        {
            if (Root == null)
            {
                Root = new Node(element);
                sortedSet.Add(element);
                return;
            }

            Node currentNode = Root;
            while (true)
            {
                if ((element.CompareTo(currentNode.Value) == -1 || element.CompareTo(currentNode.Value) == 0) 
                    && CheckNode(element, ref currentNode, ref currentNode.Left)) 
                    return;
                if (element.CompareTo(currentNode.Value) == 1
                    && CheckNode(element, ref currentNode, ref currentNode.Right))
                    return;
            }
        }

        private bool CheckNode(T element, ref Node currentNode, ref Node nextNode)
        {
            if (nextNode != null)
            {
                currentNode = nextNode;
                return false;
            }
            else
            {
                nextNode = new Node(element);
                sortedSet.Add(element);
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var element in sortedSet)
                yield return element;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
