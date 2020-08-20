using System;

namespace Adventure.GameEngine
{
    public interface IStartUpNotify
    {
        void Fail(Exception e);

        void Succed(Game game);
    }
}