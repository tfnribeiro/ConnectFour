# ConnectFour
Working Connect Four bot, right now it works on a console, will work on making a graphics interface now. 
This project was originaly developed in the AI subject I had at ITU, in JAVA. 
Decided to make this into C# to recall the concepts learned.

## AiAlphaBeta

This contains the code for the AI. 

The main method is **CalculateNextMove** which will return the move with the highest utility after exploring the tree and applying Alpha-Beta Prunning. This is done by calling the methods **MinValue** / **MaxValue** alternatively to explore the tree. The depth can be specified when creating the class. Over 9/10 depth the algorithm starts to slow down substantially. The order of search is done by calling **GetOrderedMoves**, and these are ordered based on the heuristic of that state. 

The Depth in this case decrements as you go furthest away in the tree. If Depth = 8, 8 will be the shallowest node and 0 will be the deepest node.

The **Utility** method will call determine how the AI values the current state of the board. If it sees a win, then it will give it 20 multiplied by the depth it is in. The lowest amount is 20 * (1 + 0) = 20 and the max is 20 * (1 + 8) = 180.

If there is no terminal state, then the Heuristic is used.

## Heuristic

The Heuristic for this AI is very simple and likely the place that needs improvement. The states are valued based on a combo according to where the token is placed. If they are sequenced with tokens of the same player, it will consider that as a better move. This is done simply for rows and collumns, the diagonals are not taken into consideration. The Heuristic also gives extra poitns for placing tokens close to the middle as those are usually move valuable to spots to take. 

## State

The State class contains all the logic to manipulate the game state. This is also where the winning condition is checked. 

I am looking into implementing Zobrist hashing to simplify the search tree, but this is not yet implemented. 

### April 2021 
I have decided to implement this in unity, and made some improvements to the AI - namely move prioritization, fixes in the end condition checking and details about time taken to calculate the next move for AI/states created. 
