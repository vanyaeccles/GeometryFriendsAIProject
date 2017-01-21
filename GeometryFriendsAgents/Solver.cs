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
        public Node start, end;
        private Node[,] nodes;
        public Graph graph;
        public CollectibleRepresentation[] diamondInfo;
        public Solver(Graph graph)
        {
            this.graph = graph;
            start = new Node(graph.startLocation, true, graph.endLocation);
            start.state = NodeState.Open;
            end = new Node(graph.endLocation, true, graph.endLocation);
        }
        public Queue<Node> solve(CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            InitializeNodes(graph.map);
            diamondInfo = colI;
            Queue<Node> queue = new Queue<Node>();
            Debug.Print("Starting search for a path to [" + end.location.X + "," + end.location.Y + "]");
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
        private IEnumerable<Point> GetAdjacentLocations(Point fromLocation)
        {
            List<Point> result = new List<Point>
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
            foreach(Point node in graph.corners)
            {
                if (fromLocation == node)
                {   
                    foreach(Point corner in graph.corners)
                    {
                        if (fromLocation == corner)
                            continue;
                        if (fromLocation.X == corner.X)
                            continue;
                        bool obstacle = false;
                        float distanceX = fromLocation.X - corner.X;
                        float distanceY = fromLocation.Y - corner.Y;
                        int divisor = (distanceX >= distanceY ? (int)distanceX : (int)distanceY);   //This chooses the number of steps that should be taken, based on the larger of the two numbers.
                        float divisorX = distanceX / divisor;                                       //determines the fraction of X/Y that we should proceed by each time.
                        float divisorY = distanceY / divisor;                                       //Since the divisor was chosen as the larger of the two distance values, this value will <= 1
                        for (int i = 1; i < divisor; i++)
                        {
                            if (!graph.trueMap[(int)(corner.X + divisorX * i), (int)(corner.Y + divisorY * i)]) //We add the divisor fragment to the location of corner, and check if the corresponding map coordinate is an obstacle or not
                                obstacle = true;
                        }
                        if (!obstacle)
                            result.Add(corner);
                    }
                    break;
                }
            }
            foreach (CollectibleRepresentation diamond in diamondInfo)
            {
                if (fromLocation.X == (int)diamond.X/16 && diamond.Y/16 < fromLocation.Y)    //checks whether the collectable is directly above it
                {
                    bool obstacle = false;
                    bool decision;
                    for(int i = (int)diamond.Y/16; i < (int) fromLocation.Y; i++)//Next, it checks whether there is any obstacles in between them
                    {
                        if (decision = graph.map[fromLocation.X, i])
                        {
                            if (graph.map[fromLocation.X, i])
                            {
                                obstacle = true;
                                break;
                            }
                        }
                    }
                    if (!obstacle)
                        result.Add(new Point((int)diamond.X / 16, ((int)diamond.Y / 16)));
                }
            }
            return result;
        }
        private void InitializeNodes(bool[,] map)
        {
            this.nodes = new Node[graph.width, graph.height];
            for (int y = 0; y < graph.height; y++)
            {
                for (int x = 0; x < graph.width; x++)
                {
                    this.nodes[x, y] = new Node(new Point(x,y), map[x, y], this.graph.endLocation);
                    if (graph.diamondMap[x, y])
                        this.nodes[x, y].isWalkable = true;
                }
            }
        }

        public void setStartEnd(Point startpoint, Point endpoint)
        {
            startpoint = new Point(startpoint.X / 16, startpoint.Y/16);
            endpoint = new Point(endpoint.X, endpoint.Y);
            start = new GeometryFriendsAgents.Node(startpoint, true, endpoint);
            graph.startLocation = startpoint;
            end = new Node(endpoint, true, endpoint);
            graph.endLocation = endpoint;
        }
    }
}