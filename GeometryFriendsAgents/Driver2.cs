//ADAPTED FROM SUBGOAL A* CODE WRITTEN BY DANIEL FISCHER
using GeometryFriends;
using GeometryFriends.AI;
using System;
using System.Collections.Generic;
using GeometryFriends.AI.Perceptions.Information;

namespace GeometryFriendsAgents
{
    class Driver2

    {
        private List<Node> nodes;
        private int[,] adjacencyMatrix;
        private int[,] directionMap;
        private Queue<Node> route;

        private Node previousNode;
        private Node nextNode;
        private Node nextNode2;

        private int previousDirection;
        private int direction;
        private int direction2;
        private int previousAction;
        private int action;
        private int action2;

        private float distance;
        private List<float> distanceList;

        bool output = false;

        enum Direction { Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp };

        public Driver2(List<Node> nodes, int[,] adjacencyMatrix, int[,] directionMap, Queue<Node> route)
        {
            this.nodes = nodes;
            this.adjacencyMatrix = adjacencyMatrix;
            this.directionMap = directionMap;
            this.route = route;
            distanceList = new List<float>();

            this.previousNode = route.Dequeue();
            this.nextNode = route.Dequeue();
            this.nextNode2 = route.Dequeue();
            this.action = adjacencyMatrix[nodes.IndexOf(previousNode), nodes.IndexOf(nextNode)];
            this.action2 = adjacencyMatrix[nodes.IndexOf(nextNode), nodes.IndexOf(nextNode2)];
            this.direction = directionMap[nodes.IndexOf(previousNode), nodes.IndexOf(nextNode)];
            this.direction2 = directionMap[nodes.IndexOf(nextNode), nodes.IndexOf(nextNode2)];
            //fall down case, create pseudo node
            if (direction2 == 2)
            {
                Node newNext = GetOppositeFallDownNode(nextNode);
                if (newNext != null)
                {
                    nextNode = newNext;
                    if (output)
                    {
                        Log.LogInformation("Driver - FALL DOWN - new nextNode - " + nextNode);
                    }
                    this.action = 2;

                    distance = (float)Math.Sqrt(Math.Pow(nodes[0].getX() - nextNode.getX(), 2) + Math.Pow(nodes[0].getY() - nextNode.getY(), 2));
                    distanceList.Add(distance);
                    if (output)
                    {
                        Log.LogInformation("Driver - FALL DOWN - new distance - " + distance);
                    }
                }
            }
            if (output)
            {
                Log.LogInformation("Driver - Constructor");
                Log.LogInformation("Driver - preNode - " + previousNode);
                Log.LogInformation("Driver - nextNode - " + nextNode);
                Log.LogInformation("Driver - nextNode2 - " + nextNode2);
                Log.LogInformation("Driver - action - " + action);
                Log.LogInformation("Driver - action2 - " + action2);
                Log.LogInformation("Driver - direction - " + direction);
                Log.LogInformation("Driver - direction2 - " + direction2);
            }

        }

