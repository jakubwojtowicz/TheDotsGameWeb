# Dots Game Web

Dots Game is a full-stack implementation of the classic territory-control game where two players (human vs AI) place dots on a board and attempt to enclose areas.  
The project consists of:

- **DotsServer** — a REST API built with **.NET 10**
- **Front-end** — an interactive **React + TypeScript** application

The game logic is fully implemented on the backend, including:
- state management,
- move validation,
- enclosure detection,
- area capturing,
- AI moves.

### 🤖 AI System  
The AI opponent is implemented using the **Minimax algorithm with Alpha-Beta pruning**, allowing efficient evaluation of possible future game states.  
To improve strategic decision-making, the algorithm uses **custom heuristics**, such as:
- board control evaluation,
- potential enclosure scoring,
- risk estimation near opponent clusters,
- and mobility/expansion heuristics.

## 🛠️ Tech Stack

- **.NET 10 / ASP.NET Core** — REST API backend  
- **C#** — game logic, AI engine, API controllers  
- **Dependency Injection**  
- **.NET Routing & Minimal API**  
- **X-Unit** (backend testing project)
- **React 18** — component-based UI  
- **TypeScript** — type-safe client logic  
- **CSS** — styling


## 🚀 Quick Start

Follow the steps below to run the **Dots Game** game in your local environment.


### 1. Install prerequisites

- .NET SDK 10

https://dotnet.microsoft.com/download

- Node.js 18+ 

https://nodejs.org

- VS Code (optional)

https://code.visualstudio.com/

### 2. Clone the repo

https://github.com/jakubwojtowicz/TheDotsGameWeb.git

### 3. Setup backend

``` bash
cd ./DotsServer
dotnet restore
dotnet run
```

The server by default will start at: http://localhost:5158

### 4. Setup frontend (in another terminal)

``` bash
cd ./dots-game-frontend
npm install
...
npm start
```
The app will open at http://localhost:3000

## 📘 Game Rules & Architecture Diagram

The backend project contains additional documentation describing the structure and logic of the game:

- **[Game Rules — rules.md](DotsServer/Docs/rules.md)**  
- **Backend class Diagram:**  
  ![Class Diagram](DotsServer/Docs/diagrams/class-diagram/out/class_diagram.png)

## 🎮 Gameplay

![Gameplay Demo](gameplay/gameplay.gif)