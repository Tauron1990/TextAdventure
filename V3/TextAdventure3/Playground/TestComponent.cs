using System;
using TextAdventures.Engine.Data;

namespace Playground
{
    public class TestComponent : ComponentBase
    {
        public string Message
        {
            get => GetData(string.Empty);
            set => SetData(value);
        }
    }
}