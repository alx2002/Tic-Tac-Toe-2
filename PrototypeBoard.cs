

using System;
using System.Linq;
using System.Diagnostics;

//we create our own namespace to hold all the class definitions we will make for our project
namespace TicTacToe
{
	//the board class holds all the definitions we need to manipulate and handle boards.
    class Board
    {
		//we define the moves that can be made  and the two players
        public enum Move { Sp, O, X};
        public enum Player {O, X };

		//a board is defined as an array of moves	
		//it must have 9 spaces
        Move[] board = // TODO define an empty board to set up the start point of the game
        Random rng = //TODO Look up the System.Random class and define a random number generator for future use
		
		//TODO Look up class Properties and get familiar with how they work
		//TODO look up the keyword static and make sure you understand what it means
		
        public static Board StartGame //TO DO Create Static property to represent starting board

		//TODO Look up class constrructors.  We have three constructors a default constructor and a 
		//constructor that takes an array of moves to create a board and a
		//constructor thet takes a string of text describing a board
        public Board() { }
        public Board(Move[] brd)
        {
			//TODO are ther any assumptions we must test to make sure we create a valid board?
            Debug.Assert(/*place assumtion check here*/);
			//our class member board is set to the input board;
            board = brd;
        }

        public Board(string input)
        {
			//TODO we need to create a function that reads a string and creates a board
            board = ReadBoard(input);           
        }

		//This function takes a row and a col and a move and verifies that this is a valid move
        public bool ValidMove(int row, int col, Move mv)
        {
			//what conditions must be true for this move to be valid.  
			//hint  ROW AND COL MUST BE BETWEEN 1 AND 2, mOVE CANNOT BE A SPACE, AND THAT PLACE ON BOARD MUST BE EMPTY
            //TODO insert expressions inside each set of empty parenthesis to check one of the necessary conditions
			// The return statement sends back the value if all conditions anded together.  i.1 cond1 AND cond2 AND cond3 And cond4
			//All conditions must be true to be a valid move
			return (() &&
                   () &&
                   () &&
                   ()) ;
        }

		//Modify the board member to male move designated in parameters to function
        public Move[] DoMove(int row, int col, Move mv)
        {
            Debug.Assert(row < 3 && row >= 0);
            Debug.Assert(col < 3 && col >= 0);
            Debug.Assert(mv != Move.Sp);
            int index = row * 3 + col;
            if (board[index] == Move.Sp)
            {
                board[index] = mv;
                return board;
            }

            throw new InvalidOperationException("Cannot place a move on an occupied board cell");
        }

		//TODO Look up WriteLine and Write function of the Console object in .NET
		// We want a nicely formatted array of board positions with a blank line above and below 
		//and a space at the beginning of each row
        public void DisplayBoard()
        {
			//TODO fill in what weach function call will write to the console
            Console.WriteLine();
            //row 1
            Console.Write();
            WriteMove();
            Console.Write("|");
            WriteMove();
            Console.Write("|");
            WriteMove();
            Console.WriteLine();

            //row 2
            Console.Write();
            WriteMove();
            Console.Write();
            WriteMove();
            Console.Write();
            WriteMove();
            Console.WriteLine();

            //row3
            Console.Write();
            WriteMove();
            Console.Write();
            WriteMove();
            Console.Write();
            WriteMove();
            Console.WriteLine();
            Console.WriteLine();
        }

		//We would like to write a color for X and O moves
		//The Console.ForegroundColor property allows us to change the color output by the Console.Write Function
		//We want Red O's and Green X's
        public void WriteMove(Move mv)
        {
            switch(mv)
            {
                case Move.Sp:
                    Console.Write(" ");
                    break;
                case Move.O:
                    Console.ForegroundColor = ConsoleColor.;
                    Console.Write();
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Move.X:
                    Console.ForegroundColor = ConsoleColor.;
                    Console.Write();
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    throw new InvalidOperationException("Invalid Move Found");
            }
        }

		//We would like to be able to save a board position as a single string of characters
		//TODO read up on the String.Format function.  We will use this function to format a string with a character repre4sentation of each board space
        public string PrintBoard()
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|",
                /*What goes in here?*/);
        }

		//We would also like to read a string that represents a board and create a new board from it.
        public static Move[] ReadBoard(string input)
        {
			//TODO read up on the string function Split
            var moves = input.Split(/*What goes in here?*/);
			//What assumption must we check here
            Debug.Assert(/*Add check for valid board here*/);
            return moves.Select(mv => CreateMove(mv)).ToArray();
        }

		//change character representation of a move into a move object
        private static Move CreateMove(string mv)
        {
            if (mv == " ") return /*what goes here*/
            else if (mv == "O") return /*what goes here*/
            else return /*what goes here*/
        }

		//We need to be able to check when we have a winner.  This function takes a player object and 
		//looks to see if that player has won
		// Remember our 9 space array represents the following board
		// [0] [1] [2]
		// [3] [4] [5]
		// [6] [7] [8]
		
        public  bool Winner(Player pl)
        {
			//What symbol are we looking to check
            var Sym = pl == Player.O ? Move.O : Move.X;

            //is player a winner?
            //check for complete rows
            if (board[0] == Sym && board[1] == Sym && board[2] == Sym) return true;
            if (/*what goes here*/) return true;
            if (/*what goes here*/) return true;
            //check for complete columns
            if (board[0] == Sym && board[3] == Sym && board[6] == Sym) return true;
            if (/*what goes here*/) return true;
            if (/*what goes here*/) return true;
            //check for diagonals
            if (board[0] == Sym && board[4] == Sym && board[8] == Sym) return true;
            if (/*what goes here*/) return true;

            return false;
        }

		//If we are playing against a computer we want the computer to select a random mve
		// we count the number of spaces in our boad then we create a random number between 
		//0 and the number of empty sdpaces and select that empty space as our move
        internal int RandomMove()
        {
			//TODO Look up the LINQ Where function and figure out what is happening on this line of code
            int spaces = board.Where(i => i == Move.Sp).Count();
			
			//TODO Read up on the System.Random object and how to use the Next function 
            int n = rng.Next(spaces);
			
			//TODO Read up on the C# 'for' control keyword and make sure you understand what is heppening here.
            for(int i = 0; i < 9; i++)
            {
                // find nth space on board
                if (board[i] == Move.Sp)
                {
                    n--;
                    if (n == 0)
                        return i;
                }
            }
			
			//TODO Why do we return a -1 here;
            return -1;
        }
    }
}
