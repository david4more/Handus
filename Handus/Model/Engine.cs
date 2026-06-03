using SFML.System;
using SFML.Window;
namespace Handus;
using SFML.Graphics;

public class Engine
{
    private RenderWindow window;
    public Player player;
    private int currentLevel = 1;
    private bool levelCompleted = false;
    public Level level { get; private set; }
    private Dictionary<string, Texture> textures = new();

    public Engine(RenderWindow window)  // initializes engine, creates player and level
    {
        this.window = window;

        LoadLevel(currentLevel);

        var texture = new Texture(Utils.FilePrefix + "Sprite-Player-Walk.png", new IntRect(new Vector2i(8, 15), new Vector2i(19, 20)));
        var texture2 = new Texture(Utils.FilePrefix + "Sprite-Player-Walk.png", new IntRect(new Vector2i(44, 14), new Vector2i(19, 22)));
        var texture3 = new Texture(Utils.FilePrefix + "Sprite-Player-Jump.png", new IntRect(new Vector2i(44, 14), new Vector2i(19, 21)));
        var texture4 = new Texture(Utils.FilePrefix + "Sprite-Player-Jump.png", new IntRect(new Vector2i(80, 13), new Vector2i(19, 20)));
        var texture5 = new Texture(Utils.FilePrefix + "Sprite-Player-Jump.png", new IntRect(new Vector2i(117, 13), new Vector2i(18, 20)));
        var texture6 = new Texture(Utils.FilePrefix + "Sprite-Player-Jump.png", new IntRect(new Vector2i(153, 13), new Vector2i(19, 21)));
        var texture7 = new Texture(Utils.FilePrefix + "Sprite-Player-Jump.png", new IntRect(new Vector2i(189, 14), new Vector2i(18, 21)));
        var texture8 = new Texture(Utils.FilePrefix + "Sprite-Player-Touch.png", new IntRect(new Vector2i(44, 15), new Vector2i(20, 20)));
        var texture9 = new Texture(Utils.FilePrefix + "Sprite-Player-Touch.png", new IntRect(new Vector2i(80, 15), new Vector2i(25, 20)));
        textures.Add("idle", texture);
        textures.Add("run1", texture);
        textures.Add("run2", texture2);
        textures.Add("jump1", texture);
        textures.Add("jump2", texture3);
        textures.Add("jump3", texture4);
        textures.Add("jump4", texture5);
        textures.Add("jump5", texture6);
        textures.Add("jump6", texture7);
        textures.Add("touch1", texture);
        textures.Add("touch2", texture8);
        textures.Add("touch3", texture9);
        player = new Player(textures, level.GetSpawnPoint(), window);

        ResetPlayerPosition();

        window.SetKeyRepeatEnabled(false);
        window.Closed += (sender, e) => window.Close();
        window.KeyPressed += player.OnKeyPressed;
        window.KeyPressed += (sender, e) => 
        { if (e.Code == Keyboard.Key.Escape) window.Close(); };

    }

    void LoadLevel(int levelNumber)
    {
        switch (levelNumber)
        {
            case 1:
                level = new Level1(window.Size);
                break;

            case 2:
                level = new Level2(window.Size);
                break;
                
            case 3:
                level = new Level3(window.Size);
                break;

            default: 
                level = new Level1(window.Size); 
                break;
        }
    }

    void ResetPlayerPosition()
    {
        if (player == null || level == null)
            return;

        Vector2f spawn = level.GetSpawnPoint();
        float w = player.GetHitbox().Width;
        float h = player.GetHitbox().Height;

        player.PositionX = spawn.X - w / 2f;
        player.PositionY = spawn.Y - h - 5f;
        player.ResetVelocity();
    }

    public void Update(float dt)
    {
        window.DispatchEvents();

        player.Update(dt);
        level.Update(dt);

        foreach (var obj in level.GetObjects()) obj.Update(dt);
        
        UpdateCollisions();

        UpdateTriggers();
        UpdateLeverInteraction();
        UpdateTriggers();

        UpdateLevelCompletion();
        UpdateDeathZones();


        if (levelCompleted)
        {
            currentLevel++;
            levelCompleted = false;

            LoadLevel(currentLevel);
            ResetPlayerPosition();
        }
    }

    void UpdateLevelCompletion()
    {
        var p = player.GetHitbox();

        foreach (var heart in level.GetObjects().Where(o => o.Type == "heart"))
        {
            if (LevelObjects.Intersects(p, heart.Hitbox_obj))
            {
                levelCompleted = true;
                return;
            }
        }
    }

    void UpdateDeathZones()
    {
        var p = player.GetHitbox();

        foreach (var obj in level.GetObjects())
        {
            if (obj.Type != "killzone")
                continue;

            if (LevelObjects.Intersects(p, obj.Hitbox_obj))
            {
                KillPlayer();
                return;
            }
        }

        if (player.PositionY > window.Size.Y + 200)
            KillPlayer();
    }
    void KillPlayer()
    {
        LoadLevel(currentLevel);
        ResetPlayerPosition();
    }

