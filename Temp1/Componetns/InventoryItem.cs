namespace Adventure.GameEngine.Componetns
{
    public sealed class InventoryItem
    {
        public string Name { get; }

        public InventoryItem(string name) 
            => Name = name;
    }
}