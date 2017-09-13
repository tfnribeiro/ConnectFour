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
            while (tries < 5)
            {
                State board = new State(6, 7, 1);
                AiAlphaBeta botP1 = new AiAlphaBeta();
                AiAlphaBeta botP2 = new AiAlphaBeta();
                botP1.StartGame(1, 6, 7, 5);
                botP2.StartGame(2, 6, 7, 5);
            
                while (board.GameIsFinished() == -1)
                {
                    int play1 = botP1.CalculateNextMove();
                    botP1.InsertCoin(play1, 1);
                    botP2.InsertCoin(play1, 1);
                    board.InsertCoin(play1, 1);
                    board.PrintBoard();
                    Console.ReadKey();
                    Console.Clear();
                    if (board.GameIsFinished() != -1) break;
                    Console.WriteLine("PLAYER:");
                    board.PrintBoard();
                    int play2 = PlayerMove();
                    botP1.InsertCoin(play2, 2);
                    botP2.InsertCoin(play2, 2);
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
                Console.WriteLine("GAME " + tries + " CONCLUDED!");
            }
            Console.WriteLine("P1: " + p1V + "\nP2: " + p2V + "\nDraws:" + draws);
            Console.ReadKey();
           

        } 
        public static int PlayerMove()
        {
            Console.Write("Play (1-7): ");
            return int.Parse(Console.ReadLine())-1;
        }
    }
}
