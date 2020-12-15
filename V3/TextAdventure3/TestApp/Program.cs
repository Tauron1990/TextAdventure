
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Tauron;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var test = Observable.StartAsync(() =>

                                                 Task.Run(async () =>
                                                          {
                                                              await Task.Delay(TimeSpan.FromMilliseconds(5000));
                                                              return "Hallo World";
                                                          })
                                            );

            test.LastAsync().Subscribe(Console.WriteLine, () => Console.WriteLine("Fertig"));

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