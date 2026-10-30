DockPanel Suite (MCS Fork)
==========================

This is the [Promicron](https://www.promicron.de) fork of [DockPanelSuite](https://github.com/dockpanelsuite/dockpanelsuite), used in MCS (Microscope Control Software).

## Branch: `net10-multitarget`

Based on upstream `Release_3.1.0` (`dc18ab0`). That is the code MCS has shipped as
`Vendor/DockPanelSuite/*.dll` (assembly version 1.0.0.0, file version 3.1.0.0): its
decompiled code equals a 3.1.0 build plus the fixes below. The older branch
`mcs/3.0.4` carries the same fixes on 3.0.4, which is *not* what MCS ships.

Fixes (cherry-picked from `mcs/3.0.4`):

- **DockContentHandler**: Dispose `TabPageContextMenuStrip` to prevent a memory leak; fix AutoHide pane activation when `ActiveAutoHideContent` is null; disable the `bRestoreFocus` `Activate()` call that steals focus
- **DockPanel.AutoHideWindow**: try/catch in `SetTimerMouseTrack`
- **DockPanel**: Call `DockPanelTheme.CleanUp()` on dispose
- **ThemeBase**: Prevent duplicate ToolStrip registration; drop disposed ToolStrips from `_stripBefore`
- **VisualStudioToolStripExtender**: Drop disposed ToolStrips from `strips`
- **VS2013SplitterControl**: try/catch in `OnPaint`; remove the `SplitterSize` `Debug.Assert`

Targets: `net40` (drop-in for the current .NET Framework build) and `net10.0-windows`.
On `net10.0-windows` the theme bitmaps are written as preserialized resources, so they
load without BinaryFormatter (see `WinFormsUI/Directory.Build.props`).

### Build

Use Visual Studio's MSBuild (VS 2026 / MSBuild 18 with the .NET 10 SDK). The dotnet CLI
cannot build the `net40` theme resources (MSB3823).

```
cd WinFormsUI
msbuild WinFormsUI.csproj  -restore -p:Configuration=Release -p:GeneratePackageOnBuild=false
msbuild ThemeVS2005.csproj -restore -p:Configuration=Release -p:GeneratePackageOnBuild=false
msbuild ThemeVS2015.csproj -restore -p:Configuration=Release -p:GeneratePackageOnBuild=false
```

Output: `WinFormsUI/bin/Release/net40/` and `WinFormsUI/bin/Release/net10.0-windows/`.

## Upstream

For the original project, see [dockpanelsuite/dockpanelsuite](https://github.com/dockpanelsuite/dockpanelsuite).
