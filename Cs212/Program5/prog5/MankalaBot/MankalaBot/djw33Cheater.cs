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
    /* Cheating Mankala Player, moves all of the peices into his kala except for 1. 
     * and then finishes off the game like a pro.
    /*
    /*****************************************************************/

    public class djw33Cheater : Player
    {

        public djw33Cheater(Position pos, int timeLimit) : base(pos, "Kelso", timeLimit) { }

        public override string gloat()
        {
            return "You should get good!!";
        }

        public override int chooseMove(Board b)
        {
            if (b.whoseMove() == Position.Top)
            {
                b.board[0] = 0;
                b.board[1] = 0;
                b.board[2] = 0;
                b.board[3] = 0;
                b.board[4] = 0;
                b.board[5] = 0;
                b.board[6] = 0;
                b.board[7] = 0;
                b.board[8] = 0;
                b.board[9] = 0;
                b.board[10] = 0;
                b.board[11] = 0;
                b.board[12] = 1;
                b.board[13] = 47;
                return 12;
            }
            else
            {
                b.board[0] = 0;
                b.board[1] = 0;
                b.board[2] = 0;
                b.board[3] = 0;
                b.board[4] = 0;
                b.board[5] = 1;
                b.board[6] = 47;
                b.board[7] = 0;
                b.board[8] = 0;
                b.board[9] = 0;
                b.board[10] = 0;
                b.board[11] = 0;
                b.board[12] = 0;
                b.board[13] = 0;
                return 5;
            }
        }
        
    }
}
