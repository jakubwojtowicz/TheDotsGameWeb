import { GameState, MoveDto } from "../types/dotsApiTypes";
import { NewGameResult } from "../types/helperTypes";

const BASE_URL = process.env.REACT_APP_API_URL!;


export async function createNewGame(boardSize: number): Promise<NewGameResult> {
  const res = await fetch(`${BASE_URL}/api/game/new?boardSize=${boardSize}`, {
    method: "POST",
  });

  if (!res.ok) throw new Error("Failed to create a new game");

  const game: GameState = await res.json();

  const location = res.headers.get("Location");

  if (!location) throw new Error("Missing Location header from server");

  const id = location.split("/").pop()!;
  return { id, game };
}

export async function apiMakeMove(gameId: string, move: MoveDto): Promise<GameState> {
  const res = await fetch(`${BASE_URL}/api/game/${gameId}/make-move`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(move),
  });

  if (!res.ok) {
    throw new Error("Failed to make move");
  }

  const updatedGame: GameState = await res.json();
  return updatedGame;
}

export async function apiMakeAiMove(gameId: string): Promise<GameState> {
  const res = await fetch(`${BASE_URL}/api/game/${gameId}/make-ai-move`, {
    method: "PUT"
  });

  if (!res.ok) {
    throw new Error("Failed to make move");
  }

  const updatedGame: GameState = await res.json();
  return updatedGame;
}


