namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed class UnloadEvent
    {
        public UnloadEvent(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}