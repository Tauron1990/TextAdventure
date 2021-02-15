using System;
using FluentValidation;
using TextAdventure.Editor.Operations.Command;

namespace TextAdventure.Editor.Operations.Process.Validators
{
    public sealed class ChangeGameNameCommandValidator : AbstractValidator<ChangeGameNameCommand>
    {
        public ChangeGameNameCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Version).NotEmpty()
                                   .Custom((s, context) =>
                                           {
                                               if (!Version.TryParse(s, out _))
                                                   context.AddFailure("Spiel Version is im Falschen Format");
                                           });
        }
    }
}