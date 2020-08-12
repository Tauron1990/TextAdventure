using System;
using EcsRx.Components;
using EcsRx.ReactiveData;
using Newtonsoft.Json;

namespace Adventure.GameEngine.Components
{
    public sealed class RoomData : IComponent
    {
        public ReactiveProperty<bool> IsPlayerIn { get; }

        public DoorWay[] DoorWays { get; internal set; } = Array.Empty<DoorWay>();

        public DoorWayConnection[] Connections { get; internal set; } = Array.Empty<DoorWayConnection>();

        [JsonConstructor]
        public RoomData(ReactiveProperty<bool> isPlayerIn, DoorWayConnection[] connections, DoorWay[] doorWays)
        {
            IsPlayerIn = isPlayerIn;
            Connections = connections;
            DoorWays = doorWays;
        }

        public RoomData() => IsPlayerIn = new ReactiveProperty<bool>();
    }
}