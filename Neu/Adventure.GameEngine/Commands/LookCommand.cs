using Adventure.GameEngine.Builder;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Core.Blueprints;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Commands
{
    public sealed class LookCommand : Command<LookCommand>
    {
        public string? Target { get; }

        public LazyString? Responsd { get; set; }

        public LookCommand(string? target) : base(nameof(LookCommand))
            => Target = target;
    }

    [PublicAPI]
    public static class LookCommandExtensions
    {
        public static RoomBuilder AddlookCommand(this RoomBuilder builder, string target, LazyString name)
        {
            builder.Blueprints.Add(new RoomCommand(new LookCommand(target), name));
            return builder;
        }
    }
}