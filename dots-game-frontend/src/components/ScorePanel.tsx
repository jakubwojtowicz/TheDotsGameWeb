import React from "react";
import { Player } from "../types/dotsApiTypes";
import "./ScorePanel.css";
import { getPlayerName } from "../helpers/helpers";

interface ScorePanelProps {
  currentPlayer: Player;
  scores: {
    Human: number;
    AI: number;
  } | null;
}

export const ScorePanel: React.FC<ScorePanelProps> = ({
  currentPlayer,
  scores,
}) => {
  return (
    <div className="score-panel">
      <div className="score-section">
        <div className="label">Current Player</div>
        <div
          className={
            "value " +
            (currentPlayer === Player.Human
              ? "player-human"
              : currentPlayer === Player.AI
              ? "player-ai"
              : "")
          }
        >
          {getPlayerName(currentPlayer)}
        </div>
      </div>

      <div className="divider" />

      <div className="score-section">
        <div className="label">Score</div>

        <div className="score-row">
          <div className="score-item">
            <span className="score-player">Human</span>
            <span className="score-value">{scores?.Human ?? 0}</span>
          </div>

          <div className="score-item">
            <span className="score-player">AI</span>
            <span className="score-value">{scores?.AI ?? 0}</span>
          </div>
        </div>
      </div>
    </div>
  );
};
