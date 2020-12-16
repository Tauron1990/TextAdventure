
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Program
    {
        private static TData Test<TData>(TData s)
            => s;

        private string Test2(string s)
            => s;
        
        private static void Main(string[] args)
        {
            
            var one = GetMethodInfo(() => Program.Test<string>(null));
            var two = GetMethodInfo<Program>(p => p.Test2(null));

            Console.ReadKey();
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            if (expression.Body is MethodCallExpression member)
                return member.Method;

            throw new ArgumentException("Expression is not a method", nameof(expression));
        }

        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            if (expression.Body is MethodCallExpression member)
                return member.Method;

            throw new ArgumentException("Expression is not a method", nameof(expression));
        }
    }
}