using SFML.System;

namespace Handus;

public class Level2 : Level
{
    public Level2(Vector2u dimensions) : base(dimensions)
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

        AddBackground(textures[19], tilescale, dimensions.X);

        // Platform 1 (Right, down)
        AddPlatform(
          textures[9],
              new Vector2f(0f, dimensions.Y * 0.55f), //position
              new Vector2f(scaleValue * scaleX, scaleValue * scaleY), //size of one tile
              dimensions.X * 0.22f //total width of platform
        );

        // Platform 2 (Right, up)
        AddPlatform(
         textures[9],
             new Vector2f(0f, dimensions.Y * 0.32f), 
             new Vector2f(scaleValue * scaleX, scaleValue * scaleY), 
             dimensions.X * 0.11f
       );

        // Platform 3 (Left)
        AddPlatform(
          textures[9],
              new Vector2f(dimensions.X * 0.68f, dimensions.Y * 0.55f), //position
              new Vector2f(scaleValue * scaleX, scaleValue * scaleY), 
              dimensions.X * 0.35f 
        );

        // Kill zone (ground)
        AddKillzone(
            "l2_kz1",
            textures[7],
            new Vector2f(0f, dimensions.Y - 140f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            dimensions.X
        );

        // Lever
        float leverX = dimensions.X * 0.85f;
        float leverY = (dimensions.Y * 0.48f) - (textures[4].Size.Y * 0.1f);

        AddObject(
            "l2_lev1", textures[21],
            new Vector2f(leverX, leverY),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "lever"
        );
        AddLink("l2_lev1", "l2_door1");  // Lever triggers Door

        // Door
        float doorX = dimensions.X * 0.11f;
        float doorY = (dimensions.Y * 0.34f) - (textures[13].Size.Y * 0.26f);

        AddObject(
            "l2_door1", textures[13],
            new Vector2f(doorX, doorY),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "door"
        );

        // Heart
        AddObject(
            "l2_heart", textures[6],
            new Vector2f(dimensions.X * 0.03f, dimensions.Y * 0.42f),
            new Vector2f(4.5f * scaleX, 4.5f * scaleY),
            "heart"
        );

        // Trampoline
        AddObject(
            "l2_trpl1",
            textures[8],
            new Vector2f(dimensions.X * 0.45f, dimensions.Y * 0.65f),
            new Vector2f(6f * scaleX, 6f * scaleY),
            "trampoline"
        );

        float spawnX = dimensions.X * 0.08f;
        float spawnY = dimensions.Y * 0.32f;

        spawnPoint = new Vector2f(spawnX, spawnY);
    }
}
