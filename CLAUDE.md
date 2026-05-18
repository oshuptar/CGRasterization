# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build CGRasterization.slnx

# Run the app
dotnet run --project CGRasterization.App

# Build in release mode
dotnet build CGRasterization.slnx -c Release
```

There are no automated tests in this project.

## Architecture

Two projects with a strict dependency direction: `CGRasterization.App` → `CGRasterization.Core`.

**CGRasterization.Core** — pure rasterization logic, no UI or Avalonia dependencies:
- `Primitives/` — `Line`, `Circle`, `Polygon`, each implementing `IShape` (which extends `IDrawable`, `ISelectable`, `IMovable`, `IEditable`). Shapes know how to rasterize themselves by calling `rasterizer.Rasterize(this, buffer)` (visitor-like dispatch).
- `Rasterizers/` — `ShapeRasterizer` is the central dispatcher implementing `IShapeRasterizer`. It holds instances of `LineRasterizer`, `CircleRasterizer`, and `PolygonRasterizer`. `LineRasterizer` switches between `BresenhamRasterizer` (aliased) and `GuptaSproullRasterizer` (anti-aliased) at runtime via `LineRasterizationMode`.
- `Buffers/PixelBuffer` — a flat `byte[]` RGBA pixel array with width/height/stride; the only shared data type between rasterizers and the canvas.
- `Brush/Brush` — builds a circular pixel stamp pattern for a given color + thickness. Thickness is always normalized to odd values (`NormalizeThickness`). `BaseRasterizer.PutPixel` stamps this brush at each rasterized point.

**CGRasterization.App** — Avalonia 12 / .NET 10 desktop app using MVVM (CommunityToolkit.Mvvm):
- `Canvas/Canvas.cs` — owns the `DirectBitmap` (raw pixel buffer), the `ObservableCollection<IShape>`, and `ShapeRasterizer`. Exposes `RedrawShapes()` (full repaint from scratch) and handles incremental add via `ObservableCollection.CollectionChanged`. Toggling `AntiAliasingEnabled` switches the rasterizer mode and redraws.
- `Canvas/Tools/` — stateful tool objects (`DrawLineTool`, `DrawCircleTool`, `DrawPolygonTool`, `SelectShapeTool`, `MoveShapeTool`, `EditShapeTool`) that receive pointer events through `ICanvasTool`. The active tool is selected by `CanvasControlViewModel`.
- `ViewModels/CanvasControlViewModel` — holds `Canvas`, the tool dictionary, and selected-shape state. `MainWindowViewModel` wraps it and exposes save/load/clear commands.
- `Views/Controls/CanvasControl` — code-behind routes Avalonia pointer events to `CurrentTool?.OnPointer*`.
- `Services/CanvasPersistenceService` — serializes/deserializes shapes to `canvas.json` (System.Text.Json, camelCase). Shapes are mapped through DTOs in `Dto/` via `Mappers/`.

**Key rendering flow:**
1. User pointer event → `CanvasControl` → active `ICanvasTool` → calls `CanvasControlViewModel.AddShape` / `Canvas.SetPreviewShape`
2. `Canvas.Shapes` change → `OnCollectionChanged` → `DrawShape` → `IShape.RasterizeWith(ShapeRasterizer, PixelBuffer)`
3. `ShapeRasterizer` dispatches to the concrete rasterizer → `BaseRasterizer.PutPixel` stamps `Brush` into `PixelBuffer.Pixels`
4. `DirectBitmap.UpdateBitmap()` flushes pixels → `Canvas.InvalidateImage()` swaps `ImageSource` to force Avalonia binding refresh
