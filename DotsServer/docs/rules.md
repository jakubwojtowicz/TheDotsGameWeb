# The Dots Game Rules

## General rules 

1. This is a two-player turn-based game:
- players act one after another 
- each move changes the game state 
- the state is fully observable.

2. The board is square and consists of n x n fields. 

3. At the beginning, all fields on the board are empty. 

## Moves 

4. In each round player can place his dot on one empty field which is not enclosed by opponent's dots. 

## Scoring 

5. If a player forms a loop with his dots, all of the enemy's dots and empty spaces inside are enclosed by this player. Player gains one point for every enclosed opponent's dot. 

6. A field is enclosed if there is no path leading from it to the edge of the board without passing through an opponent's dot, moving in 4 directions: up, down, left, right. A dot cannot escape the loop by moving diagonally.

Examples:

- P - Player1 
- H - Player2 
- E - Empty field

```
E H E 
H P H 
E H E 
```
Player1's dot at field (1,1) is enclosed by player 2. 
Player2 gains 1 point.

```
E H E 
H E H 
E H E 
```
Field at position (1,1) is enclosed by Player2. Players cannot place theirs dots at this position from now. Player2 gains 0 points because no enemy's dots have been captured. 

## Game over 

7. The game is over when there is no empty, non enclosed fields on the board left. 

8. Player with bigger score wins. If both players have the same score, it is a draw. 