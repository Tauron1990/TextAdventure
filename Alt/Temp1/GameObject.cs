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
using Adventure.GameEngine.Interfaces;
using DefaultEcs;
using JetBrains.Annotations;

namespace Adventure.GameEngine
{
    /// <summary>
    /// Represents a game object that can be picked up and used by the player. An example could be a key, or weapon.
    /// </summary>
    [PublicAPI]
    public class GameObject : IObject
    {
        /// <summary>
        /// This is the name of the object. This name could be displayed by the game.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// This is the description of the object that can be displayed in game.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// This is the long description of the object that can be displayed in game.
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// This is a message that cen be displayed when an object is collected, something like "You put the key in your 
        /// pocket".
        /// </summary>
        public string PickUpMessage { get; set; }
        
        /// <summary>
        /// You can set a DateTime of when this object is picked up.
        /// </summary>
        public DateTime PickedUpDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="T:Adventure.GameEngine.GameObject"/> is edible.
        /// </summary>
        /// <value><c>true</c> if edible; otherwise, <c>false</c>.</value>
        public bool Edible { get; set; }

        /// <summary>
        /// Gets the stats adjustment.
        /// </summary>
        /// <value>The stats adjustment.</value>
        public List<PlayerStatsAdjustments> StatsAdjustment { get; }

        /// <summary>
        /// Gets or sets the eaten message.
        /// </summary>
        /// <value>The eaten message.</value>
        public string EatenMessage { get; set; }

        public Entity Self { get; }

        /// <summary>
        /// Constructor that sets an objects initial state.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="name">The name of the object.</param>
        /// <param name="description">A description of the object.</param>
        /// <param name="pickUpMessage">The message that is displayed when an object is picked up.</param>
        public GameObject(IGame game, string name, string description, string pickUpMessage)
        {
            Self = game.World.CreateEntity();
            Self.Set<IObject>(this);

            Name = name;
            Description = description;
            PickUpMessage = pickUpMessage;
            Edible = false;

            StatsAdjustment = new List<PlayerStatsAdjustments>();

            EatenMessage = string.Empty;
        }

        public GameObject(IGame game, string name, string description, string pickUpMessage,
                          bool edible, int pointsToApplyAfterEaten, string statToModifyAfterEaten, string eatenMessage)
            : this(game, name, description, pickUpMessage)
        {
            Edible = edible;

            StatsAdjustment = new List<PlayerStatsAdjustments>();

            PlayerStatsAdjustments adjustment = new PlayerStatsAdjustments(statToModifyAfterEaten, pointsToApplyAfterEaten);
            StatsAdjustment.Add(adjustment);

            EatenMessage = eatenMessage;
        }

        public GameObject(IGame game, string name, string description, string pickUpMessage,
                  bool edible, IEnumerable<PlayerStatsAdjustments> statsToUpdate, string eatenMessage)
            : this(game, name, description, pickUpMessage)
        {
            Edible = edible;

            StatsAdjustment = new List<PlayerStatsAdjustments>(statsToUpdate);

            EatenMessage = eatenMessage;
        }
    }
}
