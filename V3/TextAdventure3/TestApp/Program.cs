
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Program
    {
        private static string Test(string s)
            => s;
        
        private static void Main(string[] args)
        {
            var neu = new Func<string, string>(Test);

            var test = Observable.StartAsync(() =>

                                                 Task.Run(async () =>
                                                          {
                                                              await Task.Delay(TimeSpan.FromMilliseconds(5000));
                                                              return true;
                                                          })
                                            );

            var bad = test.Where(f => !f).Select(b => "Bad Hallo");
            var good = test.Where(f => f).Select(_ => "Good Hallo");

            bad.Concat(good).Subscribe(Console.WriteLine);

            Console.ReadKey();
        }

        private static IEnumerable<int> Fib(int count)
        {
            int? first = null;
            int? second = null;

            for (int i = 0; i < count; i++)
            {
                if (first == null)
                {
                    first = 1;
                    yield return 1;
                    continue;
                }

                if (second == null)
                {
                    second = 1;
                    yield return 1;
                    continue;
                }

                var next = first.Value + second.Value;
                second = first;
                first = next;

                yield return next;
            }
        }
        private static int FibSpec(int count)
        {
            int? first = null;
            int? second = null;

            for (int i = 0; i < count; i++)
            {
                if (first == null)
                {
                    first = 1;
                    continue;
                }

                if (second == null)
                {
                    second = 1;
                    continue;
                }

                var next = first.Value + second.Value;
                second = first;
                first = next;
            }

            return first ?? 0;
        }
    }
}