using SFML.System;

namespace Handus;

public class Level2 : Level
{
    public Level2(Vector2u dimensions) : base(dimensions)
    {
        float scaleValue = 0.5f;
        var width = (float)textures[0].Size.X;
        Vector2f tilescale = new Vector2f(scaleValue, scaleValue);
        float tileWidth = textures[0].Size.X * tilescale.X;
        int tilesCount = (int)Math.Ceiling(dimensions.X / tileWidth);
        Vector2f groundScale = new Vector2f(0.5f, 0.5f);

        float baseWidth = 2560f;
        float baseHeight = 1440f;
        float scaleX = dimensions.X / baseWidth;
        float scaleY = dimensions.Y / baseHeight;

        // Platform 1 (Right, down)
        AddPlatform(
          textures[1],
              new Vector2f(0f, dimensions.Y * 0.55f), //position
              new Vector2f(0.4f * scaleX, 0.5f * scaleY), //size of one tile
              dimensions.X * 0.30f //total width of platform
        );

        // Platform 2 (Right, up)
        AddPlatform(
         textures[1],
             new Vector2f(0f, dimensions.Y * 0.32f), 
             new Vector2f(0.4f * scaleX, 0.5f * scaleY), 
             dimensions.X * 0.15f
       );

        // Platform 3 (Left)
        AddPlatform(
          textures[1],
              new Vector2f(dimensions.X * 0.68f, dimensions.Y * 0.55f), //position
              new Vector2f(0.4f * scaleX, 0.5f * scaleY), 
              dimensions.X * 0.35f 
        );

        // Kill zone (ground)
        AddKillzone(
            "l2_kz1",
            textures[7],
            new Vector2f(0f, dimensions.Y - 40f),
            new Vector2f(0.5f * scaleX, 0.5f * scaleY),
            dimensions.X
        );

        // Lever
        float leverX = dimensions.X * 0.85f;
        float leverY = (dimensions.Y * 0.55f) - (textures[4].Size.Y * 0.1f);

        AddObject(
            "l2_lev1", textures[4],
            new Vector2f(leverX, leverY),
            new Vector2f(0.1f * scaleX, 0.1f * scaleY),
            "lever"
        );
        AddLink("l2_lev1", "l2_door1"); // Lever triggers Door

        // Door
        float doorX = dimensions.X * 0.11f;
        float doorY = (dimensions.Y * 0.55f) - (textures[5].Size.Y * 0.26f);

        AddObject(
            "l2_door1", textures[5],
            new Vector2f(doorX, doorY),
            new Vector2f(0.24f * scaleX, 0.26f * scaleY),
            "door"
        );

        // Heart
        float heartX = dimensions.X * 0.04f;
        float heartY = dimensions.Y * 0.46f;

        AddObject(
            "l2_heart", textures[6],
            new Vector2f(heartX, heartY),
            new Vector2f(0.2f * scaleX, 0.2f * scaleY),
            "heart"
        );

        // Trampoline
        AddObject(
            "l2_trpl1",
            textures[8],
            new Vector2f(dimensions.X * 0.45f, dimensions.Y * 0.65f),
            new Vector2f(0.2f * scaleX, 0.2f * scaleY),
            "trampoline"
        );

        float spawnX = dimensions.X * 0.04f;
        float spawnY = dimensions.Y * 0.32f;

        spawnPoint = new Vector2f(spawnX, spawnY);
    }
}
