using Adventure.GameEngine.Core;
using Adventure.TextProcessing.Synonyms;

namespace Adventure.GameEngine.Events
{
    public sealed class TransferObjectToInventory
    {
        public TransferObjectToInventory(string id, VerbCodes codes)
        {
            Id = id;
            Codes = codes;
        }

        public string Id { get; }

        public VerbCodes Codes { get; }

        public LazyString? Result { get; set; }
    }
}