        public Moves GetAction(float[] squareInfo)
        {
            int x = (int)squareInfo[0];
            int y = (int)squareInfo[1];
            int vX = (int)squareInfo[2];
            int vY = (int)squareInfo[3];
            int h = (int)squareInfo[4];

            int hHalf = h/2;
            int w = 10000 / h;
            
            int centerY = y;

            int alwaysCorrectH = h;
            int alwaysCorrectHHalf = hHalf;
            int alwaysCorrectW = w;
            y = y + alwaysCorrectHHalf;

            distance = (float)Math.Sqrt(Math.Pow(x - nextNode.getX(), 2) + Math.Pow(y - nextNode.getY(), 2));
            distanceList.Add(distance);
            if (output)
            {
                Log.LogInformation("Driver - Distance - " + distance);
            }

            //Algorithm
            if (distanceList.Count == 40 && distanceList[0] == distanceList[39] && CircleAgent.nCollectiblesLeft > 0)
            {
                distanceList = new List<float>();
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                int astarAction = UseSubgoalAStar(x, centerY);
                Log.LogInformation("Driver - Route recalc with Subgoal AStar in ms: " + sw.ElapsedMilliseconds);
                if (astarAction >= 0)
                {
                    return (Moves)astarAction;
                }
            }
            //Algorithm end

            if(distanceList.Count >= 40)
            {
                distanceList = new List<float>();
            }

            if (((distance - 3) <= (alwaysCorrectW / 2) && !nextNode.getPseudo()) || (direction == 6 && distance <= alwaysCorrectH) || ((direction == 2 || direction == 1 || direction == 3) && (nextNode.getY() - y) < 4 && distance < 3 * alwaysCorrectW) || (nextNode.getPseudo() && (distance - 3 < 3)))// (direction == 2)
            {
                distanceList = new List<float>();
                if (output)
                {
                    Log.LogInformation("Driver - old preNode - " + previousNode);
                }
                this.previousAction = action;
                this.previousDirection = direction;
                previousNode = nextNode;
                if (output)
                {
                    Log.LogInformation("Driver - new preNode - " + previousNode);
                }
                if(nextNode2 != null)
                {
                    nextNode = nextNode2;
                    if (output)
                    {
                        Log.LogInformation("Driver - new nextNode - " + nextNode);
                    }
                    this.action = action2; 
                    this.direction = direction2;
                    if (output)
                    {
                        Log.LogInformation("Driver - new action - " + action);
                        Log.LogInformation("Driver - new direction - " + direction);
                    }

                    distance = (float)Math.Sqrt(Math.Pow(x - nextNode.getX(), 2) + Math.Pow(y - nextNode.getY(), 2));
                    distanceList.Add(distance);
                    if (output)
                    {
                        Log.LogInformation("Driver - new distance - " + distance);
                    }
                }
                else
                {
                    if (output)
                    {
                        Log.LogInformation("Driver - end of route, nextNode, no actions");
                    }
                    //Algorithms
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    UseSubgoalAStar(x, centerY);
                    Log.LogInformation("Driver - Route recalc with Subgoal AStar in ms: " + sw.ElapsedMilliseconds);
                    
                    return Moves.NO_ACTION;
                    //Algorithms end
                }
                if (route.Count > 0)
                { 
                    nextNode2 = route.Dequeue();
                    if (output)
                    {
                        Log.LogInformation("Driver - new nextNode2 - " + nextNode2);
                    }
                    this.action2 = adjacencyMatrix[nodes.IndexOf(nextNode), nodes.IndexOf(nextNode2)];
                    if (output)
                    {
                        Log.LogInformation("Driver - new action2 - " + action2);
                    }
                    this.direction2 = directionMap[nodes.IndexOf(nextNode), nodes.IndexOf(nextNode2)];
                    if (output)
                    {
                        Log.LogInformation("Driver - new direction2 - " + direction2);
                    }

                    //fall down case, create pseudo node
                    if(direction2 == 2)
                    {
                        Node newNext = GetOppositeFallDownNode(nextNode);
                        if(newNext != null)
                        {
                            nextNode = newNext;
                            if (output)
                            {
                                Log.LogInformation("Driver - FALL DOWN - new nextNode - " + nextNode);
                            }
                            this.action = 2;
                         
                            distance = (float)Math.Sqrt(Math.Pow(x - nextNode.getX(), 2) + Math.Pow(y - nextNode.getY(), 2));
                            distanceList.Add(distance);
                            if (output)
                            {
                                Log.LogInformation("Driver - FALL DOWN - new distance - " + distance);
                            }
                        }
                    }
                }
                else
                {
                    if (output)
                    {
                        Log.LogInformation("Driver - end of route, nextNode2, no actions");
                    }
                    nextNode2 = null;
                    this.action2 = -1;
                    this.direction2 = -1;
                }
                
            }

            if(nextNode.getPseudo() && distanceList.Count >= 10 && distanceList[0] == distanceList[9])
            {
                if(direction == 0 )
                {
                    return Moves.ROLL_LEFT;
                }
                if (direction == 4 )
                {
                    return Moves.ROLL_RIGHT;
                }
            }


            if(previousDirection == 6 && (direction == 0 || direction == 4) && (Math.Abs(y - previousNode.getY())  > 4 ) )
            {
                Random random = new Random();
                int ran = random.Next(2);
                if (ran == 0)
                {
                    return Moves.MORPH_UP;
                }
                else if(direction == 0)
                {
                    return Moves.ROLL_RIGHT;
                }
                else if(direction == 4)
                {
                    return Moves.ROLL_LEFT;
                }

            }

            // possible actions for direction upper right and upper left
            if (direction == 7 || direction == 5)
            {
                if (CircleAgent.obstacleOpenSpace[y - (int)CircleAgent.radius, x])
                {
                    return Moves.JUMP;
                }
                else if (direction == 7)
                {
                    return Moves.ROLL_RIGHT;
                }
                else if (direction == 5)
                {
                    return Moves.ROLL_LEFT;
                }
            }

            if(direction == 0 || direction == 1 )
            {         
                if((distance < 110 && vX > 50 && (direction2 != 0) ) || vX > 200 )
                {
                    return Moves.ROLL_RIGHT;
                }
                else
                {
                    if(nextNode.getPseudo() && distance < 12 && vX > 2 )
                    {
                        return Moves.ROLL_LEFT;
                    }
                    if (nextNode.getPseudo() && distance < 6 && vX < 2 && vX > -2)
                    {
                        return Moves.NO_ACTION;
                    }
                    return Moves.ROLL_RIGHT;
                }
                
            }
            if (direction == 4 || direction == 3 )
            {
                if ((distance < 110 && vX < -50 && (direction2 != 4)) || vX < -200 )
                {
                    return Moves.ROLL_LEFT;
                }
                else
                {
                    if (nextNode.getPseudo() && distance < 12 && vX < -2)
                    {
                        return Moves.ROLL_RIGHT;
                    }
                    if (nextNode.getPseudo() && distance < 6 && vX < 2 && vX > -2)
                    {
                        return Moves.NO_ACTION;
                    }
                    return Moves.ROLL_LEFT;
                }
            }
            if(direction == 6)
            {

                Random random = new Random();
                int ran = random.Next(100);
                if (ran < 98)
                {
                    return Moves.JUMP;
                } else if (ran == 98)
                {
                    return Moves.ROLL_LEFT;
                } else if (ran == 99)
                {
                    return Moves.ROLL_RIGHT;
                }
            }
            if (direction == 2)
            {
                if(previousDirection == 0 || previousDirection == 1)
                {
                    return Moves.ROLL_RIGHT;
                }
                if(previousDirection == 3 || previousDirection == 4)
                {
                    return Moves.ROLL_LEFT;
                }
                Random rnd = new Random();
                switch (rnd.Next(3))
                {
                    case 0:
                        return Moves.ROLL_LEFT;
                    case 1:
                        return Moves.ROLL_RIGHT;
                    case 2:
                        return Moves.NO_ACTION;
                }
            }
            if ((!previousNode.getPseudo() && direction == 2 && (previousDirection == 0 || previousDirection == 4) && (Math.Abs(y - previousNode.getY()) < 5)) || ((previousDirection == 0 || previousDirection == 4)))
            {
                if (previousDirection == 0 && vX < 40)
                {
                    return Moves.ROLL_RIGHT;
                }
                else if(previousDirection == 4 && vX < -40)
                {
                    return Moves.ROLL_LEFT;
                }
            }
            if (previousNode.getLeadsToFallDown() && !previousNode.getPseudo() && direction == 2 || direction == 3 || direction == 1)
            {
                if(previousNodeToObstacleDistance() <= 125)
                {
                    return Moves.JUMP;
                }
            }
            if (output)
            {
                Log.LogInformation("Driver - end of driver, no actions");
            }
            return Moves.NO_ACTION;
        }

