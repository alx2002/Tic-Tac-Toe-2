using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TicTacToe.Board;

namespace TicTacToe
{
    //class MoveSelector
    //{
        class Node
        {
            string position;
            int wins = 0;
            int losses = 0;
            int row = -1;
            int col = -1;
            List<Node> children = new List<Node>();

            public Node(string board, int row = -1, int col = -1)
            {
                position = board;
                this.row = row;
                this.col = col;
            }

            public Node ChildExists(string move)
            {
                return children.Where(nd => nd.position == move).FirstOrDefault();
            }

            public Node AddChild(string move, int row, int col)
            {
                Node nd = new Node(move, row, col);
                children.Add(nd);
                return nd;
            }

            public void RegisterWinningMove()
            {
                wins++;
            }

            float Quality { get { return (float)wins / (float)losses; } }

            public void RegisterLosingMove()
            {
                losses++;
            }

            public Tuple<int,int, Move> OptimumMove(Move player)
            {
                if (children.Count() > 0)
                {
                    float maxQuality = children.Max(nd => nd.Quality);
                    Node optimum = children.Where(nd => nd.Quality == maxQuality).FirstOrDefault();
                    return Tuple.Create(optimum.row, optimum.col, player);
                }
                else
                {
                    //no moves to select make random move
                    Board brd = new Board(position);
                    int mv = brd.RandomMove();
                    return Tuple.Create(mv % 3, mv / 3, player);
                }
            }
        }

        class MoveTree
        {
            Node root = new Node(Board.StartGame.ToString());
            Node current;

            public MoveTree()
            {
                current = root; 
            }

            Node AddMove(string move, int row, int col)
            {
                var node = current.ChildExists(move);
                if (node != null)
                {
                    current = node;
                }
                else
                {
                    node = current.AddChild(move, row, col);
                    current = node;
                }

                return node;
            }

            void UpdateTreeWithGame(List<String> gameMoves, Board.Player winner)
            {
                //Bt Default X is the first move in all games
                //if the winner was X then first move and every other move is a winning move
                //otherwise first move and all alternate subsequent moves are losing moves
                //if game was a tie update nothing
                bool XWinner = winner == Board.Player.X;
                current = root;
                foreach(string move in gameMoves)
                {
                    current = current.ChildExists(move);
                    if (XWinner)
                    {
                        current.RegisterWinningMove()
;                    }
                    else
                    {
                        current.RegisterLosingMove();
                    }
                    XWinner = !XWinner;
                }
            }

            public Tuple<int, int, Move> ChooseNextMove(List<String> currentMoves, Move player)
            {
                Node place = root;
                foreach (string move in currentMoves)
                {
                    place = place.ChildExists(move);
                }

                return place.OptimumMove(player);
            }
        }


    //}
}
