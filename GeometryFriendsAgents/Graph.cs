using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    class Graph
    {
        private List<Node> nodeList;

        Graph()
        {
            nodeList = new List<Node>();
        }

        Graph(ObstacleRepresentation oI, ObstacleRepresentation coI, CollectibleRepresentation cI, Rectangle rect)
        {
            nodeList = new List<Node>();
        }

        void addNode(ref Node node)
        {
            nodeList.Add(node);
        }

        List<Node> getList()
        {
            return nodeList;
        }
    }
}
