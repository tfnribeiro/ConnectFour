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
            int tries = 0;
            int humanV = 0;
            int botV = 0;
            int draws = 0;
            int cols = 7;
            int rows = 6;
            int depth = 6;
            while (tries < 5)
            {
                Console.Clear();
                State board = new State(rows, cols, 1);
                AiAlphaBeta bot = new AiAlphaBeta();
                int botTurn = (tries % 2 == 0) ?  2 : 1;
                int playerTurn = botTurn == 1 ? 2 : 1;
                bot.StartGame(botTurn, rows, cols, depth);

                while (board.GameIsFinished() == -1)
                {
                    Console.Clear();
                    if(botTurn == 1)
                    {
                        Console.WriteLine("Computer Piece | X |");
                        Console.WriteLine("Player Piece | O |");
                        ComputerMove(bot, botTurn, board);
                        PlayerMove(bot, playerTurn, board);
                    }
                    else
                    {
                        Console.WriteLine("Player Piece | X |");
                        Console.WriteLine("Computer Piece | O |");
                        PlayerMove(bot, playerTurn, board);
                        ComputerMove(bot, botTurn, board);
                        
                    }
                    Console.WriteLine();
                    
                }
                if (board.GetWinner() == playerTurn) humanV++;
                else if (board.GetWinner() == botTurn) botV++;
                else draws++;
                tries++;
                Console.WriteLine(board.PrintBoard());
                Console.WriteLine("GAME " + tries + " CONCLUDED!");
                Console.WriteLine($"Human ({humanV}) VS Bot ({botV}), Draws: {draws}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
           

        } 
        private static void ComputerMove(AiAlphaBeta ai, int playerID, State board)
        {
            int statesSeen = State.STATES_CREATED;
            DateTime calcStart = DateTime.Now;
            int play1 = ai.CalculateNextMove();
            ai.InsertCoin(play1, playerID);
            board.InsertCoin(play1, playerID);
            Console.WriteLine($"COMPUTER MOVE: C{play1 + playerID}");
            Console.WriteLine($"States looked at: {State.STATES_CREATED - statesSeen}");
            Console.WriteLine($"Time taken: {DateTime.Now.Subtract(calcStart).TotalSeconds} s");
            Console.WriteLine(board.PrintBoard());
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

        }
        private static void PlayerMove(AiAlphaBeta ai, int playerID, State board)
        {
            Console.WriteLine(board.PrintBoard());
            Console.Write($"PLAYER MOVE C({1} - {board.GetCols()}): ");
            int move = -1;
            bool isInt = true;
            do
            {
                if (isInt == false) Console.WriteLine("Invalid Col!");
                isInt = int.TryParse(Console.ReadLine(), out move);
            } while ((move < 0 || move > board.GetCols()) || !isInt );
            ai.InsertCoin(move-1, playerID);
            board.InsertCoin(move-1, playerID);
            Console.Clear();
        }
    }
}
