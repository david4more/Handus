using SFML.System;
namespace Handus;

public class Level1 : Level
{
    
    public Level1(Vector2u dimensions) : base(dimensions)   // initializes level with textures and sprites
    {

        float scaleValue = 9.5f;
        var width = (float)textures[0].Size.X;
        Vector2f tilescale = new Vector2f(scaleValue, scaleValue);
        float tileWidth = textures[0].Size.X * tilescale.X;
        int tilesCount = (int)Math.Ceiling(dimensions.X / tileWidth);
        Vector2f groundScale = new Vector2f(0.5f, 0.5f);
        float groundY = dimensions.Y - (textures[0].Size.Y * tilescale.Y);

        float baseWidth = 2560f;
        float baseHeight = 1440f;
        float scaleX = dimensions.X / baseWidth;
        float scaleY = dimensions.Y / baseHeight;

        //Platform 1 (Ground)
        AddPlatform(textures[0], new Vector2f(0f, dimensions.Y - 50), tilescale, dimensions.X);


        //Platform 2 (Island with box)
        AddPlatform(
          textures[0],
              new Vector2f(dimensions.X * 0.65f, dimensions.Y * 0.75f), //position
              new Vector2f(scaleValue * scaleX, scaleValue * scaleY), //scale
              dimensions.X * 0.65f  // physical hitbox length
        );

        //Platform 3 (Island with lever)
        AddPlatform(
          textures[0],
              new Vector2f(0f, dimensions.Y * 0.50f),
              new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
              dimensions.X * 0.60f 
        );

        //Platform 4 (Island with heart)
        AddPlatform(
          textures[0],
            new Vector2f(dimensions.X * 0.20f, dimensions.Y * 0.25f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            dimensions.X * 0.60f 
        );

        // Button
        AddObject(
             "l1_btn1", textures[3],
            new Vector2f(dimensions.X * 0.35f, dimensions.Y * 0.948f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "button"
        );
        AddLink("l1_btn1", "l1_lev1"); // Button triggers Lever

        // Box 
        AddObject(
            "l1_box1", textures[2],
            new Vector2f(dimensions.X * 0.8f, dimensions.Y * 0.69f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "box"
        );


        // Lever
        AddObject(
              "l1_lev1", textures[4],
             new Vector2f(dimensions.X * 0.52f, dimensions.Y * 0.428f),
             new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
             "lever"
         );
        AddLink("l1_lev1", "l1_door1"); // Lever triggers Door

        //Door
        AddObject(
            "l1_door1", textures[5],
            new Vector2f(dimensions.X * 0.36f, dimensions.Y * 0.28f),
            new Vector2f(scaleValue * scaleX, scaleValue * scaleY),
            "door"
        );

        // Heart
        AddObject(
            "l1_heart", textures[6],
            new Vector2f(dimensions.X * 0.60f, dimensions.Y * 0.10f),
            new Vector2f(4.5f * scaleX, 4.5f * scaleY),
            "heart"
        );

        float spawnX = dimensions.X * 0.20f;
        float spawnY = dimensions.Y - 50f;

        //spawnPoint = new Vector2f(120f, dimensions.Y - 50f - 10f);
        spawnPoint = new Vector2f(dimensions.X * 0.1f, groundY - 100f);
    }
}
