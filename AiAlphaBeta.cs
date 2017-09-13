using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    class AiAlphaBeta
    {
        private int cols;
        private int rows;
        private int playerID;
        private State board;
        private int depth; //How far the tree goes to find a play ( 8 is Default ) 
        public int CalculateNextMove()
        {
            /*
             * If all moves are worth the same I will randomize the play.
             * So all values calculated for each play are stored and then from the max I will randomize;
             */
            int bestAction = -1;  //Holds the best move in this turn
            int v = int.MinValue; //Value of the current evaluated turn
            int alpha = int.MinValue; //Value of Alpha
            int beta = int.MaxValue; //Value of Beta             
            List<int> availableMoves = Actions(board);
            Dictionary<int, int> plays = new Dictionary<int, int>(); 
            foreach (int a in availableMoves)
            {
                //For each a (action) we will add to the dictionary the move that was made
                //and the value calculated by the heuristic
                v = MinValue(Result(a, board), alpha, beta, depth, a);
                plays.Add(a, v);
            }
            Console.WriteLine("PLAYER " + playerID + " PLAYS: ");
            PrintDictionary(plays);
            //From the plays available we only want to consider the ones that grant the best value
            plays = plays.Where(i => i.Value == plays.Values.Max()).ToDictionary(i => i.Key, i => i.Value);
            //We update the best value
            v = plays.Values.Max();
            //The action taken will be a random action from the best plays available. 
            bestAction = RandomKey(plays);
            return bestAction;
        }
        private int RandomKey(Dictionary<int, int> dict)
        {
            //Picks a random Key from the dictionary
            List<int> keyList = new List<int>(dict.Keys);
            Random rand = new Random();
            return keyList[rand.Next(keyList.Count)];
        } 

        private void PrintDictionary(Dictionary<int, int> dict)
        {
            //Debugging method to see if it's acting as expected
            foreach (KeyValuePair<int, int> pair in dict)
            {
                Console.WriteLine("Key: {0} Values: {1}", pair.Key, pair.Value);
            };
        }

        private Boolean TerminalTest(State state) { return state.GameIsFinished() != -1; }

        private int MinValue(State state, int alpha, int beta, int depth, int actionTaken)
        {
            //We try to minimize the value of the play.
            //If we are at the end of depth or we reach a terminal state we evaluate how good it is
            //This will be based on the heuristic provided in the class. 
            //This method will apply alpha beta pruning, so it means that if an action was already found a bette option
            //in previous methods it is not longer considered and we prune the tree. 
            //This is based from the book: Artificial Intelligence A Modern Approach, by Stuart Russel and Peter Norvig
            if(TerminalTest(state) || depth <= 0)
            {
                return Utility(state, actionTaken, depth);
            }

            int minVal = int.MaxValue;
            depth = depth - 1;

            List<int> availableMoves = Actions(state);

            foreach(int a in availableMoves)
            {
                minVal = Math.Min(minVal, MaxValue(Result(a, state), alpha, beta, depth, a));
                if(minVal <= alpha) { return minVal; }
                beta = Math.Min(beta, minVal);
            }

            return minVal;
        }

        private int MaxValue(State state, int alpha, int beta, int depth, int actionTaken)
        {
            if(TerminalTest(state) || depth <= 0)
            {
                return Utility(state, actionTaken, depth);
            }
            int maxValue = int.MinValue;
            depth = depth - 1;
            List<int> availableMoves = Actions(state);
            foreach(int a in availableMoves)
            {
                maxValue = Math.Max(maxValue, MinValue(Result(a, state), alpha, beta, depth, a));
                if(maxValue >= beta) { return maxValue; }
                alpha = Math.Max(alpha, maxValue);
            }
            return maxValue;
        }

        private int Utility(State state, int action, int depth)
        {
            int gameEnd = state.GameIsFinished();
            if(gameEnd == 1 || gameEnd == 2 || gameEnd == 0)
            {
                if(state.GetWinner() == playerID) { return 20 + depth * 5;  } //The Heuristic of when the state is winning returns a value that rewards a play that wins faster (depth is higher)
                else if (state.GetWinner() == 0) { return 0; } //The Heuristic returns 0 if there is a draw
                else { return -20 - depth * 5; } //The Heuristic returns a lower value if it results in a loss faster 
            }
            else
            {
                if(state.GetTurn() == playerID)
                {
                    return Heuristic.TokenInRow(state, action);
                }
                else
                {
                    return -Heuristic.TokenInRow(state, action);
                }
            }
        }

        private State Result(int action, State state)
        {
            // Returns the resulting state aften taking the action
            State nextState = new State(state);
            nextState.InsertCoin(action, state.GetTurn());
            nextState.ChangeTurn();
            return nextState; 
        }

        public int GameFinished()
        {
            switch (board.GameIsFinished())
            {
                case 1: return 1;
                case 2: return 2; 
                case 0: return 0;
                default: return -1;
            }
        }

        public void InsertCoin(int column, int playerID)
        {
            board.InsertCoin(column, playerID);
        }

        public void StartGame(int player, int rows, int columns)
        {
            this.cols = columns;
            this.rows = rows;
            this.playerID = player;
            this.depth = 8;
            this.board = new State(rows, cols, playerID);
        }

        public void StartGame(int player, int rows, int columns, int depth)
        {
            this.cols = columns;
            this.rows = rows;
            this.playerID = player;
            this.depth = depth;
            this.board = new State(rows, cols, playerID);
        }

        private List<int> Actions(State state)
        {
            //Returns the available list of actions by checking if the last row is still empty
            //Each index of col that is empty is added to the list. 
            List<int> actions = new List<int>();
            for(int c = 0; c < cols; c++)
            {
                if(state.GetBoard()[(rows-1), c] == 0)
                {
                    actions.Add(c);
                }
            }                  
            return actions;
        }
        private String listPrint(List<int> a)
        {
            String print = "[";
            foreach(int ele in a)
            {
                print += ele + ";";
            }
            print += "]";
            return print;
        }
    }
}

