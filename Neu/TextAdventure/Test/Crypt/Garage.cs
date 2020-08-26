/*
MIT License

Copyright (c) 2019 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Adventure.GameEngine.Builder;
using Adventure.GameEngine.Builder.RoomData;
namespace TextAdventure.Test.Crypt
{
    public sealed class Garage : IRoomFactory
    {
        public string Name => nameof(Garage);

        public RoomBuilder Apply(RoomBuilder builder, GameConfiguration gameConfiguration)
        {
            return builder
                .WithDescription("Sie stehen in einer Garage, umgeben von Farbtöpfen, einem alten Fahrrad und einem Rasenmäher.")
                .WithDisplayName("Garage");
        }
    }
}