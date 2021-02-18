using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp
{
    public interface IPlugin
    {
        object Process(object input);
    }

    internal static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hallo Welt");

            Console.ReadKey();
        }
    }
}