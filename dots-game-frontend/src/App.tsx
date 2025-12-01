import React, { useState } from "react";
import { Field, Player, GameState } from "./types/types";
import Board from "./components/Board";

function App() {

  return (
    <div style={{ padding: 20 }}>
      <h1>Dots Game</h1>
      <p>Current Player: Human</p>
      <Board size={15} />
    </div>
  );
  
}

export default App;
