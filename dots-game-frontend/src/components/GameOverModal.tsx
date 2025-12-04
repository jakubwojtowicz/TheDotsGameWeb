import React from "react";
import "./GameOverModal.css";

interface GameOverModalProps {
  winner: string | null;
  onNewGame: () => void;
}

export const GameOverModal: React.FC<GameOverModalProps> = ({
  winner,
  onNewGame,
}) => {
  if (!winner) return null;

  const winnerText =
    winner === "None" ? "It's a Draw!" : `Winner: ${winner}`;

  // assign class for colored winner text
  let winnerClass = "";
  if (winner === "Human") winnerClass = "winner-human";
  else if (winner === "AI") winnerClass = "winner-ai";

  return (
    <div className="game-over-modal-overlay">
      <div className="game-over-modal-container">
        <h2 className={winnerClass}>{winnerText}</h2>
        <button className="game-over-modal-btn" onClick={onNewGame}>
          New Game
        </button>
      </div>
    </div>
  );
};
