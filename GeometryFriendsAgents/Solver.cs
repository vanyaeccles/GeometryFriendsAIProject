using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    class Solver
    {
        public Queue<Node> solve(Graph initialGraph, CircleRepresentation cI)
        {
            ArrayList layout = initialGraph.getNodes();
            Queue<Node> queue = new Queue<Node>();
            return queue;
        }
    }
}