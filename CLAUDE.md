# CLAUDE.md - AI Assistant Guide for Epic_POC4

This document provides comprehensive guidance for AI assistants working with the Epic_POC4 Unity project.

## Project Overview

**Project Name**: Epic_POC4 (에픽 프로젝트 4번째 POC)
**Project Type**: Unity 2D Game Development (Proof of Concept)
**Unity Version**: 6000.0.61f1 (Unity 6)
**Render Pipeline**: Universal Render Pipeline (URP) 17.0.4
**Primary Language**: C#

This is the 4th proof of concept project for an Epic game development initiative, currently in early setup phase.

## Repository Structure

```
Epic_POC4/
├── Assets/                          # Game assets, scenes, scripts, and resources
│   ├── Scenes/                      # Unity scene files
│   │   └── SampleScene.unity        # Default sample scene
│   ├── Settings/                    # URP and rendering settings
│   │   ├── Renderer2D.asset         # 2D renderer configuration
│   │   ├── UniversalRP.asset        # URP pipeline settings
│   │   ├── Lit2DSceneTemplate.scenetemplate
│   │   └── Scenes/                  # Scene templates
│   ├── InputSystem_Actions.inputactions  # Input System action maps
│   ├── DefaultVolumeProfile.asset   # Post-processing volume settings
│   └── UniversalRenderPipelineGlobalSettings.asset
│
├── Packages/                        # Unity package management
│   ├── manifest.json                # Package dependencies
│   └── packages-lock.json           # Locked package versions
│
├── ProjectSettings/                 # Unity project configuration
│   ├── ProjectVersion.txt           # Unity editor version
│   ├── ProjectSettings.asset        # Main project settings
│   ├── TagManager.asset             # Tags and layers
│   ├── InputManager.asset           # Legacy input settings
│   ├── Physics2DSettings.asset      # 2D physics configuration
│   ├── QualitySettings.asset        # Graphics quality settings
│   └── [Other Unity settings files]
│
├── .gitignore                       # Git ignore patterns (Unity standard)
├── .vsconfig                        # Visual Studio configuration
└── README.md                        # Project description
```

## Technology Stack

### Core Unity Packages

- **com.unity.feature.2d** (2.0.1) - 2D game development features
- **com.unity.render-pipelines.universal** (17.0.4) - Universal Render Pipeline for optimized 2D rendering
- **com.unity.inputsystem** (1.14.2) - New Unity Input System for flexible input handling
- **com.unity.multiplayer.center** (1.0.0) - Multiplayer networking capabilities
- **com.unity.visualscripting** (1.9.7) - Visual node-based scripting
- **com.unity.timeline** (1.8.9) - Cutscenes and animation sequences
- **com.unity.ugui** (2.0.0) - Unity UI system
- **com.unity.test-framework** (1.6.0) - Unity Test Framework for unit/integration tests

### IDE Support

- **com.unity.ide.visualstudio** (2.0.25) - Visual Studio integration
- **com.unity.ide.rider** (3.0.38) - JetBrains Rider integration

### Development Tools

- **Visual Studio** - Primary IDE (Workload: ManagedGame)
- **Git** - Version control

## Development Workflows

### Working with Unity Scenes

1. **Scene Files**: Located in `Assets/Scenes/`
   - Main scene: `SampleScene.unity`
   - Scene templates available in `Assets/Settings/Scenes/`

2. **Scene Best Practices**:
   - Always commit .unity files with proper merge settings
   - Use scene templates for consistent lighting and rendering setup
   - Test scenes in both Editor and Play mode before committing

### Input System

The project uses Unity's new Input System (not the legacy Input Manager):

- **Input Actions**: Defined in `Assets/InputSystem_Actions.inputactions`
- **Configuration**: Actions should be defined in the .inputactions file, not hard-coded
- **Usage Pattern**: Reference Input Actions through C# generated classes

### 2D Rendering & URP

The project is configured for 2D rendering with URP:

- **Renderer**: `Assets/Settings/Renderer2D.asset`
- **Pipeline Settings**: `Assets/Settings/UniversalRP.asset`
- **Global Settings**: `Assets/UniversalRenderPipelineGlobalSettings.asset`
- **Post-Processing**: `Assets/DefaultVolumeProfile.asset`

