import React from "react";
import Board from "./components/Board";
import { useDotsGame } from "./hooks/useDotsGame";
import { Player } from "./types/dotsApiTypes";

export default function App() {
  const { game, gameId, loading, error, newGame, makeMove } = useDotsGame();

  return (
    <div style={{ padding: 20 }}>
      <h1>Dots Game</h1>

      <button onClick={() => newGame()} disabled={loading}>
        {loading ? "Creating..." : "New Game"}
      </button>

      {error && <p style={{ color: "red" }}>{error}</p>}

      {game?.board && (
        <>
          <p>Current Player: {Player[game.currentPlayer]}</p>
          <Board
            board={game.board}
            currentPlayer={game.currentPlayer}
            onFieldClick={(row, col) => makeMove(row, col)}
          />
        </>
      )}
    </div>
  );
}
