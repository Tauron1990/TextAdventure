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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Adventure.GameEngine.Componetns;
using Adventure.GameEngine.Interfaces;
using DefaultEcs;

namespace Adventure.GameEngine
{
    /// <summary>
    /// The Inventory object is where the player will store objects they collect from rooms in the game.
    /// </summary>
    public class Inventory : IInventory 
    {
        private readonly EntitySet _inventory;

        public Entity Self { get; }

        public Inventory(IGame game, Entity player)
        {
            Self = game.World.CreateEntity();
            Self.Set<IInventory>(this);

            player.SetAsParentOf(Self);

            _inventory = game.World.GetEntities().With<IObject>().With<InventoryItem>().AsSet();
        }


        /// <summary>
        /// Add a game object to the inventory.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="gameObject">The game object that the player has collected.</param>
        /// <exception cref="ArgumentNullException">If the name or game object are null or empty then throw an 
        /// ArgumentNullException.</exception>
        public void Add(string name, Entity gameObject)
        {
            if (string.IsNullOrEmpty(name) || !gameObject.Has<IObject>())
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();
            
            if (!Exists(name)) 
                gameObject.Set(new InventoryItem(name));
        }

        /// <summary>
        /// Clear all object from the inventory.
        /// </summary>
        public void Clear()
        {
            foreach (var entity in _inventory.GetEntities()) 
                entity.Remove<InventoryItem>();
        }

        /// <summary>
        /// Check if an object exists in the inventory.
        /// </summary>
        /// <param name="name">The name of the object to check for.</param>
        /// <returns>True if the object exists, and False otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the name of the object is null or empty then throw an 
        /// ArgumentNullException.</exception>
        public bool Exists(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();

            foreach (var entity in _inventory.GetEntities())
            {
                if (entity.Get<InventoryItem>().Name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Remove an object from the inventory.
        /// </summary>
        /// <param name="name">The name of the object to remove from the inventory.</param>
        /// <returns>True of the object is removed successfully, False otherwise.</returns>
        /// <exception cref="ArgumentNullException">If the name of the object is null or empty then throw an 
        /// ArgumentNullException.</exception>
        public bool RemoveObject(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();

            foreach (var entity in _inventory.GetEntities())
            {
                if (entity.Get<InventoryItem>().Name != name) continue;
                
                entity.Remove<InventoryItem>();
                return true;
            }
            
            return false;

        }

        /// <summary>
        /// Retrieve an object from the Inventory.
        /// </summary>
        /// <param name="name">The name of the object to retrieve.</param>
        /// <returns>A reference to the retrieved object. If the object doesn't exist return null.</returns>
        /// <exception cref="ArgumentNullException">If the name is null or empty, then throw an ArgumentNullException.</exception>
        public Entity Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            name = name.ToLower();

            foreach (var entity in _inventory.GetEntities())
            {
                if (entity.Get<InventoryItem>().Name == name)
                    return entity;
            }

            return default;
        }

        /// <summary>
        /// Return the number of objects in the Inventory.
        /// </summary>
        /// <returns>The number of objects in the Inventory.</returns>
        public int Count() => _inventory.Count;

        /// <summary>
        /// Return a read only collection of the object names and descriptions from the Inventory. This is to display
        /// the inventory details oi game.
        /// </summary>
        /// <returns>A read only collection of objects from the inventory.</returns>
        public ReadOnlyCollection<string> GetInventory()
        {
            var inventory = new List<string>();

            foreach (var entry in _inventory.GetEntities())
            {
                var obj = entry.Get<IObject>();
                var item = entry.Get<InventoryItem>();

                inventory.Add(item.Name + " : " + obj.Description);
            }

            return new ReadOnlyCollection<string>(inventory);
        }
    }
}