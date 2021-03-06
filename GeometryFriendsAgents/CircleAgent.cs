﻿//ADAPTED FROM SUBGOAL A* CODE WRITTEN BY DANIEL FISCHER
using GeometryFriends;
using GeometryFriends.AI;
using GeometryFriends.AI.Interfaces;
using GeometryFriends.AI.Perceptions.Information;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace GeometryFriendsAgents
{
    /// <summary>
    /// Dummy circle agent to prevent game craches since there was no implementation of a subgoalA* circle agent
    /// </summary>
    class CircleAgent : AbstractCircleAgent
    {
        //Node list
        public static List<Node> nodes;
        private bool implementedAgent;
        private Moves currentAction;
        List<Moves> possibleMoves;

        CountInformation nI;
        RectangleRepresentation rI;
        public CircleRepresentation cI;
        ObstacleRepresentation[] oI;
        ObstacleRepresentation[] rPI;
        ObstacleRepresentation[] cPI;
        public static CollectibleRepresentation[] colI;
        Rectangle area;
        double timeLimit;

        //Sensors Information
        private int[] numbersInfo;
        private float[] rectangleInfo;
        private float[] circleInfo;
        private float[] obstaclesInfo;
        private float[] rectanglePlatformsInfo;
        private float[] circlePlatformsInfo;
        public static float[] collectiblesInfo;

        bool firstUpdate;

        //Obstacle and open space
        public static int fullHeight = 800;
        public static int fullWidth = 1280;
        public static bool[,] obstacleOpenSpace;

        //Rectangle size
        public static int[] nSquareSize = { 100, 100 };
        public static int[] hSquareSize = { 200, 50 };
        public static int[] vSquareSize = { 50, 200 };

        //Adjacency matrix
        public static int[,] adjacencyMatrix;
        public static int[,] directionMap;

        enum Direction { Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp };

        int moveStep;
        Driver2 driver;
        public static float radius;

        public static int[,] directDistanceMap;


        //Extra diamond fall down node
        public static int diamondFallDownThreshold = 80;

        public static int nCollectiblesLeft;

        private string agentName = "Mr. Bump";

        private Random rnd;


        public CircleAgent()
        {
            //Change flag if agent is not to be used
            implementedAgent = true;

            possibleMoves = new List<Moves>();
            possibleMoves.Add(Moves.ROLL_LEFT);
            possibleMoves.Add(Moves.ROLL_RIGHT);
            possibleMoves.Add(Moves.JUMP);
            possibleMoves.Add(Moves.GROW);
            possibleMoves.Add(Moves.NO_ACTION);

            rnd = new Random();
            currentAction = Moves.NO_ACTION;

        }

        public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            moveStep = 0;

            radius = cI.Radius;

            //convert to old way in order to maintain compatibility
            DeprecatedSetup(
                nI.ToArray(),
                rI.ToArray(),
                cI.ToArray(),
                ObstacleRepresentation.RepresentationArrayToFloatArray(oI),
                ObstacleRepresentation.RepresentationArrayToFloatArray(rPI),
                ObstacleRepresentation.RepresentationArrayToFloatArray(cPI),
                CollectibleRepresentation.RepresentationArrayToFloatArray(colI),
                area,
                timeLimit);
        }

        public void DeprecatedSetup(int[] nI, float[] sI, float[] cI, float[] oI, float[] sPI, float[] cPI, float[] colI, Rectangle area, double timeLimit)
        {
            // Time limit is the time limit of the level - if negative then there is no time constrains
            this.area = area;
            int temp;

            // numbersInfo[] Description
            //
            // Index - Information
            //
            //   0   - Number of Obstacles
            //   1   - Number of Rectangle Platforms
            //   2   - Number of Circle Platforms
            //   3   - Number of Collectibles

            numbersInfo = new int[4];
            int i;
            for (i = 0; i < nI.Length; i++)
            {
                numbersInfo[i] = nI[i];
            }

            nCollectiblesLeft = nI[3];

            // rectangleInfo[] Description
            //
            // Index - Information
            //
            //   0   - Rectangle X Position
            //   1   - Rectangle Y Position
            //   2   - Rectangle X Velocity
            //   3   - Rectangle Y Velocity
            //   4   - Rectangle Height

            rectangleInfo = new float[5];

            rectangleInfo[0] = sI[0];
            rectangleInfo[1] = sI[1];
            rectangleInfo[2] = sI[2];
            rectangleInfo[3] = sI[3];
            rectangleInfo[4] = sI[4];

            // circleInfo[] Description
            //
            // Index - Information
            //
            //   0  - Circle X Position
            //   1  - Circle Y Position
            //   2  - Circle X Velocity
            //   3  - Circle Y Velocity
            //   4  - Circle Radius

            circleInfo = new float[5];

            circleInfo[0] = cI[0];
            circleInfo[1] = cI[1];
            circleInfo[2] = cI[2];
            circleInfo[3] = cI[3];
            circleInfo[4] = cI[4];


            // Obstacles and Platforms Info Description
            //
            //  X = Center X Coordinate
            //  Y = Center Y Coordinate
            //
            //  H = Platform Height
            //  W = Platform Width
            //
            //  Position (X=0,Y=0) = Left Superior Corner

            // obstaclesInfo[] Description
            //
            // Index - Information
            //
            // If (Number of Obstacles > 0)
            //  [0 ; (NumObstacles * 4) - 1]      - Obstacles' info [X,Y,H,W]
            // Else
            //   0                                - 0
            //   1                                - 0
            //   2                                - 0
            //   3                                - 0

            if (numbersInfo[0] > 0)
                obstaclesInfo = new float[numbersInfo[0] * 4];
            else obstaclesInfo = new float[4];

            temp = 1;
            if (nI[0] > 0)
            {
                while (temp <= nI[0])
                {
                    obstaclesInfo[(temp * 4) - 4] = oI[(temp * 4) - 4];
                    obstaclesInfo[(temp * 4) - 3] = oI[(temp * 4) - 3];
                    obstaclesInfo[(temp * 4) - 2] = oI[(temp * 4) - 2];
                    obstaclesInfo[(temp * 4) - 1] = oI[(temp * 4) - 1];
                    temp++;
                }
            }
            else
            {
                obstaclesInfo[0] = oI[0];
                obstaclesInfo[1] = oI[1];
                obstaclesInfo[2] = oI[2];
                obstaclesInfo[3] = oI[3];
            }

            // rectanglePlatformsInfo[] Description
            //
            // Index - Information
            //
            // If (Number of Rectangle Platforms > 0)
            //  [0; (numRectanglePlatforms * 4) - 1]   - Rectangle Platforms' info [X,Y,H,W]
            // Else
            //   0                                  - 0
            //   1                                  - 0
            //   2                                  - 0
            //   3                                  - 0

            if (numbersInfo[1] > 0)
                rectanglePlatformsInfo = new float[numbersInfo[1] * 4];
            else
                rectanglePlatformsInfo = new float[4];

            temp = 1;
            if (nI[1] > 0)
            {
                while (temp <= nI[1])
                {
                    rectanglePlatformsInfo[(temp * 4) - 4] = sPI[(temp * 4) - 4];
                    rectanglePlatformsInfo[(temp * 4) - 3] = sPI[(temp * 4) - 3];
                    rectanglePlatformsInfo[(temp * 4) - 2] = sPI[(temp * 4) - 2];
                    rectanglePlatformsInfo[(temp * 4) - 1] = sPI[(temp * 4) - 1];
                    temp++;
                }
            }
            else
            {
                rectanglePlatformsInfo[0] = sPI[0];
                rectanglePlatformsInfo[1] = sPI[1];
                rectanglePlatformsInfo[2] = sPI[2];
                rectanglePlatformsInfo[3] = sPI[3];
            }

            // circlePlatformsInfo[] Description
            //
            // Index - Information
            //
            // If (Number of Circle Platforms > 0)
            //  [0; (numCirclePlatforms * 4) - 1]   - Circle Platforms' info [X,Y,H,W]
            // Else
            //   0                                  - 0
            //   1                                  - 0
            //   2                                  - 0
            //   3                                  - 0

            if (numbersInfo[2] > 0)
                circlePlatformsInfo = new float[numbersInfo[2] * 4];
            else
                circlePlatformsInfo = new float[4];

            temp = 1;
            if (nI[2] > 0)
            {
                while (temp <= nI[2])
                {
                    circlePlatformsInfo[(temp * 4) - 4] = cPI[(temp * 4) - 4];
                    circlePlatformsInfo[(temp * 4) - 3] = cPI[(temp * 4) - 3];
                    circlePlatformsInfo[(temp * 4) - 2] = cPI[(temp * 4) - 2];
                    circlePlatformsInfo[(temp * 4) - 1] = cPI[(temp * 4) - 1];
                    temp++;
                }
            }
            else
            {
                circlePlatformsInfo[0] = cPI[0];
                circlePlatformsInfo[1] = cPI[1];
                circlePlatformsInfo[2] = cPI[2];
                circlePlatformsInfo[3] = cPI[3];
            }

            //Collectibles' To Catch Coordinates (X,Y)
            //
            //  [0; (numCollectibles * 2) - 1]   - Collectibles' Coordinates (X,Y)

            collectiblesInfo = new float[numbersInfo[3] * 2];

            temp = 1;
            while (temp <= nI[3])
            {

                collectiblesInfo[(temp * 2) - 2] = colI[(temp * 2) - 2];
                collectiblesInfo[(temp * 2) - 1] = colI[(temp * 2) - 1];

                temp++;
            }

            //DebugSensorsInfo();

            firstUpdate = true;
        }


        private void setColI(CollectibleRepresentation[] inputColI)
        {
            colI = inputColI;
        }

        public override void SensorsUpdated(int nC, RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {

        }


        public override bool ImplementedAgent()
        {
            return implementedAgent;
        }

        //Manager gets this action from agent
        public override Moves GetAction()
        {
            if (currentAction == Moves.NO_ACTION)
            {
                return RandomAction();
            }
            return currentAction;
        }

        public override void Update(TimeSpan elapsedGameTime)
        {
            if (firstUpdate)
            {
                Log.LogInformation("First update start");
                //calc route
                Queue<Node> route = calculateRoute();
                //Create driver
                Log.LogInformation("Create driver start");
                driver = new Driver2(nodes, adjacencyMatrix, directionMap, route);
                Log.LogInformation("Create driver end");
                firstUpdate = false;
                Log.LogInformation("First update end");
            }

            moveStep = moveStep % 4;
            if (moveStep == 0)
            {
                currentAction = driver.GetAction(circleInfo);
                if (currentAction == Moves.MOVE_LEFT)
                {
                    currentAction = Moves.ROLL_LEFT;
                } else if (currentAction == Moves.MOVE_RIGHT)
                {
                    currentAction = Moves.ROLL_RIGHT;
                }
                else if (currentAction == Moves.MORPH_UP)
                {
                    currentAction = Moves.JUMP;
                }
            }
            moveStep++;
        }

        private Moves RandomAction()
        {
            switch (rnd.Next(5))
            {
                case 0:
                    return Moves.JUMP;
                case 1:
                case 2:
                    return Moves.ROLL_LEFT;
                case 3:
                case 4:
                    return Moves.ROLL_RIGHT;
            }
            return Moves.NO_ACTION;
        }
        
        public override void EndGame(int collectiblesCaught, int timeElapsed)
        {
        }

        public override string AgentName()
        {
            return agentName;
        }

        public static Queue<Node> calcShortestRouteAStar()
        {
            //All diamond nodes
            List<int> diamondIndex = new List<int>();
            for (int n = 0; n < nodes.Count; n++)
            {
                if (nodes[n].getDiamond())
                {
                    diamondIndex.Add(n);
                }
            }

            int startIndex = 0;
            List<Queue<Node>> queueList = new List<Queue<Node>>();
            while (diamondIndex.Count > 0)
            {
                //calc AStar to each diamond
                List<Queue<Node>> queueListTemp = new List<Queue<Node>>();
                List<int> queueDistanceListTemp = new List<int>();
                AStar astar;
                for (int i = 0; i < diamondIndex.Count; i++)
                {
                    astar = new AStar(startIndex, diamondIndex[i]);
                    queueListTemp.Add(astar.Run());
                    queueDistanceListTemp.Add(astar.GetCompleteDistance());
                }
                //search shortest
                int shortest = queueDistanceListTemp[0];
                for (int i = 1; i < queueDistanceListTemp.Count; i++)
                {
                    if (queueDistanceListTemp[i] < shortest)
                    {
                        shortest = queueDistanceListTemp[i];
                    }
                }
                //if shortest is a route add to route list, set new start, remove diamond

                if (shortest != int.MaxValue)
                {
                    int shortestIndex = queueDistanceListTemp.IndexOf(shortest);
                    queueList.Add(queueListTemp[shortestIndex]);
                    startIndex = diamondIndex[shortestIndex];
                    diamondIndex.Remove(startIndex);
                }
                else
                {
                    break;
                }
            }

            //merge all single routes to one
            if (queueList.Count == 0 || queueList[0] == null)
            {
                return new Queue<Node>();
            }
            List<Node> completeList = new List<Node>();
            completeList.AddRange(queueList[0].ToList());

            for (int i = 1; i < queueList.Count; i++)
            {
                List<Node> temp = queueList[i].ToList();
                temp.RemoveAt(0);
                completeList.AddRange(temp);
            }

            Queue<Node> completeQueue = new Queue<Node>();
            for (int i = 0; i < completeList.Count; i++)
            {
                completeQueue.Enqueue(completeList[i]);
            }
            return completeQueue;
        }

        public Queue<Node> calculateRoute()
        {
            Log.LogInformation("Abstraction calc start");
            System.Diagnostics.Stopwatch sw2 = System.Diagnostics.Stopwatch.StartNew();
            CreateObstacleOpenSpaceArray();
            CreateNodes();
            CreateEdgesAndAdjacencyMatrix();
            Log.LogInformation("Elapsed abstraction calculation time in ms: " + sw2.ElapsedMilliseconds);
            Log.LogInformation("Abstraction calc end");

            Log.LogInformation("Route calc start");

            Queue<Node> route = new Queue<Node>();

            //THESE LINES ARE ONLY FOR GREEDY GOAL
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            route = calcShortestRouteAStar();
            Log.LogInformation("Elapsed Greedy Goal AStar time in ms: " + sw.ElapsedMilliseconds);
            //END OF GREEDY GOAL ONLY LINES
            Log.LogInformation("Route calc end");

            return route;
        }

        private void CreateObstacleOpenSpaceArray()
        {
            obstacleOpenSpace = new bool[fullHeight, fullWidth];
            int obstaclePixelCounter = 0;
            //create black borders of the game area
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < fullWidth; j++)
                {
                    obstacleOpenSpace[i, j] = true;
                    obstaclePixelCounter++;
                    obstacleOpenSpace[fullHeight - 1 - i, j] = true;
                    obstaclePixelCounter++;
                }
                for (int k = 40; k < fullHeight - 40; k++)
                {
                    obstacleOpenSpace[k, i] = true;
                    obstaclePixelCounter++;
                    obstacleOpenSpace[k, fullWidth - 1 - i] = true;
                    obstaclePixelCounter++;
                }
            }

            //Fill in obstacles
            for (int i = 0; i < obstaclesInfo.Length; i = i + 4)
            {
                int x = (int)obstaclesInfo[i];
                int y = (int)obstaclesInfo[i + 1];
                int h = (int)obstaclesInfo[i + 2];
                int w = (int)obstaclesInfo[i + 3];
                int upperLeftX = x - (w / 2);
                int upperLeftY = y - (h / 2);

                for (int j = upperLeftY; j < upperLeftY + h; j++)
                {
                    for (int k = upperLeftX; k < upperLeftX + w; k++)
                    {
                        obstacleOpenSpace[j, k] = true;
                        obstaclePixelCounter++;
                    }
                }
            }

            //Fill in circle specific obstacles
            for (int i = 0; i < circlePlatformsInfo.Length; i = i + 4)
            {
                int x = (int)circlePlatformsInfo[i];
                int y = (int)circlePlatformsInfo[i + 1];
                int h = (int)circlePlatformsInfo[i + 2];
                int w = (int)circlePlatformsInfo[i + 3];
                int upperLeftX = x - (w / 2);
                int upperLeftY = y - (h / 2);

                for (int j = upperLeftY; j < upperLeftY + h; j++)
                {
                    for (int k = upperLeftX; k < upperLeftX + w; k++)
                    {
                        obstacleOpenSpace[j, k] = true;
                        obstaclePixelCounter++;
                    }
                }
            }

        }

        private void CreateNodes()
        {
            nodes = new List<Node>();


            //Square node
            int squareX = (int)circleInfo[0];
            int squareY = (int)circleInfo[1];

            int s = 1;
            while (!obstacleOpenSpace[squareY + s, squareX])
            {
                s++;
            }
            Node square = new Node(squareX, squareY + s - 1, false);
            nodes.Add(square);


            //Nodes created by obstacles
            for (int i = 0; i < obstaclesInfo.Length; i = i + 4)
            {
                int x = (int)obstaclesInfo[i];
                int y = (int)obstaclesInfo[i + 1];
                int h = (int)obstaclesInfo[i + 2];
                int w = (int)obstaclesInfo[i + 3];
                int rawX = x - (w / 2);
                int rawY = y - (h / 2);

                //if upper is free create left node
                if (!obstacleOpenSpace[rawY - 1, rawX])
                {
                    Node node1;
                    //if upper left and left is free create upper left node
                    if (!obstacleOpenSpace[rawY - 1, rawX - 1] && !obstacleOpenSpace[rawY, rawX - 1])
                    {
                        node1 = new Node(rawX - 1, rawY - 1, false);
                        nodes.Add(node1);
                        //Node created by obstacles fall down
                        for (int j = rawY; j < fullHeight; j++)
                        {
                            if (obstacleOpenSpace[j, rawX - 1])
                            {
                                Node node2 = new Node(rawX - 1, j - 1, false);
                                if (!nodes.Contains(node2))
                                {
                                    nodes.Add(node2);
                                }
                                break;
                            }
                        }
                    }
                    //if upper left and left is obstacle create upper node
                    else if (obstacleOpenSpace[rawY - 1, rawX - 1] && obstacleOpenSpace[rawY, rawX - 1])
                    {
                        node1 = new Node(rawX, rawY - 1, false);
                        if (!nodes.Contains(node1))
                        {
                            nodes.Add(node1);
                        }
                    }

                }

                rawX = x + (w / 2) - 1;
                //If upper is free create right node
                if (!obstacleOpenSpace[rawY - 1, rawX])
                {
                    Node node1;
                    //if upper right and right is free create upper right node
                    if (!obstacleOpenSpace[rawY - 1, rawX + 1] && !obstacleOpenSpace[rawY, rawX + 1])
                    {
                        node1 = new Node(rawX + 1, rawY - 1, false);
                        nodes.Add(node1);
                        //Node created by obstacles fall down
                        for (int j = rawY; j < fullHeight; j++)
                        {
                            if (obstacleOpenSpace[j, rawX + 1])
                            {
                                Node node2 = new Node(rawX + 1, j - 1, false);
                                if (!nodes.Contains(node2))
                                {
                                    nodes.Add(node2);
                                }
                                break;
                            }
                        }
                    }
                    //if upper right and right is obstacle create upper node
                    else if (obstacleOpenSpace[rawY - 1, rawX + 1] && obstacleOpenSpace[rawY, rawX + 1])
                    {
                        node1 = new Node(rawX, rawY - 1, false);
                        if (!nodes.Contains(node1))
                        {
                            nodes.Add(node1);
                        }
                    }

                }

            }

            //Nodes created by circle specific obstacles
            for (int i = 0; i < circlePlatformsInfo.Length; i = i + 4)
            {
                int x = (int)circlePlatformsInfo[i];
                int y = (int)circlePlatformsInfo[i + 1];
                int h = (int)circlePlatformsInfo[i + 2];
                int w = (int)circlePlatformsInfo[i + 3];
                int rawX = x - (w / 2);
                int rawY = y - (h / 2);

                //if upper is free create left node
                if (!obstacleOpenSpace[rawY - 1, rawX])
                {
                    Node node1;
                    //if upper left and left is free create upper left node
                    if (!obstacleOpenSpace[rawY - 1, rawX - 1] && !obstacleOpenSpace[rawY, rawX - 1])
                    {
                        node1 = new Node(rawX - 1, rawY - 1, false);
                        nodes.Add(node1);
                        //Node created by obstacles fall down
                        for (int j = rawY; j < fullHeight; j++)
                        {
                            if (obstacleOpenSpace[j, rawX - 1])
                            {
                                Node node2 = new Node(rawX - 1, j - 1, false);
                                if (!nodes.Contains(node2))
                                {
                                    nodes.Add(node2);
                                }
                                break;
                            }
                        }
                    }
                    //if upper left and left is obstacle create upper node
                    else if (obstacleOpenSpace[rawY - 1, rawX - 1] && obstacleOpenSpace[rawY, rawX - 1])
                    {
                        node1 = new Node(rawX, rawY - 1, false);
                        if (!nodes.Contains(node1))
                        {
                            nodes.Add(node1);
                        }
                    }

                }

                rawX = x + (w / 2) - 1;
                //If upper is free create right node
                if (!obstacleOpenSpace[rawY - 1, rawX])
                {
                    Node node1;
                    //if upper right and right is free create upper right node
                    if (!obstacleOpenSpace[rawY - 1, rawX + 1] && !obstacleOpenSpace[rawY, rawX + 1])
                    {
                        node1 = new Node(rawX + 1, rawY - 1, false);
                        nodes.Add(node1);
                        //Node created by obstacles fall down
                        for (int j = rawY; j < fullHeight; j++)
                        {
                            if (obstacleOpenSpace[j, rawX + 1])
                            {
                                Node node2 = new Node(rawX + 1, j - 1, false);
                                if (!nodes.Contains(node2))
                                {
                                    nodes.Add(node2);
                                }
                                break;
                            }
                        }
                    }
                    //if upper right and right is obstacle create upper node
                    else if (obstacleOpenSpace[rawY - 1, rawX + 1] && obstacleOpenSpace[rawY, rawX + 1])
                    {
                        node1 = new Node(rawX, rawY - 1, false);
                        if (!nodes.Contains(node1))
                        {
                            nodes.Add(node1);
                        }
                    }

                }

            }


            //Nodes created by diamonds
            for (int i = 0; i < collectiblesInfo.Length; i = i + 2)
            {
                int x = (int)collectiblesInfo[i];
                int y = (int)collectiblesInfo[i + 1];
                Node node1 = new Node(x, y, true);
                Node node2 = null;
                //Fall down nodes of diamonds
                int j = 1;
                while (!obstacleOpenSpace[y + j, x])
                {
                    j++;
                }
                if (j > 1)
                {
                    node2 = new Node(x, y + j - 1, false);

                }

                if (j == 1)
                {
                    nodes.Add(node1);
                }
                else if (j <= diamondFallDownThreshold)
                {
                    node2.setDiamond(true);
                    nodes.Add(node2);
                }
                else if (j > diamondFallDownThreshold)
                {
                    nodes.Add(node1);
                    nodes.Add(node2);
                }
            }

            Log.LogInformation("SQUARE - Number of nodes: " + nodes.Count);
            Log.LogInformation("SQUARE - Number of diamonds: " + collectiblesInfo.Length / 2);
        }

        public static int[] CheckEdge(Node n1, Node n2)
        {
            int deltaX = n1.getX() - n2.getX();
            int deltaY = n1.getY() - n2.getY();

            int edge = 0;
            int direction = -1;
            int distance = -1;
            if (deltaX < 0 && deltaY == 0)
            {
                direction = (int)Direction.Right;
                distance = deltaX * -1;
            }
            if (deltaX < 0 && deltaY < 0 && !(n1.getDiamond() || n2.getDiamond()))
            {
                direction = (int)Direction.RightDown;
                distance = (int)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            }
            if (deltaX == 0 && deltaY < 0)
            {
                direction = (int)Direction.Down;
                distance = deltaY * -1;
            }
            if (deltaX > 0 && deltaY < 0 && !(n1.getDiamond() || n2.getDiamond()))
            {
                direction = (int)Direction.LeftDown;
                distance = (int)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            }
            if (deltaX > 0 && deltaY == 0)
            {
                direction = (int)Direction.Left;
                distance = deltaX;
            }
            if (deltaX > 0 && deltaY > 0 && !(n1.getDiamond() || n2.getDiamond()))
            {
                direction = (int)Direction.LeftUp;
                distance = (int)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            }
            if (deltaX == 0 && deltaY > 0)
            {
                direction = (int)Direction.Up;
                distance = deltaY;
            }
            if (deltaX < 0 && deltaY > 0 && !(n1.getDiamond() || n2.getDiamond()))
            {
                direction = (int)Direction.RightUp;
                distance = (int)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            }


            //Square    
            bool obstacle = checkSquareSize(nSquareSize, direction, n1, n2);

            //if obstacle false ok else horizontal
            if (!obstacle)
            {
                return new int[] { 1, direction, distance };
            }
            else
            {
                obstacle = checkSquareSize(hSquareSize, direction, n1, n2);
            }

            //if obstacle false ok else vertical
            if (!obstacle)
            {
                return new int[] { 2, direction, distance };
            }
            else
            {
                obstacle = checkSquareSize(vSquareSize, direction, n1, n2);
            }

            //if obstacle false ok else nothing
            if (!obstacle)
            {
                return new int[] { 3, direction, distance };
            }
            

            return new int[] { edge, direction, distance };
        }

        public static void CreateEdgesAndAdjacencyMatrix()
        {
            adjacencyMatrix = new int[nodes.Count, nodes.Count];
            directionMap = new int[nodes.Count, nodes.Count];
            directDistanceMap = new int[nodes.Count, nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    Node n1 = nodes[i];
                    Node n2 = nodes[j];
                    //if (abstractionOutput)
                    //{
                        Log.LogInformation("N1: " + n1 + " N2: " + n2);
                    //}
                    int[] actionDirectionDistance = CheckEdge(n1, n2);
                    adjacencyMatrix[i, j] = actionDirectionDistance[0];
                    directionMap[i, j] = actionDirectionDistance[1];
                    directDistanceMap[i, j] = actionDirectionDistance[2];
                    if (actionDirectionDistance[1] == 2 && actionDirectionDistance[0] != 0)
                    {
                        nodes[i].setLeadsToFallDown(true);
                    }
                    Log.LogInformation("Edge: " + adjacencyMatrix[i, j]);
                }
            }
        }

        public static bool checkSquareSize(int[] nSquareSize, int direction, Node n1, Node n2)
        {
            int x0 = n1.getX();
            int y0 = n1.getY();
            int x1 = n2.getX();
            int y1 = n2.getY();

            int x = Math.Abs(x1 - x0);
            int y = Math.Abs(y1 - y0);

            Log.LogInformation("X: " + x + "Y: " + y);

            if (direction == (int)Direction.Right)
            {
                //Shift edge to right
                int distanceWithoutSurface = 0;
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < nSquareSize[1]; j++)
                    {
                        if (obstacleOpenSpace[n1.getY() - j, n1.getX() + i])
                        {
                            return true;
                        }
                    }
                    //distance where is no surface
                    if (!obstacleOpenSpace[n1.getY() + 1, n1.getX() + i])
                    {
                        distanceWithoutSurface++;
                        if (distanceWithoutSurface > 200 || (distanceWithoutSurface + 5) >= Math.Abs(n1.getX() - n2.getX()))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        distanceWithoutSurface = 0;
                    }
                }
            }
            else if (direction == (int)Direction.Left)
            {
                //Shift edge to left
                int distanceWithoutSurface = 0;
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < nSquareSize[1]; j++)
                    {
                        if (obstacleOpenSpace[n1.getY() - j, n1.getX() - i])
                        {
                            return true;
                        }
                    }
                    //distance where is no surface
                    if (!obstacleOpenSpace[n1.getY() + 1, n1.getX() - i])
                    {
                        distanceWithoutSurface++;
                        if (distanceWithoutSurface > 200 || (distanceWithoutSurface + 5) >= Math.Abs(n1.getX() - n2.getX()))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        distanceWithoutSurface = 0;
                    }
                }
            }
            else if (direction == (int)Direction.Down)
            {
                //Shift edge to down
                bool toRight = false;
                bool toLeft = false;
                if (n1.getX() + nSquareSize[0] - 1 > fullWidth)
                {
                    toRight = true;
                }
                else
                {
                    for (int i = 0; i < y; i++)
                    {
                        for (int j = 0; j < nSquareSize[0]; j++)
                        {
                            if (obstacleOpenSpace[n1.getY() + i, n1.getX() + j])
                            {
                                toRight = true;
                            }
                        }
                    }
                }
                if (n1.getX() - nSquareSize[0] + 1 < 0)
                {
                    toLeft = true;
                }
                else
                {
                    for (int i = 0; i < y; i++)
                    {
                        for (int j = 0; j < nSquareSize[0]; j++)
                        {
                            if (obstacleOpenSpace[n1.getY() + i, n1.getX() - j])
                            {
                                toLeft = true;
                            }
                        }
                    }
                }
                if (toRight && toLeft)
                {
                    return true;
                }
            }
            else if (direction == (int)Direction.Up && nSquareSize[1] >= y)
            {
                //Shift edge to up
                bool toRight = false;
                bool toLeft = false;
                if (n1.getX() + nSquareSize[0] - 1 > fullWidth)
                {
                    toRight = true;
                }
                else
                {
                    for (int i = 0; i < y; i++)
                    {
                        for (int j = 0; j < nSquareSize[0]; j++)
                        {
                            if (obstacleOpenSpace[n1.getY() - i, n1.getX() + j])
                            {
                                toRight = true;
                            }
                        }
                    }
                }
                if (n1.getX() - nSquareSize[0] + 1 < 0)
                {
                    toLeft = true;
                }
                else
                {
                    for (int i = 0; i < y; i++)
                    {
                        for (int j = 0; j < nSquareSize[0]; j++)
                        {
                            if (obstacleOpenSpace[n1.getY() - i, n1.getX() - j])
                            {
                                toLeft = true;
                            }
                        }
                    }
                }
                if (toRight && toLeft)
                {
                    return true;
                }
            }
            else if (direction == (int)Direction.RightDown)
            {
                Log.LogInformation(nSquareSize + "RightDown: N1: " + n1 + " X: " + x + " Y: " + y);
                Log.LogInformation(nSquareSize + "RightDown: N2: " + n2);

                List<int[]> pixels = PixelLine(x0, y0, x1, y1);
                if (pixels == null)
                {
                    return true;
                }
                //squaresize control
                bool squareSizeControl = CheckSquareSizeDiagonalLine(pixels);
                if (squareSizeControl)
                {
                    return true;
                }
            }
            else if (direction == (int)Direction.LeftDown)
            {
                List<int[]> pixels = PixelLine(x0, y0, x1, y1);
                if (pixels == null)
                {
                    return true;
                }
                //squaresize control
                bool squareSizeControl = CheckSquareSizeDiagonalLine(pixels);
                if (squareSizeControl)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        // Bresenham's line algorithm
        public static List<int[]> PixelLine(int x0, int y0, int x1, int y1)
        {
            List<int[]> pixels = new List<int[]>();

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;
            int e2;

            while (true)
            {
                //if obstacle return empty list
                if (obstacleOpenSpace[y0, x0])
                {
                    return null;
                }
                pixels.Add(new int[] { x0, y0 });
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }
                e2 = 2 * err;
                if (e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return pixels;
        }

        public static bool CheckSquareSizeDiagonalLine(List<int[]> pixels)
        {
            //squaresize control
            foreach (int[] p in pixels)
            {
                bool rightObstacle = false;
                int i;
                for (i = 0; i < nSquareSize[0]; i++)
                {
                    if (obstacleOpenSpace[p[1], p[0] + i])
                    {
                        rightObstacle = true;
                        break;
                    }
                }
                if (rightObstacle)
                {
                    for (int j = 0; j < nSquareSize[0] - i; j++)
                    {
                        if (obstacleOpenSpace[p[1], p[0] - j])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}


