import { Player } from "../types/dotsApiTypes";

export function getPlayerName(player: Player | null): string{
  switch (player) {
    case Player.Human:
      return "Human";
    case Player.AI:
      return "AI";
    case Player.None:
    default:
      return "None";
  }
};