﻿using System.Collections.Generic;
using System.Linq;
using Adventure.GameEngine.Components;
using Adventure.GameEngine.Events;
using Adventure.TextProcessing.Interfaces;
using EcsRx.Collections.Entity;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.ReactiveSystems.Custom;

namespace Adventure.GameEngine.Systems
{
    public sealed class CommandProcessor : EventReactionSystem<ICommand>
    {
        public CommandProcessor(IEventSystem eventSystem) 
            : base(eventSystem)
        {
        }

        public override IGroup Group { get; } = new Group(typeof(RoomData), typeof(RoomCommands));

        public override void EventTriggered(ICommand eventData)
        {
            var target = ObservableGroup.FirstOrDefault(e => e.GetComponent<RoomData>().IsPlayerIn.Value);
            string? result = null;
            if (target == null)
            {
                foreach (var func in target.GetComponent<RoomCommands>().Handler)
                {
                    result = func(eventData);
                    if (result != null)
                        break;
                }
            }

            if (result != null)
            {
                var generic = new GenericCommand(eventData);
                EventSystem.Publish(generic);
                result = generic.Result;
            }

            EventSystem.Publish(new CommandExecutionCompled(result));
        }
    }
}