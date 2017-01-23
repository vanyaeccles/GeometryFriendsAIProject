//The A* Algorithm used in the code below was adapted from the tutorial found at http://blog.two-cats.com/2014/06/a-star-example/, which details the basic principles that underline A*.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeometryFriendsAgents
{
    public enum NodeState { Untested, Open, Closed };
    class Node
    {
        public Node parent;
        public Point location; 
        public bool isWalkable = false;     //Whether its an open space or not
        public NodeState state = NodeState.Untested;
        public float G; //cost to get here from the start. Set from the last path that this node was used in.
        public float H; //cost from here to the goal
        public float F
        {
            get { return this.G + this.H; }
        }

        //Constructor
        public Node() { }
        public Node(Point location, bool isWalkable, Point endLocation)
        {
            this.location = location;
            this.state = NodeState.Untested;
            this.isWalkable = isWalkable;
            this.H = GetTraversalCost(this.location, endLocation);
            this.G = 0;
        }

        public void addToChain(Node _parent)
        {
            parent = _parent;
            G = parent.G + GetTraversalCost(this.location, parent.location);

        }

        internal static float GetTraversalCost(Point location, Point otherLocation)
        {
            float deltaX = otherLocation.X - location.X;
            float deltaY = otherLocation.Y - location.Y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}