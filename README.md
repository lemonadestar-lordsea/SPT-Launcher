# Launcher

Custom launcher for Escape From Tarkov to start the game in offline mode

**Project**        | **Function**
------------------ | --------------------------------------------
Aki.Build          | Build script
Aki.ByteBanger     | Assembly-CSharp.dll patcher
Aki.Launcher       | Launcher frontend
Aki.Launcher.Base  | Launcher backend

## Requirements

- .NET 6 SDK
- VSCode

### For UI Development

- Visual Studio Community 2022 (.NET desktop workload)
- Avalonia Visual Studio Extension

## Build

1. Open Launcher.code-workspace in VSCode.
2. Run the build task: (top toolbar) Terminal -> Run Build Task...
3. Copy-paste all files inside `Build` into `game root directory`, overwrite when prompted.
