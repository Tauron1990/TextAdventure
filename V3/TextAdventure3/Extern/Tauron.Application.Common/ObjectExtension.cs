﻿using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public enum RoundType : short
    {
        None = 0,

        Hour = 60,

        HalfHour = 30,

        QuaterHour = 15
    }

    [PublicAPI]
    public static class ObjectExtension
    {
        public static void DynamicUsing(this object resource, Action action)
        {
            try
            {
                action();
            }
            finally
            {
                if (resource is IDisposable d)
                    d.Dispose();
            }
        }
        
        public static DateTime CutSecond(this DateTime source) 
            => new(source.Year, source.Month, source.Day, source.Hour, source.Minute, 0);

        public static T? GetService<T>(this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            if (provider.GetService(typeof(T)) is T t)
                return t;
            return default;
        }

        public static bool IsAlive<TType>(this WeakReference<TType> reference)
            where TType : class 
            => reference.TryGetTarget(out _);

        public static DateTime Round(this DateTime source, RoundType type)
        {
            if (!Enum.IsDefined(typeof(RoundType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(RoundType));
            if (type == RoundType.None)
                throw new ArgumentNullException(nameof(type));

            return Round(source, (double) type);
        }

        public static DateTime Round(this DateTime source, double type)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (type == 0)
                throw new ArgumentNullException(nameof(type));

            var result = source;

            var minutes = type;

            Math.DivRem(source.Minute, (int) minutes, out var modulo);

            if (modulo <= 0) return result;

            result = modulo >= minutes / 2 ? source.AddMinutes(minutes - modulo) : source.AddMinutes(modulo * -1);

            result = result.AddSeconds(source.Second * -1);

            return result;
        }

        [StringFormatMethod("format")]
        public static string SFormat(this string format, params object[] args) 
            => string.Format(CultureInfo.InvariantCulture, format, args);

        public static TType? TypedTarget<TType>(this WeakReference<TType> reference) where TType : class 
            => (reference.TryGetTarget(out var obj) ? obj : default);
    }
}