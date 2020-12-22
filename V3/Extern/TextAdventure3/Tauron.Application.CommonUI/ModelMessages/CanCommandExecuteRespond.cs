using System;

namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed record CanCommandExecuteRespond(string Name, Func<bool> CanExecute);
}