using GeometryFriends;
using GeometryFriends.AI;
using GeometryFriends.AI.ActionSimulation;
using GeometryFriends.AI.Communication;
using GeometryFriends.AI.Debug;
using GeometryFriends.AI.Interfaces;
using GeometryFriends.AI.Perceptions.Information;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;



namespace GeometryFriendsAgents
{
    /// <summary>
    /// A circle agent implementation for the GeometryFriends game that demonstrates prediction and history keeping capabilities.
    /// </summary>
    public class CircleAgent : AbstractCircleAgent
    {
        //agent implementation specificiation
        private string agentName = "RandPredictorCircle";
        //auxiliary variables for agent action
        private Moves currentAction;
        private List<Moves> possibleMoves;
        private long lastMoveTime;
        //predictor of actions for the circle
        private DebugInformation[] debugInfo = null;
        //Sensors Information and level state
        private CountInformation numbersInfo;
        private CircleRepresentation circleInfo;
        private ObstacleRepresentation[] obstaclesInfo;
        private ObstacleRepresentation[] rectanglePlatformsInfo;
        private ObstacleRepresentation[] circlePlatformsInfo;
        private CollectibleRepresentation[] collectiblesInfo;
        private int nCollectiblesLeft;
        //Area of the game screen
        private Rectangle area;

        bool firstSetup;


        Queue<Node> solution;
        Solver solver;

        Driver driver;
        int moveStepSize;
        int moveStepSizeReSolve;

        public CircleAgent()
        {

            //setup for action updates
            lastMoveTime = DateTime.Now.Second;
            currentAction = Moves.ROLL_RIGHT;
            //prepare the possible moves  
            possibleMoves = new List<Moves>();
            possibleMoves.Add(Moves.ROLL_LEFT);
            possibleMoves.Add(Moves.ROLL_RIGHT);
            possibleMoves.Add(Moves.JUMP);
            possibleMoves.Add(Moves.GROW);
            possibleMoves.Add(Moves.NO_ACTION);
            Debug.WriteLine("test");
            Log.LogInformation("Circle Agent - " + numbersInfo.ToString());
        }
        //implements abstract circle interface: used to setup the initial information so that the agent has basic knowledge about the level
        public override void Setup(CountInformation nI, RectangleRepresentation rI, CircleRepresentation cI, ObstacleRepresentation[] oI, ObstacleRepresentation[] rPI, ObstacleRepresentation[] cPI, CollectibleRepresentation[] colI, Rectangle area, double timeLimit)
        {
            numbersInfo = nI;
            nCollectiblesLeft = nI.CollectiblesCount;
            circleInfo = cI;
            obstaclesInfo = oI;
            rectanglePlatformsInfo = rPI;
            circlePlatformsInfo = rPI;
            collectiblesInfo = colI;
            this.area = area;
            solver = new Solver(new Graph(obstaclesInfo, circlePlatformsInfo, collectiblesInfo, circleInfo, this.area));
            solution = solver.solve(cI, collectiblesInfo);
            int path = 0;
            bool written = false;
            moveStepSize = 0;
            for (int y = 0; y < solver.graph.height; y++)
            {
                string line = "|";
                for (int x = 0; x < solver.graph.width; x++)
                {
                    foreach (Node node in solution)
                    {
                        if (node.location == new Point(x,y))
                        {
                            line += path.ToString();
                            path++;
                            if (path > 9)
                                path = 0;
                            written = true;
                            continue;
                        }
                    }
                    if (written)
                        written = false;
                    else if (x == solver.graph.startLocation.X && y == solver.graph.startLocation.Y)
                        line += "O";
                    else if (x == solver.graph.endLocation.X && y == solver.graph.endLocation.Y)
                        line += "D";
                    else if (solver.graph.trueMap[x, y])
                        line += " ";
                    else
                        line += "X";

                }
                line += "|";
                Debug.Print(line);
            }
            driver = new Driver(solution);
        }

        public override void SensorsUpdated(int nC, RectangleRepresentation rI, CircleRepresentation cI, CollectibleRepresentation[] colI)
        {
            nCollectiblesLeft = nC;
            circleInfo = cI;
            collectiblesInfo = colI;
        }

        public void updateSolution(CircleRepresentation cI)
        {
            solution = solver.solve(cI, collectiblesInfo);
            driver = new GeometryFriendsAgents.Driver(solution);
        }

        //implements abstract circle interface: signals if the agent is actually implemented or not
        public override bool ImplementedAgent()
        {
            return true;
        }
        //implements abstract circle interface: provides the name of the agent to the agents manager in GeometryFriends
        public override string AgentName()
        {
            return agentName;
        }
        //implements abstract circle interface: GeometryFriends agents manager gets the current action intended to be actuated in the enviroment for this agent
        public override Moves GetAction()
        {
            return currentAction;
        }


        //implements abstract circle interface: updates the agent state logic and predictions
        public override void Update(TimeSpan elapsedGameTime)
        {
            moveStepSize = moveStepSize % 4;
            //moveStepSizeReSolve = moveStepSize % 3;
            if (moveStepSize == 0)
            {

            //    if (moveStepSize == 0)
            //    {
                    int x = ((int)circleInfo.X);
                    int y = ((int)circleInfo.Y);

                    //Get circle current point
                    Point circlePoint = new Point(x, y);


                    //Get closest diamond point
                    Point closestDiamond = solver.graph.getClosestDiamond(collectiblesInfo, circlePoint);

                    solver.setStartEnd(circlePoint, closestDiamond);
                    solution = solver.solve(circleInfo, collectiblesInfo);
                    driver.updateSolution(solution);
                //}



                currentAction = driver.getAction(circleInfo);



                
                //if (currentAction == Moves.MOVE_LEFT)
                //{
                //    currentAction = Moves.ROLL_LEFT;
                //}
                //else if (currentAction == Moves.MOVE_RIGHT)
                //{
                //    currentAction = Moves.ROLL_RIGHT;
                //}
                //else if (currentAction == Moves.JUMP)
                //{
                //    currentAction = Moves.JUMP;
                //}
            }
            moveStepSize++;

        }


        //typically used console debugging used in previous implementations of GeometryFriends
        protected void DebugSensorsInfo()
        {
            Log.LogInformation("Circle Agent - " + numbersInfo.ToString());
            Log.LogInformation("Circle Agent - " + circleInfo.ToString());
            foreach (ObstacleRepresentation i in obstaclesInfo)
            {
                Log.LogInformation("Circle Agent - " + i.ToString("Obstacle"));
            }
            foreach (ObstacleRepresentation i in rectanglePlatformsInfo)
            {
                Log.LogInformation("Circle Agent - " + i.ToString("Rectangle Platform"));
            }
            foreach (ObstacleRepresentation i in circlePlatformsInfo)
            {
                Log.LogInformation("Circle Agent - " + i.ToString("Circle Platform"));
            }
            foreach (CollectibleRepresentation i in collectiblesInfo)
            {
                Log.LogInformation("Circle Agent - " + i.ToString());
            }
        }
        //implements abstract circle interface: signals the agent the end of the current level
        public override void EndGame(int collectiblesCaught, int timeElapsed)
        {
            Log.LogInformation("CIRCLE - Collectibles caught = " + collectiblesCaught + ", Time elapsed - " + timeElapsed);
        }
        //implements abstract circle interface: gets the debug information that is to be visually represented by the agents manager
        public override DebugInformation[] GetDebugInformation()
        {
            return debugInfo;
        }

    }
}