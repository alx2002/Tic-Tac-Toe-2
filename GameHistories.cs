using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class GameHistory
    {
        static Random rng = new Random(42);
        [DebuggerDisplay("{ToString()}")]
        public class BoardMove
        {
            string brd;
            GameMove mv;
            internal BoardMove(GameBoard board, GameMove mv)
            {
                Board = board.Key;
                Move = mv;
            }

            public string Board
            {
                get
                {
                    return brd;
                }

                set
                {
                    brd = value;
                }
            }

            public GameMove Move
            {
                get
                {
                    return mv;
                }

                set
                {
                    mv = value;
                }
            }
            public string PrintBoard()
            {
                return Board;
            }

            public override string ToString()
            {
                return String.Format("Board:[{0}] Move: {2}", PrintBoard(), Move.ToString());
            }

        }
        public class GameMove
        {
            GameBoard.Move player;            
            int position;
            
            internal GameMove()  {}

            internal GameMove(GameBoard.Move pl, int pos)
            {
                Player = pl;
                Position = pos;               
            }

            public GameBoard.Move Player
            {
                get { return player; }
                set { player = value; }                
            }

            public int Position
            {
                get { return position; }
                set { position = value; }
            }
            
            //Factory method to generate a random move for this player
            internal static GameMove Random(GameBoard brd, GameBoard.Move player)
            {
                //get indexes of all open positions
                var openPositions = brd.Board
                    .Zip(Enumerable.Range(0, 9), (mv, i) => Tuple.Create(mv, i))
                    //number all positions
                    .Where(pr => pr.Item1 == GameBoard.Move.Sp)
                    //get positions that are a space and place them in an array
                    .Select(pr => pr.Item2).ToArray();

                //randomly choose an open position from array of positions
                return new GameMove(player, openPositions[rng.Next(openPositions.Count())]);
            }

            public override string ToString()
            {
                return String.Format(" {0} at position {1}", player.ToString("G"), position);
            }
        }
        public class MoveStats
        {
            uint wins = 100;
            uint losses = 100;

            internal MoveStats() { }

            internal MoveStats(uint _wins, uint _losses)
            {
                wins = _wins;
                losses = _losses;
            }

            public uint Wins
            {
                get { return wins; }
                set { wins = value > 1 ?value: 1; }
            }

            public uint Losses
            {
                get { return losses; }
                set { losses = value > 1 ? value : 1; }
            }

            internal void RegisterWin() { Wins += 1; Losses -= 1; }

            internal void RegisterLosses() { Losses += 1; Wins -= 1; }

            internal void RegisterResult(bool win)
            {
                if (win)
                    RegisterWin();
                else
                    RegisterLosses();
            }

            internal float Worth()
            {
                if (wins == 0 && losses == 0)
                    return 0.0f;
                else if (wins > 0 && losses == 0)
                    return (float)wins;
                else if (wins == 0 && losses > 0)
                    return (float) 1.0/losses;
                //else if (losses > wins)
                //    return -(float)losses / (float)wins;
                else
                    return (float)wins / (float)losses;
            }

            public override string ToString()
            {
                return String.Format("{0}/{1}={2}", Wins, Losses, Worth());
            }
        }

        //records a board position and all moves made from that board by players
        public class GameTree
        {
            //GameBoard board;

            //for each possible board keep a record of the 
            //statistics on each move seen from that board
            Dictionary<String, Dictionary<int, Tuple<GameMove, MoveStats>>> tree;
            
            //GameTree Parent;

            public Dictionary<String, Dictionary<int, Tuple<GameMove, MoveStats>>> Tree
            {
                get { return tree; }
                set { tree = value; }
            }

            //public GameBoard Board
            //{
            //    get {  return board; }
            //    set { board = value; }
            //}

            public GameTree()
            {
                Tree = new Dictionary<String, Dictionary<int, Tuple<GameMove, MoveStats>>>();                
            }

            GameTree(GameBoard brd)
            {                
                Tree = new Dictionary<String, Dictionary<int, Tuple<GameMove, MoveStats>>>();
                Tree.Add(brd.Key, new Dictionary<int, Tuple<GameMove, MoveStats>>());
            }

            //add new move to GameTree
            internal MoveStats RegisterGameResult(BoardMove mv, bool win) // GameBoard brd, GameMove mv, bool win)
            {
                Tuple<GameMove, MoveStats> res =  Tuple.Create(null as GameMove, null as MoveStats);          
                if (tree.ContainsKey(mv.Board))
                {
                    //do we have this move registered for this board?
                    var brdMoves = tree[mv.Board];
                    if (brdMoves.ContainsKey(mv.Move.Position))
                    {
                        res =  brdMoves[mv.Move.Position];
                        //register win
                        res.Item2.RegisterResult(win);
                        //store modified stats into moves dictionary
                        brdMoves[mv.Move.Position] = res;
                        //store modified move dictionary into boards disctionary
                        tree[mv.Board] = brdMoves;
                    }
                    else  
                    {
                        //no make new entry for this board and register win
                        res = Tuple.Create(mv.Move, new MoveStats());
                        res.Item2.RegisterResult(win);
                        //add stats to moves dictionary
                        brdMoves.Add(mv.Move.Position, res);
                        //add gamemove to board dictionary
                        tree[mv.Board] = brdMoves;
                    }

                    //res.Parent = this;
                }
                else
                {
                    //board not found create new board
                    //create new move stat
                    var mvStat = new MoveStats();
                    mvStat.RegisterResult(win);
                    //create board entry 
                    //add to board dictionary
                    var entry = new Dictionary<int, Tuple<GameMove, MoveStats>>()
                    {
                        { mv.Move.Position, Tuple.Create(mv.Move, mvStat) }
                    };

                    tree.Add(mv.Board, entry);      
                }

                return res.Item2;             
            }

            public GameMove GetMove(GameBoard board, GameBoard.Player player, bool best)
            {
                //Get stored moves for this board position with their associated worthiness values
                var moves = tree.ContainsKey(board.Key) ? tree[board.Key] : new Dictionary<int, Tuple<GameMove, MoveStats>>();
                var possibleMoves = moves.Select(kvp => Tuple.Create(kvp.Key, kvp.Value.Item1, kvp.Value.Item2.Worth())).OrderByDescending(pr => pr.Item3);
                if (possibleMoves.Count() == 0)
                    return GameMove.Random(board, Game.MoveFromPlayer(player));
                if (best)
                    //Get Best Move
                    return possibleMoves.First().Item2;
                else
                {
                    // get nth move in order of decreasing worth
                    int pos = rng.Next(possibleMoves.Count());
                    return possibleMoves.ElementAt(pos).Item2;
                }
                //return GameMove.Random(board, Game.MoveFromPlayer(player));                                
            }
        }        

        internal GameHistory()
        {
            History = new List<BoardMove>();
            MoveTree = new GameTree();
        }

        internal void viewStatistics()
        {
            foreach(var kvp in MoveTree.Tree)
            {
                Console.WriteLine(kvp.Key);
                var moves = kvp.Value;
                foreach(var mvs in moves)
                {
                    Console.WriteLine(String.Format("           {0}, {1}", mvs.Key, mvs.Value));
                }

            }
        }

        internal void MakeMove(GameBoard board, GameMove mv)
        {
            History.Add(new BoardMove(board, mv));
        }
    
        //win represents the win status of the first players game
        internal void RegisterGameResults(bool win)
        {
            var board = GameBoard.StartGame;
            foreach (var mv in History)
            {                
                MoveTree.RegisterGameResult(mv, win);
                //board.DoMove(mv.Move);
                win = !win;
            }            
        }

        internal void ResetGame() { History.Clear();  }

        GameTree gameTree;
        List<BoardMove> gameHistory;

        public GameTree MoveTree
        {
            get
            {
                return gameTree;
            }

            set
            {
                gameTree = value;
            }
        }

        public List<BoardMove> History
        {
            get
            {
                return gameHistory;
            }

            set
            {
                gameHistory = value;
            }
        }

        public void SaveGameHistory()
        {
            string directory = Path.Combine(@"C:\TicTacToeGames", "Data");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, "TicTacToe.dat");
            using (var sw = new StreamWriter(filePath))
            {
                foreach (var itm in MoveTree.Tree)
                    sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(itm));
            }
        }

        public void ReadGameHistory()
        {
            string directory = Path.Combine(@"C:\TicTacToeGames", "Data");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, "TicTacToe.dat");
            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    string data = sr.ReadToEnd();
                    var items = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var records = items.Select(itm => 
                        Newtonsoft.Json.JsonConvert.DeserializeObject <Tuple<string, Dictionary<int, Tuple<GameMove, MoveStats>>>> (itm));
                    MoveTree = new GameTree();
                    foreach(var rec in records)
                    {
                        MoveTree.Tree.Add(rec.Item1, rec.Item2);
                    }                    
                }
            }
        }

    }
}
