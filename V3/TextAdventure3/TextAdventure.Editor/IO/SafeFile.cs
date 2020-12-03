using System;
using System.IO;
using System.Reactive.Linq;

namespace TextAdventure.Editor.IO
{
    public sealed record IoResult<TTResult>(Exception? Error, TTResult Result, string OriginalPath);

    public static class MapIoResult
    {
        public static IObservable<TValue> Map<TResult, TValue>(this IObservable<IoResult<TResult>> op, Func<TResult, TValue> mapper, Func<Exception, TValue> errorMapper) 
            => op.Select(r => r.Error != null ? errorMapper(r.Error) : mapper(r.Result));

        public static IObservable<TValue> Map<TResult, TValue>(this IObservable<IoResult<TResult>> op, Func<TResult, string, TValue> mapper, Func<Exception, string, TValue> errorMapper)
            => op.Select(r => r.Error != null ? errorMapper(r.Error, r.OriginalPath) : mapper(r.Result, r.OriginalPath));
    }

    public static class SafeFile
    {
        public static IObservable<IoResult<bool>> SafeFileExists(this IObservable<string> paths)
        {
            return paths.Select(s =>
                                {
                                    try
                                    {
                                        return new IoResult<bool>(null, File.Exists(s), s);
                                    }
                                    catch (Exception e)
                                    {
                                        return new IoResult<bool>(e, false, s);
                                    }
                                });
        }

        public static IObservable<IoResult<string>> SafeReadAllText(this IObservable<string> paths)
            => paths.Select(p =>
                            {
                                try
                                {
                                    return new IoResult<string>(null, File.ReadAllText(p), p);
                                }
                                catch (Exception e)
                                {
                                    return new IoResult<string>(e, string.Empty, p);
                                }
                            });

    }
}