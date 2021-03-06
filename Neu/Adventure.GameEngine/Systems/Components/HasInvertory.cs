﻿using System.Collections.Immutable;
using System.IO;
using Adventure.GameEngine.Core.Persistence;
using EcsRx.Components;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Systems.Components
{
    [PublicAPI]
    public sealed class HasInvertory : IComponent, IPersistComponent
    {
        public ImmutableDictionary<string, int> Items { get; private set; }
        
        public HasInvertory() => Items = ImmutableDictionary<string, int>.Empty;


        public void Add(string item) 
            => Items = Items.ContainsKey(item) ? Items.SetItem(item, Items[item] + 1) : Items.SetItem(item, 1);

        public void Remove(string item)
        {
            if (!Items.TryGetValue(item, out var amount)) return;
            
            Items = amount == 1 ? Items.Remove(item) : Items.SetItem(item, amount - 1);
        }

        void IPersitable.WriteTo(BinaryWriter writer) 
            => BinaryHelper.WriteDic(Items, writer, (w, i) => w.Write(i));

        void IPersitable.ReadFrom(BinaryReader reader)
        {
            Items = BinaryHelper.ReadDic(reader, r => r.ReadString(), r => r.ReadInt32())
               .ToImmutableDictionary();
        }

        string IPersistComponent.Id => "HasInvetory";
    }
}