using SFML.System;

namespace Handus;
using SFML.Graphics;
public class Level1 : Level
{
    public Level1(Vector2u dimensions) : base(dimensions)   // initializes level with textures and sprites
    {
        textures.Add(new Texture("Files/ground.png"));
        var size = new Vector2i(1920, 1080);

        var width = (float)textures[0].Size.X;
        var distance = 100;
        var scaleCoeff = (size.X - distance * 2) / width;
        var scale = new Vector2f(scaleCoeff, scaleCoeff);
        sprites.Add(new Sprite(textures[0]));
        sprites[0].Scale = scale;
        sprites[0].Position = new Vector2f(distance, dimensions.Y - sprites[0].Texture.Size.Y);
        GenerateHitbox(sprites[0], scale);

        scaleCoeff = 500 / width;
        scale = new Vector2f(scaleCoeff, scaleCoeff);
        sprites.Add(new Sprite(textures[0]));
        sprites[1].Scale = scale;
        sprites[1].Position = new Vector2f(size.X * 0.5f, size.Y * 0.75f);
        GenerateHitbox(sprites[1], scale);
        
        sprites.Add(new Sprite(textures[0]));
        sprites[2].Scale = scale;
        sprites[2].Position = new Vector2f(size.X * 0.25f, size.Y * 0.5f);
        GenerateHitbox(sprites[2], scale);
        
        spawnPoint = new Vector2f(dimensions.X / (float)5, dimensions.Y - sprites[0].Texture.Size.Y - 5);
    }
}