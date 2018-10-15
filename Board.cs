using System;
using System.Diagnostics;
using System.Linq;

namespace TicTacToe
{
    public class GameBoard
    {
        public enum Move { Sp, O, X};
        public enum Player {O, X, None };

        private Move[] board = Enumerable.Repeat(Move.Sp, 9).ToArray();
        public static Random rng = new Random(Guid.NewGuid().GetHashCode());
        /// <summary>
        /// Factory Function for Board class
        /// </summary>
        public static GameBoard StartGame { get { return new GameBoard(); } }

        
        public String Key
        {
            get
            {
                return board.Aggregate("", (a, i) => a + i.ToString());
            }
        }

        internal Move[] Board
        {
            get { return board; }
            set { board = value; }
        }

        public GameBoard() { }
        public GameBoard(Move[] brd)
        {
            Debug.Assert(brd.Length == 9);
            Board = brd;
        }

        public GameBoard(string input)
        {
            Board = ReadBoard(input);           
        }

        public bool ValidMove(int row, int col, Move mv)
        {
            return ((row < 3 && row >= 0) &&
                   (col < 3 && col >= 0) &&
                   (mv != Move.Sp) &&
                   Board[IndexFromRowCol(row,col)] == Move.Sp);
        }

        public bool ValidMove(GameHistory.GameMove mv )//int index, Move mv)
        {
            return ((mv.Position < 9 && mv.Position >= 0) &&                   
                   //(GameBoard.MoveFromPlayer(mv.Player) != Move.Sp) &&
                   Board[mv.Position] == Move.Sp);
        }
        internal static int IndexFromRowCol(int row, int col) { return row * 3 + col;  }

        internal Move[] DoMove(GameHistory.GameMove mv)
        {
            //Debug.Assert(row < 3 && row >= 0);
            //Debug.Assert(col < 3 && col >= 0);
            //Debug.Assert(mv != Move.Sp);
            //int index = row * 3 + col;
            if (Board[mv.Position] == Move.Sp)
            {
                Board[mv.Position] =  mv.Player;
                return Board;
            }

            throw new InvalidOperationException("Cannot place a move on an occupied board position");
        }

        public void DisplayBoard()
        {
            Console.WriteLine("");
            //row 1
            Console.Write("");
            WriteMove(Board[0]);
            Console.Write("|");
            WriteMove(Board[1]);
            Console.Write("|");
            WriteMove(Board[2]);
            Console.WriteLine("");

            //row 2
            Console.Write("");
            WriteMove(Board[3]);
            Console.Write("|");
            WriteMove(Board[4]);
            Console.Write("|");
            WriteMove(Board[5]);
            Console.WriteLine("");

            //row3
            Console.Write("");
            WriteMove(Board[6]);
            Console.Write("|");
            WriteMove(Board[7]);
            Console.Write("|");
            WriteMove(Board[8]);
            Console.WriteLine("");
            Console.WriteLine("");
        }

        public void WriteMove(Move mv)
        {
            switch(mv)
            {
                case Move.Sp:
                    Console.Write(" ");
                    break;
                case Move.O:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("O");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Move.X:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("X");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    throw new InvalidOperationException("Invalid Move Found");
            }
        }

        public string PrintBoard()
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|",
                Board[0], Board[1], Board[2], Board[3], Board[4], 
                Board[5], Board[6], Board[7], Board[8]);
        }

        public static Move[] ReadBoard(string input)
        {
            var moves = input.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(moves.Length == 9);
            return moves.Select(mv => CreateMove(mv)).ToArray();
        }

        private static Move CreateMove(string mv)
        {
            if (mv == " ") return Move.Sp;
            else if (mv == "O") return Move.O;
            else return Move.X;
        }

        public bool IsTie()
        {
            int nspaces = Board.Where(i => i == Move.Sp).Count();
            return (nspaces == 0) &&
                   (!Winner(Player.O)) &&
                   (!Winner(Player.X));
        }

        public  bool Winner(Player pl)
        {
            var Sym = pl == Player.O ? Move.O : Move.X;
            // tic tac toe board layout
            // 0 | 1 | 2
            // 3 | 4 | 5
            // 6 | 7 | 8
            //is player a winner?
            //check for complete rows
            if (Board[0] == Sym && Board[1] == Sym && Board[2] == Sym) return true;
            if (Board[3] == Sym && Board[4] == Sym && Board[5] == Sym) return true;
            if (Board[6] == Sym && Board[7] == Sym && Board[8] == Sym) return true;
            //check for complete columns
            if (Board[0] == Sym && Board[3] == Sym && Board[6] == Sym) return true;
            if (Board[1] == Sym && Board[4] == Sym && Board[7] == Sym) return true;
            if (Board[2] == Sym && Board[5] == Sym && Board[8] == Sym) return true;
            //check for diagonals
            if (Board[0] == Sym && Board[4] == Sym && Board[8] == Sym) return true;
            if (Board[2] == Sym && Board[4] == Sym && Board[6] == Sym) return true;

            return false;
        }

        internal int RandomMove()
        {
            int nspaces = Board.Where(i => i == Move.Sp).Count();
            int n = rng.Next(nspaces);
            for(int i = 0; i < 9; i++)
            {
                // find nth space on board
                if (Board[i] == Move.Sp)
                {                    
                    if (n == 0)
                        return i;
                    n--;
                }
            }
            return -1;
        }
    }
}
