using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;
namespace GeometryFriendsAgents
{
    class Graph
    {
        //final graph size needs to be 75*47
        public bool[,] map { get; set; }
        public int height, width;
        public Point startLocation { get; set; }
        public List<Node> diamondNode;
        public Point endLocation { get; set; }


        public Graph(ObstacleRepresentation[] oI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, CircleRepresentation cI, Rectangle area)
        {
            map = new bool[area.X, area.Y]; //A Map of the points within the system. True means that the point has no obstacle within it.

            //Initial Setup. All points are set to true
            for (int y = 0; y < area.Y; y++)
                for (int x = 0; x < area.X; x++)
                    map[x, y] = true;


            //Next phase, any point which is filled by a regular obstacle is set to false
            foreach (ObstacleRepresentation obstacle in oI)
            {
                int width = (int)obstacle.Width;
                int height = (int)obstacle.Height;
                Point center = new Point((int)obstacle.X, (int)obstacle.Y);
                for (int i=(center.X - width/2); i <= (center.X+width/2); i++)
                {
                    for (int j = (center.Y - height / 2); j <= (center.Y + height / 2); j++)
                    {
                        map[i, j] = false;
                    }
                }
            }


            //Next phase, any point which is filled by a circle specific obstacle is set to false
            foreach (ObstacleRepresentation obstacle in cPI)
            {
                int width = (int)obstacle.Width;
                int height = (int)obstacle.Height;
                Point center = new Point((int)obstacle.X, (int)obstacle.Y);
                for (int i = (center.X - width / 2); i <= (center.X + width / 2); i++)
                {
                    for (int j = (center.Y - height / 2); j <= (center.Y + height / 2); j++)
                    {
                        map[i, j] = false;
                    }
                }
            }

            //Finally, the start and end locations for the eventual path are determined from the Circle, and a function that finds the closest diamond.
            startLocation = new Point((int)cI.X, (int)cI.Y);
            endLocation = getClosestDiamond(colI, startLocation);

        }
        public void setPoints(Point start, Point end)
        {
            endLocation = end;
        }


        public Point getClosestDiamond(CollectibleRepresentation[] colI, Point curr)
        {
            Point result = new Point (0,0);
            float distance = -1;
            foreach(CollectibleRepresentation collectable in colI)
            {

                    float i;
                    //Simply uses Pythagoras' equation to determine the distance between the two points, and overrides the current point if its closer.
                    if ((i = ((collectable.X - curr.X) * (collectable.X - curr.X) + (collectable.Y - curr.Y) * (collectable.Y - curr.Y))) < distance*distance)
                    {
                        distance = (float)Math.Sqrt(i);
                        result = new Point((int)collectable.X, (int)collectable.Y);
                    }
            }
            return result;
        }
    }
}