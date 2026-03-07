using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components.Tile;

/// <summary>
/// Represents a layer of tiles. Multiple layers can be combined for depth effects.
/// </summary>
public class TileLayer(string name, int width, int height, int tileWidth, int tileHeight)
{
    public string Name { get; set; } = name;
    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;
    public int TileWidth { get; private set; } = tileWidth;
    public int TileHeight { get; private set; } = tileHeight;
    public bool Visible { get; set; } = true;
    public float Opacity { get; set; } = 1f;
    public float LayerDepth { get; set; } = 0f;
    
    private int[,] _tileData = new int[width, height];
    private Tile[] _tileset;

    /// <summary>
    /// Get the bounds of this layer in world space
    /// </summary>
    public Rectangle GetBounds(Vector2 offset = default)
    {
        return new Rectangle(
            (int)offset.X,
            (int)offset.Y,
            Width * TileWidth,
            Height * TileHeight
        );
    }

    public void SetTileset(Tile[] tileset)
    {
        _tileset = tileset;
    }

    public void SetTileData(int[,] data)
    {
        if (data.GetLength(0) != Width || data.GetLength(1) != Height)
        {
            throw new ArgumentException(
                $"Tile data dimensions must match layer dimensions. " +
                $"Expected: ({Width}, {Height}), Got: ({data.GetLength(0)}, {data.GetLength(1)})");
        }
        _tileData = data;
    }

    public void SetTileData(int[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Tile data cannot be null");
        }

        int expectedLength = Width * Height;
        if (data.Length != expectedLength)
        {
            throw new ArgumentException(
                $"Tile data length must match width * height. " +
                $"Expected: {expectedLength} (Width={Width} * Height={Height}), " +
                $"Got: {data.Length}");
        }

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _tileData[x, y] = data[y * Width + x];
            }
        }
    }

    public int GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return -1;
        return _tileData[x, y];
    }

    public void SetTile(int x, int y, int tileId)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            _tileData[x, y] = tileId;
        }
    }

    public Tile GetTileObject(int x, int y)
    {
        int tileId = GetTile(x, y);
        if (tileId < 0 || tileId >= _tileset?.Length)
            return null;
        return _tileset[tileId];
    }

    public bool IsTileSolidAt(Vector2 worldPosition)
    {
        int tileX = (int)(worldPosition.X / TileWidth);
        int tileY = (int)(worldPosition.Y / TileHeight);
        
        Tile tile = GetTileObject(tileX, tileY);
        return tile?.IsSolid ?? false;
    }

    /// <summary>
    /// Convert world position to tile coordinates
    /// </summary>
    public Point WorldToTile(Vector2 worldPosition)
    {
        return new Point(
            (int)(worldPosition.X / TileWidth),
            (int)(worldPosition.Y / TileHeight)
        );
    }

    /// <summary>
    /// Convert tile coordinates to world position (top-left corner)
    /// </summary>
    public Vector2 TileToWorld(int tileX, int tileY)
    {
        return new Vector2(tileX * TileWidth, tileY * TileHeight);
    }

    /// <summary>
    /// Get the rectangle bounds of a specific tile
    /// </summary>
    public Rectangle GetTileBounds(int tileX, int tileY)
    {
        return new Rectangle(
            tileX * TileWidth,
            tileY * TileHeight,
            TileWidth,
            TileHeight
        );
    }

    /// <summary>
    /// Check if tile coordinates are within bounds
    /// </summary>
    public bool IsValidTile(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    /// <summary>
    /// Draw the tile layer. Uses similar pattern to SpriteRenderer.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 offset = default)
    {
        if (!Visible || _tileset == null)
            return;

        // Optimize: Only draw visible tiles would go here (viewport culling)
        // For now, draw all tiles
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                DrawTile(spriteBatch, x, y, offset);
            }
        }
    }

    /// <summary>
    /// Draw a single tile at the specified tile coordinates
    /// </summary>
    private void DrawTile(SpriteBatch spriteBatch, int tileX, int tileY, Vector2 offset)
    {
        int tileId = _tileData[tileX, tileY];
        if (tileId < 0 || tileId >= _tileset.Length)
            return;

        Tile tile = _tileset[tileId];
        if (tile?.Texture == null)
            return;

        // Calculate position using helper method
        Vector2 position = TileToWorld(tileX, tileY) + offset;
        Color tintColor = tile.Tint * Opacity;

        // Draw using same pattern as SpriteRenderer
        spriteBatch.Draw(
            tile.Texture,
            position,
            tile.SourceRectangle,
            tintColor,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            LayerDepth
        );
    }
}
