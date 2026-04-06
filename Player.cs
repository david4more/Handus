using System.Numerics;
using SFML.Window;

namespace Handus;

using SFML.System;
using SFML.Graphics;

public class Player
{
    private Sprite Sprite;
    private Dictionary<string, Texture> Textures;
    private IntRect Hitbox;
    private const float Scale = 0.2f;
    private const float Speed = 150.0f;
    private const float Dumping = 10.0f;
    private const float MaxSpeed = 300.0f;
    private const float MaxDumpingCoeff = 0.995f;
    private const float Gravity = 1000.0f;
    private const float JumpSpeed = 800.0f;
    private const float JumpSpeedMultiplier = 2.0f;
    private const float BumpingDumpingCoeff = 0.25f;

    private Vector2f Velocity;
    private bool IsOnGround = false;

    public void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Space && IsOnGround)
        {
            Velocity.Y = -JumpSpeed;
            Velocity.X *= JumpSpeedMultiplier;
            IsOnGround = false;
        }
    }

    public void Update(float dt)
    {
        if (!IsOnGround)
        {
            Velocity.Y += Gravity * dt;
            Move(Velocity * dt);
            return;
        }
        
        float move = 0f;
        
        if (Velocity.X > MaxSpeed || Velocity.X < -MaxSpeed) 
            Velocity.X *= MaxDumpingCoeff;
        else
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) move -= 1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) move += 1;
                    
            if (move != 0)
                Velocity.X += move * Speed * dt;
            if (move == 0 || (move < 0 != Velocity.X < 0)) Velocity.X -= Velocity.X * Dumping * dt;
        }
            
        Move(Velocity * dt);

        IsOnGround = false;
    }
    
    public Player(Dictionary<string, Texture> textures, Vector2f spawnPoint)    // initializes player with a map of textures
    {
        this.Textures = textures;
        Sprite = new Sprite(textures["idle1"]);
        Sprite.Scale = new Vector2f(Scale, Scale);
        Hitbox.Size = new Vector2i((int)(Sprite.TextureRect.Size.X * Scale), (int)(Sprite.TextureRect.Size.Y * Scale));
        
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
                Move(new Vector2f(penetration, 0));
                Velocity.X *= -BumpingDumpingCoeff;
                break;
            case Direction.Right:
                Move(new Vector2f(-penetration, 0));
                Velocity.X *= -BumpingDumpingCoeff;
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