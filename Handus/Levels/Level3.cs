using SFML.System;
namespace Handus;

public class Level3 : Level
{

    public Level3(Vector2u dimensions) : base(dimensions)   // initializes level with textures and sprites
    {
        float scaleValue = 9.5f;
        var width = (float)textures[0].Size.X;
        Vector2f tilescale = new Vector2f(scaleValue, scaleValue);
        float tileWidth = textures[0].Size.X * tilescale.X;
        int tilesCount = (int)Math.Ceiling(dimensions.X / tileWidth);
        Vector2f groundScale = new Vector2f(0.5f, 0.5f);

        float baseWidth = 2560f;
        float baseHeight = 1440f;
        float scaleX = dimensions.X / baseWidth;
        float scaleY = dimensions.Y / baseHeight;

        //Platform 1 (Ground)
        AddPlatform(textures[10], new Vector2f(0f, dimensions.Y - 50), tilescale, dimensions.X);

        //Platform 2 (left island with green lever)
        AddPlatform(
             textures[10],
             new Vector2f(0f, dimensions.Y * 0.68f),
             new Vector2f(scaleValue * scaleX, scaleValue * scaleY), 
             dimensions.X * 0.05f
         );

        //Platform 3 (central down island)
        AddPlatform(
              textures[10],
              new Vector2f(dimensions.X * 0.38f, dimensions.Y * 0.68f),
              new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
              dimensions.X * 0.33f
        );

        //Platform 4 (central up island)
        AddPlatform(
             textures[10],
             new Vector2f(dimensions.X * 0.33f, dimensions.Y * 0.44f),
             new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
             dimensions.X * 0.25f
        );

        //Platform 5 (Island with heart)
        AddPlatform(
            textures[10],
            new Vector2f(dimensions.X * 0.82f, dimensions.Y * 0.36f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            dimensions.X * 0.18f
        );

        // Platform 6 (Island upon the heart)
        AddPlatform(
            textures[10],
            new Vector2f(dimensions.X * 0.9f, dimensions.Y * 0.17f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY), 
            dimensions.X * 0.1f
        );

        // Kill zone
        AddKillzone(
            "l3_kz1", textures[7], 
            new Vector2f(dimensions.X * 0.64f, dimensions.Y - 275f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            dimensions.X * 0.36f
        );

        // Box on the platform 2 (blue)
        float box1Y = (dimensions.Y * 0.60f) - (textures[11].Size.Y * 0.1f);
        AddObject(
            "l3_box1", textures[11],
            new Vector2f(dimensions.X * 0.57f, box1Y),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "box"
        );

        // Box on the platform 3 (green)
        float box2Y = (dimensions.Y * 0.36f) - (textures[11].Size.Y * 0.1f);
        AddObject(
            "l3_box2", textures[11],
            new Vector2f(dimensions.X * 0.39f, dimensions.Y * 0.36f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "box"
        );

        //Button(blue)
        float btn1Y = dimensions.Y * 0.66f - (textures[12].Size.Y * 0.15f);
        AddObject(
            "l3_btn1", textures[12],
            new Vector2f(dimensions.X * 0.41f, btn1Y),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "button"
        );
        AddLink("l3_btn1", "l3_lev2"); // Button blue triggers Lever blue

        // Button (green)
        AddObject(
             "l3_btn2", textures[12],
            new Vector2f(dimensions.X * 0.32f, dimensions.Y - 75f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "button"
        );
        AddLink("l3_btn2", "l3_lev1"); // Button green triggers Lever green

        // Lever (green)
        float lev1Y = dimensions.Y * 0.61f - (textures[4].Size.Y * 0.1f);
        AddObject(
            "l3_lev1", textures[4],
            new Vector2f(dimensions.X * 0.04f, lev1Y),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "lever"
        );
        AddLink("l3_lev1", "l3_door1"); // Lever green triggers Door green

        // Lever (blue)
        float lev2Y = dimensions.Y * 0.61f - (textures[4].Size.Y * 0.1f);
        AddObject(
            "l3_lev2", textures[4],
            new Vector2f(dimensions.X * 0.67f, lev2Y),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "lever"
        );
        AddLink("l3_lev2", "l3_door2"); // Lever blue triggers Door blue

        // Door (green)
        float door1Y = (dimensions.Y * 0.46f) - (textures[14].Size.Y * 0.26f);
        AddObject(
            "l3_door1", textures[14],
            new Vector2f(dimensions.X * 0.50f, door1Y),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "door"
        );

        // Door (green + blue)
        float door2Y = (dimensions.Y * 0.175f) - (textures[14].Size.Y * 0.26f);
        AddObject(
            "l3_door2", textures[14],
            new Vector2f(dimensions.X * 0.89f, door2Y),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "door"
        );

        // Heart
        AddObject(
             "l3_heart", textures[6],
             new Vector2f(dimensions.X * 0.92f, dimensions.Y * 0.24f),
             new Vector2f(4.5f * scaleX, 4.5f * scaleY),
             "heart"
        );

        float spawnX = dimensions.X * 0.2f;
        float spawnY = dimensions.Y * 0.85f;

        spawnPoint = new Vector2f(spawnX, spawnY);
    }
}

