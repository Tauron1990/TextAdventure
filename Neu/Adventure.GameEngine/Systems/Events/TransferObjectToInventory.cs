using Adventure.GameEngine.Core;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Events
{
    [PublicAPI]
    public sealed class TransferObjectToInventory
    {
        public TransferObjectToInventory(string id, string targetCommand)
        {
            Id = id;
            TargetCommand = targetCommand;
        }

        public string Id { get; }
        public string TargetCommand { get; }

        public LazyString? Result { get; set; }
    }
}