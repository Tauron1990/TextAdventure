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
using System.Collections.ObjectModel;
using Adventure.GameEngine.Componetns;
using Adventure.GameEngine.Interfaces;
using DefaultEcs;

namespace Adventure.GameEngine
{
    /// <summary>
    /// The player has the ability to drop objects from their inventory around different rooms. The DroppedObjects class
    /// represents a list of objects that are dropped in a room. For an object to be dropped, it has to be present in
    /// the players inventory.
    /// </summary>
    public class DroppedObjects : IDroppedObjects
    {
        private readonly Entity _self;

        // Constructor that assigns the master game object for this game instance.
        public DroppedObjects(IGame gameObject)
        {
            _game = gameObject;
            _self = gameObject.World.CreateEntity();
            _self.Set<IDroppedObjects>(this);
        }

        private readonly IGame _game;
        private readonly List<Entity> _droppedObjects = new List<Entity>();

        /// <summary>
        /// Return a read only collection of object instances that have been dropped in this room.
        /// </summary>
        public ReadOnlyCollection<Entity> DroppedObjectsList => new ReadOnlyCollection<Entity>(_droppedObjects);
        
        /// <summary>
        /// Drop an object into a room by specifying the objects name in the inventory. The object has to be in the
        /// players inventory before you can drop it. The name is treated as case insensitive.
        /// </summary>
        /// <param name="objectName">The name of the object from the inventory to drop in the room.</param>
        /// <returns>true if the object is dropped; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the objectName to drop is null or empty, then throw an 
        /// ArgumentNullException.</exception>
        public bool DropObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                throw new ArgumentNullException(nameof(objectName));
            }

            objectName = objectName.ToLower();

            if (!_game.Player.Inventory.Exists(objectName)) return false;
            
            var droppedObject = _game.Player.Inventory.Get(objectName);
            _game.Player.Inventory.RemoveObject(objectName);

            _self.SetAsParentOf(droppedObject);
            droppedObject.Set<DroppedItem>();
            _droppedObjects.Add(droppedObject);

            return true;

        }

        /// <summary>
        /// Pick up an object from the dropped objects list and add it to the players inventory. The object will be removed
        /// from the dropped objects list.
        /// </summary>
        /// <param name="objectName">The name of the object to pick up.</param>
        /// <returns>true if the object is picked up; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the objectName to pick up is null or empty, then throw an 
        /// ArgumentNullException.</exception>
        public bool PickUpDroppedObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException(nameof(objectName));

            objectName = objectName.ToLower();
            
            foreach (var entity in _droppedObjects)
            {
                var obj = _game.World.GetComponents<IObject>()[entity];
                if (obj.Name.ToLower() != objectName) continue;
                
                _droppedObjects.Remove(entity);
                entity.Remove<DroppedItem>();
                _game.Player.Inventory.Add(obj.Name, entity);
            
                return true;
            }
            
            return false;
        }
    }
}