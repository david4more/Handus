using SFML.Window;
using SFML.System;
using SFML.Graphics;
namespace Handus;

public class Player
{
    private Sprite Sprite;
    private Dictionary<string, Texture> Textures;
    private IntRect Hitbox;
    private float Scale;
    private const float Gravity = 1000.0f;
    private const float JumpSpeed = 1000.0f;
    private const float JumpSpeedMultiplier = 2.5f;
    private const float BumpingDumpingCoeff = 0.9f;

    private Vector2f Velocity;
    private bool IsOnGround = false;
    private float knockbackTimer = 0f;

    public void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Space && IsOnGround)
        {
            Velocity.Y = -JumpSpeed;
            Velocity.X *= JumpSpeedMultiplier;
            IsOnGround = false;
        }
    }

    public void ApplyBounce(float force)
    {
        Velocity.Y = force; // repulsive force
    }

    private float Lerp(float a, float b, float t) => a + (b - a) * t;

    public void Update(float dt)
    {
        float move = 0f;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= dt;
            // During the rebound, the speed is smoothly slowed down by the air
            Velocity.X = Lerp(Velocity.X, 0f, 4f * dt);
        }
        else
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))move -= 1f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) move += 1f;

            float targetSpeed = move * 370f;
            float acceleration = 10f;

            Velocity.X = Lerp(Velocity.X, targetSpeed, acceleration * dt);
        }

        if (!IsOnGround)
            Velocity.Y += Gravity * dt;

        Move(Velocity * dt);

        IsOnGround = false;
    }

    public Player(Dictionary<string, Texture> textures, Vector2f spawnPoint, RenderWindow window)    // initializes player with a map of textures
    {
        this.Textures = textures;
        Sprite = new Sprite(textures["idle1"]);
        float scaleX = window.Size.X / 2560f;
        float scaleY = window.Size.Y / 1440f;
        Scale = MathF.Min(scaleX, scaleY);
        Sprite.Scale = new Vector2f(0.2f * Scale, 0.2f * Scale);
        Hitbox.Size = new Vector2i((int)(Sprite.GetGlobalBounds().Width), (int)(Sprite.GetGlobalBounds().Height));
        SetMiddleBottom(spawnPoint);
    }

    public void Collide(Direction direction, float penetration = 0)
    {
        if (direction == Direction.None) return;

        if (direction == Direction.Down)
        {
            Move(new Vector2f(0, -penetration + 1));
            Velocity.Y = 0;
            IsOnGround = true;
            return;
        }

        switch (direction)
        {
            case Direction.Up:
                Move(new Vector2f(0, penetration));
                Velocity.Y *= -BumpingDumpingCoeff;
                break;
            case Direction.Left:
                Move(new Vector2f(penetration + 1f, 0));
                Velocity.X = 550f;
                knockbackTimer = 0.8f;
                break;
            case Direction.Right:
                Move(new Vector2f(-penetration - 1f, 0));
                Velocity.X = -550f;
                knockbackTimer = 0.8f;
                break;
        }
    }

    void Move(Vector2f direction)
    {
        Sprite.Position += direction;
        Hitbox.Position = (Vector2i)Sprite.Position;
    }

    void SetMiddleBottom(Vector2f position) // sets player's middle bottom to passed coords
    {
        var size = Hitbox.Size;
        Sprite.Position = new Vector2f(position.X - size.X / 2.0f, position.Y - size.Y - 5);
        Hitbox.Position = (Vector2i)Sprite.Position;
    }

    public void ResetVelocity()
    {
        Velocity = new Vector2f(0, 0);
    }

    public Sprite GetSprite() => Sprite;
    public IntRect GetHitbox() => Hitbox;
    public float PositionX
    {
        get => Sprite.Position.X;
        set
        {
            var pos = Sprite.Position;
            pos.X = value;
            Sprite.Position = pos;
            Hitbox.Position = (Vector2i)Sprite.Position;
        }
    }

    public float PositionY
    {
        get => Sprite.Position.Y;
        set
        {
            var pos = Sprite.Position;
            pos.Y = value;
            Sprite.Position = pos;
            Hitbox.Position = (Vector2i)Sprite.Position;
        }
    }
}