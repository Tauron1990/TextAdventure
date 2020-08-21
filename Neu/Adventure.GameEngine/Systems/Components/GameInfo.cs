using System;
using System.Diagnostics;
using System.IO;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;

namespace Adventure.GameEngine.Systems.Components
{
    public sealed class GameInfo : IComponent, IPersistComponent
    {
        private readonly DateTimeOffset _creationTime = DateTimeOffset.UtcNow;
        private readonly Stopwatch _updateInterval = Stopwatch.StartNew();

        public int Version { get; private set; }

        public DateTimeOffset SinceStart { get; private set; }

        public string? LastDescription { get; set; }

        public string? LastContent { get; set; }

        public GameInfo()
        {
        }

        public GameInfo(int version, DateTimeOffset sinceStart, string lastDescription, string lastContent)
        {
            Version = version;
            SinceStart = sinceStart;
            LastDescription = lastDescription;
            LastContent = lastContent;
        }

        public GameTime CreateGameTime()
        {
            var time = _updateInterval.Elapsed;
            _updateInterval.Restart();

            return new GameTime(_creationTime, time);
        }

        void IPersitable.WriteTo(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(SinceStart.ToUnixTimeMilliseconds());
            BinaryHelper.WriteNull(LastDescription, writer, writer.Write);
            BinaryHelper.WriteNull(LastContent, writer, writer.Write);
        }

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Version = reader.ReadInt32();
            SinceStart = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64());
            LastDescription = BinaryHelper.ReadNull(reader, r => r.ReadString());
            LastContent = BinaryHelper.ReadNull(reader, r => r.ReadString());
        }

        string IPersistComponent.Id => "GameInfo";
    }
}