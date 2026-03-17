using SFML.System;

namespace Handus;
using SFML.Graphics;
public class Level1 : Level
{
    public Level1(Vector2u dimensions) : base(dimensions)   // initializes level with textures and sprites
    {
        textures.Add(new Texture("Files/ground.png"));
        
        foreach (var t in textures)
        {
            sprites.Add(new Sprite(t));
        }
        sprites[0].Position = new Vector2f(0, (float)(dimensions.Y - sprites[0].Texture.Size.Y));
        
        
        spawnPoint = new Vector2f(dimensions.X / (float)5, dimensions.Y - sprites[0].Texture.Size.Y);
    }
}