# UniGame Game Features

## Bootstrap

The bootstrap feature creates the main game context, runs startup commands, and registers async data sources in a predictable order.

The `UNIGAME_BOOTSTRAP_ENABLED` define is no longer required to use the bootstrap API.

- Without the define, you can start the bootstrap manually.
- With the define, `GameBootstrap.AutoInitializeGame()` runs automatically after scene load.

## What It Does

`GameBootstrap` is responsible for:

- creating the root `EntityContext`
- assigning `GameContext.Context`
- initializing Addressables
- loading `GameBootSettings`
- executing boot commands
- registering configured async sources

Public entry points:

- `GameBootstrap.InitializeGame()` starts bootstrap in fire-and-forget mode
- `GameBootstrap.InitializeGameAsync()` runs the full pipeline as an awaitable task
- `GameBootstrap.Restart()` disposes the current lifetime and starts bootstrap again
- `GameBootstrap.Dispose()` terminates the current bootstrap lifetime
- `GameBootstrap.Context` exposes the current root context
- `GameBootstrap.LifeTime` exposes the current bootstrap lifetime

## Boot Pipeline

The runtime pipeline is executed in this order:

1. `InitializeAddressableAsync`
2. `InitializeAsync`
3. `ExecuteBootInitStepsAsync`
4. `InitializeServicesAsync`

### 1. Initialize Addressables

The bootstrap begins with `Addressables.InitializeAsync()`.

### 2. Load `GameBootSettings`

`GameBootSettings` is loaded by the addressable key `GameBootSettings`.

If the addressable asset is not found, the bootstrap falls back to `Resources.Load<GameBootSettings>()`.

This gives you two supported setup options:

1. Create a `GameBootSettings` asset anywhere in the project and mark it addressable with the key `GameBootSettings`.
2. Put a `GameBootSettings` asset in a `Resources` folder.

Asset menu:

```text
UniGame/Bootstrap/GameBootSettings
```

### 3. Execute Boot Commands

Boot commands are defined in `GameBootSettings.gameInitCommands` and executed sequentially.

Each command implements `IGameBootCommand`:

```csharp
public interface IGameBootCommand
{
	UniTask<BootStepResult> ExecuteAsync(IContext context);
}
```

`BootStepResult` controls error handling:

- `success = true`: continue normally
- `success = false` and `canContinue = true`: log the error and continue
- `success = false` and `canContinue = false`: stop the bootstrap

Example:

```csharp
using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UniGame.Features.Bootstrap;

public class WarmupCommand : IGameBootCommand
{
	public async UniTask<BootStepResult> ExecuteAsync(IContext context)
	{
		await UniTask.Yield();

		return new BootStepResult
		{
			success = true,
			canContinue = true,
			error = string.Empty,
		};
	}
}
```

The package also includes a sample command: `CheckAndClearBundleCacheByVersionCommand`.

### 4. Register Services and Features

After all boot commands complete, `GameBootstrap` registers the `GameBootSettings.source` asset.

This field is an `AsyncContextSource`, which means bootstrap sources are executed as an ordered pipeline of async data sources.

Create source assets from:

```text
UniGame/Context/Async Data Sources
```

## `awaitLoading` and Service Steps

Each entry inside `GameBootSettings.source.asyncSources` has an `awaitLoading` flag.

This flag defines whether a source is a synchronization barrier in the bootstrap pipeline.

- `awaitLoading = false`: the source is grouped with adjacent non-awaited sources and loaded in parallel inside the same step
- `awaitLoading = true`: the source becomes its own awaited step, and bootstrap does not continue until it finishes

In practice, this lets you split initialization into stages.

Example configuration:

- `AnalyticsSource` with `awaitLoading = false`
- `DebugServiceSource` with `awaitLoading = false`
- `RemoteSettingsSource` with `awaitLoading = true`
- `AudioSource` with `awaitLoading = false`
- `GameplaySource` with `awaitLoading = true`

Effective execution steps:

1. Load `AnalyticsSource` and `DebugServiceSource` in parallel
2. Await `RemoteSettingsSource`
3. Load `AudioSource`
4. Await `GameplaySource`

Use this when some services can start in the background, but later systems must wait for critical dependencies such as remote config, save migration, authentication, or content manifests.

## `GameBootSettings`

`GameBootSettings` contains two main parts:

- `gameInitCommands`: sequential startup commands
- `source`: async source pipeline used to register services and features into the root context

Editor helpers:

- `Fill()` scans available `DataSourceAsset` assets and adds missing ones to the source list
- `Save()` persists the asset after editing

## Manual and Automatic Startup

### Manual startup

Use manual startup when you want to control bootstrap timing yourself.

```csharp
using Cysharp.Threading.Tasks;
using Game.Runtime.Services.Bootstrap;

public static class EntryPoint
{
	public static async UniTask StartAsync()
	{
		await GameBootstrap.InitializeGameAsync();
	}
}
```

You can also call:

```csharp
GameBootstrap.InitializeGame();
```

### Automatic startup

If `UNIGAME_BOOTSTRAP_ENABLED` is defined, bootstrap starts automatically through `RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)`.

This define only enables auto-start. It does not gate the bootstrap feature itself.

## Recommended Usage

- Keep `gameInitCommands` focused on explicit boot actions that must happen before source registration.
- Put long-lived services into async sources instead of commands when they belong to context composition.
- Use `awaitLoading = true` only for sources that are true dependencies for later boot steps.
- Use `awaitLoading = false` for services that can be registered in parallel without blocking the rest of startup.
- Prefer `InitializeGameAsync()` in tests or controlled startup flows where you need deterministic completion.

## Summary

Use `GameBootstrap` when you need a single entry point for startup orchestration:

- manual or automatic game initialization
- ordered boot commands with failure control
- context creation and lifetime management
- step-based async service registration with `awaitLoading`

