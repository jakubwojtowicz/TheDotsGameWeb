import React from "react";
import Field from "./Field";
import { Player } from "../types/types";
import "./Board.css";

type BoardProps = {
  size: number;
};

const Board: React.FC<BoardProps> = ({ size }) => {
    // Prepare empty board first
    const mockBoard = Array.from({ length: size }, () =>
        Array.from({ length: size }, () => ({
        player: Player.None as Player,
        enclosedBy: Player.None as Player,
        }))
    );

    mockBoard[4][4] = {
        player: Player.Human,
        enclosedBy: Player.AI,
    };

    mockBoard[3][4] = {
        player: Player.AI,
        enclosedBy: Player.None,
    };

    
    mockBoard[3][5] = {
        player: Player.AI,
        enclosedBy: Player.None,
    };

    mockBoard[4][3] = {
        player: Player.AI,
        enclosedBy: Player.None,
    };

    mockBoard[5][4] = {
        player: Player.AI,
        enclosedBy: Player.None,
    };

    mockBoard[5][5] = {
        player: Player.AI,
        enclosedBy: Player.None,
    };

    mockBoard[4][5] = {
        player: Player.None,
        enclosedBy: Player.AI,
    };

    mockBoard[4][6] = {
        player: Player.AI,
        enclosedBy: Player.None,
    };

    mockBoard[9][8] = {
        player: Player.Human,
        enclosedBy: Player.None,
    };    
    
    mockBoard[8][9] = {
        player: Player.Human,
        enclosedBy: Player.None,
    };

    mockBoard[7][8] = {
        player: Player.Human,
        enclosedBy: Player.None,
    };

    mockBoard[8][7] = {
        player: Player.Human,
        enclosedBy: Player.None,
    };

    mockBoard[8][8] = {
        player: Player.AI,
        enclosedBy: Player.Human,
    };




    return (
    <div className="board-wrapper">
        <div className="board-container">
        <div
            className="board"
            style={{ ["--size" as any]: size }}
        >
            {mockBoard.map((row, rIdx) =>
            row.map((cell, cIdx) => (
                <Field
                key={`${rIdx}-${cIdx}`}
                player={cell.player}
                enclosedBy={cell.enclosedBy}
                />
            ))
            )}
        </div>
        </div>
    </div>
    );
};

export default Board;
