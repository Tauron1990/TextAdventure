using System;
using System.Collections.Generic;
using System.Linq;
using EcsRx.Components;
using EcsRx.ReactiveData;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class RoomData : IComponent
    {
        public ReactiveProperty<bool> IsPlayerIn { get; }

        public ReactiveProperty<bool> IsVisited { get; }

        public DoorWay[] DoorWays { get; internal set; } = Array.Empty<DoorWay>();

        public DoorWayConnection[] Connections { get; internal set; } = Array.Empty<DoorWayConnection>();

        [JsonIgnore]
        public Dictionary<Direction, IDoorway> TransitionMap 
            => Connections.Cast<IDoorway>()
                .Concat(DoorWays)
                .ToDictionary(dw => dw.Direction);

        public ReactiveProperty<string> Description { get; }

        [JsonConstructor]
        public RoomData(ReactiveProperty<bool> isPlayerIn, DoorWayConnection[] connections, DoorWay[] doorWays, ReactiveProperty<bool> isVisited, ReactiveProperty<string> description)
        {
            IsPlayerIn = isPlayerIn;
            Connections = connections;
            DoorWays = doorWays;
            IsVisited = isVisited;
            Description = description;
        }

        public RoomData()
        {
            Description = new ReactiveProperty<string>();
            IsVisited = new ReactiveProperty<bool>();
            IsPlayerIn = new ReactiveProperty<bool>();
        }
    }
}