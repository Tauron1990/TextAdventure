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

using System.Windows.Input;
using Adventure.GameEngine;
using Adventure.GameEngine.BuilderAlt;
using Adventure.GameEngine.BuilderAlt.RoomData;
using Adventure.GameEngine.Commands;
using Adventure.GameEngine.Systems.Components;

namespace TextAdventure.Test.Downstairs
{
    public sealed class DiningRoom : IRoomFactory
    {
        private const string FlashlightPickupEvent = nameof(FlashlightPickupEvent);

        private const string Flashlight = nameof(Flashlight);

        private const string Cupboard = nameof(Cupboard);

        public string Name => nameof(DiningRoom);

        public RoomBuilder Apply(RoomBuilder builder, GameConfiguration gameConfiguration)
        {
            LookCommand lookCommand = null;
            
            return builder
                    .WithDropItem(Flashlight,
                        b =>
                        {
                            b.WithDisplayName("Taschenlampe")
                                .PickUpCommand(c =>
                                {
                                    c.WithRespond("Du nimmst die Taschenlampe.")
                                        .TriggersEvent(FlashlightPickupEvent);
                                })
                                .WithDescription("Es handelt sich um eine kleine batteriebetriebene Taschenlampe.");
                        })

                    .WithInteriorItem(Cupboard,
                        b =>
                        {
                            b.WithDisplayName("Schrank")
                                .WithLook("Ein Schrank", out lookCommand)
                                .WithDescription("Man öffnet den Schrank und es befindet sich eine kleine Taschenlampe darin.")
                                .WithPoi(new PointOfInterst(true, "Ein Schrack. Was wohl drin ist"));
                        })

                    .ReactOnEvent(FlashlightPickupEvent, ChangeItem.Description(Cupboard, "Sie öffnen den Schrank und er ist leer."))

                    .WithDisplayName("Speisesaal")
                    .WithDescription("Sie stehen im Speisesaal. In der Mitte des Raumes steht ein Esstisch, auf dem nichts steht, und in der Ecke des Raumes steht ein kleiner Schrank.")

                ;
        }
    }

    public class DiningRoomAlt : Room
    {
        public override string ProcessCommand(ICommand command)
        {
            switch (command.Verb)
            {
                case VerbCodes.Use:
                    switch (command.Noun)
                    {
                        case "cupboard":
                        {
                            Game.IncreaseScore(1);
                            Game.NumberOfMoves++;
                            _openedCupboard = true;

                            if (!Game.Player.Inventory.Exists("torch"))
                            {
                                return Game.ContentManagement.RetrieveContentItem("LookedAtCupboard");
                            }
                            else
                            {
                                return Game.ContentManagement.RetrieveContentItem("LookedAtCupboardNoTorch");
                            }
                        }                        
                    }
                    break;

                case VerbCodes.Take:
                    if (command.Noun == "torch")
                    {
                        if (_openedCupboard)
                        {
                            if (!Game.Player.Inventory.Exists("torch"))
                            {
                                Game.Player.Inventory.Add(_torch.Name, _torch);
                                Game.IncreaseScore(1);
                                Game.NumberOfMoves++;
                                return _torch.PickUpMessage;
                            }

                            if (Game.Player.Inventory.Exists("torch"))
                            {
                                return Game.ContentManagement.RetrieveContentItem("AlreadyHaveTorch");
                            }
                        }
                        else
                        {
                            Game.NumberOfMoves++;
                            return Game.ContentManagement.RetrieveContentItem("WhatTorch");
                        }
                    }
                    break;

            }

            if (command.ProfanityDetected)
            {
                return Game.ContentManagement.RetrieveContentItem("NoNeedToBeRude");
            }

            var reply = base.ProcessCommand(command);
            return reply;
        }    
    }
}