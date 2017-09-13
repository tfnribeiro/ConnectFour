using System;


namespace ConnectFour
{
    public class Heuristic
    {
        public static int TokenInRow(State state, int action)
        {
            /* We will consider that placing a token with open spaces is beneficial
             * If there are friendly tokens around it's more beneficial to the player
             * So, if there is an open space we give +1 for each space, if there is 
             * token in one of those spaces we give +2 for each token.
             * The algorithm will use the checks used in the code to check for winning player
             * in the class State.
             */
            return CountRow(state, action) + CountCol(state, action);

        }
        private static int CountRow(State evState, int a)
        {
            int combo = 0; //Will store the utility in terms of row
            int advPlayer = evState.GetTurn(); // The current turn is of the opponent player, as the action was taken already
            int player = GetOtherPlayer(advPlayer);
            int row = evState.GetLastPlacedRow();
            for(int c = Math.Max(0, a-3); c < Math.Min(a + 3, evState.GetCols()); c++)
            {
                int token = evState.GetToken(row, c);
                if (token == 0) combo++;
                else if (token == advPlayer)
                {
                    //We find a token which means we can't win, combo goes to 0; and we move to the other row in front.
                    if(c >= a)
                    {
                        return 0;
                    }
                    c = a;
                    combo = 0;
                }
                else combo += 3; //If it's our token we give +3 
            }
            return combo;
        }
        private static int CountCol(State evState, int a)
        {
            int combo = 0; //Will store the utility in terms of row
            int advPlayer = evState.GetTurn(); // The current turn is of the opponent player, as the action was taken already
            int player = GetOtherPlayer(advPlayer);
            int col = a;
            for (int r = Math.Max(0, evState.GetLastPlacedRow() - 3); r < 
               Math.Min(evState.GetLastPlacedRow() + 3, evState.GetRows()-1); r++)
            {
                int token = evState.GetToken(r, col);
                if (token == advPlayer)
                {
                    if(r >= evState.GetLastPlacedRow())
                    { return 0; }
                    r = evState.GetLastPlacedRow();
                    combo = 0;
                }
                else if (token == player) combo += 3;
                else combo++;
            }
            return combo;
        }
        private static int GetOtherPlayer(int player)
        {
            if (player == 1) return 2;
            else return 2;
        }
    }
}

