using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryFriendsAgents
{
    class Solver
    {
        private Graph workingGraph;
        private Graph solutionGraph;

        Solver(Graph newGraph)
        {
            workingGraph = newGraph;
        }

        Graph solve()
        {
            solutionGraph = workingGraph;
            return solutionGraph;
        }
    }
}
