using System.Linq;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Core;
using Adventure.GameEngine.Systems.Components;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Extensions;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems
{
    [PublicAPI]
    public sealed class TravelProcessor : CommandProcessor<TravelCommand>
    {
        public TravelProcessor(IEventSystem eventSystem, IObservableGroupManager manager) 
            : base(eventSystem, manager)
        {
        }

        protected override void ProcessCommand(TravelCommand command)
        {
            if (CurrentRoom.Value == null) return;

            switch (command)
            {
                case DirectionTravelCommand directionTravel:
                    HandleMovement(directionTravel);
                    break;
                case ForceTravelTo forceTravel:
                    HandleMovement(forceTravel);
                    break;
            }
        }

        private void HandleMovement(ForceTravelTo command)
        {
            MoveToRoom(command.Room, CurrentRoom.Value.GetComponent<RoomData>());
            UpdateTextContent(LazyString.New(GameConsts.NewRoomEntered).AddParameters(StringParameter.Resolved(command.Room)));
        }

        private void HandleMovement(DirectionTravelCommand command)
        {
            var data = CurrentRoom.Value.GetComponent<RoomData>();

            var potenalDoor = data.Connections.Cast<IDoorway>().Concat(data.DoorWays).FirstOrDefault(dw => dw.Direction == command.Direction);
            if (potenalDoor == null)
                return;

            if (string.IsNullOrWhiteSpace(potenalDoor.UnlockEvent))
            {
                MoveToRoom(potenalDoor.TargetRoom, data);
                UpdateTextContent(LazyString.New(GameConsts.NewRoomEntered).AddParameters(StringParameter.Resolved(potenalDoor.TargetRoom)));
            }
            else
                UpdateTextContent(LazyString.New(GameConsts.DoorwayLooked));
        }

        private void MoveToRoom(string name, RoomData from)
        {
            var to = ObservableGroup.First(e => e.GetComponent<Room>().Name == name).GetComponent<RoomData>();

            from.IsPlayerIn.Value = false;
            to.IsPlayerIn.Value = true;
            to.IsVisited.Value = true;
        }
    }
}