using GeometryFriends;
using GeometryFriends.AI;
using System;
using System.Collections.Generic;
using GeometryFriends.AI.Perceptions.Information;
namespace GeometryFriendsAgents
{
    class Driver
    {
        private List<Node> nodes;
        private int[,] adjacencyMatrix;
        // private float[,] distanceMap;
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
        private float distance, oneNodeDistance;
        private List<float> distanceList;
        bool output = false;
        enum Direction { Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp };




        public Driver(List<Node> nodes, Queue<Node> route)
        {
            this.nodes = nodes;
            this.route = route;
            distanceList = new List<float>();
            this.previousNode = route.Dequeue();
            this.nextNode = route.Dequeue();
            this.nextNode2 = route.Dequeue();
           
        }
            

        //TODO (maybe) Update the A* search path each time getAction is called?


        public Moves getAction(float[] shapeInfo)
        {
            // circleInfo[] Description
            //
            // Index - Information
            //
            //   0  - Circle X Position
            //   1  - Circle Y Position
            //   2  - Circle X Velocity
            //   3  - Circle Y Velocity
            //   4  - Circle Radius

            int x = (int)shapeInfo[0];
            int y = (int)shapeInfo[1];
            int vX = (int)shapeInfo[2];
            int vY = (int)shapeInfo[3];
            int h = (int)shapeInfo[4];

            int direction;

            //Get the x,y coords of the next node in the route
            int nextX = (int)(nextNode.location.X);
            int nextY = (int)(nextNode.location.Y);


            //Euclidean distance between circle and next node
            distance = (float)Math.Sqrt(Math.Pow(x - nextNode.location.X, 2) + Math.Pow(y - nextNode.location.Y, 2));

            oneNodeDistance = (float)Math.Sqrt(Math.Pow((nextNode.location.X * 2) - nextNode.location.X, 2) + Math.Pow((nextNode.location.Y * 2) - nextNode.location.Y, 2));



            //Make decision on next movement

            bool fast = (vX > 2 || vX < -2);

            //NextNode is adjacent and to the left and going fast on x
            if ( nextX < x && distance < oneNodeDistance && fast)
            {
                direction = 6; //Do nothing
            }

            //NextNode is adjacent and to the left and going slow on x
            else if (nextX < x && distance < oneNodeDistance && !fast)
            {
                direction = 0; //Roll left
            }

            //NextNode is far and to the left
            else if (nextX < x && distance >= oneNodeDistance)
            {
                direction = 1; //Jump left!
            }

            //NextNode is directly above
            else if (nextY > y && nextX == x)
            {
                direction = 2; //Jump!
            }

            //NextNode is directly below
            else if (nextY < y && nextX == x)
            {
                direction = 6; //Stay still and fall!
            }

            //NextNode is adjacent and to the right and going fast on x
            else if (nextX > x && distance < oneNodeDistance && fast)
            {
                direction = 6; //Do nothing
            }

            //NextNode is adjacent and to the right
            else if (nextX > x && distance < oneNodeDistance && !fast)
            {
                direction = 4; //Roll right
            }

            //NextNode is far and to the right
            else if (nextX < x && distance >= oneNodeDistance)
            {
                direction = 3; //Jump right!
            }
            else
            {
                direction = 6; // Do nothing
            }

            
           


            // Directions
            //  1 2 3 
            //  0   4
            //  7 6 5  

            //Return the movement
            if (direction == 0)
            {
                return Moves.MOVE_LEFT;
            }
            else if (direction == 1)
            {
                return Moves.JUMP;
            }
            else if (direction == 2)
            {
                return Moves.JUMP;
            }
            else if (direction == 3)
            {
                return Moves.JUMP;
            }
            else if (direction == 4)
            {
                return Moves.MOVE_RIGHT;
            }
            else if (direction == 5)
            {
                return Moves.MOVE_RIGHT;
            }
            else if (direction == 6)
            {
                return Moves.NO_ACTION;
            }
            else if (direction == 7)
            {
                return Moves.MOVE_LEFT;
            }
            else
            {
                return Moves.NO_ACTION;
            }   


        }

        
       
    }
}
 