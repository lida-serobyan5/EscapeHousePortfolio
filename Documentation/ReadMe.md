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
- UModeler-X

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
- **Main Menu**: Player SignUp/SignIn, MainMenu - Play, Options Menu, Help, Exit Game 
- **EscapeHouse**: Main gameplay 

## Key Systems
- **GameManager**: Central controller for loading and saving.
- **ProgressManager**: Handles player progress and save/load system.
- **OptionsMenu**: Volume, fullscreen, quality settings.
- **Puzzle Logic**: Each puzzle has its own logic script and interaction trigger.

## Save System
- JSON file sent to a remote MySQL database using PHP
- Saves player position, solved puzzles, opened doors, etc.


