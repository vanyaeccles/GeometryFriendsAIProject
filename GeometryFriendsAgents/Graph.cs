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
        private List<String> nodeList;

        Graph()
        {
            nodeList = new List<String>();
        }

        Graph(ObstacleRepresentation oI, ObstacleRepresentation coI, CollectibleRepresentation cI, Rectangle rect)
        {
            nodeList = new List<String>();
        }

        void addNode(ref String node)
        {
            nodeList.Add(node);
        }

        List<String> getList()
        {
            return nodeList;
        }
    }
}
