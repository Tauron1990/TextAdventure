using System;
using Adventure.GameEngine.Core;

namespace Adventure.GameEngine.Commands
{
    public static class ChangeAction
    {
        public static ChangeItemCommand WithItemDescription(string id, LazyString description)
            => new ChangeDescription(id, description);

        public static ObjectChangeCommand WithObjectAction<TObject>(TObject target, Action<TObject> changer)
            => new ObjectChangeCommand<TObject>(changer, target);
    }
}