using System;
using System.Security.Cryptography;
using Adventure.GameEngine.Interfaces;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Core
{
    [PublicAPI]
    public sealed class DiceRoll : IDiceRoll
    {
        readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        public int Roll(int numberSides)
        {
            if (numberSides == 0)
                throw new InvalidOperationException("You can not have a 0 sided dice.");

            if (numberSides > 255)
                throw new InvalidOperationException("You can not have dice with " + numberSides + " sides.");

            var oneByte = new byte[1];
            var max = byte.MaxValue - byte.MaxValue % numberSides;

            while (true)
            {
                _rng.GetBytes(oneByte);              

                if (oneByte[0] < max)
                    return oneByte[0] % numberSides + 1;
            }
        }
    }
}
