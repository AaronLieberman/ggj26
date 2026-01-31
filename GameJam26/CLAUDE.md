# CLAUDE.md - GameJam26

This file provides guidance to Claude Code (claude.ai/code) when working on the Unity project.

## Technology Stack

- **Unity** with C#

## Code Style

- PascalCase for classes and enums.
- Private fields are camelCase prefixed with `_` (e.g. _playerController). 
- Only use access keywords for fields and methods when they're not the default.
- All methods, public and private do not have a prefix and are PascalCase (e.g. GoDoStuff()).
- Don't add section comments to files.
- Use ternary ?: operator but don't nest them. Instead, use a local variable, assign it to the most common case and then use if statements to change it.
- Use LF (Unix line endings) on new files.

## Unity Usage

### Utilities class
Liberally use the Utilities class, it's got our best practices encoded in it.

### Accessing Unity Components
We want to streamline component and singleton access for the game jam, quick iteration over more correct Unity usage and performance. The goal is to be able to swap out prefabs in scenes without having to do a lot of setup; the various singletons should automatically find each other.

##Take root singletons using Utilities.GetRootComponent
```
	PlayerController _playerController;

    public void Start()
    {
        _playerController = Utilities.GetRootComponent<PlayerController>();
    }
```

### Acquire and store components in private member fields
```
	private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
```

### Liberally use GetComponent<T>() and GetComponentInChildren<T>()
```
var damageHandler = GetComponentInChildren<EntityDamageHandler>();
```

