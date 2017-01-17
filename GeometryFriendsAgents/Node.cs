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
        Node[] node_pointers; //Array of node objects
                              //C# doesn't like pointers

        //Constructor
        public Node(int x_coord, int y_coord)
        {
            x = x_coord;
            y = y_coord;
        }
        //AddNeighbour Method
        public void AddNeighbour(ref Node neighbour)
        {
        }
    }
}