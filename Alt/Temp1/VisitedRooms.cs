/*MIT License

Copyright(c) 2019 

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

using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Componetns;
using Adventure.GameEngine.Interfaces;
using DefaultEcs;

namespace Adventure.GameEngine
{
    /// <summary>
    /// Visited rooms.
    /// </summary>
    public class VisitedRooms : IVisitedRooms, IDisposable
    {
        private readonly EntitySet _entitySet;

        public VisitedRooms(IGame game) 
            => _entitySet = game.World.GetEntities().With<IRoom>().With<VisitedRoom>().AsSet();

        /// <summary>
        /// Adds the visited room.
        /// </summary>
        /// <param name="roomEnt">Room.</param>
        public void AddVisitedRoom(Entity roomEnt)
        {
            if (!roomEnt.Has<IRoom>())
                throw new ArgumentNullException(nameof(roomEnt));

            roomEnt.Set<VisitedRoom>();
        }

        /// <summary>
        /// Checks the room visited.
        /// </summary>
        /// <returns><c>true</c>, if room visited was checked, <c>false</c> otherwise.</returns>
        /// <param name="roomName">Room name.</param>
        public bool CheckRoomVisited(string roomName)
        {
            foreach (var entity in _entitySet.GetEntities())
            {
                if (entity.Get<IRoom>().Name == roomName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the room instanve.
        /// </summary>
        /// <returns>The room instanve.</returns>
        /// <param name="roomName">Room name.</param>
        public Entity GetRoomInstance(string roomName)
        {
            foreach (var entity in _entitySet.GetEntities())
            {
                if (entity.Get<IRoom>().Name == roomName)
                    return entity;
            }

            return default;
        }

        /// <summary>
        /// Gets the visited rooms.
        /// </summary>
        /// <returns>The visited rooms.</returns>
        public IEnumerable<(string name, string description)> GetVisitedRooms() 
            => _entitySet.GetEntities().ToArray().Select(entity => entity.Get<IRoom>()).Select(room => (room.Name, room.Description));

        public void Dispose() => _entitySet.Dispose();
    }
}
