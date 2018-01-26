using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace Mankalah

{
    /*****************************************************************/
    /*
    /* A Dumb Mankalah player.  This player always takes the first
    /* first available go-again, if there is one. If not, it takes 
    /* the first available move.
    /*
    /*****************************************************************/

    public class djw33Player : Player
    {

        public djw33Player(Position pos, int timeLimit) : base(pos, "Daniel", timeLimit) { }
        
        public override string gloat() {
            return "Get good!!!";
        }

        public override String getImage() { return "djw33.jpg"; }


        public override int chooseMove(Board b) 
        {
            Stopwatch timer = new Stopwatch();//create a timer so that the player only goes to timelimit
            timer.Restart();//start the timer
            Board newboard = new Board(b);//make a copy of the board so you dont accidently change the board and cheat
            int[] value = new int[2]; //contains best score and best move
            int depth = 0;//initial depth
            int[] valid = new int[2];//checks to see if the dfs was inturrupted
            while (timer.ElapsedMilliseconds < getTimePerMove()-10)
            {
                depth++;//if there is enough time, increment the depth and go again.
                if(b.whoseMove()==Position.Top)//different last best scores for max and mins
                    valid= prunedminimax(newboard, depth, timer,1111111,b.whoseMove(),0);
                else
                    valid = prunedminimax(newboard, depth, timer, -1111111, b.whoseMove(), 0);
                if (valid[1] != -1) value = valid;//sets value to valid if the result was valid
            }

  //          Console.Write("Depth: {0}, Best Move: {1}, Best Score: {2}",depth,value[1],value[0] ); //used to monitor bot
            return value[1];		        // returns the move
        }
        /*
         * Evaluate function to evaluate the nodes of my tree. I added a variable to keep track of how many turns players got.
         * if each player got equal number of turns then the number is 0, if top got more is <0 and opposite for bottom
         * Note: my Max is for the bottom because I could visualize it better that way.
         * */
        public int evaluate(Board b, int playsinarow) 
        {
            int score = b.stonesAt(6) - b.stonesAt(13);
            int stones = 0;
            int distributedstones = 0;

            
                
            for (int i=0; i<6; i++)
            {
                stones = stones + (b.stonesAt(i) - b.stonesAt(12- i));//who has controll of more stones

                
            }
          
            if (b.gameOver()) score += stones;//if game is over add the extra stones to the scores

            int goagains = 0; //figure out how many go agains may be possible at the begininng of the next turn
            if (b.whoseMove() == Position.Top)
            {
                for (int i = 12; i >= 7; i--)
                {               // try first go-again
                    if (b.stonesAt(i) == 13 - i) goagains--;
                    distributedstones =  i - 13 + b.stonesAt(i);//find the distribution of the stones
                }
            }
            else
            {
                for (int i = 5; i >= 0; i--)
                {
                    if (b.stonesAt(i) == 6 - i) goagains++;
                    distributedstones = 6 - i - b.stonesAt(i);
                }

            }    
            //note that plays and go agains are raised to the power of three to conserve signs and help promote sprees.       
            return 40 * score +2*Convert.ToInt32(Math.Pow((playsinarow+ goagains), 3))+ 30*distributedstones;//weights were found be tweeking to find the most defensive and offensive bot.
        }

        //Minimax function. Note that the Max is for the bottom play, It just made more sense to me that way. returns best score and best move.
        private int[] prunedminimax(Board b, int depth, Stopwatch timer,int lastbestscore,Position lastplayer, int playsinarow)
        {
            int[] value = new int[2];
            int bestscore;
            if (b.whoseMove() == Position.Top) //set the infinity max or infinity min depending on whose turn it is.
                bestscore = 1111111;
            else
                bestscore = -1111111;
            

            int bestmove = -1; //returns this number if DFS doesnt finish
            int[] tempvalue = new int[2]; //temporary value that will be used to find the best score and value
            if ((depth > 0 && !b.gameOver())) //as long as game isnt over, or you reached the depth
            {
                bool firstturn = true; //used to keep track of the first turn
                for (int i = 0; i < 6; i++)
                {
                    if (b.whoseMove() == Position.Top)
                    {
                        if (b.legalMove(i + 7))
                        {
                            Board currentBoard = new Board(b);
                            currentBoard.makeMove(i + 7, false);
                            if (lastplayer == Position.Top)//this is used to keep track of who has had the most turns
                                tempvalue = prunedminimax(currentBoard, depth - 1,timer, bestscore,Position.Top, playsinarow-1);
                            else
                                tempvalue = prunedminimax(currentBoard, depth - 1, timer, bestscore, Position.Top, playsinarow);
                            if (firstturn)
                            {
                                bestscore = tempvalue[0];
                                bestmove = i + 7;
                                firstturn = false;//no longer first turn so set to false
                            }
                            //for non-first turns, check to see if tempvalue is lower then best score.
                            else if (tempvalue[0] < bestscore)
                            {
                                bestscore = tempvalue[0];
                                bestmove = i + 7;
                            }

                            if ((lastplayer == Position.Bottom) && (bestscore < lastbestscore)) break;//prunes the branches if necesary. 
                            
                        }
                    }
                    else
                    {
                        if (b.legalMove(i))
                        {
                            Board currentBoard = new Board(b);
                            currentBoard.makeMove(i, false);
                            if (lastplayer == Position.Bottom)
                                tempvalue = prunedminimax(currentBoard, depth - 1, timer, bestscore, Position.Bottom, playsinarow + 1);
                            else
                                tempvalue = prunedminimax(currentBoard, depth - 1, timer, bestscore, Position.Bottom, playsinarow);
                            if (firstturn)
                            {
                                bestscore = tempvalue[0];
                                bestmove = i;
                                firstturn = false;
                            }
                            else if (tempvalue[0] > bestscore)
                            {
                                bestscore = tempvalue[0];
                                bestmove = i;
                            }
                            if ((lastplayer == Position.Top) && (bestscore > lastbestscore)) break;
                            
                        }
                    }
                    if (!(timer.ElapsedMilliseconds < getTimePerMove() - 10))//if there are only 10 ms till the turn is up,  return -1 for the best move
                    {
                        value[1] = -1;
                       return value;
                    }
                }
            }
            else //reaches this if it is a node.
            {
                bestscore = evaluate(b,playsinarow);
            }
            value[0] = bestscore;
            value[1] = bestmove;
            
            return value;
        }
    }



}