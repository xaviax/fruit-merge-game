# FruitMatch

FruitMatch is a portrait-oriented 2D fruit-merging game built with Unity and C#. The player positions and drops fruit into a container, combines matching tiers, and tries to build the highest-value fruit without allowing the stack to remain above the game-over line.

The project was developed as a gameplay-cloning exercise with an emphasis on modular systems, data-driven configuration, responsive input, clear feedback, and mobile monetization integration.

## Features

- Touch and mouse input with UI blocking, gesture tracking, and horizontal boundary clamping
- Weighted fruit spawning with current and next-fruit queues
- Data-driven fruit tiers configured through ScriptableObjects
- Physics-based dropping and collision-driven merge progression
- Tier-based scoring with event-driven HUD updates
- Timed overflow detection that avoids false game-over triggers during active merges
- Explicit game states for menu, loading, gameplay, pause, and loss flows
- Asynchronous scene loading with visible progress feedback
- Audio, particle effects, impact feedback, and held-fruit guidance
- Camera fitting for different portrait aspect ratios
- Banner, interstitial, and rewarded ad support through the project ad framework
- In-game ad debug controls and callback logging

## Technology

- Unity 6000.5.1f1
- C#
- Universal Render Pipeline
- Unity Input System with Enhanced Touch
- Unity 2D Physics
- TextMeshPro and Unity UI
- ScriptableObjects
- Unity LevelPlay and Google Mobile Ads packages

## Architecture

The game is divided into focused modules under `Assets/FruitMatch/Modules`:

| Module | Responsibility |
| --- | --- |
| `GameManager` | Owns the game-state flow, scene transitions, pause behavior, restart behavior, and loss state. |
| `FruitSpawner` | Maintains the weighted spawn queue and creates the current fruit after a configurable delay. |
| `FruitAimController` | Handles mouse and touch gestures, ignores UI interactions, and constrains fruit movement to valid bounds. |
| `FruitDefinition` | Stores per-tier visual, physics, and scoring data as a ScriptableObject. |
| `FruitCatalog` | Provides ordered tier lookup and resolves the next fruit definition during a merge. |
| `Fruit` | Applies definition data, manages held and dropped states, and validates merge eligibility. |
| `FruitMergeSystem` | Replaces two matching fruits with the next tier and coordinates scoring, audio, and VFX. |
| `ScoreManager` | Calculates tier-based rewards and publishes score changes to the HUD. |
| `GameOverZone` | Tracks eligible fruits above the limit and triggers a loss only after sustained overlap. |
| `UIManager` | Creates and removes full-screen interfaces and popups across scene changes. |
| `SoundManager` and `VFXManager` | Centralize gameplay audio and visual feedback. |
| `AdsManager` | Wraps banner, interstitial, and rewarded ads with availability checks and guarded callbacks. |

## Project Structure

```text
Assets/
|-- FruitMatch/
|   |-- Modules/
|   |   |-- Ads/
|   |   |-- Gameplay/
|   |   |-- Sound/
|   |   `-- UI/
|-- Scenes/
|   |-- MainMenu.unity
|   `-- Gameplay.unity
|-- LevelPlay/
`-- _AdsData/
```

## Getting Started

### Requirements

- Unity Hub
- Unity Editor 6000.5.1f1
- Git
- Android Build Support for Android builds

### Run in the Editor

1. Clone the repository:

   ```bash
   git clone https://github.com/xaviax/fruit-merge-game.git
   ```

2. Add the cloned directory as a project in Unity Hub.
3. Open the project with Unity 6000.5.1f1 and allow Unity to import packages and assets.
4. Open `Assets/Scenes/MainMenu.unity`.
5. Enter Play Mode and start the game from the main menu.

### Controls

| Platform | Action |
| --- | --- |
| Mouse | Hold the left mouse button to position the fruit, then release to drop it. |
| Touch | Drag with one finger to position the fruit, then lift the finger to drop it. |

Input that begins over a UI element is ignored so menu and gameplay interactions do not conflict.

## Android Build

1. Install Android Build Support for Unity 6000.5.1f1.
2. Open Unity's Build Profiles window and select Android.
3. Confirm that `MainMenu` and `Gameplay` are enabled in the scene list.
4. Configure signing and application identifiers for the target environment.
5. Build an APK or Android App Bundle.

Live advertisements require valid network and application configuration. The core gameplay systems can still be exercised in the Unity Editor without production ad credentials.

## Design Notes

- Fruit configuration is separated from runtime behavior, allowing tiers to be rebalanced without editing gameplay code.
- Merge eligibility is guarded on both fruits to prevent duplicate collision callbacks from creating multiple results.
- The next-fruit preview is driven by an event rather than direct UI polling.
- The loss zone accumulates overlap time only for active, dropped, non-merging fruit and resets when a fruit leaves the zone.
- Persistent managers coordinate scene-level systems while gameplay-specific objects remain inside the gameplay scene.

## Repository

Source: [github.com/xaviax/fruit-merge-game](https://github.com/xaviax/fruit-merge-game)
