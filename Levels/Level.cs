using SFML.System;

namespace Handus;

using SFML.Graphics;

public abstract class Level
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
    public List<Sprite> getSprites() => sprites;
    public Vector2f getSpawnPoint() => spawnPoint;
    public Sprite getBackground() => background;
}