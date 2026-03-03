using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components.Tile;

/// <summary>
/// Tilemap component for rendering and managing tile-based levels
/// </summary>
public class Tilemap : Component
{
    private readonly List<TileLayer> _layers = [];
    private Tile[] _tileset;

    public int TileWidth { get; set; } = 32;
    public int TileHeight { get; set; } = 32;
    
    public void CreateTileset(Texture2D texture, int tileWidth, int tileHeight, int[] solidTiles = null)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;

        int tilesPerRow = texture.Width / tileWidth;
        int tilesPerColumn = texture.Height / tileHeight;
        int totalTiles = tilesPerRow * tilesPerColumn;

        _tileset = new Tile[totalTiles];

        for (int i = 0; i < totalTiles; i++)
        {
            int x = i % tilesPerRow * tileWidth;
            int y = i / tilesPerRow * tileHeight;

            bool isSolid = solidTiles?.Contains(i) ?? false;

            _tileset[i] = new Tile(
                i,
                texture,
                new Rectangle(x, y, tileWidth, tileHeight),
                isSolid
            );
        }
    }

    public void CreateTilesetFromTextures(Texture2D[] textures, bool[] solidFlags = null)
    {
        _tileset = new Tile[textures.Length];

        for (int i = 0; i < textures.Length; i++)
        {
            bool isSolid = solidFlags?[i] ?? false;
            _tileset[i] = new Tile(
                i,
                textures[i],
                new Rectangle(0, 0, textures[i].Width, textures[i].Height),
                isSolid
            );
        }
    }

    public TileLayer AddLayer(string name, int width, int height)
    {
        var layer = new TileLayer(name, width, height, TileWidth, TileHeight);
        layer.SetTileset(_tileset);
        _layers.Add(layer);
        return layer;
    }

    public TileLayer GetLayer(string name)
    {
        return _layers.FirstOrDefault(l => l.Name == name);
    }

    public TileLayer GetLayer(int index)
    {
        if (index >= 0 && index < _layers.Count)
            return _layers[index];
        return null;
    }

    public void RemoveLayer(string name)
    {
        _layers.RemoveAll(l => l.Name == name);
    }

    public bool CheckCollision(Vector2 worldPosition, string layerName = null)
    {
        if (layerName != null)
        {
            var layer = GetLayer(layerName);
            return layer?.IsTileSolidAt(worldPosition) ?? false;
        }

        // Check all layers
        foreach (var layer in _layers)
        {
            if (layer.IsTileSolidAt(worldPosition))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check collision with a rectangle. Optimized to check corners and center.
    /// </summary>
    public bool CheckCollisionRect(Rectangle rect, string layerName = null)
    {
        // Check corners and center point for better accuracy
        Vector2[] checkPoints =
        [
            new Vector2(rect.Left, rect.Top),         // Top-left
            new Vector2(rect.Right, rect.Top),        // Top-right
            new Vector2(rect.Left, rect.Bottom),      // Bottom-left
            new Vector2(rect.Right, rect.Bottom),     // Bottom-right
            new Vector2(rect.Center.X, rect.Center.Y) // Center
        ];

        foreach (var point in checkPoints)
        {
            if (CheckCollision(point, layerName))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Get all solid tiles that intersect with a rectangle.
    /// Useful for precise collision response.
    /// </summary>
    public List<Rectangle> GetCollidingTiles(Rectangle rect, string layerName = null)
    {
        List<Rectangle> collidingTiles = new();
        
        var layers = layerName != null 
            ? new[] { GetLayer(layerName) }.Where(l => l != null) 
            : _layers;

        foreach (var layer in layers)
        {
            // Calculate tile range to check (optimization)
            int startX = Math.Max(0, rect.Left / TileWidth);
            int endX = Math.Min(layer.Width - 1, rect.Right / TileWidth);
            int startY = Math.Max(0, rect.Top / TileHeight);
            int endY = Math.Min(layer.Height - 1, rect.Bottom / TileHeight);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    var tile = layer.GetTileObject(x, y);
                    if (tile?.IsSolid ?? false)
                    {
                        // Use layer helper method for consistency
                        collidingTiles.Add(layer.GetTileBounds(x, y));
                    }
                }
            }
        }

        return collidingTiles;
    }

    /// <summary>
    /// Get all layers in this tilemap
    /// </summary>
    public IReadOnlyList<TileLayer> GetAllLayers()
    {
        return _layers.AsReadOnly();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 offset = GameObject?.Position ?? Vector2.Zero;

        foreach (var layer in _layers)
        {
            layer.Draw(spriteBatch, offset);
        }
    }
}
