# Game Features

# Bootstrap Feature

To Enable the Bootstrap feature add constraint define: "UNIGAME_BOOTSTRAP_ENABLED" to your game settings

Bootstrap feature allow you:

- run a set of commands on game start
- register services and features on game start

## SetUp

To configure the bootstrap feature, you need to create a `GameBootSettings` scriptable object

```
menuName = "UniGame/Features/Game Boot Settings"
```

1. You can create asset in your project custom location. When you need to setup the addressable asset path to 

`GameBootSettings`

2. Or you can create asset in the Resources folder.


## Bootstrap Commands

Some custom game start commands can be registered in the `GameBootSettings` 
as implementations of `IGameBootCommand` interface.

## Data Sources

Data sources allow you to load some services from game start

Data Source asset can be created from the menu: `UniGame/Context/Async Data Sources`

All data sources executed after all `IGameBootCommand` commands.

## Getting started

