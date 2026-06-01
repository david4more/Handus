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

        // Platform textures lvl1

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-poll.png", 
            new IntRect(new Vector2i(0, 13), new Vector2i(35, 6)))); // idx 0

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-poll.png",
            new IntRect(new Vector2i(0, 13), new Vector2i(35, 6))));  // idx 1

        // Objects textures lvl1
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-Boxt.png",// idx 2
            new IntRect(new Vector2i(12, 27), new Vector2i(8, 8))));

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-Playbutton.png",// idx 3
            new IntRect(new Vector2i(0, 0), new Vector2i(10, 3))));

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-Ruchag.png",// idx 4
            new IntRect(new Vector2i(14, 8), new Vector2i(14, 11))));

        textures.Add(new Texture(Utils.FilePrefix + "doorlv1.png",// idx 5
            new IntRect(new Vector2i(0, 0), new Vector2i(3, 35))));

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-hart.png",// idx 6
              new IntRect(new Vector2i(2, 2), new Vector2i(31, 27))));

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-KillPikk.png",// idx 7
           new IntRect(new Vector2i(49, 9), new Vector2i(31, 23))));
        //lvl 2 trampoline
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-battyt.png",// idx 8
           new IntRect(new Vector2i(50, 9), new Vector2i(33, 5))));

        // Platform textures lvl2d
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-poll.png",
            new IntRect(new Vector2i(47, 13), new Vector2i(35, 6))));
        // Objects textures lvl2

        // Platform textures lvl3

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-poll.png",
            new IntRect(new Vector2i(96, 13), new Vector2i(35, 6))));// idx 10

        // Objects textures lvl3
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-Boxt.png",// idx 11
           new IntRect(new Vector2i(84, 27), new Vector2i(8, 8))));

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-Playbutton.png",// idx 12
            new IntRect(new Vector2i(10, 0), new Vector2i(10, 3))));

        //lv2 door
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-door.png",// idx 13
            new IntRect(new Vector2i(19, 0), new Vector2i(3, 35))));
        //lv3 door
        textures.Add(new Texture(Utils.FilePrefix + "Sprite-door.png",// idx 14
            new IntRect(new Vector2i(35, 0), new Vector2i(3, 35))));

        textures.Add(new Texture(Utils.FilePrefix + "Sprite-KillPikk.png",// idx 15
           new IntRect(new Vector2i(97, 9), new Vector2i(31, 23))));

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
