using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI.Perceptions.Information;
using System.Drawing;

namespace GeometryFriendsAgents
{
    class Solver
    {
        private Node start, end;
        private Node[,] nodes;
        private Graph graph;
        public Solver(Graph graph)
        {
            this.graph = graph;
            start = new Node(graph.startLocation, true, graph.endLocation);
            start.state = NodeState.Open;
            end = new Node(graph.endLocation, true, graph.endLocation);
        }
        public Queue<Node> solve(CircleRepresentation cI)
        {
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
                    return true;
                }
                else
                {
                    // If not, check the next set of nodes
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

            foreach (var location in nextLocations)
            {
                int x = location.X;
                int y = location.Y;

                // Stay within the grid's boundaries
                if (x < 0 || x >= graph.width || y < 0 || y >= graph.height)
                    continue;

                Node node = this.nodes[x, y];
                // Ignore non-walkable nodes
                if (!node.isWalkable)
                    continue;

                // Ignore already-closed nodes
                if (node.state == NodeState.Closed)
                    continue;

                // Already-open nodes are only added to the list if their G-value is lower going via this route.
                if (node.state == NodeState.Open)
                {
                    float traversalCost = Node.GetTraversalCost(node.location, node.parent.location);
                    float gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G)
                    {
                        node.parent = fromNode;
                        walkableNodes.Add(node);
                    }
                }
                else
                {
                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    node.parent = fromNode;
                    node.state = NodeState.Open;
                    walkableNodes.Add(node);
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
        }
    }
}