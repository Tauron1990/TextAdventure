using System;
using JetBrains.Annotations;

namespace Adventure.GameEngine.GUI
{
    [PublicAPI]
    public sealed class RealConsole : IConsole
    {
        public int BufferWidth => Console.BufferWidth;
        public int BufferHeight => Console.BufferHeight;
        public bool CursorVisible
        {
            get => Console.CursorVisible;
            set => Console.CursorVisible = value;
        }

        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }
        public void WriteLine(string text) => Console.WriteLine(text);

        public void WriteLine() => Console.WriteLine();

        public void Write(string text) => Console.Write(text);

        public void ClearConsoleLine(int line)
        {
            var currentLineCursor = CursorTop;
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public void ClearCurrentConsoleLine() => ClearConsoleLine(CursorTop);
    }
}