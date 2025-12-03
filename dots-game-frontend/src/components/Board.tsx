import React from "react";
import Field from "./Field";
import { Player, Field as FieldType } from "../types/dotsApiTypes";
import "./Board.css";

type BoardProps = {
  board: FieldType[][];
  currentPlayer: Player;
  onFieldClick: (row: number, col: number) => void;
};

const Board: React.FC<BoardProps> = ({ board, currentPlayer, onFieldClick }) => {
  return (
    <div className="board-wrapper">
      <div className="board-container">
        <div
          className="board"
          style={{ ["--size" as any]: board.length }}
        >
          {board.map((row, rIdx) =>
            row.map((cell, cIdx) => (
              <Field
                key={`${rIdx}-${cIdx}`}
                player={cell.player}
                enclosedBy={cell.enclosedBy}
                row={rIdx}
                col={cIdx}
                onClick={onFieldClick}
              />
            ))
          )}
        </div>
      </div>
    </div>
  );
};

export default Board;
