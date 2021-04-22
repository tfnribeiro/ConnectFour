using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    public class State
    {
        private int[,] zobristTable;
        public static int HASH_INIT = 8096;
        public static int STATES_CREATED; //Number of total states generated.
        // winCdt - Number of tokens to be in line (Default 4)
        private int winCdt = 4;
        // board - Contains the matrix with the dispaly of the tokens
        private int[,] board;
        private int cols;
        private int rows;
        private int turn;
        // winner - Returns the ID of the player that wins, 0 if state is draw or -1 if game goes on
        private int winner = -1;
        private int count = 0;
        private int current = 0;
        private int lastPlacedRow = 0; // This will old the row where the last token was placed, this is used for the Heuristics.

        //Get Methods
        public int GetRows() { return this.rows; }
        public int GetCols() { return this.cols; }
        public int GetWinner() { return this.winner; }
        public int GetTurn() { return this.turn; }
        public int GetLastPlacedRow() { return lastPlacedRow; }
        //public int getLastRow() 
        public int[,] GetBoard() { return this.board; }
        public int GetToken(int row, int col) { return board[row, col]; }

        private void init_zobrist()
        {
            zobristTable = new int[rows * cols, 2];
            var rand = new System.Random();
            for (int r = 0; r < rows * cols; r++)
            {
                for (int p = 0; p < 2; p++)
                {
                    zobristTable[r, p] = rand.Next(HASH_INIT);
                }
            }

        }
        public override int GetHashCode()
        {
            int h = 0;
            try
            {

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (board[r, c] != 0)
                        {
                            int piece = board[r, c] - 1;
                            h = h ^ zobristTable[rows * r + c, piece];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return h;
        }
        public string PrintBoard()
        {
            string boardString = String.Empty;
            for(int c = 0; c < cols; c++)
            {
                boardString += $" |{c+1}|";
            }
            boardString += "\n\n";
            for (int r = rows - 1; r >= 0; r--)
            {
                for(int c = 0; c < cols; c++)
                {
                    string symbol = string.Empty;
                    switch(board[r, c])
                    {
                        case 1:
                            symbol = "X";
                            break;
                        case 2:
                            symbol = "O";
                            break;
                        default:
                            symbol = "_";
                            break;
                    }
                    boardString += $" |{symbol}|";
                }
                boardString += "\n";
            }
            return boardString;
        }

        public State(int rows, int cols, int playerID)
        {
            //This constructor initializes the state with an empty board.
            this.board = new int[rows, cols];
            this.rows = rows;
            this.cols = cols;
            this.turn = playerID;
            STATES_CREATED = 0;
            init_zobrist();
        }
        public State(State oldState)
        {
            //Takes in an old state and generetes a new one.
            this.board = oldState.GetBoard().Clone() as int[,]; //Generates a deep copy and not shallow. 
            this.rows = oldState.GetRows();
            this.cols = oldState.GetCols();
            this.turn = oldState.GetTurn();
            STATES_CREATED++;
        }
        public void ChangeTurn()
        {
            if (turn == 1) { turn = 2; }
            else { turn = 1; }
        }
        public Boolean InsertCoin(int col, int playerID)
        {
            //Finds the first available row in the col if possible, returning true
            //If can't place a token it will return false.
            Boolean validMove = false;
            for (int r = 0; r < this.rows; r++)
            {
                if (col < 0) continue;
                if (board[r, col] == 0)
                {
                    validMove = true;
                    board[r, col] = playerID;
                    lastPlacedRow = r;
                    break;
                }
            }
            return validMove;
        }
        public int GameIsFinished()
        {
            /* Tests all conditions and returns the winner if there
             * is one.
             */
            winner = CheckWinCol();
            if (winner != -1) return winner;
            winner = CheckWinRow();
            if (winner != -1) return winner;
            winner = CheckWinDigT_L();
            if (winner != -1) return winner;
            winner = CheckWinDigT_R();
            if (winner != -1) return winner;
            if (BoardIsFull()) return winner = 0;
            return winner = -1;
        }

        private Boolean BoardIsFull()
        {
            //To check if board is full we need only to see the last row
            //If there is space, then it's not full;
            for (int i = 0; i < cols; i++)
            {
                if (board[rows - 1, i] == 0) return false;
            }
            return true;
        }

        private int CountCoin(int[,] board, int r, int c, int winCdt)
        {
            switch (board[r, c])
            {
                case 1: //Belongs to player1
                    if (current == 1) { count++; } //The one before was a 1, we add to the count
                    else
                    {
                        //It is a new token we set the current player to 1 and 1 token counted.
                        current = 1;
                        count = 1;
                    }
                    break;
                case 2: //Belongs to player2
                    if (current == 2) { count++; }
                    else
                    {
                        current = 2;
                        count = 1;
                    }
                    break;
                default:
                    //None are found.
                    current = 0;
                    count = 0;
                    break;
            }
            if (count == winCdt)
            {
                //If the count of tokens in a row is equal to the win condition
                //we have a victor, so we return the current.
                return current;
            }
            return -1;
        }

        private int CheckWinRow()
        {
            for (int r = 0; r < rows; r++)
            {
                //Variables are reset every time we will check a new row/col/dig
                current = 0; //Current Player checking for win condition
                count = 0; //Number of tokens in a row
                for (int c = 0; c < cols; c++)
                {
                    //Checks if it is still possible to win in the row
                    if (winCdt - count > cols - c) { continue; }
                    int check = CountCoin(board, r, c, winCdt);
                    if (check != -1)
                    {
                        return check;
                    }
                }
            }
            return -1; //Default value if nothing is found is -1
        }
        private int CheckWinCol()
        {
            //Same idea as rows, but instead we iterate through the rows.
            for (int c = 0; c < cols; c++)
            {
                current = 0;
                count = 0;
                for (int r = 0; r < rows; r++)
                {
                    if (winCdt - count > rows - r) { continue; }
                    int check = CountCoin(board, r, c, winCdt);
                    if (check != -1)
                    {
                        return check;
                    }
                }
            }
            return -1; //Default value if nothing is found is -1
        }
        private int CheckWinDigT_R()
        {
            //The row will have to be the limit - 4(winCdt) because that's 
            //where it will be possible to find a win in the diagonal
            //This will check the diagonals where the starting column is 0
            //and goes to the right.
            for (int r = rows - winCdt; r >= 0; r--)
            {
                int c = 0;
                int tempR = r;
                count = 0;
                current = 0;
                while (c < cols && tempR < rows)
                {
                    int check = CountCoin(board, tempR, c, winCdt);
                    if (check != -1)
                    {
                        return check;
                    }
                    c++;
                    tempR++;
                }
            }
            for (int c = 1; c <= cols - winCdt; c++)
            {
                int r = 0;
                int tempC = c;
                count = 0;
                current = 0;
                while (r < rows && tempC < cols)
                {
                    int check = CountCoin(board, r, tempC, winCdt);
                    if (check != -1)
                    {
                        return check;
                    }
                    r++;
                    tempC++;
                }
            }
            return -1;
        }
        private int CheckWinDigT_L()
        {
            //The row will have to be the limit - 4(winCdt) because that's 
            //where it will be possible to find a win in the diagonal
            //This will check the diagonals where the starting column is col (the last one)
            //and go to the left
            for (int r = rows - winCdt; r >= 0; r--)
            {
                int c = cols - 1;
                int tempR = r;
                count = 0;
                current = 0;
                while (c >= 0 && tempR < rows)
                {
                    int check = CountCoin(board, tempR, c, winCdt);
                    if (check != -1)
                    {
                        return check;
                    }
                    c--;
                    tempR++;
                }
            }
            for (int c = cols - 1; c >= winCdt - 1; c--)
            {
                int r = 0;
                int tempC = c;
                count = 0;
                current = 0;
                while (r < rows && tempC >= 0)
                {
                    int check = CountCoin(board, r, tempC, winCdt);
                    if (check != -1)
                    {
                        return check;
                    }
                    r++;
                    tempC--;
                }
            }
            return -1;
        }
    }
}