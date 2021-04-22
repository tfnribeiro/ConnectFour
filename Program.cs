using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            State board = new State(6, 7, 1);
            AiAlphaBeta botP1 = new AiAlphaBeta();
            AiAlphaBeta botP2 = new AiAlphaBeta();
            botP1.StartGame(1, 6, 7, 4);
            botP2.StartGame(2, 6, 7, 2);*/
            /*
            while(board.GameIsFinished() == -1)
            {
                Console.WriteLine("PLAYER: " + board.GetTurn());
                board.PrintBoard();
                int play = int.Parse(Console.ReadLine());
                board.InsertCoin(play,board.GetTurn());
                Console.WriteLine("Play Heuristic: " + Heuristic.TokenInRow(board, play));
                
            }
            Console.WriteLine("VICTORY FOR PLAYER: " + board.GetWinner());
            */
            int tries = 0;
            int p1V = 0;
            int p2V = 0;
            int draws = 0;
            int cols = 7;
            int rows = 6;
            int depth = 9;
            while (tries < 5)
            {

                State board = new State(rows, cols, 1);
                AiAlphaBeta botP1 = new AiAlphaBeta();
                //AiAlphaBeta botP2 = new AiAlphaBeta();
                botP1.StartGame(1, rows, cols, depth);
                //botP2.StartGame(2, rows, cols, depth);
            
                while (board.GameIsFinished() == -1)
                {
                    Console.Clear();
                    Console.WriteLine($"Computer Piece | X |");
                    Console.WriteLine($"Player Piece | O |");
                    Console.WriteLine();
                    int statesSeen = State.STATES_CREATED;
                    DateTime calcStart = DateTime.Now;
                    int play1 = botP1.CalculateNextMove();
                    botP1.InsertCoin(play1, 1);
                    //botP2.InsertCoin(play1, 1);
                    board.InsertCoin(play1, 1);
                    Console.WriteLine($"COMPUTER MOVE: C{play1+1}");
                    Console.WriteLine($"States looked at: {State.STATES_CREATED - statesSeen}");
                    Console.WriteLine($"Time taken: {DateTime.Now.Subtract(calcStart).TotalSeconds} s");
                    Console.WriteLine(board.PrintBoard());
                    if (board.GameIsFinished() != -1) break;
                    Console.Write($"PLAYER MOVE C({1} - {cols}): ");
                    int play2 = PlayerMove();
                    botP1.InsertCoin(play2, 2);
                    //botP2.InsertCoin(play2, 2);
                    board.InsertCoin(play2, 2);
                    Console.Clear();
                }
                switch (board.GetWinner())
                {
                    case 1: p1V++;
                        break;
                    case 2: p2V++;
                        break;
                    default: draws++;
                        break;
                }
                tries++;
                Console.WriteLine(board.PrintBoard());
                Console.WriteLine("GAME " + tries + " CONCLUDED!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            Console.WriteLine("P1: " + p1V + "\nP2: " + p2V + "\nDraws:" + draws);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
           

        } 
        public static int PlayerMove()
        {
            return int.Parse(Console.ReadLine())-1;
        }
    }
}
