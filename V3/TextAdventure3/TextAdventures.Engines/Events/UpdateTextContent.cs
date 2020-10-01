namespace TextAdventures.Engine.Events
{
    public sealed class UpdateTextContent : TransistentEvent<UpdateTextContent>
    {
        public string Description { get; }

        public string Content { get; }

        public UpdateTextContent(string description, string content)
        {
            Description = description;
            Content = content;
        }
    }
}