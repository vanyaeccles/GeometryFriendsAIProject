using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;
namespace GeometryFriendsAgents
{
    class Graph
    {
        //final graph size needs to be 75*47
        private ArrayList nodeList;
        private int height, width;
        Graph(){}
        public Graph(ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] coI, Rectangle area)
        {
            height = area.Height;
            width = area.Width;
            for(int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    bool typeset = false;
                    Node node = new Node(i, j);
                    //Check if Node Contains a Diamond
                    foreach (CollectibleRepresentation diamond in coI)
                    {
                        if (diamond.X == i && diamond.Y == j)
                        {
                            node.diamond = true;
                            typeset = true;
                        }
                    }
                    if (!typeset)
                    {
                        foreach(ObstacleRepresentation obstacle in oI)
                        {
                            if ((obstacle.X-(obstacle.Width/2)<= i) && (obstacle.X + (obstacle.Width / 2) >= i) && (obstacle.Y-(obstacle.Height/2)<= j) && (obstacle.Y + (obstacle.Height / 2) >= j) )
                            {
                                node.obstacle = true;
                                typeset = true;
                            }
                        }
                        foreach (ObstacleRepresentation obstacle in rPI)
                        {
                            if ((obstacle.X - (obstacle.Width / 2) <= i) && (obstacle.X + (obstacle.Width / 2) >= i) && (obstacle.Y - (obstacle.Height / 2) <= j) && (obstacle.Y + (obstacle.Height / 2) >= j))
                            {
                                node.obstacle = true;
                                typeset = true;
                            }
                        }
                        foreach (ObstacleRepresentation obstacle in cPI)
                        {
                            if ((obstacle.X - (obstacle.Width / 2) <= i) && (obstacle.X + (obstacle.Width / 2) >= i) && (obstacle.Y - (obstacle.Height / 2) <= j) && (obstacle.Y + (obstacle.Height / 2) >= j))
                            {
                                node.obstacle = true;
                                typeset = true;
                            }
                        }
                    }
                }
            }


        }
        public ArrayList getNodes()
        {
            return nodeList;
        }
    }
}