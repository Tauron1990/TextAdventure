namespace TextAdventures.Engine.Events
{
    public sealed class UpdateTextContent : TransistentEvent<UpdateTextContent>
    {
        public UpdateTextContent(string description, string content)
        {
            Description = description;
            Content     = content;
        }

        public string Description { get; }

        public string Content { get; }
    }
}