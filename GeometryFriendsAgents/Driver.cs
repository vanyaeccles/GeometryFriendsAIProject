using GeometryFriends;
using GeometryFriends.AI;
using System;
using System.Collections.Generic;
using GeometryFriends.AI.Perceptions.Information;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryFriends.AI.Perceptions.Information;
using System.Drawing;
using System.Diagnostics;

namespace GeometryFriendsAgents
{
    class Driver
    {
       
        private Queue<Node> solution;
        private Node previousNode;
        private Node nextNode;
        private Node nextNode2;
        private Node nextNode3;
        private int previousDirection;
        private int direction;
        private int direction2;
        private int previousAction;
        private int action;
        private int action2;
        private float distance, oneNodeDistance, previousDistance;
        private List<float> distanceList;
        enum Direction { Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp };
        private bool round1 = false;



        public Driver(Queue<Node> solution) //List<Node> nodes, 
        {
            
            this.solution = solution;
            distanceList = new List<float>();
            this.previousNode = solution.Dequeue();
            this.nextNode = solution.Dequeue();
            this.nextNode2 = solution.Dequeue();
            this.nextNode3 = solution.Dequeue();

        }
            
        public void updateSolution(Queue<Node> solution)
        {
            this.solution = solution;
            distanceList = new List<float>();
            this.previousNode = solution.Dequeue();
            this.nextNode = solution.Dequeue();
            this.nextNode2 = solution.Dequeue();
            this.nextNode3 = solution.Dequeue();

        }

        //TODO (maybe) Update the A* search path each time getAction is called?


        public Moves getAction(CircleRepresentation shapeInfo)
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
            
            int x = ((int)shapeInfo.X)/16;
            int y = ((int)shapeInfo.Y)/16;
            int vX = ((int)shapeInfo.VelocityX)/16;
            int vY = ((int)shapeInfo.VelocityY)/16;
            int h = ((int)shapeInfo.Radius)/16;

            int direction;

            //Get the x,y coords of the next node in the route
            int nextX = (int)(nextNode.location.X);
            int nextY = (int)(nextNode.location.Y);


            //Euclidean distance between circle and next node
            distance = (float)Math.Sqrt(Math.Pow(x - nextNode.location.X, 2) + Math.Pow(y - nextNode.location.Y, 2));

            oneNodeDistance = (float)Math.Sqrt(Math.Pow((nextNode.location.X * 2) - nextNode.location.X, 2) + Math.Pow((nextNode.location.Y * 2) - nextNode.location.Y, 2));


            distanceList.Add(distance);

            if (distanceList.Count == 40 && distanceList[0] == distanceList[39])
            {
                direction = 4;
            }

            if (distanceList.Count >= 40)
            {
                distanceList = new List<float>();
            }



            //Make decision on next movement

            bool fast = (vX > 5 || vX < -5);

            //NextNode is adjacent and to the left and going fast on x
            if (nextX < x && distance < oneNodeDistance && fast)
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
                direction = 0; //Jump left!
            }

            //NextNode is directly above
            else if (nextY < y && nextX == x)
            {
                direction = 2; //Jump!
            }

            //NextNode is directly below
            else if (nextY > y && nextX == x)
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
                direction = 4; //Jump right!
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
                return Moves.ROLL_LEFT;
            }
            else if (direction == 1)
            {
                return Moves.ROLL_LEFT;
            }
            else if (direction == 2)
            {
                return Moves.JUMP;
            }
            else if (direction == 3)
            {
                return Moves.ROLL_RIGHT;
            }
            else if (direction == 4)
            {
                return Moves.ROLL_RIGHT;
            }
            else if (direction == 5)
            {
                return Moves.ROLL_RIGHT;
            }
            else if (direction == 6)
            {
                return Moves.NO_ACTION;
            }
            else if (direction == 7)
            {
                return Moves.ROLL_LEFT;
            }
            else
            {
                return Moves.NO_ACTION;
            }

            //switch (direction)
            //{
            //    case 0:
            //        return Moves.ROLL_LEFT;
            //        break;
            //    case 1:
            //        return Moves.ROLL_LEFT;
            //        break;
            //    case 2:
            //        return Moves.JUMP;
            //        break;
            //    case 3:
            //        return Moves.ROLL_RIGHT;
            //        break;
            //    case 4:
            //        return Moves.ROLL_RIGHT;
            //        break;
            //    case 5:
            //        return Moves.ROLL_RIGHT;
            //        break;
            //    case 6:
            //        return Moves.NO_ACTION;
            //        break;
            //    case 7:
            //        return Moves.ROLL_LEFT;
            //        break;

            //}


        }

        
       
    }
}
 