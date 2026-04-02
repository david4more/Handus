using SFML.System;

namespace Handus;

using SFML.Graphics;

public abstract class Level // abstract class for all levels
{
    protected List<Texture> textures = new();
    protected List<Sprite> sprites = new();
    protected List<IntRect> hitboxes = new();
    protected Sprite background;
    protected Vector2f spawnPoint;
    protected Vector2u dimensions;

    public Level(Vector2u dimensions)
    {
        this.dimensions = dimensions;
    }
    public List<Sprite> GetSprites() => sprites;
    public List<IntRect> GetHitboxes() => hitboxes;
    public Vector2f GetSpawnPoint() => spawnPoint;
    public Sprite GetBackground() => background;
    public void Update(float dt) { }

    protected void GenerateHitbox(Sprite sprite, Vector2f scale)
    {
        var hitbox = new IntRect();
        hitbox.Size = new Vector2i((int)(sprite.TextureRect.Size.X * scale.X), (int)(sprite.TextureRect.Size.Y * scale.Y));
        hitbox.Position = (Vector2i)sprite.Position;
        hitboxes.Add(hitbox);
    }
}