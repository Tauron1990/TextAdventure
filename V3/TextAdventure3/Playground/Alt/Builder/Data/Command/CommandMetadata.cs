using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TextAdventures.Builder.Data.Command
{
    [PublicAPI]
    public sealed class CommandMetadata : IReadOnlyDictionary<string, string>
    {
        public static readonly CommandMetadata Empty = new(ImmutableDictionary<string, string>.Empty);

        [JsonProperty(PropertyName = "CommandMetadata")]
        private readonly ImmutableDictionary<string, string> _data;

        private CommandMetadata(ImmutableDictionary<string, string> data) => _data = data;

        [JsonConstructor]
        private CommandMetadata(IEnumerable<KeyValuePair<string, string>> data)
            => _data = ImmutableDictionary<string, string>.Empty.AddRange(data);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _data).GetEnumerator();

        public int Count => _data.Count;

        public bool ContainsKey(string key) => _data.ContainsKey(key);

        public bool TryGetValue(string key, out string value) => _data.TryGetValue(key, out value);

        public string this[string key] => _data[key];

        public IEnumerable<string> Keys => _data.Keys;

        public IEnumerable<string> Values => _data.Values;

        public CommandMetadata Clear() => new(_data.Clear());

        public CommandMetadata Add(string key, string value) => new(_data.Add(key, value));

        public CommandMetadata AddRange(IEnumerable<KeyValuePair<string, string>> pairs) => new(_data.AddRange(pairs));

        public CommandMetadata SetItem(string key, string value) => new(_data.SetItem(key, value));

        public CommandMetadata SetItems(IEnumerable<KeyValuePair<string, string>> items) => new(_data.SetItems(items));

        public CommandMetadata RemoveRange(IEnumerable<string> keys) => new(_data.RemoveRange(keys));

        public CommandMetadata Remove(string key) => new(_data.Remove(key));

        public bool Contains(KeyValuePair<string, string> pair) => _data.Contains(pair);

        public bool TryGetKey(string equalKey, out string actualKey) => _data.TryGetKey(equalKey, out actualKey);

        public override string? ToString() => _data.ToString();
    }
}