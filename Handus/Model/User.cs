namespace Handus
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public float PositionX { get; set; }
        public float PositionY { get; set; }
    }
}
