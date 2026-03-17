using SFML.System;

namespace Handus;

using SFML.Graphics;

public abstract class Level // abstract class for all levels
{
    protected List<Texture> textures = new();
    protected List<Sprite> sprites = new();
    protected List<FloatRect> hitboxes = new();
    protected Sprite background;
    protected Vector2f spawnPoint;
    protected Vector2u dimensions;

    public Level(Vector2u dimensions)
    {
        this.dimensions = dimensions;
    }
    public List<Sprite> GetSprites() => sprites;
    public Vector2f GetSpawnPoint() => spawnPoint;
    public Sprite GetBackground() => background;
    public void Update() { }
}