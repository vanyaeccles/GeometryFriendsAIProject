using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GeometryFriendsAgents
{
    class Node
    {
        private int x;
        private int y;
        public bool diamond = false;
        public bool obstacle = false;

        //Constructor
        public Node() { }
        public Node(int x_coord, int y_coord)
        {
            x = x_coord;
            y = y_coord;
        }
    }
}