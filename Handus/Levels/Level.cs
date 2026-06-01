using SFML.Graphics;
using SFML.System;
using System.Buffers.Text;
using System.Drawing;
namespace Handus;

public abstract class Level // abstract class for all levels
{
    protected List<Texture> textures = new();
    protected List<Sprite> sprites = new();
    protected List<IntRect> hitboxes = new();
    protected List<LevelObjects> objects = new();
    protected List<TriggerLink> links = new();

    protected Sprite background;
    public Vector2f spawnPoint { get; protected set; }
    protected Vector2u dimensions;


    public Level(Vector2u dimensions)
    {
        this.dimensions = dimensions;

        // Platform textures
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-poll.png", 
            new IntRect(new Vector2i(0, 0), new Vector2i(35, 32)))); // idx 0
        textures.Add(new Texture(Utils.FilePrefix + "Island.png")); // idx 1
        // Objects textures 
        textures.Add(new Texture(Utils.FilePrefix + "box.png"));     // idx 2
        textures.Add(new Texture(Utils.FilePrefix + "button.png"));  // idx 3
        textures.Add(new Texture(Utils.FilePrefix + "lever.png"));   // idx 4
        textures.Add(new Texture(Utils.FilePrefix + "door.png"));    // idx 5
        textures.Add(new Texture(Utils.FilePrefix + "heart.png"));   // idx 6
        textures.Add(new Texture(Utils.FilePrefix + "killzone.png")); // idx 7
        textures.Add(new Texture(Utils.FilePrefix + "trampoline.png")); // idx 8
    }

    public List<Sprite> GetSprites() => sprites;
    public List<IntRect> GetHitboxes() => hitboxes;
    public Vector2f GetSpawnPoint() => spawnPoint;
    public Sprite GetBackground() => background;
    public List<LevelObjects> GetObjects() => objects;
    public List<TriggerLink> GetLinks() => links;
    public void Update(float dt) {}

    protected void GenerateHitbox(Sprite sprite, Vector2f scale)
    {
        var hitbox = new IntRect();
        hitbox.Size = new Vector2i((int)(sprite.TextureRect.Size.X * scale.X), (int)(sprite.TextureRect.Size.Y * scale.Y));
        hitbox.Position = (Vector2i)sprite.Position;
        hitboxes.Add(hitbox);
    }

    protected void AddPlatform(Texture texture, Vector2f startPos,
                             Vector2f scale, float totalWidth)
    {

        float tileWidth = texture.Size.X * scale.X;
        int length = (int)Math.Ceiling(totalWidth / tileWidth);

        for (int i = 0; i < length; i++)
        {

            var s = new Sprite(texture);
            s.Scale = scale;

            s.Position = new Vector2f(
                startPos.X + i * tileWidth,
                startPos.Y
            );

            sprites.Add(s);
            GenerateHitbox(s, scale);
        }
    }

    protected void AddObject(string id, Texture texture, Vector2f position,
                              Vector2f scale, string type)
    {
        objects.Add(
            new LevelObjects(
                id, texture, position,
                scale, type
            )
        );

        if (type == "box")
            objects[^1].IsMovable = true;
    }

    protected void AddKillzone(string id, Texture texture, Vector2f startPos,
                                Vector2f scale, float totalWidth)
    {
        float tileWidth = texture.Size.X * scale.X;
        int length = (int)Math.Ceiling(totalWidth / tileWidth);

        for (int i = 0; i < length; i++)
        {
            Vector2f tilePos = new Vector2f(
                startPos.X + i * tileWidth,
                startPos.Y
            );

            string uniqueId = $"{id}_{i}";

            objects.Add(
                new LevelObjects(
                    uniqueId, texture, tilePos,
                    scale, "killzone"
                )
            );
        }
    }


    protected void AddLink(string from, string to)
    {
        links.Add(new TriggerLink(from, to));
    }
}
