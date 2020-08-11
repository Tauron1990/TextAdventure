namespace Adventure.GameEngine.Components
{
    public sealed class Room
    {
        public string Name { get; set; }

        public Room()
        {
            
        }

        public Room(string name) 
            => Name = name;
    }
}