        private int previousNodeToObstacleDistance()
        {
            int iter = 7;
            int i = 1;
            int x = previousNode.getX();
            int y = previousNode.getY();
            if(previousDirection == 3 || previousDirection == 4)
            {
                iter = iter * -1;
            }
            //while(!RectangleAgent.obstacleOpenSpace[y,x+(i*iter)])
            while (!CircleAgent.obstacleOpenSpace[y, x + (i * iter)])
            {
                i++;
            }
            return Math.Abs(i * iter);
        }

        public Node GetOppositeFallDownNode(Node nextNode)
        {
            List<Node> possibleNodes = new List<Node>();
            foreach (Node node in nodes)
            {
                if (node.getLeadsToFallDown() && node.getY() == nextNode.getY() && Math.Abs(node.getX() - nextNode.getX()) <= 200 && (node.getX() - nextNode.getX()) != 0)
                {
                    Node newNode = new Node(((node.getX() + nextNode.getX()) / 2), nextNode.getY(), false);
                    newNode.setPseudo(true);
                    possibleNodes.Add(newNode);
                }
            }
            Node selectedNode = null;
            if (output)
            {
                Log.LogInformation("Driver - GetOppositeFallDownNode, found: " + possibleNodes.Count);
            }
            for (int i = 0; i < possibleNodes.Count; i++)
            {
                if (output)
                {
                    Log.LogInformation("Driver - GetOppositeFallDownNode, found: " + possibleNodes[i]);
                }
                if (direction == 0 && possibleNodes[i].getX() > previousNode.getX())
                {
                    selectedNode = possibleNodes[i];
                }
                if (direction == 4 && possibleNodes[i].getX() < previousNode.getX())
                {
                    selectedNode = possibleNodes[i];
                }
            }
            if (output)
            {
                Log.LogInformation("Driver - GetOppositeFallDownNode, selected: " + selectedNode);
            }
            return selectedNode;
        }

