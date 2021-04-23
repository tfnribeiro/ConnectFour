using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConnectFour
{
    class AiAlphaBeta
    {
        private TimeSpan MOVE_LIMIT = new TimeSpan(0, 0, 30); // 30 Sec limit
        private int cols;
        private int rows;
        private int playerID;
        private State board;
        private int depth; //How far the tree goes to find a play ( 8 is Default ) 
        public int GetDepth() { return this.depth; }
        public int GetTimeLimit() { return this.MOVE_LIMIT.Seconds; }
        public TimeSpan lastMoveTime = new TimeSpan(0, 0, 0);
        public int lastMovesAnalysed = 0;
        public int CalculateNextMove()
        {
            DateTime start = DateTime.Now;
            int stateStart = State.STATES_CREATED;
            
            /*
             * If all moves are worth the same I will randomize the play.
             * So all values calculated for each play are stored and then from the max I will randomize;
             */
            int bestAction = -1;  //Holds the best move in this turn
            int v = int.MinValue; //Value of the current evaluated turn
            int alpha = int.MinValue; //Value of Alpha
            int beta = int.MaxValue; //Value of Beta         
            int middlePoint = (int)(board.GetCols() / 2);
            if (board.GetBoard()[0, middlePoint] == 0)
                return middlePoint;
            else if (board.GetBoard()[0, middlePoint - 1] == 0)
                return middlePoint - 1;
            else if (board.GetBoard()[0, middlePoint + 1] == 0 )
                return middlePoint + 1;
            List<int> availableMoves = Actions(board);
            Dictionary<int, int> plays = new Dictionary<int, int>();
            foreach (int a in availableMoves)
            {
                //For each a (action) we will add to the dictionary the move that was made
                //and the value calculated by the heuristic
                v = MinValue(Result(a,board), alpha, beta, depth, a, start);
                plays.Add(a, v);
            }
            List<int> possibleMoves = new List<int>();
            foreach(int key in plays.Keys)
            {
                if (plays[key] == plays.Values.Max())
                {
                    possibleMoves.Add(key);
                }
            }
            System.Random rand = new System.Random();
            bestAction = possibleMoves[rand.Next(possibleMoves.Count)];
            lastMoveTime = DateTime.Now - start;
            lastMovesAnalysed = State.STATES_CREATED - stateStart;
            return bestAction;
              
        }
        private int RandomKey(Dictionary<int, int> dict)
        {
            //Picks a random Key from the dictionary
            List<int> keyList = new List<int>(dict.Keys);
            System.Random rand = new System.Random();
            return keyList[rand.Next(keyList.Count)];
        }

        private Boolean TerminalTest(State state) { return state.GameIsFinished() != -1; }
        private Dictionary<int, (State, int)> GetActionVal(List<int> availableMoves, State state)
        {
            Dictionary<int, (State, int)> utilityNextMove = new Dictionary<int, (State, int)>();
            foreach (int move in availableMoves)
            {
                State nextState = Result(move, state);
                utilityNextMove.Add(move, (nextState, Utility(nextState, move, this.depth)));
            }
            return utilityNextMove;
        }
        private IEnumerable<int> GetOrderedMoves(Dictionary<int, (State, int)> uNextMove, int sortOrder)
        {
            if(sortOrder==1)
                return from util in uNextMove orderby util.Value.Item2 ascending select util.Key;
            else
                return from util in uNextMove orderby util.Value.Item2 descending select util.Key;
        }
        private IEnumerable<int> GetOrderedMoves(Dictionary<int, int> uNextMove, int sortOrder)
        {
            if (sortOrder == 1)
                return from util in uNextMove orderby util.Value ascending select util.Key;
            else
                return from util in uNextMove orderby util.Value descending select util.Key;
        }
        private int MinValue(State state, int alpha, int beta, int depth, int actionTaken, DateTime startTime)
        {
            //We try to minimize the value of the play.
            //If we are at the end of depth or we reach a terminal state we evaluate how good it is
            //This will be based on the heuristic provided in the class. 
            //This method will apply alpha beta pruning, so it means that if an action was already found a bette option
            //in previous methods it is not longer considered and we prune the tree. 
            //This is based from the book: Artificial Intelligence A Modern Approach, by Stuart Russel and Peter Norvig
            if (TerminalTest(state) || depth <= 0)
            {
                return Utility(state, actionTaken, depth);
            }
            else if (DateTime.Now - startTime > MOVE_LIMIT)
            {
                return Utility(state, actionTaken, depth);
            }

            int minVal = int.MaxValue;
            depth = depth - 1;

            List<int> availableMoves = Actions(state);
            Dictionary<int, (State, int)> uNextMove = GetActionVal(availableMoves,state);
            var movesOrdered = GetOrderedMoves(uNextMove, 1);
            foreach (int a in movesOrdered)
            {
                minVal = Math.Min(minVal, MaxValue(uNextMove[a].Item1, alpha, beta, depth, a, startTime));
                //minVal = Math.Min(minVal, MaxValue(Result(a, state), alpha, beta, depth, a, startTime));
                if (minVal <= alpha) { return minVal; }
                beta = Math.Min(beta, minVal);
            }

            return minVal;
        }

        private int MaxValue(State state, int alpha, int beta, int depth, int actionTaken, DateTime startTime)
        {
            if (TerminalTest(state) || depth <= 0)
            {
                return Utility(state, actionTaken, depth);
            }
            else if (DateTime.Now - startTime > MOVE_LIMIT)
            {
                return Utility(state, actionTaken, depth);
            }
            int maxValue = int.MinValue;
            depth = depth - 1;
            List<int> availableMoves = Actions(state);
            Dictionary<int, (State, int)> uNextMove = GetActionVal(availableMoves,state);
            var movesOrdered = GetOrderedMoves(uNextMove, -1);
            foreach (int a in movesOrdered)
            {
                maxValue = Math.Max(maxValue, MinValue(uNextMove[a].Item1, alpha, beta, depth, a, startTime));
                //maxValue = Math.Max(maxValue, MinValue(Result(a, state), alpha, beta, depth, a, startTime));
                if (maxValue >= beta) { return maxValue; }
                alpha = Math.Max(alpha, maxValue);
            }
            return maxValue;
        }

        private int Utility(State state, int action, int depth)
        {
            int gameEnd = state.GameIsFinished();
            if (gameEnd == 1 || gameEnd == 2 || gameEnd == 0)
            {
                if (state.GetWinner() == playerID) { return 20 * (1 + depth); } //The Heuristic of when the state is winning returns a value that rewards a play that wins faster (depth is higher)
                else if (state.GetWinner() == 0) { return 0; } //The Heuristic returns 0 if there is a draw
                else { return -20 * (1 + depth); } //The Heuristic returns a lower value if it results in a loss faster 
            }
            else
            {
                if (state.GetTurn() == playerID)
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
            StartGame(player, rows, columns);
            this.depth = depth;
        }
        public void StartGame(int player, int rows, int columns, int depth, int seconds)
        {
            StartGame(player, rows, columns);
            this.depth = depth;
            this.MOVE_LIMIT = new TimeSpan(0, 0, seconds);
        }

        private List<int> Actions(State state)
        {
            //Returns the available list of actions by checking if the last row is still empty
            //Each index of col that is empty is added to the list. 
            List<int> actions = new List<int>();
            for (int c = 0; c < cols; c++)
            {
                if (state.GetBoard()[(rows - 1), c] == 0)
                {
                    actions.Add(c);
                }
            }
            return actions;
        }
        private String listPrint(List<int> a)
        {
            String print = "[";
            foreach (int ele in a)
            {
                print += ele + ";";
            }
            print += "]";
            return print;
        }
    }
}

