export enum Player {
  None = 0,   
  Human = 1,  
  AI = 2      
}

export interface Field {
  player: Player;       
  enclosedBy: Player;   
}

export interface Move {
  player: Player;
  x: number;  // row
  y: number;  // column
}

export interface MoveDto {
  x: number;  // row
  y: number;  // column
}

export interface GameState {
  board: Field[][] | null;  
  currentPlayer: Player;
  isGameOver: boolean;
  winner: Player | null;
  scores: {
    None: number;
    Human: number;
    AI: number;
  } | null;
  lastMove: Move | null;
}
