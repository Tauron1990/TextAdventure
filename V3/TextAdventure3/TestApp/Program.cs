using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Construction;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string testPath = Path.GetFullPath("Test");

            if (Directory.Exists(testPath))
                Directory.Delete(testPath, true);
            Directory.CreateDirectory(testPath);

            var testFile = Path.Combine(testPath, "test.sln");

            File.WriteAllText(testFile, string.Empty);
            SolutionFile file = SolutionFile.Parse(testFile);

            //MSBuildLocator.RegisterDefaults();

            //var workspace = MSBuildWorkspace.Create();

            //await workspace.OpenSolutionAsync(testPath);

            //SolutionEditor

        }
    }
}