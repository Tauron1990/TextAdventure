namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed class InitEvent
    {
        public InitEvent(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}