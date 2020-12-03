using System;

namespace TextAdventure.Editor.Models.Data
{
    public record ProjectInfo(string GameName, Version Version, string ProjectName, string ContentFile);
}