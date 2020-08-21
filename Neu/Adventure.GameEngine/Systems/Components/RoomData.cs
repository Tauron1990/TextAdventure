using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;
using EcsRx.ReactiveData;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class RoomData : IComponent, IPersistComponent
    {
        public ReactiveProperty<bool> IsPlayerIn { get; }

        public ReactiveProperty<bool> IsVisited { get; }

        public DoorWay[] DoorWays { get; internal set; } = Array.Empty<DoorWay>();

        public DoorWayConnection[] Connections { get; internal set; } = Array.Empty<DoorWayConnection>();

        public List<PointOfInterst> Pois { get; } = new List<PointOfInterst>();

        [PublicAPI]
        public Dictionary<Direction, IDoorway> TransitionMap 
            => Connections.Cast<IDoorway>()
                .Concat(DoorWays)
                .ToDictionary(dw => dw.Direction);

        public ReactiveProperty<string> Description { get; }

        public RoomData()
        {
            Description = new ReactiveProperty<string>();
            IsVisited = new ReactiveProperty<bool>();
            IsPlayerIn = new ReactiveProperty<bool>();
        }

        string IPersistComponent.Id => "RoomData";

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(IsPlayerIn.Value);
            writer.Write(IsVisited.Value);
            writer.Write(Description.Value);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            IsPlayerIn.Value = reader.ReadBoolean();
            IsVisited.Value = reader.ReadBoolean();
            Description.Value = reader.ReadString();
        }
    }
}