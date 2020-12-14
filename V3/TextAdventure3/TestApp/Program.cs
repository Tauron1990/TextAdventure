
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            foreach (var i in Fib(1000000))
            {
                Console.WriteLine(i);
                if(i > 1_000_000_000)
                    break;
            }
            
            Console.WriteLine(FibSpec(0));
            Console.WriteLine(FibSpec(10));
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