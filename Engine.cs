namespace Handus;

using SFML.Graphics;
public class Engine
{
    private RenderWindow window;
    private Player player;
    private Level level;
    private Dictionary<string, Texture> textures = new();

    public Engine(RenderWindow window)  // initializes engine, creates player and level
    {
        this.window = window;

        level = new Level1(window.Size);
        
        var texture = new Texture("Files/player.jpg");
        textures.Add("idle1", texture);

        player = new Player(textures, level.GetSpawnPoint());
    }
    
    void Update()
    {
        window.DispatchEvents();
        window.Closed += (sender, e) => window.Close();
        player.Update();
        level.Update();
    }

    void Render()   // draws all sprites
    {
        window.Clear();
        foreach (var s in level.GetSprites()) window.Draw(s);
        window.Draw(player.GetSprite());
        window.Display();
    }
    
    public void Loop()  // launches the game's loop
    {
        while (window.IsOpen)   // TODO: delta time
        {
            Update();
            Render();
        }
    }
}