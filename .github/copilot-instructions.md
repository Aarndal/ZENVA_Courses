# Copilot Instructions for ZENVA_Courses

## Repository Overview

This is a **Unity 6** (version 6000.3.6f1) project used as the source code for ZENVA game development courses. It contains multiple mini-game projects and a shared reusable systems library, all written in **C#**. The project uses the **Universal Render Pipeline (URP)**.

## Project Layout

```
ZENVA_Courses/
├── Assets/
│   ├── _ZENVA_Courses/        # Main course content
│   │   ├── 2D_JumpnRun/       # 2D platformer mini-game
│   │   ├── Balloon_Popper/    # Balloon Popper mini-game
│   │   ├── Bowling/           # Bowling mini-game
│   │   ├── Input/             # Shared input readers
│   │   ├── ProjectArt/        # Shared art assets
│   │   ├── Scripts/           # Shared reusable systems (see below)
│   │   └── UI Toolkit/        # Shared UI Toolkit assets
│   ├── Packages/              # Unity package assets
│   ├── Plugins/               # Third-party plugins
│   └── Settings/              # URP and other render settings
├── Packages/
│   ├── manifest.json          # Unity package dependencies
│   └── com.singularitygroup.hotreload/  # Hot Reload package (local)
├── ProjectSettings/
│   └── ProjectVersion.txt     # Unity version: 6000.3.6f1
└── .gitignore                 # Standard Unity + Visual Studio ignores
```

### Shared Scripts Library (`Assets/_ZENVA_Courses/Scripts/`)

| Folder | Purpose |
|---|---|
| `Checker/` | `IChecker` interface |
| `CollectibleSystem/` | `ICollectible`, `ICollector` interfaces |
| `DataProvider/` | `IDataProvider`, `NoData` |
| `Debugging/` | `DebugLogger`, `LogFormatter`, `DebugLogHandler`, `LogMessageType` |
| `EventSystem/` | Event channels, publishers, subscribers, queues, transmitters |
| `Factories/` | `IFactory` interface |
| `InteractableSystem/` | Interactable/toggleable/clickable/triggerable interfaces and `PC_Interaction` component |
| `MovementBehaviours/` | `IMoveable` interface |
| `ObjectPools/` | `IObjectPool` interface |
| `ScoreManagement/` | `ScoreManager`, `ScoreDisplay`, `IScoreChanger` |
| `Sequences/` | `IIntervalSequence`, `IntervalSequenceSO` |
| `SpawnSystem/` | `Spawner`, `SpawnablePool`, `SpawnableCatcher`, `SpawnableFactory`, ScriptableObjects |
| `TransformExtensions.cs` | Extension methods for `Transform` |

## Key Technologies & Packages

- **Unity 6** (6000.3.6f1) with **URP** 17.3.0
- **UniTask** (`com.cysharp.unitask`) – async/await support in Unity; used for delays and coroutine replacements (e.g., `UniTask.Delay`, `UniTask.Yield`)
- **Unity Input System** (`com.unity.inputsystem` 1.18.0) – new input system with `InputReader` ScriptableObjects
- **NuGetForUnity** (`com.github-glitchenzo.nugetforunity` 4.5.0) – NuGet package management inside Unity
- **Eflatun.SceneReference** (`com.eflatun.scenereference` 4.1.1) – type-safe scene references
- **Hot Reload** (`com.singularitygroup.hotreload`) – live code patching during Play Mode

## Architecture Patterns

- **ScriptableObjects** are used extensively for data (`SpawnableDataSO`, `SpawnInstructionSO`, `SpawnContextSO`, `BalloonDataSO`, `CoinDataSO`, etc.) and configuration.
- **EventSystem**: Custom event channel system under the `EventSystem` namespace (`EventChannel<TEventArgs>`, `IEventChannel`, `IPublisher`, `ISubscriber`, `EventTransmitter`).
- **Spawn System**: `Spawner` MonoBehaviour driven by `ISpawnerInstruction` queue, pulling from `SpawnablePool` (local) or a global pool.
- **Object Pools**: `IObjectPool` interface, implemented via `SpawnablePool`.
- **Interfaces first**: Game entities implement interfaces (e.g., `ISpawnable`, `ICollectible`, `IInteractable`) to decouple systems.
- **Debugging**: Use `DebugLogger.Log(LogMessageType, ...)` from the `Debugging` namespace instead of `Debug.Log` directly. Wrap editor-only logs in `#if UNITY_EDITOR`.
- **Namespaces**: Each system has its own namespace (e.g., `EventSystem`, `SpawnSystem`, `Debugging`, `ScoreManagement`).

## Coding Conventions

- Use `#region` / `#endregion` blocks to organize class members (e.g., `Unity Lifecycle Methods`, `Private Methods`, `Public Methods`, `IInterface Implementation`).
- XML `<summary>` doc-comments on public types and methods.
- `[SerializeField]` with `Tooltip` attributes on inspector-exposed fields; use `private` backing fields.
- Null checks and validation before logic; log errors/warnings using `DebugLogger`.
- `CancellationTokenSource` for async task cancellation; dispose properly in `Dispose()` / `OnDestroy()`.
- Event accessors use the add/remove pattern with de-duplication (`-= value; += value` in `add`).

## Build & Validation

This is a Unity project — there is **no command-line build script** included. Validation is performed by opening the project in Unity Editor 6000.3.6f1.

- **No automated test runner scripts** are present. The project includes `com.unity.test-framework` 1.6.0 but no test assembly definitions or test files are committed.
- **No CI/CD pipelines** (no `.github/workflows/` directory).
- To validate C# changes: open the project in Unity, ensure the Console has no compile errors, and enter Play Mode to test the affected scene.
- Always ensure `.meta` files are committed alongside any new asset or script file you add — Unity requires a `.meta` file for every asset.

## Important Notes

- **Never delete or modify `.meta` files** unless you are also deleting the corresponding asset.
- ScriptableObject assets live alongside the scripts that define them; new SOs should be created via the Unity Editor (`Assets > Create` menu), not by hand.
- The `Packages/` directory at the repo root contains Unity package source (including `com.singularitygroup.hotreload`) — do not confuse it with `Assets/Packages/` (NuGet packages).
- When adding new C# files, place them in the appropriate sub-folder under `Assets/_ZENVA_Courses/` and match the namespace to the folder name.