**Important**: When creating visual effects, materials, or shaders, ensure they're compatible with URP, not the built-in render pipeline.

### Scripts and Code Organization

**Current State**: No custom scripts exist yet in the project.

**Recommended Structure** (for future development):
```
Assets/
├── Scripts/
│   ├── Core/              # Core game systems
│   ├── Gameplay/          # Gameplay mechanics
│   ├── UI/                # UI controllers
│   ├── Managers/          # Manager classes (GameManager, etc.)
│   ├── Utils/             # Utility and helper classes
│   └── Tests/             # Unit and integration tests
├── Prefabs/               # Reusable game objects
├── Materials/             # URP materials
├── Sprites/               # 2D sprite assets
├── Audio/                 # Sound effects and music
└── Animations/            # Animation clips and controllers
```

### C# Coding Conventions

Follow Unity C# conventions:

1. **Naming**:
   - Classes, methods, properties: `PascalCase`
   - Private fields: `camelCase` or `_camelCase`
   - Constants: `UPPER_CASE` or `PascalCase`
   - Public fields: `PascalCase`

2. **MonoBehaviour Lifecycle**:
   - Use `Awake()` for internal initialization
   - Use `Start()` for setup that depends on other objects
   - Use `OnEnable()/OnDisable()` for event subscription
   - Avoid heavy operations in `Update()`

3. **Serialization**:
   - Use `[SerializeField]` for private fields that need Inspector visibility
   - Use `[HideInInspector]` for public fields that shouldn't show
   - Prefer composition over inheritance

4. **Performance**:
   - Cache component references in `Awake()/Start()`
   - Use object pooling for frequently instantiated objects
   - Avoid `GameObject.Find()` and `GetComponent()` in loops

## Version Control Guidelines

### What's Tracked

- All files in `Assets/` (including .meta files)
- All files in `Packages/`
- All files in `ProjectSettings/`
- `.gitignore`, `.vsconfig`, `README.md`

### What's Ignored

Per `.gitignore`:
- `/Library/` - Unity-generated cache (regenerated automatically)
- `/Temp/` - Temporary build files
- `/Obj/` - Compiled objects
- `/Build/` and `/Builds/` - Built game executables
- `/Logs/` - Unity log files
- `/UserSettings/` - User-specific editor settings
- IDE files: `.vs/`, `.vscode/`, `*.csproj`, `*.sln`
- All `.meta` files are tracked (critical for Unity asset references)

### Commit Best Practices

1. **Before Committing**:
   - Ensure all scenes are saved
   - Close Unity Editor if modifying project settings directly
   - Test that the project opens without errors
   - Run any existing tests

2. **Commit Message Format**:
   ```
   <type>: <subject>

   <body>
   ```
   Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

3. **Branch Strategy**:
   - Current branch: `claude/claude-md-mhysw5nmpt69cxdf-01GqtdANWHWZGHvsHsdL5aMW`
   - Create feature branches from main for new features
   - Use descriptive branch names: `feature/player-movement`, `fix/audio-bug`

## Testing Guidelines

The project includes Unity Test Framework (1.6.0):

### Test Structure
```
Assets/Tests/
├── EditMode/          # Editor tests (no Play mode required)
└── PlayMode/          # Runtime tests (requires Play mode)
```

### Running Tests
- Unity Editor: Window > General > Test Runner
- Command line: Use Unity's test runner CLI
- All tests should pass before committing

## Common Tasks for AI Assistants

### Creating New Scripts

1. Create script in appropriate `Assets/Scripts/` subfolder
2. Use Unity's C# template structure:
   ```csharp
   using UnityEngine;

   public class ClassName : MonoBehaviour
   {
       void Awake()
       {
           // Initialization
       }

       void Start()
       {
           // Setup after Awake
       }
   }
   ```
3. Ensure namespace matches folder structure (if using namespaces)
4. Add XML documentation comments for public APIs

### Adding New Assets

1. Place assets in logical folders under `Assets/`
2. Never manually edit `.meta` files
3. Ensure URP-compatible materials for 3D/2D rendering
4. Compress textures appropriately (2D sprites should use Sprite mode)

### Modifying Project Settings

