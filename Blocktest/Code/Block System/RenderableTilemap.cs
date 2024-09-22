using Blocktest.Code.Rendering;
using Blocktest.Rendering;
using Shared.Code;
using Shared.Code.Block_System;
using System.Diagnostics;
namespace Blocktest.Block_System;

public class RenderableTilemap {
    /// <summary>
    ///     A list of <see cref="Vector2Int" />s that specify which blocks should be refreshed when a tile is placed/destroyed.
    ///     Defaults to the changed block and all cardinal directions.
    /// </summary>
    private static readonly List<Vector2Int> Adjacencies = new()
        { Vector2Int.Up, Vector2Int.Down, Vector2Int.Left, Vector2Int.Right };

    private readonly Camera _camera;
    private readonly HashSet<Renderable> _cameraRenderables;
    //private readonly RenderableContainer _cameraRenderables;

    private readonly RenderableTile[,] _renderables;
    private readonly TilemapShared _tilemap;
    //private readonly bool _isForeground;


    public RenderableTilemap(TilemapShared newTilemap, Camera camera, bool isForeground) {
        _tilemap = newTilemap;
        _camera = camera;
        _renderables = new RenderableTile[_tilemap.TilemapSize.X, _tilemap.TilemapSize.Y];

        // determines if this tilemap should draw to the foreground or background blocks
        _cameraRenderables = isForeground ? _camera.RenderableComponents.ForegroundBlocks : _camera.RenderableComponents.BackgroundBlocks;

        UpdateRenderables();
        newTilemap.OnTileChanged += OnTilemapChanged;
    }

    private void OnTilemapChanged(TileShared tile, Vector2Int location) {
        _cameraRenderables.Remove(_renderables[location.X, location.Y].Renderable);


        foreach (Vector2Int dir in Adjacencies) {
            if (!_tilemap.TryGetTile(location + dir, out TileShared? _)) {
                continue;
            }
            _renderables[location.X + dir.X, location.Y + dir.Y].UpdateAdjacencies(location + dir, _tilemap);
        }

        RenderableTile newTile = new(tile, _tilemap.Background);
        _renderables[location.X, location.Y] = newTile;
        newTile.UpdateAdjacencies(location, _tilemap);
        _cameraRenderables.Add(newTile.Renderable);
    }

    private void UpdateRenderables() {
        //if (_cameraRenderables != null) { _cameraRenderables.Clear(); }
        _cameraRenderables.Clear();
        for (int x = 0; x < _tilemap.TilemapSize.X; x++)
        for (int y = 0; y < _tilemap.TilemapSize.Y; y++) {
            if (!_tilemap.TryGetTile(new Vector2Int(x, y), out TileShared? tile)) {
                continue;
            }
            RenderableTile newTile = new(tile, _tilemap.Background);
            _renderables[x, y] = newTile;
            _cameraRenderables.Add(newTile.Renderable);
            newTile.UpdateAdjacencies(new Vector2Int(x, y), _tilemap);
        }
    }
}