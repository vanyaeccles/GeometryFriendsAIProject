using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeometryFriends.AI.Perceptions.Information;
using System.Diagnostics;

namespace GeometryFriendsAgents
{
    class Graph
    {
        //final graph size needs to be 75*47
        public bool[,] map { get; set; }
        public bool[,] trueMap { get; set; }
        public bool[,] diamondMap { get; set; }
        public List<Point> corners { get; set; }
        public int height, width;
        public Point startLocation { get; set; }
        public Point endLocation { get; set; }


        public Graph(ObstacleRepresentation[] oI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, CircleRepresentation cI, Rectangle area)
        {
            corners = new List<Point>();

            height = area.Height + 1;
            width = area.Width + 1;
            bool[,] map_temp = new bool[width, height]; // A Map of the points within the system. True means that the point has no obstacle within it.

            //Initial Setup. All points are set to true
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    map_temp[x, y] = true;

            //Next phase, any point which is filled by a regular obstacle is set to false
            foreach (ObstacleRepresentation obstacle in oI)
            {
                int width = (int)obstacle.Width;
                int height = (int)obstacle.Height;
                Point center = new Point((int)obstacle.X-40, (int)obstacle.Y-40);
                for (int i=(center.X - width/2); i <= (center.X+width/2); i++)
                {
                    for (int j = (center.Y - height / 2); j <= (center.Y + height / 2); j++)
                    {
                        map_temp[i, j] = false;
                    }
                }
            }


            //Next phase, any point which is filled by a circle specific obstacle is set to false
            foreach (ObstacleRepresentation obstacle in cPI)
            {
                int width = (int)obstacle.Width;
                int height = (int)obstacle.Height;
                Point center = new Point((int)obstacle.X-40, (int)obstacle.Y-40);
                for (int i = (center.X - width / 2); i <= (center.X + width / 2); i++)
                {
                    for (int j = (center.Y - height / 2); j <= (center.Y + height / 2); j++)
                    {
                        map_temp[i, j] = false;
                    }
                }
            }
            height = height / 16;
            width = width / 16;
            //Next, setting up the actual map, which is 16x smaller so as to reduce the stress I'm under from this bullshit
            map = new bool[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    map[x, y] = map_temp[x*16+5, y*16+5]; //sets the smaller grid value to be the value of the point right in the middle of the cell.

            //Finally, the start and end locations for the eventual path are determined from the Circle, and a function that finds the closest diamond.
            startLocation = new Point((int)cI.X / 16, (int)cI.Y / 16);
            endLocation = getClosestDiamond(colI, startLocation);

            trueMap = new bool[map.GetLength(0), map.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j])
                        trueMap[i,j] = true;
                    else
                        trueMap[i, j] = false;
                }

            removeFloatingPoints(colI);
            findCorners();
            //Quick Debug Setup to print out what the program thinks the area looks like
            for (int y = 0; y < height; y++)
            {
                string line = "|";
                for (int x = 0; x < width; x++)
                {
                    if (x == startLocation.X && y == startLocation.Y)
                        line += "O";
                    else if (x == endLocation.X && y == endLocation.Y)
                        line += "D";
                    else if (map[x, y])
                    {
                        string space = " ";
                        foreach (Point corner in corners)
                            if (corner.X == x && corner.Y == y)
                                space = "c";
                        line += space;
                    }
                    else
                        line += "X";
                }
                line += "|";
                Debug.Print(line);
            }
                    

        }

        //This function's purpose is to remove any points that have no ground directly beneath them.
        public void removeFloatingPoints(CollectibleRepresentation[] colI)
        {
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (j+2 >= height)//Is the point directly below this one outside the graph? (Point would then be on the bottom of the graph, and is considered on ground)
                    {
                        if ((j + 3 == height))
                            map[i, j] = false;
                        continue;
                    } 
                    if (!map[i, j+2])//Is the point directly below this one considered to be unwalkable? (Point would then be over a point considered an obstacle, and thus considered ground)
                        continue;
                    map[i, j] = false;
                }
            diamondMap = new bool[map.GetLength(0), map.GetLength(1)];
            foreach (CollectibleRepresentation diamond in colI)
                diamondMap[(int)diamond.X / 16, (int)diamond.Y / 16] = true;
        }

        //This function finds any corner points, ie, any point that is itself accessible, but its left or right neighbours are not.
        public void findCorners()
        {
            for (int i= 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if(map[i,j])
                    {
                        if (i - 1 < 0 || i + 1 >= map.GetLength(0))
                        {
                                corners.Add(new Point(i, j));
                        }
                        else if (!map[i - 1, j] || !map[i + 1, j])
                        {         
                                corners.Add(new Point(i, j));
                        }
                    }
                }
        }

        public void setPoints(Point start, Point end)
        {
            endLocation = end;
        }


        public Point getClosestDiamond(CollectibleRepresentation[] colI, Point curr)
        {
            Point result = new Point (0,0);
            float distance = 9999999999999999999999.0f;
            foreach(CollectibleRepresentation collectable in colI)
            {

                    float i;
                    //Simply uses Pythagoras' equation to determine the distance between the two points, and overrides the current point if its closer.
                    if ((i = ((collectable.X/16 - curr.X) * (collectable.X/16 - curr.X) + (collectable.Y/16 - curr.Y) * (collectable.Y/16 - curr.Y))) < distance*distance)
                    {
                        distance = (float)Math.Sqrt(i);
                        result = new Point((int)collectable.X/16, (int)collectable.Y/16);
                    }
            }
            return result;
        }
    }
}