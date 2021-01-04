using System;
using System.Linq;
using System.Reactive.Linq;
using FluentValidation;
using Tauron.Application.Workshop.Mutating;
using Tauron.Application.Workshop.StateManagement.Attributes;
using Tauron.ObservableExt;
using TextAdventure.Editor.Data.PartialData;
using TextAdventure.Editor.Operations.Command;
using TextAdventure.Editor.Operations.Events;
using TextAdventure.Editor.Operations.Process.Validators;

namespace TextAdventure.Editor.Operations.Process
{
    [BelogsToState(typeof(CommonDataState))]
    public static class CommonDataProcessor
    {
        private static IValidator<ChangeGameNameCommand> ChangeNameValidator { get; } = new ChangeGameNameCommandValidator();

        [Reducer]
        public static IObservable<MutatingContext<CommonData>> ChangeName(IObservable<MutatingContext<CommonData>> input, ChangeGameNameCommand command)
        {
            return input.Select(data => (data, ChangeNameValidator.Validate(command)))
                        .ConditionalSelect()
                        .ToResult<MutatingContext<CommonData>>(b =>
                                                               {
                                                                   b.When(d => !d.Item2.IsValid,
                                                                          observable => observable
                                                                             .Select(d => d.data
                                                                                           .WithChange(new NameVersionChangedEvent(false, d.Item2.Errors.First().ErrorMessage, new Version(0, 0)))));

                                                                   b.When(d => d.Item2.IsValid,
                                                                          observable => observable
                                                                             .Select(d => d.data
                                                                                           .WithChange(new NameVersionChangedEvent(true, command.Name, Version.Parse(command.Version)))));
                                                               });
        }
    }
}