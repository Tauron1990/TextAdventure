using System;
using DynLib1;
using TestApp;

namespace DynLib2
{
    public class Class1 : IPlugin
    {
        private sealed class MsgClass : IMessage
        {
            private readonly string _msg;

            public MsgClass(string msg) => _msg = msg;

            public string Get() => _msg;
        }

        public object? Process(object input) => new MsgClass(input.ToString());
    }
}
