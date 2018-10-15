using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        //Launch Tic Tac Toe game in a console window
        static void Main(string[] args)
        {
            Game tictactoe = new TicTacToe.Game();
            tictactoe.Run();
        }
    }
}
