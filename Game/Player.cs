using ComputerGameFinal.Engine;
using ComputerGameFinal.Engine.Components;
using ComputerGameFinal.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Collision;

namespace ComputerGameFinal.Game;

public class Player : GameObject
{
    private const float MoveSpeed = 300f;

    private SpriteRenderer _spriteRenderer;

    public override void Initialize()
    {
        _spriteRenderer = AddComponent<SpriteRenderer>();
        _spriteRenderer.LayerDepth = 0.5f;
        _spriteRenderer.Texture = ResourceManager.Instance.GetTexture("bird");
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 speed = Vector2.Zero;

        if (InputManager.Instance.IsKeyDown(Keys.A))
        {
            speed.X = -MoveSpeed;
        }
        if (InputManager.Instance.IsKeyDown(Keys.D))
        {
            speed.X = MoveSpeed;
        }
        if (InputManager.Instance.IsKeyDown(Keys.W))
        {
            speed.Y = -MoveSpeed;
        }
        if (InputManager.Instance.IsKeyDown(Keys.S))
        {
            speed.Y = MoveSpeed;
        }

        Position += speed * dt;
    }
}