        private int UseSubgoalAStar(int x, int centerY)
        {
            Log.LogInformation("Driver Subgoal AStar - Subgoal AStar start");

            int s = 1;
            while (!CircleAgent.obstacleOpenSpace[centerY + s, x])
                {
                s++;
            }
            Node square = new Node(x, centerY + s - 1, false);
            this.nodes[0] = square;
            int y = square.getY();

            deleteCollectedDiamonds();

            List<int> diamondNodes = new List<int>();
            for (int n = 0; n < nodes.Count; n++)
            {
                if (nodes[n].getDiamond())
                {
                    diamondNodes.Add(n);
                }
            }

            CircleAgent.nodes = this.nodes;
            CircleAgent.CreateEdgesAndAdjacencyMatrix();
            this.adjacencyMatrix = CircleAgent.adjacencyMatrix;
            this.directionMap = CircleAgent.directionMap;

            SubgoalAStar sgAstar = new SubgoalAStar(0, diamondNodes, 2000, 0);
            route = sgAstar.Run();
            int diamondsToCollect = diamondNodes.Count - 1;
            while (route == null)
            {
                sgAstar = new SubgoalAStar(0, diamondNodes, 2000, diamondsToCollect);
                route = sgAstar.Run();
                diamondsToCollect--;
                if(diamondsToCollect == 0)
                {
                    route = new Queue<Node>();
                }
            }         

            return recalcNextNodes("Subgoal AStar", x, y);
        }
        
        private void deleteCollectedDiamonds()
        {
            float[] colInfo = CollectibleRepresentation.RepresentationArrayToFloatArray(CircleAgent.colI);
            List<Node> colLeftList = new List<Node>();
            for (int i = 0; i < colInfo.Length; i = i + 2)
            {
                Node node = new Node((int)colInfo[i], (int)colInfo[i + 1], true);
                colLeftList.Add(node);
            }

            for (int index = 0; index < nodes.Count; index++)
            {
                Node nodeOfFullList = nodes[index];
                if (nodeOfFullList.getDiamond())
                {
                    bool isDiamond = false;
                    foreach (Node leftDiamond in colLeftList)
                    {
                        if ((nodeOfFullList.getX() == leftDiamond.getX() && nodeOfFullList.getY() == leftDiamond.getY()) || ((nodeOfFullList.getY() - leftDiamond.getY()) <= 80 && (nodeOfFullList.getY() - leftDiamond.getY()) > 0 && nodeOfFullList.getX() == leftDiamond.getX()))
                        {
                            isDiamond = true;
                        }
                    }
                    if (!isDiamond)
                    {
                        nodeOfFullList.setDiamond(false);
                        nodes[index] = nodeOfFullList;
                    }
                }
            }
        }

        public int recalcNextNodes(String output, int x, int y)
        {
            if(route == null)
            {
                return -1;
            }
            if (route.Count > 0)
            {
                this.previousNode = route.Dequeue();
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - preNode - " + previousNode);
                }
            }
            else
            {
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - end of route, preNode,  no actions");
                }
                return (int)Moves.NO_ACTION;
            }
            if (route.Count > 0)
            {
                this.nextNode = route.Dequeue();
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - nextNode - " + nextNode);
                }
                this.action = adjacencyMatrix[nodes.IndexOf(previousNode), nodes.IndexOf(nextNode)];
                this.direction = directionMap[nodes.IndexOf(previousNode), nodes.IndexOf(nextNode)];
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - action - " + action);
                    Log.LogInformation("Driver " + output + " - direction - " + direction);
                }
                distance = (float)Math.Sqrt(Math.Pow(x - nextNode.getX(), 2) + Math.Pow(y - nextNode.getY(), 2));
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - Distance - " + distance);
                }

            }
            else
            {
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - end of route, nextNode, no actions");
                }
                return (int)Moves.NO_ACTION;
            }
            if (route.Count > 0)
            {
                this.nextNode2 = route.Dequeue();
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - nextNode2 - " + nextNode2);
                }
                this.action2 = adjacencyMatrix[nodes.IndexOf(nextNode), nodes.IndexOf(nextNode2)];
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - action2 - " + action2);
                }
                this.direction2 = directionMap[nodes.IndexOf(nextNode), nodes.IndexOf(nextNode2)];
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - direction2 - " + direction2);
                }
            }
            else
            {
                if (this.output)
                {
                    Log.LogInformation("Driver " + output + " - end of route, nextNode2, no actions");
                }
                nextNode2 = null;
                this.action2 = -1;
                this.direction2 = -1;
            }
            //fall down case, create pseudo node
            if (direction2 == 2)
            {
                Node newNext = GetOppositeFallDownNode(nextNode);
                if (newNext != null)
                {
                    nextNode = newNext;
                    if (this.output)
                    {
                        Log.LogInformation("Driver - FALL DOWN - new nextNode - " + nextNode);
                    }
                    this.action = 2;

                    distance = (float)Math.Sqrt(Math.Pow(nodes[0].getX() - nextNode.getX(), 2) + Math.Pow(nodes[0].getY() - nextNode.getY(), 2));
                    distanceList.Add(distance);
                    if (this.output)
                    {
                        Log.LogInformation("Driver - FALL DOWN - new distance - " + distance);
                    }
                }
            }
            Log.LogInformation("Driver " + output + " - " + output + " end");
            return -1;
        }
    }
}
