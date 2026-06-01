using SFML.Graphics;
using SFML.System;
namespace Handus;

public class LevelObjects
{
    public Sprite Sprite_obj { get; private set; }
    public IntRect Hitbox_obj { get; private set; }
    public string Type { get; private set; }
    public bool IsActive { get; set; } = false;
    public bool IsEnabled { get; set; } = false;
    public string Id { get; set; }

    public bool IsMovable = false;
    public bool IsOnGround = false;
    public bool IsOpening = false;
    private float openedDistance = 0f;
    private const float MaxOpenDistance = 225f;
    private const float Gravity = 1000f;
    public Vector2f Velocity = new Vector2f(0, 0);

    public LevelObjects(string id_obj,Texture texture_obj, Vector2f position_obj, Vector2f scale_obj, string type_obj)
    {
        Id = id_obj;

        Sprite_obj = new Sprite(texture_obj);
        Sprite_obj.Position = position_obj;
        Sprite_obj.Scale = scale_obj;

        var hitbox = new IntRect();

        hitbox.Size = new Vector2i(
            (int)(Sprite_obj.Texture.Size.X * scale_obj.X),
            (int)(Sprite_obj.Texture.Size.Y * scale_obj.Y)
        );

        Hitbox_obj = hitbox;
        Type = type_obj;

        UpdateHitbox();
    }

    public void UpdateHitbox()
    {
        var hitbox = Hitbox_obj;
        hitbox.Position = (Vector2i)Sprite_obj.Position;
        Hitbox_obj = hitbox;
    }

    public void Update(float dt)
    {
        if (IsMovable)
        {
            if (!IsOnGround)
            {
                Velocity.Y += Gravity * dt;
                Velocity.X *= 1f - 0.5f * dt;
            }
            else
            {
                Velocity.X *= 1f - 4f * dt;
            }

            Move(Velocity * dt);
        }

        // Door opening
        if (Type == "door" && IsOpening)
        {
            float speed = 120f * dt;

            if (openedDistance < MaxOpenDistance)
            {
                float remaining = MaxOpenDistance - openedDistance;
                float moveAmount = Math.Min(speed, remaining);

                Move(new Vector2f(0, moveAmount));
                openedDistance += moveAmount;
            }
        }
    }

    public void Move(Vector2f offset)
    {
        Sprite_obj.Position += offset;

        var hitbox = Hitbox_obj;
        hitbox.Position = new Vector2i(
            (int)MathF.Round(Sprite_obj.Position.X),
            (int)MathF.Round(Sprite_obj.Position.Y)
        );

        Hitbox_obj = hitbox;
    }

    public static bool Intersects(IntRect a, IntRect b)
    {
        return a.Left < b.Left + b.Width && a.Left + a.Width > b.Left &&
               a.Top < b.Top + b.Height && a.Top + a.Height > b.Top;
    }
}

