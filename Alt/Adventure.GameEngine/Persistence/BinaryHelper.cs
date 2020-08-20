using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;

namespace Adventure.GameEngine.Persistence
{
    [PublicAPI]
    public static class BinaryHelper
    {
        public static Dictionary<TKey, TValue> ReadDic<TKey, TValue>(BinaryReader reader,
            Func<BinaryReader, TKey> keyConversion, Func<BinaryReader, TValue> valueConversion)
        {
            var count = reader.ReadInt32();
            var builder = new Dictionary<TKey, TValue>();

            for (var i = 0; i < count; i++)
                builder.Add(keyConversion(reader), valueConversion(reader));

            return builder;
        }

        public static void WriteDic(IDictionary<string, string> dic, BinaryWriter writer)
        {
            writer.Write(dic.Count);
            foreach (var (key, value) in dic)
            {
                writer.Write(key);
                writer.Write(value);
            }
        }

        public static void WriteDic<TValue>(IDictionary<string, TValue> dic, BinaryWriter writer, Action<BinaryWriter, TValue> writeValue)
        {
            writer.Write(dic.Count);
            foreach (var (key, value) in dic)
            {
                writer.Write(key);
                writeValue(writer, value);
            }
        }

        public static void WriteDic<TKey>(Dictionary<TKey, string> dic, BinaryWriter writer)
            where TKey : IPersitable
        {
            writer.Write(dic.Count);
            foreach (var (key, value) in dic)
            {
                key.WriteTo(writer);
                writer.Write(value);
            }
        }

        public static List<TType> ReadList<TType>(BinaryReader reader)
            where TType : IPersitable, new()
        {
            var builder = new List<TType>();
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var n = new TType();
                n.ReadFrom(reader);
                builder.Add(n);
            }

            return builder;
        }

        public static List<TType> ReadList<TType>(BinaryReader reader, Func<BinaryReader, TType> converter)
        {
            var builder = new List<TType>();
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++) builder.Add(converter(reader));

            return builder;
        }

        public static List<string> ReadString(BinaryReader reader) 
            => ReadList(reader, binaryReader => binaryReader.ReadString());

        public static TType Read<TType>(BinaryReader reader)
            where TType : IPersitable, new()
        {
            var result = new TType();
            result.ReadFrom(reader);

            return result;
        }

        public static void WriteList<TType>(IList<TType> list, BinaryWriter writer)
            where TType : IPersitable
        {
            writer.Write(list.Count);
            foreach (var writeable in list)
                writeable.WriteTo(writer);
        }

        public static void WriteList(IList<string> list, BinaryWriter writer)
        {
            writer.Write(list.Count);
            foreach (var writeable in list)
                writer.Write(writeable);
        }


        public static void WriteNull<TValue>([AllowNull] TValue value, BinaryWriter writer, Action<TValue>? action = null)
        {
            if (Equals(value, null))
                writer.Write(false);
            else
            {
                writer.Write(true);
                action?.Invoke(value);
            }
        }

        [return: MaybeNull]
        public static TType ReadNull<TType>(BinaryReader reader, Func<BinaryReader, TType> builder)
            => reader.ReadBoolean() ? builder(reader) : default;

        public static void Write(BinaryWriter writer, IPersitable persitable)
            => persitable.WriteTo(writer);
    }
}