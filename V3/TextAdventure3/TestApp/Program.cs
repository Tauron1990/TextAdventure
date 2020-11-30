using Newtonsoft.Json;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var test = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(null));
        }
    }
}