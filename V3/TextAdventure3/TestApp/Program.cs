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
            var context = System.Runtime.Loader.AssemblyLoadContext.Default;

            var dyn1 = context.LoadFromAssemblyPath(Path.GetFullPath(Path.Combine("Dyn1\\DynLib1.dll")));
            var dyn2 = context.LoadFromAssemblyPath(Path.GetFullPath(Path.Combine("Dyn2\\DynLib2.dll")));

            var plugin1 = (IPlugin)Activator.CreateInstance(dyn1.ExportedTypes.Single(t => t.Name == "Class1"))!;
            var plugin2 = (IPlugin) Activator.CreateInstance(dyn2.ExportedTypes.Single())!;

            Console.WriteLine(plugin1.Process(plugin2.Process("Hallo Welt")));
        }
    }
}