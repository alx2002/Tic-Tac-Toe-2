using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    internal class Game
    {
        /// <summary>
        /// Ths class actually plays a game on the goame board class
        /// </summary>
        public class QuitGameException : Exception
        {
            public QuitGameException(string message): base(message)
            {               
            }
        }


        //Create current board for new game
        bool best = true;
        bool run = true;
        bool play = true;
        GameBoard currentBoard = GameBoard.StartGame;
        GameHistory gameHistory = new GameHistory();
        public Game() { }

        public void ConsoleWriteLine(string line, bool useNewLine = true)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (useNewLine)
                Console.WriteLine(line);
            else
                Console.Write(line);
            Console.ForegroundColor = color;

        }
        /// <summary>
        /// Run Game
        /// </summary>
        public void Run()
        {
            bool run = true;
            PrintBanner();

            //While games are being played
            try
            {
                while (run)
                {
                    Menu();
                }
            }
            catch(QuitGameException q)
            {
                Console.WriteLine(q.Message);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleWriteLine("Good Bye");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        internal void PrintMenu()
        {
            Console.WriteLine("Enter 'Q' to Quit");
            Console.WriteLine("Enter 'P' to Play Game");
            Console.WriteLine("Enter 'H' for History of last game");
            Console.WriteLine("Enter 'L' to Load Game History");
            Console.WriteLine("Enter 'S' to Save Game History");
            Console.WriteLine("Enter 'T' to Train Game History");
            Console.WriteLine("Enter 'V' to View Game Statistics");
        }

        public void PlayGames()
        {
            while (play)
            {
                //make a move
                var player = PlayGame();

                //show move on board
                currentBoard.DisplayBoard();

                //do we have a winner yet
                if (currentBoard.Winner(player))
                {
                    Console.Write("Congratulations: Player ");
                    if (player == GameBoard.Player.O)
                        currentBoard.WriteMove(GameBoard.Move.O);
                    else
                        currentBoard.WriteMove(GameBoard.Move.X);
                    ConsoleWriteLine(" is the winner");
                    //gameMoves.Add(currentBoard.ToString());
                    currentBoard = GameBoard.StartGame;
                    play = false;
                }
            }
        }

        public void Menu()
        {
            //display a menu of choices to user
            PrintMenu();
            string input = Console.ReadLine();
            switch (input.Trim().ToUpper())
            {
                case "?":
                    PrintMenu();
                    break;
                case "Q":
                    run = false;
                    throw new QuitGameException("Exiting Game");
                    //break;
                case "H":
                    int move = 1;
                    foreach (var mv in gameHistory.History)
                    {
                        Console.WriteLine("Move {0}", mv.ToString());
                        move++;
                        //new GameBoard(GameBoard.ReadBoard(mv)).DisplayBoard();
                        Console.Write("Press any key to see next move...");
                        Console.ReadKey(true);
                    }
                    break;
                case "L":
                    gameHistory.ReadGameHistory();
                    break;
                case "P":
                    play = true;
                    PlayGames();
                    break;
                case "T":
                    TrainComputer();
                    break;
                case "S":
                    gameHistory.SaveGameHistory();
                    break;
                case "V":
                    gameHistory.viewStatistics();
                    break;
                default:
                    ConsoleWriteLine("Unknown Command " + input.Trim().ToUpper());
                    break;
            }
        }

        private void TrainComputer()
        {
            Console.Write("Number of games to train(recommended 10000):");
            int iterations = ReadIterations();
            for (int i = 0; i < iterations; i++)
            {
                TrainingGame(i);
            }
        }
        int ReadIterations()
        {
            int iterations;
            while (true)
            {
                string input = Console.ReadLine();
                //quit trying
                if (input.ToUpper() == "Q")
                    break;
                //return input
                if (Int32.TryParse(input, out iterations))
                    return iterations;
                else
                {
                    //try again
                    Console.WriteLine(String.Format("Must input a number.  Current input was [{0}]", input));
                    Console.WriteLine("If you wish to stop training enter 'q'");
                }
            }
            return 0;
        }

        double threshold = 0.5;
        public void TrainingGame(int i)
        {
            GameBoard.Player player = GameBoard.Player.O;

            try
            {
                DoGame(player, GetComputerPlayerMove, GetComputerPlayerMove);
                if (i % 50 == 0)
                    gameHistory.viewStatistics();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleWriteLine("Error in game" + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        //Return Game Winner or if tie return Player.None
        public GameBoard.Player PlayGame()
        {
            //play starts with X so set player to O to force first move to X
            GameBoard.Player player = GameBoard.Player.O;
            
            try
            {
                DoGame(player, GetHumanPlayerMove, GetComputerPlayerMove);                
            }
            catch(QuitGameException q)
            {
                ConsoleWriteLine(q.Message);
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleWriteLine("Error in game" + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            //return winner
            if (currentBoard.IsTie())
                player = GameBoard.Player.None;
            return player;
        }

        //Functor to get a move for a human player
        public GameHistory.GameMove GetHumanPlayerMove(GameBoard brd, GameBoard.Player player)
        {
            GameHistory.GameMove move;
            string prompt;
            while (!GetNextMove(player, out move, out prompt))
            {
                ConsoleWriteLine(prompt);
            }
            return move;
        }

        //Functor to get a move for the computer player
        public GameHistory.GameMove GetComputerPlayerMove(GameBoard brd, GameBoard.Player player)
        {
            GameHistory.GameMove mv;
            int tries = 0;
            do
            {
                tries++;
                var dbl = GameBoard.rng.NextDouble();
                if (dbl > threshold)
                    mv = gameHistory.MoveTree.GetMove(brd, player, best);
                else
                {
                    threshold *= 1.001;
                    mv = GetRandomComputerMove(brd, player);
                }
            } while (!brd.ValidMove(mv) && tries < 20);

            return mv;
        }

        public void DoGame(GameBoard.Player player, 
            Func<GameBoard, GameBoard.Player, GameHistory.GameMove> player1Move,
            Func<GameBoard, GameBoard.Player, GameHistory.GameMove> player2Move)
        {
            currentBoard = GameBoard.StartGame;
            ConsoleWriteLine("Start New Game");
            currentBoard.DisplayBoard();
            while (!currentBoard.Winner(player) && !currentBoard.IsTie())
            {
                //reverse player
                player = (player == GameBoard.Player.O) ? GameBoard.Player.X : GameBoard.Player.O;

                GameHistory.GameMove move =  player1Move(currentBoard, player);
                ConsoleWriteLine(String.Format("The First player places {0}.", move));
                gameHistory.MakeMove(currentBoard, move);
                currentBoard.DoMove(move);
                currentBoard.DisplayBoard();
                if (currentBoard.Winner(player)) break;
                
                player = (player == GameBoard.Player.O) ? GameBoard.Player.X : GameBoard.Player.O;
                //query game stats to get next move
                move = player2Move(currentBoard,player);
                ConsoleWriteLine(String.Format("The Second player places {0}.", move));
                //register move in game hidtory
                gameHistory.MakeMove(currentBoard, move);
                //update current board
                currentBoard.DoMove(move);
                currentBoard.DisplayBoard();
            }
            //by default all games start with X. Game win is relative to player X
            gameHistory.RegisterGameResults(currentBoard.Winner(GameBoard.Player.X));
        }

        public static GameBoard.Move MoveFromPlayer(GameBoard.Player player)
        {
            return player == GameBoard.Player.O ? GameBoard.Move.O : GameBoard.Move.X;
        }

        private GameHistory.GameMove GetRandomComputerMove(GameBoard board, GameBoard.Player player)
        {
            //return gameHistory.MoveTree.GetMove(board, player, best);
            int move = currentBoard.RandomMove();
            return new GameHistory.GameMove(MoveFromPlayer(player), move);
        }

        private void PrintBanner()
        {
            ConsoleWriteLine("TIC TAC TOE Version 2.0.   \nLet the Games Begin!");
        }
        
        public bool GetNextMove(GameBoard.Player player, out GameHistory.GameMove mv, out string prompt)
        {
            prompt = String.Empty;
             
            //keep trying until the user quits or enters a valid input
            //while (true)
            //{
                //prompt user for imnput
            ConsoleWriteLine("Enter row and column for your move: row , column. \nor enter 'Q' to quit the game: ", false);
            string input = Console.ReadLine();

            //if player is quiting throw quit exception
            if (input.Trim().ToUpper() == "Q")
                throw new QuitGameException("Quiting TicTacToe Game");

            //split inputs into row and col numbers
            var inputs = input.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int digit = -1;
            List<int> move = new List<int>();
            foreach (string i in inputs)
                if (Int32.TryParse(i, out digit))
                {
                    move.Add(digit);
                }

            //check move input for valid input
            if (move.Count == 2)
            {
                if (currentBoard.ValidMove(move[0] -1, move[1] -1, player == GameBoard.Player.O ? GameBoard.Move.O : GameBoard.Move.X))
                {
                    var row = move[0] - 1;
                    var col = move[1] - 1;
                    mv = new GameHistory.GameMove(MoveFromPlayer(player), GameBoard.IndexFromRowCol(row,col)); 
                    return true;
                }
                        
                prompt = "\nInvalid Input read. Must enter an integer between 1 and 3 a comma and an integer between 1 and 3\n";
            }
            else if (move.Count > 2)
            {
                prompt = 
                    "\nToo many inputs entered" + "\nUser input was " + input + 
                    "\nMust enter an integer between 0 and 2 a comma and an integer between 0 and 2\n";                    
            }
            else if (move.Count < 2)
            {
                prompt = 
                    "\nToo few inputs entered" + "\nUser input was " + input +
                    "Must enter an integer between 0 and 2 a comma and an integer between 0 and 2\n";                
            }

            mv = null;
            return false;
            //}
        }
    }
}
