namespace Handus;

using SFML.Graphics;
public class Engine
{
    private RenderWindow window;
    private Player player;
    private Level level;
    private Dictionary<string, Texture> textures = new();

    public Engine(RenderWindow window)
    {
        this.window = window;

        level = new Level1(window.Size);
        
        var texture = new Texture("Files/player.jpg");
        textures.Add("idle1", texture);

        player = new Player(textures, level.getSpawnPoint());
    }
    
    void update()
    {
        window.DispatchEvents();
        window.Closed += (sender, e) => window.Close();
    }

    void render()
    {
        window.Clear();
        foreach (var s in level.getSprites()) window.Draw(s);
        window.Draw(player.getSprite());
        window.Display();
    }
    
    public void loop()
    {
        while (window.IsOpen)
        {
            update();
            render();
        }
    }
}