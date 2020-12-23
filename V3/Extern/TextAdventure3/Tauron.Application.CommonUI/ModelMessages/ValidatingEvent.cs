namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed record ValidatingEvent(string? Reason, string Name)
    {
        public bool Error => !string.IsNullOrWhiteSpace(Reason);
    }
}