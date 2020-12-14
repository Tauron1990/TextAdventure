namespace TextAdventure.Editor.ViewModels.Shared
{
    public sealed record PropagateError(string Error, bool Fatal);

    public sealed record StatusBarInfo(string Info);

    //public sealed class ApplicationInfo : IDisposable
    //{
    //    private readonly Subject<PropagateError> _errors = new();
    //    private readonly Subject<string> _infoUpdate = new();

    //    public IObservable<PropagateError> Errors => _errors.AsObservable();

    //    public IObservable<string> InfoUpdate => _infoUpdate.AsObservable();MessageBus

    //    public void Error(PropagateError error)
    //        => _errors.OnNext(error);

    //    public void Dispose() => _errors.Dispose();
    //}
}