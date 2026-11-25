DockPanel Suite (MCS Fork)
==========================

This is the [Promicron](https://www.promicron.de) fork of [DockPanelSuite](https://github.com/dockpanelsuite/dockpanelsuite), used in the MCS (Microscope Control Software) product.

## Branch: `mcs/3.1.0`

The DLLs in MCS's `Vendor/DockPanelSuite` are built from this branch. It is based on upstream `Release_3.1.0`
(AssemblyVersion 1.0.0.0, FileVersion 3.1.0.0, separate ThemeVS2005 assembly) with these changes:

- **DockContentHandler**: Dispose `TabPageContextMenuStrip` to prevent memory leak; fix AutoHide pane activation when `ActiveAutoHideContent` is null; disable `bRestoreFocus` `Activate()` call that causes unwanted MDI child activation
- **DockPanel.AutoHideWindow**: Add try/catch in `SetTimerMouseTrack` to prevent unhandled exceptions
- **DockPanel**: Call `DockPanelTheme.CleanUp()` during disposal to release theme resources
- **ThemeBase**: Prevent duplicate ToolStrip registration; subscribe to `Disposed` event to clean up `_stripBefore` dictionary
- **VisualStudioToolStripExtender**: Subscribe to `ToolStrip.Disposed` to remove entries from strips dictionary
- **VS2013SplitterControl**: Add try/catch in `OnPaint`; remove `Debug.Assert` for SplitterSize
- **DockContentHandler.TabAccentColor**: a coloured bar at the leading edge of a document tab and of its entry in the drop-down document list (VS2005 and VS2013 strips); MCS marks preview documents with it

The same six fixes are on `mcs/3.0.4`, but MCS does not use a build of that branch: its assemblies differ
(version 3.0.4.0, VS2005 theme inside the main assembly).

## Building

Use Visual Studio's MSBuild (not `dotnet build`: the theme projects need the .NET Framework resource generator):

    MSBuild WinFormsUI\ThemeVS2005.csproj /restore /p:Configuration=Release /p:TargetFramework=net40
    MSBuild WinFormsUI\ThemeVS2015.csproj /restore /p:Configuration=Release /p:TargetFramework=net40

Both build `WeifenLuo.WinFormsUI.Docking.dll` as a dependency.

## Upstream

For the original project, see [dockpanelsuite/dockpanelsuite](https://github.com/dockpanelsuite/dockpanelsuite).
