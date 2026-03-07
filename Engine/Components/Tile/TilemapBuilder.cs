using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components.Tile;

/// <summary>
/// Fluent API builder for creating tilemaps easily.
/// Integrates with existing GameObject/Component system.
/// </summary>
public class TilemapBuilder(GameObject gameObject)
{
    private readonly Tilemap _tilemap = gameObject.AddComponent<Tilemap>();

    /// <summary>
    /// Set the tileset from a texture atlas
    /// </summary>
    public TilemapBuilder WithTileset(Texture2D texture, int tileWidth, int tileHeight, int[] solidTiles = null)
    {
        _tilemap.CreateTileset(texture, tileWidth, tileHeight, solidTiles);
        return this;
    }

    /// <summary>
    /// Add a layer with 1D tile data (row-major order)
    /// </summary>
    public TilemapBuilder AddLayer(string name, int width, int height, int[] tileData, float layerDepth = 0f)
    {
        var layer = _tilemap.AddLayer(name, width, height);
        layer.SetTileData(tileData);
        layer.LayerDepth = layerDepth;
        return this;
    }

    /// <summary>
    /// Add a layer with 2D tile data
    /// </summary>
    public TilemapBuilder AddLayer(string name, int width, int height, int[,] tileData, float layerDepth = 0f)
    {
        var layer = _tilemap.AddLayer(name, width, height);
        layer.SetTileData(tileData);
        layer.LayerDepth = layerDepth;
        return this;
    }

    /// <summary>
    /// Build and return the completed tilemap
    /// </summary>
    public Tilemap Build()
    {
        return _tilemap;
    }

    // ====================================================================
    // Static helper methods for common level patterns
    // ====================================================================

    /// <summary>
    /// Create an empty layer filled with -1 (no tiles)
    /// </summary>
    public static int[] CreateEmptyLayer(int width, int height)
    {
        int[] data = new int[width * height];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = -1;
        }
        return data;
    }

    /// <summary>
    /// Create a ground layer with solid tiles at the bottom
    /// </summary>
    public static int[] CreateGroundLayer(int width, int height, int groundTileId, int groundHeight)
    {
        int[] data = CreateEmptyLayer(width, height);

        // Fill bottom rows with ground
        for (int y = height - groundHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y * width + x] = groundTileId;
            }
        }

        return data;
    }

    /// <summary>
    /// Create platforms at specific positions
    /// </summary>
    public static int[] CreatePlatformLayer(int width, int height, int platformTileId, params (int x, int y, int length)[] platforms)
    {
        int[] data = CreateEmptyLayer(width, height);

        foreach (var (x, y, length) in platforms)
        {
            for (int i = 0; i < length; i++)
            {
                if (x + i < width && y < height && y >= 0)
                {
                    data[y * width + (x + i)] = platformTileId;
                }
            }
        }

        return data;
    }

    /// <summary>
    /// Fill a rectangular area with a specific tile
    /// </summary>
    public static void FillRect(int[] data, int width, int height, int tileId, int startX, int startY, int rectWidth, int rectHeight)
    {
        for (int y = startY; y < startY + rectHeight && y < height; y++)
        {
            for (int x = startX; x < startX + rectWidth && x < width; x++)
            {
                if (x >= 0 && y >= 0)
                {
                    data[y * width + x] = tileId;
                }
            }
        }
    }
}
