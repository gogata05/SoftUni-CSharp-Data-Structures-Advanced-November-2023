using System.Runtime.CompilerServices;

namespace AVLTree
{
    using System;

    public class AVL<T> where T : IComparable<T>
    {
        public class Node
        {
            public Node(T value)
            {
                this.Value = value;
                this.Height = 1;
            }

            public T Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public int Height { get; set; }
        }

        public Node Root { get; private set; }

        public bool Contains(T element)
        {
            Node node = this.FindNode(this.Root, element);

            return node != null;
        }

        private Node FindNode(Node node, T element)
        {
            if (node is null)
            {
                return null;
            }

            if (element.CompareTo(node.Value) < 0)
            {
                return this.FindNode(node.Left, element);
            }
            else if (element.CompareTo(node.Value) > 0)
            {
                return this.FindNode(node.Right, element);
            }
            else
            {
                return node;
            }
        }

        public void Delete(T element)
        {
            if (this.Root is null)
            {
                return;
            }

            this.Root = this.Delete(this.Root, element);
        }

        private Node Delete(Node node, T element)
        {
            if (node is null)
            {
                return null;
            }

            if (element.CompareTo(node.Value) < 0)
            {
                node.Left = this.Delete(node.Left, element);
            }
            else if (element.CompareTo(node.Value) > 0)
            {
                node.Right = this.Delete(node.Right, element);
            }
            else
            {
                if (node.Left is null && node.Right is null)
                {
                    return null;
                }
                else if (node.Left is null)
                {
                    node = node.Right;
                }
                else if (node.Right is null)
                {
                    node = node.Left;
                }
                else
                {
                    Node minRightChild = this.FindSmallestNode(node.Right);

                    node.Value = minRightChild.Value;

                    node.Right = this.Delete(node.Right, minRightChild.Value);
                }
            }

            node = this.Balance(node);
            node.Height = this.UpdateHeight(node);
            return node;
        }

        private Node FindSmallestNode(Node node)
        {
            if (node.Left is null)
            {
                return node;
            }

            return this.FindSmallestNode(node.Left);
        }

        public void DeleteMin()
        {
            if (this.Root is null)
            {
                return;
            }

            Node deleteNode = this.FindSmallestNode(this.Root);

            this.Delete(deleteNode.Value);
        }

        public void Insert(T element)
        {
            this.Root = this.Insert(this.Root, element);
        }

        private Node Insert(Node node, T element)
        {
            if (node is null)
            {
                return new Node(element);
            }

            if (element.CompareTo(node.Value) < 0)
            {
                node.Left = this.Insert(node.Left, element);
            }
            else if (element.CompareTo(node.Value) > 0)
            {
                node.Right = this.Insert(node.Right, element);
            }
            
            node = this.Balance(node);
            node.Height = this.UpdateHeight(node);

            return node;
        }

        private Node Balance(Node node)
        {
            int balanceFactory = Height(node.Left) - Height(node.Right);
            if (balanceFactory > 1)
            {
                int childBalance = this.Height(node.Left.Left) - this.Height(node.Left.Right);
                if (childBalance < 0)
                {
                    node.Left = this.RotateLeft(node.Left);
                }
                node = RotateRight(node);
            }
            else if (balanceFactory < - 1)
            { 
                int childBalance = this.Height(node.Right.Left) - this.Height(node.Right.Right);

                if (childBalance > 0)
                {
                    node.Right = this.RotateRight(node.Right);
                }

                node = RotateLeft(node);
            }
            
            return node;
        }

        public void EachInOrder(Action<T> action)
        {
            this.EachInOrder(this.Root, action);
        }

        private void EachInOrder(Node node, Action<T> action)
        {
            if (node is null)
            {
                return;
            }

            this.EachInOrder(node.Left, action);
            action(node.Value);
            this.EachInOrder(node.Right, action);
        }

        //rotations methods
        private Node RotateLeft(Node node)
        {
            var temp = node.Right;
            node.Right = temp.Left;
            temp.Left = node;

            node.Height = this.UpdateHeight(node);

            return temp;
        }
        private Node RotateRight(Node node)
        {
            var temp = node.Left;
            node.Left = temp.Right;
            temp.Right = node;

            node.Height = this.UpdateHeight(node);

            return temp;
        }

        //Height methods
        private int Height(Node node)
        {
            if (node is null)
            {
                return 0;
            }
            return node.Height;
        }

        private int UpdateHeight(Node node)
        {
            return node.Height = Math.Max(this.Height(node.Left), this.Height(node.Right)) + 1;
        }
    }
}
