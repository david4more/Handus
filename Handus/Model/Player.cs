using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
namespace Handus;

public class Player
{
    float scaleValue = 9.5f;
    float scaleX;
    float scaleY;

    private float animationTimer = 0f;
    private int currentFrame = 1;

    private Sprite Sprite;
    private Dictionary<string, Texture> Textures;
    private IntRect Hitbox;
    private const float Gravity = 1000.0f;
    private const float JumpSpeed = 1000.0f;
    private const float JumpSpeedMultiplier = 2.5f;
    private const float BumpingDumpingCoeff = 0.9f;

    private Vector2f Velocity;
    private bool IsOnGround = false;
    private float knockbackTimer = 0f;

    private bool facingRight = true;
    private bool canMove = true;

    private int frameCounter = 0;
    private enum AnimationState
    {
        Idle,
        Run,
        Touch,
        Jump
    }
    AnimationState animState = AnimationState.Idle;

    public void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        if (!canMove)
            return;
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
        if (canMove)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                move -= 1f;
                facingRight = false;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                move += 1f;
                facingRight = true;
            }
        }

        if (animState != AnimationState.Touch)
        {
            if (!IsOnGround)
            {
                if (Velocity.Y < -50f)
                    animState = AnimationState.Jump;   // вгору
                else if (Velocity.Y > 50f)
                    animState = AnimationState.Jump;   // вниз
                else
                    animState = AnimationState.Jump;   // вершина
            }
            else if (Math.Abs(move) > 0)
            {
                animState = AnimationState.Run;
            }
            else
            {
                animState = AnimationState.Idle;
            }
        }

            if (facingRight)
        {
            Sprite.Scale = new Vector2f(scaleValue * scaleX, scaleValue * scaleY);
        }
        else
        {
            Sprite.Scale = new Vector2f(-scaleValue * scaleX, scaleValue * scaleY);
        }

        Hitbox.Size = new Vector2i((int)(Sprite.GetGlobalBounds().Width), (int)(Sprite.GetGlobalBounds().Height));

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

        Animate(dt);
    }

    public Player(Dictionary<string, Texture> textures, Vector2f spawnPoint, RenderWindow window)    // initializes player with a map of textures
    {
        this.Textures = textures;
        Sprite = new Sprite(textures["idle"]);
        scaleX = window.Size.X / 2560f;
        scaleY = window.Size.Y / 1440f;
        Sprite.Origin = new Vector2f(
        Sprite.TextureRect.Size.X / 2f, 0);
        //Sprite.Scale = new Vector2f(scaleValue * scaleX, scaleValue * scaleY);
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
        var bounds = Sprite.GetGlobalBounds();
        Hitbox.Position = new Vector2i((int)bounds.Left, (int)bounds.Top);
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

    public void PlayTouchAnimation()
    {
        canMove = false;
        animState = AnimationState.Touch;
    }
    private void Animate(float dt)
    {
        animationTimer += dt;
        switch (animState) {
            case AnimationState.Run:
                {
                    if (animationTimer >= 0.15f)
                    {
                        animationTimer = 0f;

                        currentFrame++;

                        if (currentFrame > 2)
                            currentFrame = 1;

                        Sprite.Texture = Textures[$"run{currentFrame}"];
                    }
                    break;
                }
            case AnimationState.Touch:
                {
                    if (animationTimer >= 0.15f)
                    {
                        animationTimer = 0f;

                        currentFrame++;
                        frameCounter++;
                        if (frameCounter > 3)
                        {
                            frameCounter = 0;
                            currentFrame = 1;
                            canMove = true;
                            animState = AnimationState.Idle;
                            return;
                        }
                        if (currentFrame > 3)
                            currentFrame = 1;
                        

                        Sprite.Texture = Textures[$"touch{currentFrame}"];
                    }
                    break;
                }
            case AnimationState.Idle:
                {
                    Sprite.Texture = Textures[$"idle"];
                    break;
                }
            case AnimationState.Jump:
                {
                    if (Velocity.Y < -400f)
                    {
                        Sprite.Texture = Textures["jump1"];
                    }
                    else if (Velocity.Y < -250f)
                    {
                        Sprite.Texture = Textures["jump2"];
                    }
                    else if (Velocity.Y < -100f)
                    {
                        Sprite.Texture = Textures["jump3"];
                    }
                    else if (Velocity.Y > 400f)
                    {
                        Sprite.Texture = Textures["jump6"];
                    }
                    else if (Velocity.Y > 250f)
                    {
                        Sprite.Texture = Textures["jump5"];
                    }
                    else if (Velocity.Y > 100f)
                    {
                        Sprite.Texture = Textures["jump4"];
                    }
                    else
                    {
                        Sprite.Texture = Textures["jump3"];
                    }

                    break;
                }
        }
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