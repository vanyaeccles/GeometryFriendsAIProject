using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI.Perceptions.Information;
using System.Drawing;
using System.Diagnostics;

namespace GeometryFriendsAgents
{
    class Solver
    {
        private Node start, end;
        private Node[,] nodes;
        public Graph graph;
        public Solver(Graph graph)
        {
            this.graph = graph;
            start = new Node(graph.startLocation, true, graph.endLocation);
            start.state = NodeState.Open;
            end = new Node(graph.endLocation, true, graph.endLocation);
        }
        public Queue<Node> solve(CircleRepresentation cI)
        {
            InitializeNodes(graph.map);
            Queue<Node> queue = new Queue<Node>();
            bool completed = search(start);

            if (completed)
            {
                Node curr = this.end;
                while(curr.parent != null)
                {
                    queue.Enqueue(curr);
                    curr = curr.parent;
                }
                queue.Reverse();
            }
            return queue;
        }
        bool search(Node curr)
        {
            curr.state = NodeState.Closed;
            List<Node> adjacentNodes = getAdjacentNodes(curr);

            adjacentNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach (var adjacentNode in adjacentNodes)
            {
                // Check whether the end node has been reached
                if (adjacentNode.location == this.end.location)
                {
                    Debug.Print("End reached at [" + adjacentNode.location.X + "," + adjacentNode.location.Y + "]");
                    end = adjacentNode;
                    return true;
                }
                else
                {
                    // If not, check the next set of nodes
                    Debug.Print("End not reached at ["+adjacentNode.location.X+"," + adjacentNode.location.Y +"], testing its adjacent nodes");
                    if (search(adjacentNode)) // Note: Recurses back into Search(Node)
                        return true;
                }
            }
            return false;
        }
        private List<Node> getAdjacentNodes(Node fromNode)
        {
            List<Node> walkableNodes = new List<Node>();
            IEnumerable<Point> nextLocations = GetAdjacentLocations(fromNode.location);

            foreach (Point location in nextLocations)
            {
                int x = location.X;
                int y = location.Y;

                
                // Stay within the grid's boundaries
                if ((x < 0) || (x >= graph.width) || (y < 0) || (y >= graph.height))
                    continue;

                Node curr = nodes[x, y];

                // Ignore non-walkable nodes
                if (!curr.isWalkable)
                    continue;

                // Ignore already-closed nodes
                if (curr.state == NodeState.Closed)
                    continue;

                // Already-open nodes are only added to the list if their G-value is lower going via this route.
                if (curr.state == NodeState.Open)
                {
                    float traversalCost = Node.GetTraversalCost(curr.location, curr.parent.location);
                    float gTemp = fromNode.G + traversalCost;
                    if (gTemp < curr.G)
                    {
                        if (curr != fromNode)
                            curr.parent = fromNode;
                        walkableNodes.Add(curr);
                    }
                }
                else
                {
                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    if (curr != fromNode)
                        curr.parent = fromNode;
                    curr.state = NodeState.Open;
                    walkableNodes.Add(curr);
                }
            }

            return walkableNodes;
        }
        private static IEnumerable<Point> GetAdjacentLocations(Point fromLocation)
        {
            return new Point[]
            {
                new Point(fromLocation.X-1, fromLocation.Y-1),
                new Point(fromLocation.X-1, fromLocation.Y  ),
                new Point(fromLocation.X-1, fromLocation.Y+1),
                new Point(fromLocation.X,   fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y+1),
                new Point(fromLocation.X+1, fromLocation.Y  ),
                new Point(fromLocation.X+1, fromLocation.Y-1),
                new Point(fromLocation.X,   fromLocation.Y-1)
            };
        }
        private void InitializeNodes(bool[,] map)
        {
            this.nodes = new Node[graph.width, graph.height];
            for (int y = 0; y < graph.height; y++)
            {
                for (int x = 0; x < graph.width; x++)
                {
                    this.nodes[x, y] = new Node(new Point(x,y), map[x, y], this.graph.endLocation);
                }
            }
           //Debug.Print(map[167,263]+"");
        }
    }
}