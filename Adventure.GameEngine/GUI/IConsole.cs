using JetBrains.Annotations;

namespace Adventure.GameEngine.GUI
{
    [PublicAPI]
    public interface IConsole
    {
		int BufferWidth { get; }

        int BufferHeight { get; }

        bool CursorVisible { get; set; }

        int CursorLeft { get; set; }

        int CursorTop { get; set; }

        void WriteLine(string text);

        void WriteLine();

        void Write(string text);

        void ClearConsoleLine(int line);

        void ClearCurrentConsoleLine();
    }
}