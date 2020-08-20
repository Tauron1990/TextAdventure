namespace Adventure.GameEngine.Events
{
    public sealed class UpdateTextContent
    {
        public string? Content { get; }

        public string? Description { get; }

        public UpdateTextContent(string? content, string? description)
        {
            Content = content;
            Description = description;
        }
    }
}