namespace Handus;

using SFML.System;
using SFML.Graphics;
public class Player
{
    private Sprite sprite;
    private Dictionary<string, Texture> textures;
    private FloatRect hitbox;
    private Vector2f velocity;
    private float scale = 0.3f;

    public void Update()
    {
        // some physics calculations
    }
    
    public Player(Dictionary<string, Texture> textures, Vector2f spawnPoint)    // initializes player with a map of textures
    {
        this.textures = textures;
        sprite = new Sprite(textures["idle1"]);
        sprite.Scale = new Vector2f(scale, scale);
        
        SetPosition(spawnPoint);
    }
    
    void SetPosition(Vector2f position) // sets player's middle bottom to passed coords
    {
        var size = sprite.TextureRect.Size;
        size = new Vector2i((int)(size.X * scale), (int)(size.Y * scale));
        sprite.Position = new Vector2f(position.X - size.X / 2.0f, position.Y - size.Y);
    }
    public Sprite GetSprite() => sprite;
}