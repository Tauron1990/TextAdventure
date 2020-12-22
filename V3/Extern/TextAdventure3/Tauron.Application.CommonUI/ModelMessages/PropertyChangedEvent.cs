namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed class PropertyChangedEvent
    {
        public string Name { get; }

        public object? Value { get; }

        public PropertyChangedEvent(string name, object? value)
        {
            Name = name;
            Value = value;
        }
    }
}