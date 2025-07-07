# Escape House

Escape House is a 3D puzzle and logic-based escape game built with Unity using the HDRP render pipeline. The game is inspired by real-life escape rooms and challenges players to solve puzzles, unlock secrets, and find their way out.

##  Features
- First-person 3D exploration
- Interactive environment
- Custom logic-based puzzles
- Sound effects and animations
- Player progress saving/loading system

##  Technologies Used
- Unity (HDRP)
- C#
- MySQL + PHP (for saving progress)
- JSON serialization
- Unity Animator and Audio systems

##  Media
Videos and screenshots are located in the [Demo](../Demo) folder.

##  Developer
**Lida Serobyan**

This project was fully developed by me, including:
- Design
- Programming
- Logic
- UI/UX
- Art setup using assets from Unity Asset Store

# Escape House - Game Structure

## Scene Layout
- **Main Menu**: Player Login, Options, Start Game
- **EscapeHouse**: Main gameplay with puzzles and logic

## Key Systems
- **GameManager**: Central controller for loading, saving, and player data.
- **ProgressManager**: Handles player progress and save/load system.
- **OptionsMenu**: Volume, fullscreen, quality settings.
- **Puzzle Logic**: Each puzzle has its own logic script and interaction trigger.

## Object Hierarchy Example
- Safe → RotatePoint → SafeDoor & SafeKeypad
- Door → RotatePoint → DoorObject & Keypad

## Save System
- JSON file sent to a remote MySQL database using PHP
- Saves player position, solved puzzles, opened doors, etc.

# How to Play

- Use 'WASD' to move, 'Mouse' to look around.
- Click on objects to interact.
- Collect cards or clues and place them in correct locations.
- Solve puzzles using logic, memory, and deduction.
- Your progress will auto-save as you play.
