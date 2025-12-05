import React from "react";
import Board from "./components/Board";
import { useDotsGame } from "./hooks/useDotsGame";
import { ScorePanel } from "./components/ScorePanel";
import "./App.css";
import { GameOverModal } from "./components/GameOverModal";
import { getPlayerName } from "./helpers/helpers";

export default function App() {
  const { game, loading, error, newGame, makeMove } = useDotsGame();

  return (
  <div className="app-wrapper">
    <div className={game?.board ? "header header-started" : "header header-initial"}>
      <h1 className="app-title">Dots Game</h1>

      <button
        className="new-game-btn"
        onClick={() => newGame()}
        disabled={loading}
      >
        New Game
      </button>
    </div>

    {error && <p style={{ color: "red" }}>{error}</p>}

    {game?.board && (
      <>
        <ScorePanel
          currentPlayer={game.currentPlayer ?? null}
          scores={
            game.scores ? { Human: game.scores.Human, AI: game.scores.AI } : null
          }
        />

        <Board
          board={game.board}
          currentPlayer={game.currentPlayer}
          onFieldClick={(row, col) => makeMove(row, col)}
          lastMove={game.lastMove}
        />
      </>
    )}

    {game?.isGameOver && (
      <GameOverModal winner={getPlayerName(game.winner)} onNewGame={() => newGame()} />
    )}

  </div>
  );
}
