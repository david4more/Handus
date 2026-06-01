using SFML.System;
namespace Handus;

public class Level3 : Level
{

    public Level3(Vector2u dimensions) : base(dimensions)   // initializes level with textures and sprites
    {
        float scaleValue = 0.5f;
        var width = (float)textures[0].Size.X;
        Vector2f tilescale = new Vector2f(scaleValue, scaleValue);
        float tileWidth = textures[0].Size.X * tilescale.X;
        int tilesCount = (int)Math.Ceiling(dimensions.X / tileWidth);
        Vector2f groundScale = new Vector2f(0.5f, 0.5f);

        //Platform 1 (Ground)
        AddPlatform(textures[0], new Vector2f(0f, dimensions.Y - 50), tilescale, dimensions.X);

        //Platform 2 (left island with green lever)
        AddPlatform(
             textures[1],
             new Vector2f(0f, dimensions.Y * 0.68f),
             new Vector2f(0.25f, 0.5f), 
             dimensions.X * 0.05f
         );

        //Platform 3 (central down island)
        AddPlatform(
              textures[1],
              new Vector2f(dimensions.X * 0.38f, dimensions.Y * 0.68f),
              new Vector2f(0.4f, 0.5f),
              dimensions.X * 0.33f
        );

        //Platform 4 (central up island)
        AddPlatform(
             textures[1],
             new Vector2f(dimensions.X * 0.33f, dimensions.Y * 0.44f),
             new Vector2f(0.4f, 0.5f),
             dimensions.X * 0.25f
        );

        //Platform 5 (Island with heart)
        AddPlatform(
            textures[1],
            new Vector2f(dimensions.X * 0.82f, dimensions.Y * 0.36f),
            new Vector2f(0.4f, 0.5f),
            dimensions.X * 0.18f
        );

        // Platform 6 (Island upon the heart)
        AddPlatform(
            textures[1],
            new Vector2f(dimensions.X * 0.9f, dimensions.Y * 0.17f),
            new Vector2f(0.4f, 0.2f), 
            dimensions.X * 0.1f
        );

        // Kill zone
        AddKillzone(
            "l3_kz1", textures[7], 
            new Vector2f(dimensions.X * 0.64f, dimensions.Y - 60f),
            new Vector2f(0.5f, 0.5f),
            dimensions.X * 0.36f
        );

        // Box on the platform 2 (blue)
        float box1Y = (dimensions.Y * 0.60f) - (textures[2].Size.Y * 0.1f);
        AddObject(
            "l3_box1", textures[2],
            new Vector2f(dimensions.X * 0.57f, box1Y),
            new Vector2f(0.1f, 0.1f),
            "box"
        );

        // Box on the platform 3 (green)
        float box2Y = (dimensions.Y * 0.36f) - (textures[2].Size.Y * 0.1f);
        AddObject(
            "l3_box2", textures[2],
            new Vector2f(dimensions.X * 0.39f, box2Y),
            new Vector2f(0.1f, 0.1f),
            "box"
        );

        // Button (blue)
        float btn1Y = dimensions.Y * 0.68f - (textures[3].Size.Y * 0.15f);
        AddObject(
            "l3_btn1", textures[3],
            new Vector2f(dimensions.X * 0.40f, btn1Y),
            new Vector2f(0.15f, 0.15f),
            "button"
        );
        AddLink("l3_btn1", "l3_lev2"); // Button blue triggers Lever blue

        // Button (green)
        AddObject(
             "l3_btn2", textures[3],
            new Vector2f(dimensions.X * 0.32f, dimensions.Y - 75f),
            new Vector2f(0.15f, 0.15f),
            "button"
        );
        AddLink("l3_btn2", "l3_lev1"); // Button green triggers Lever green

        // Lever (green)
        float lev1Y = dimensions.Y * 0.68f - (textures[4].Size.Y * 0.1f);
        AddObject(
            "l3_lev1", textures[4],
            new Vector2f(dimensions.X * 0.04f, lev1Y),
            new Vector2f(0.1f, 0.1f),
            "lever"
        );
        AddLink("l3_lev1", "l3_door1"); // Lever green triggers Door green

        // Lever (blue)
        float lev2Y = dimensions.Y * 0.68f - (textures[4].Size.Y * 0.1f);
        AddObject(
            "l3_lev2", textures[4],
            new Vector2f(dimensions.X * 0.67f, lev2Y),
            new Vector2f(0.1f, 0.1f),
            "lever"
        );
        AddLink("l3_lev2", "l3_door2"); // Lever blue triggers Door blue

        // Door (green)
        float door1Y = (dimensions.Y * 0.68f) - (textures[5].Size.Y * 0.26f);
        AddObject(
            "l3_door1", textures[5],
            new Vector2f(dimensions.X * 0.50f, door1Y), 
            new Vector2f(0.24f, 0.26f),
            "door"
        );

        // Door (green + blue)
        float door2Y = (dimensions.Y * 0.36f) - (textures[5].Size.Y * 0.26f);
        AddObject(
            "l3_door2", textures[5],
            new Vector2f(dimensions.X * 0.89f, door2Y),
            new Vector2f(0.24f, 0.26f),
            "door"
        );

        // Heart
        AddObject(
            "l3_heart", textures[6],
            new Vector2f(dimensions.X * 0.94f, dimensions.Y * 0.25f),
            new Vector2f(0.2f, 0.2f),
            "heart"
        );

        float spawnX = dimensions.X * 0.2f;
        float spawnY = dimensions.Y * 0.85f;

        spawnPoint = new Vector2f(spawnX, spawnY);
    }
}