    // Вutton-lever-door connection algorithm
    void UpdateTriggers()
    {
        var objects = level.GetObjects();
        var links = level.GetLinks();

        // Button trigger on the box
        foreach (var btn in objects.Where(o => o.Type == "button"))
        {
            btn.IsActive = objects
                .Where(o => o.Type == "box")
                .Any(b => LevelObjects.Intersects(btn.Hitbox_obj, b.Hitbox_obj));
        }

        // Lever trigger on the button
        foreach (var lever in objects.Where(o => o.Type == "lever"))
        {
            var linkedButtons = links
               .Where(l => l.ReceiverId == lever.Id)
               .Select(l => objects.FirstOrDefault(o => o.Id == l.ActivatorId))
               .Where(o => o != null && o.Type == "button")
               .ToList();

            if (linkedButtons.Count == 0)
                lever.IsEnabled = true;
            else
                lever.IsEnabled = linkedButtons.All(b => b!.IsActive);
        }

        // Door trigger on the lever
        foreach (var door in objects.Where(o => o.Type == "door"))
        {
            var linkedLevers = links
               .Where(l => l.ReceiverId == door.Id)
               .Select(l => objects.FirstOrDefault(o => o.Id == l.ActivatorId))
               .Where(o => o != null && o.Type == "lever")
               .ToList();

            bool shouldOpen = linkedLevers.Count > 0 &&
                  linkedLevers.All(l => l!.IsActive);

            door.IsActive = shouldOpen;

            if (shouldOpen)
                door.IsOpening = true;
        }
    }

    void UpdateLeverInteraction()
    {
        if (!Keyboard.IsKeyPressed(Keyboard.Key.E))
            return;

        var p = player.GetHitbox();

        foreach (var lever in level.GetObjects().Where(o => o.Type == "lever"))
        {
            if (!lever.IsEnabled)
                continue;

            if (LevelObjects.Intersects(p, lever.Hitbox_obj))
            {
                if (!lever.IsActive)
                {
                    player.PlayTouchAnimation();
                    lever.Activate();
                }
            }
        }
    }

    void UpdateCollisions()
    {
        var hitboxes = level.GetHitboxes();

        PlayerVsLevel(hitboxes);
        PlayerVsBoxes();
        BoxesVsLevelAndDoors(hitboxes);
        PlayerVsDoors();
        PlayerVsTrampolines();
        LevelBounds();
    }
    //Helper for intersection collide methods
    private static bool TryGetOverlap(IntRect a, IntRect b, 
                    out float overlapX, out float overlapY)
    {
        bool horizontalOverlap =
            a.Left < b.Left + b.Width &&
            a.Left + a.Width > b.Left;

        bool verticalOverlap =
            a.Top < b.Top + b.Height &&
            a.Top + a.Height > b.Top;

        if (!horizontalOverlap || !verticalOverlap)
        {
            overlapX = 0;
            overlapY = 0;
            return false;
        }

        overlapX =
            Math.Min(a.Left + a.Width, b.Left + b.Width) -
            Math.Max(a.Left, b.Left);

        overlapY =
            Math.Min(a.Top + a.Height, b.Top + b.Height) -
            Math.Max(a.Top, b.Top);

        return true;
    }

    //Player collision with platforms
    private void PlayerVsLevel(List<IntRect> hitboxes)
    {
        var p = player.GetHitbox();

        foreach (var h in hitboxes)
        {
            if (!TryGetOverlap(p, h, out float overlapX, out float overlapY))
                continue;

            if (overlapX < overlapY)
            {
                if (p.Left < h.Left)
                    player.Collide(Direction.Right, overlapX);
                else
                    player.Collide(Direction.Left, overlapX);
            }
            else
            {
                if (p.Top < h.Top)
                    player.Collide(Direction.Down, overlapY);
                else
                    player.Collide(Direction.Up, overlapY);
            }

            p = player.GetHitbox();
        }
    }

    private void PlayerVsBoxes()
    {
        var p = player.GetHitbox();

        foreach (var obj in level.GetObjects().Where(o => o.Type == "box"))
        {
            var b = obj.Hitbox_obj;

            if (!TryGetOverlap(p, b, out float overlapX, out float overlapY))
                continue;

            if (overlapX < overlapY)
            {
                float pushForce = 200f;

                if (p.Left < b.Left)
                {
                    obj.Velocity.X = pushForce;
                    player.PositionX -= overlapX * 0.5f;
                }
                else
                {
                    obj.Velocity.X = -pushForce;
                    player.PositionX += overlapX * 0.5f;
                }
            }
            else
            {
                if (p.Top < b.Top)
                    player.Collide(Direction.Down, overlapY);
                else
                    player.Collide(Direction.Up, overlapY);
            }

            p = player.GetHitbox();
        }
    }

