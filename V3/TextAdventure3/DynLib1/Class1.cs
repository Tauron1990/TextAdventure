using System;
using TestApp;

namespace DynLib1
{
    public interface IMessage
    {
        string Get();
    }

    public class Class1 : IPlugin
    {
        public object? Process(object input) => ((IMessage)input).Get();
    }
}