1. Close Unity Editor before editing settings files directly
2. Settings are in YAML format - maintain structure
3. Test project opens correctly after changes
4. Document any non-standard settings changes

### Working with Input System

1. Open `InputSystem_Actions.inputactions` in Unity's Input Actions editor
2. Define action maps, actions, and bindings
3. Generate C# class for compile-time safety
4. Reference actions in scripts through generated class

### Debugging Common Issues

1. **"Missing Reference" errors**:
   - Likely caused by missing .meta files
   - Ensure .meta files are committed and not in .gitignore

2. **"Incompatible Shader" warnings**:
   - Material created for Built-in pipeline instead of URP
   - Convert materials using: Edit > Rendering > Materials > Convert to URP

3. **Input not working**:
   - Check if Input System package is enabled in Player Settings
   - Verify Input Actions are enabled and generated class exists

4. **Build failures**:
   - Check Platform-specific settings in Player Settings
   - Ensure all referenced assets are included in build
   - Verify no compile errors in scripts

## Project-Specific Conventions

### Korean Language Support

- Project name/description includes Korean (한국어)
- Consider supporting Korean text rendering if UI will have Korean text
- Use Unicode-supporting fonts for Korean characters

### Multiplayer Considerations

- `com.unity.multiplayer.center` is installed
- When implementing features, consider multiplayer implications
- Test both single-player and multiplayer scenarios
- Use proper network synchronization for multiplayer objects

### Visual Scripting

- Visual Scripting is available as alternative to C#
- Visual scripts can be found in `Assets/` with `.asset` extension
- Consider team preference when choosing Visual Scripting vs C#

## Performance Optimization Tips

### 2D-Specific

1. Use Sprite Atlases to reduce draw calls
2. Enable 2D Sorting Layer batching
3. Use 2D Physics instead of 3D Physics for 2D games
4. Optimize sprite sheet texture sizes

### URP-Specific

1. Use URP's 2D Renderer for better 2D performance
2. Configure appropriate MSAA levels in URP Asset
3. Disable unused renderer features
4. Use URP's simplified lighting for 2D

### General Unity

1. Profile using Unity Profiler before optimizing
2. Use object pooling for frequently spawned objects
3. Minimize Update() calls - use events when possible
4. Cache transform/component references

## Resources and Documentation

- **Unity 6 Documentation**: https://docs.unity3d.com/6000.0/Documentation/Manual/
- **URP Documentation**: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/
- **Input System**: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/
- **Unity Test Framework**: https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/
- **Multiplayer**: https://docs.unity3d.com/Packages/com.unity.multiplayer.center@1.0/

## Important Notes for AI Assistants

1. **Never Edit Binary Files**: .unity, .asset, .prefab files are YAML-formatted but complex. Prefer Unity Editor for editing.

2. **Meta Files Are Critical**: Every asset has a .meta file with GUID. Never delete or regenerate these manually.

3. **Unity Version Sensitivity**: Project uses Unity 6 (6000.x). Features/APIs may differ from Unity 5 or 2020-2023 versions.

4. **URP vs Built-in**: This project uses URP. Built-in pipeline shaders/features won't work.

5. **Input System vs Input Manager**: Project uses new Input System. `Input.GetKey()` patterns should be avoided.

6. **Project is Early Stage**: Currently minimal custom code. Structure can be established following best practices.

7. **Testing**: Always recommend running tests before commits if test coverage exists.

8. **Documentation**: Update this file when adding significant features or changing conventions.

## Quick Reference Commands

### Git Operations
```bash
# Check current branch
git branch

# Create and switch to feature branch
git checkout -b feature/feature-name

# Commit changes
git add .
git commit -m "feat: description of changes"

# Push to remote
git push -u origin branch-name
```

### Unity CLI (when Unity is installed)
```bash
# Run tests
Unity -runTests -projectPath /path/to/project -testResults results.xml

# Build project
Unity -quit -batchmode -projectPath /path/to/project -buildTarget StandaloneWindows64 -executeMethod BuildScript.Build
```

---

**Document Version**: 1.0
**Last Updated**: 2025-11-14
**Maintained By**: AI Assistants working on Epic_POC4

For questions or updates to this guide, modify this file and commit changes with appropriate documentation.
