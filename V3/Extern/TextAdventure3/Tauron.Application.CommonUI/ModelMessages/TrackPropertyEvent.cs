namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed class TrackPropertyEvent
    {
        public string Name { get; }

        public TrackPropertyEvent(string name) => Name = name;
    }
}