    //Collision of boxes with platforms and doors
    private void BoxesVsLevelAndDoors(List<IntRect> hitboxes)
    {
        var objects = level.GetObjects();
        var doors = objects.Where(o => o.Type == "door" && !o.IsOpening).ToList();

        foreach (var obj in objects.Where(o => o.Type == "box"))
        {
            var b = obj.Hitbox_obj;

            obj.IsOnGround = false;

            foreach (var h in hitboxes)
            {
                if (b.Top + b.Height == h.Top ||
                    b.Top + b.Height == h.Top + 1)
                {
                    if (b.Left < h.Left + h.Width &&
                        b.Left + b.Width > h.Left)
                    {
                        obj.IsOnGround = true;
                    }
                }
            }

            // with platforms
            foreach (var h in hitboxes)
            {
                if (!TryGetOverlap(b, h, out float overlapX, out float overlapY))
                    continue;

                if (overlapX < overlapY)
                {
                    if (b.Left < h.Left)
                        obj.Move(new Vector2f(-overlapX, 0));
                    else
                        obj.Move(new Vector2f(overlapX, 0));

                    obj.Velocity.X = 0;
                }
                else
                {
                    if (b.Top < h.Top)
                    {
                        obj.Move(new Vector2f(0, -overlapY));
                        obj.Velocity.Y = 0;
                        obj.IsOnGround = true;
                    }
                    else
                    {
                        obj.Move(new Vector2f(0, overlapY));
                        obj.Velocity.Y = 0;
                    }
                }

                b = obj.Hitbox_obj;
            }

            // with doors
            foreach (var door in doors)
            {
                b = obj.Hitbox_obj;

                if (!TryGetOverlap(b, door.Hitbox_obj, out float overlapX, out float overlapY))
                    continue;

                if (overlapX < overlapY)
                {
                    if (b.Left < door.Hitbox_obj.Left)
                        obj.Move(new Vector2f(-overlapX, 0));
                    else
                        obj.Move(new Vector2f(overlapX, 0));

                    obj.Velocity.X = 0;
                }
                else
                {
                    if (b.Top < door.Hitbox_obj.Top)
                    {
                        obj.Move(new Vector2f(0, -overlapY));
                        obj.Velocity.Y = 0;
                        obj.IsOnGround = true;
                    }
                    else
                    {
                        obj.Move(new Vector2f(0, overlapY));
                        obj.Velocity.Y = 0;
                    }
                }

                b = obj.Hitbox_obj;
            }
        }
    }

    private void PlayerVsDoors() //Player collision with door
    {
        var p = player.GetHitbox();

        foreach (var door in level.GetObjects().Where(o => o.Type == "door"))
        {
            if (door.IsOpening)
                continue;

            if (!TryGetOverlap(p, door.Hitbox_obj, out float overlapX, out float overlapY))
                continue;

            if (overlapX < overlapY)
            {
                if (p.Left < door.Hitbox_obj.Left)
                    player.Collide(Direction.Right, overlapX);
                else
                    player.Collide(Direction.Left, overlapX);
            }
            else
            {
                if (p.Top < door.Hitbox_obj.Top)
                    player.Collide(Direction.Down, overlapY);
                else
                    player.Collide(Direction.Up, overlapY);
            }

            p = player.GetHitbox();
        }
    }

    private void PlayerVsTrampolines() // Trampoline collision resolution
    {
        var p = player.GetHitbox();

        foreach (var tramp in level.GetObjects().Where(o => o.Type == "trampoline"))
        {
            if (!TryGetOverlap(p, tramp.Hitbox_obj, out float overlapX, out float overlapY))
                continue;

            if (overlapY < overlapX && p.Top < tramp.Hitbox_obj.Top)
            {
                player.PositionY -= overlapY;
                player.ApplyBounce(-1200f);
                p = player.GetHitbox();
            }
        }
    }
    private void LevelBounds() //Level limits
    {
        var p = player.GetHitbox();

        if (p.Left < 0)
            player.PositionX += -p.Left;

        if (p.Left + p.Width > window.Size.X)
            player.PositionX -= (p.Left + p.Width - window.Size.X);

        if (p.Top < 0)
            player.PositionY = 0;
    }

    public void Render()   // draws all sprites
    {
        foreach (var s in level.GetSprites()) window.Draw(s);
        foreach (var obj in level.GetObjects()) window.Draw(obj.Sprite_obj);
        window.Draw(player.GetSprite());
    }
    
    //public void Loop()  // launches the game's loop
    //{
    //    var clock = new Clock();
        
        
    //    while (window.IsOpen)   // TODO: delta time
    //    {
    //        float dt = clock.Restart().AsSeconds();
            
    //        Update(dt);
    //        Render();
    //    }
    //}
}

public class Utils
{
    public static string FilePrefix => "../../../Files/"; 
}

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
};
