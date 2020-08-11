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
using Adventure.GameEngine.Interfaces;
using DefaultEcs;

namespace Adventure.GameEngine
{
    /// <summary>
    /// The RoomExits class contains a list of the exits assigned to a room for a given direction.
    /// </summary>
    public sealed class RoomExits : IRoomExits
    {
        private readonly Entity _room;

        private sealed class DoorWayList : Dictionary<Direction, DoorWay>
        {
            
        }

        public RoomExits(Entity room)
        {
            _room = room;
            _room.Set(new DoorWayList());
        }

        /// <summary>
        /// Add an exit to the list for a specified direction that leads to the room that is passed in.
        /// </summary>
        /// <param name="direction">The direction to assign the exit too.</param>
        /// <param name="room">The room that the exit leads too.</param>
        /// <exception cref="ArgumentNullException">If the room is null, throw an ArgumentNullException.</exception>
        /// <exception cref="InvalidOperationException">If you try to map a room to the same direction twice an
        /// InvalidOperationException will be thrown.</exception>
        public void AddExit(Direction direction, Entity room) 
            => AddExit(new DoorWay(room, direction, false, string.Empty), room);

        /// <summary>
        /// Add an exit to the list for a specified direction that leads to the room that is passed in.
        /// </summary>
        /// <param name="doorway">Doorway object that encapsulates the direction.</param>
        /// <param name="room">The room that the exit leads too.</param>
        /// <exception cref="ArgumentNullException">If the room is null, throw an ArgumentNullException.</exception>
        /// <exception cref="InvalidOperationException">If you try to map a room to the same direction twice an
        /// InvalidOperationException will be thrown.</exception>
        public void AddExit(DoorWay doorway, Entity room)
        {
            if (!room.Has<IRoom>())
                throw new ArgumentNullException(nameof(room));

            var direction = doorway.Direction;
            var list = _room.Get<DoorWayList>();
            if (list.ContainsKey(direction))
                throw new InvalidOperationException("A room mapping already exists for the direction <" + direction + ">.");

            list[direction] = new DoorWay(room, direction, false, string.Empty);
        }

        /// <summary>
        /// Return the room assigned for a specific direction.
        /// </summary>
        /// <param name="direction">The direction to return the exist for.</param>
        /// <returns>The room that is assigned to this exit direction.</returns>
        public Entity GetRoomForExit(Direction direction) 
            => _room.Get<DoorWayList>().TryGetValue(direction, out var w) ? w.TargetRoom : default;

        /// <summary>
        /// For a given exit, is that exit locked.
        /// </summary>
        /// <param name="direction">The direction to check the lock status for.</param>
        /// <returns>True if the exit is locked, False otherwise.</returns>
        public bool IsDoorLocked(Direction direction) 
            => !_room.Get<DoorWayList>().TryGetValue(direction, out var way) || way.Locked;

        /// <summary>
        /// Return the door way object for the given direction.
        /// </summary>
        /// <param name="direction">The direction to get the door way object.</param>
        /// <returns>The retrieved door way.</returns>
        public DoorWay GetDoorWay(Direction direction)
            => _room.Get<DoorWayList>().TryGetValue(direction, out var w) ? w : default;

        /// <summary>
        /// Set the door lock status for the given direction.
        /// </summary>
        /// <param name="locked">True if you want the door locked, False otherwise.</param>
        /// <param name="direction">The direction of the exit that you want to set the lock status of.</param>
        public void SetDoorLock(bool locked, Direction direction)
        {
            if (_room.Get<DoorWayList>().TryGetValue(direction, out var w)) 
                w.Locked = locked;
        }
    }
}
