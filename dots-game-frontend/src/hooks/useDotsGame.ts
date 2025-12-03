import { useState } from "react";
import { createNewGame, apiMakeMove, apiMakeAiMove } from "../api/dotsApi";
import { GameState, MoveDto, Player } from "../types/dotsApiTypes";
import { NewGameResult } from "../types/helperTypes";

export function useDotsGame() {
  const [game, setGame] = useState<GameState | null>(null);
  const [gameId, setGameId] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function newGame(boardSize: number = 10) {
    try {
      setLoading(true);
      setError(null);

      const result: NewGameResult = await createNewGame(boardSize);
      setGame(result.game);
      setGameId(result.id);
    } catch (err: any) {
      setError(err.message || "Failed to create a new game");
    } finally {
      setLoading(false);
    }
  }

  // Human move + automatic AI move
  async function makeMove(row: number, col: number) {
    if (!gameId || !game || game.isGameOver) return;

    try {
      setLoading(true);
      setError(null);

      const dto: MoveDto = { x: row, y: col };

      const humanMoveGame = await apiMakeMove(gameId, dto);
      setGame(humanMoveGame);

      if (!humanMoveGame.isGameOver && humanMoveGame.currentPlayer === Player.AI) {
        const aiMoveGame = await apiMakeAiMove(gameId);
        setGame(aiMoveGame);
      }

    } catch (err: any) {
      setError(err.message || "Failed to make move");
    } finally {
      setLoading(false);
    }
  }

  return { game, gameId, loading, error, newGame, makeMove };
}