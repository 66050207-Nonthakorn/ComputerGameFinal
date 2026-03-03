using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components.Tile;

public class Tile(int id, Texture2D texture, Rectangle sourceRectangle, bool isSolid = false)
{
    public int Id { get; set; } = id;
    public Texture2D Texture { get; set; } = texture;
    public Rectangle SourceRectangle { get; set; } = sourceRectangle;
    public bool IsSolid { get; set; } = isSolid;
    public Color Tint { get; set; } = Color.White;
}
