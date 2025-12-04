import React, { useEffect, useState } from "react";
import Field from "./Field";
import { Player, Field as FieldType, Move } from "../types/dotsApiTypes";
import "./Board.css";

type BoardProps = {
  board: FieldType[][];
  currentPlayer: Player;
  lastMove?: Move | null; // last move from game state
  onFieldClick: (row: number, col: number) => void;
};

const Board: React.FC<BoardProps> = ({
  board,
  currentPlayer,
  lastMove,
  onFieldClick,
}) => {
  // local state to temporarily highlight AI move
  const [highlight, setHighlight] = useState<Move | null>(null);

  useEffect(() => {
    if (lastMove && currentPlayer === Player.Human) {
      // AI just played
      setHighlight(lastMove);
      const timer = setTimeout(() => setHighlight(null), 1500);
      return () => clearTimeout(timer);
    }
  }, [lastMove, currentPlayer]);

  return (
    <div className="board-wrapper">
      <div className="board-container">
        <div className="board" style={{ ["--size" as any]: board.length }}>
          {board.map((row, rIdx) =>
            row.map((cell, cIdx) => {
              const isLastAIMove =
                !!highlight &&
                highlight.x === rIdx &&
                highlight.y === cIdx &&
                cell.player === Player.AI;

              return (
                <Field
                  key={`${rIdx}-${cIdx}`}
                  player={cell.player}
                  enclosedBy={cell.enclosedBy}
                  row={rIdx}
                  col={cIdx}
                  onClick={onFieldClick}
                  isLastAIMove={isLastAIMove}
                />
              );
            })
          )}
        </div>
      </div>
    </div>
  );
};

export default Board;
