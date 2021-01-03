using System;
using Newtonsoft.Json;

namespace TextAdventure.Editor.Models.Data
{
    public record ProjectInfo(string GameName, Version Version, string ProjectName, string ContentFile)
    {
        [JsonIgnore]
        public string RootPath { get; init; }
    }
}