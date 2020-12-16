using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using JetBrains.Annotations;
using Tauron.Application.Workshop.Mutating;
using Tauron.Application.Workshop.StateManagement.Attributes;
using Tauron.Application.Workshop.StateManagement.DataFactorys;

namespace Tauron.Application.Workshop.StateManagement.Internal
{
    public class ReflectionSearchEngine
    {
        private static readonly MethodInfo ConfigurateStateMethod = typeof(ReflectionSearchEngine).GetMethod(nameof(ConfigurateState), BindingFlags.Instance | BindingFlags.NonPublic)
         ?? throw new InvalidOperationException("Method not Found");

        private readonly Assembly _assembly;
        private readonly IComponentContext? _context;

        public ReflectionSearchEngine(Assembly assembly, IComponentContext? context)
        {
            _assembly = assembly;
            _context = context;
        }

        public void Add(ManagerBuilder builder, IDataSourceFactory factory)
        {
            Func<TType> CreateFactory<TType>(Type target)
            {
                if (_context != null)
                    return () => (TType) (_context.ResolveOptional(target) ?? Activator.CreateInstance(target))!;
                return () => (TType)Activator.CreateInstance(target)!;
            }

            var types = _assembly.GetTypes();
            var states = new List<(Type, string?)>();
            var reducers = new GroupDictionary<Type, Type>();
            var factorys = new List<AdvancedDataSourceFactory>();
            var processors = new List<Type>();

            foreach (var type in types)
            {
                foreach (var customAttribute in type.GetCustomAttributes(false))
                {
                    switch (customAttribute)
                    {
                        case StateAttribute state:
                            states.Add((type, state.Key));
                            break;
                        case EffectAttribute _:
                            builder.WithEffect(CreateFactory<IEffect>(type));
                            break;
                        case MiddlewareAttribute _:
                            builder.WithMiddleware(CreateFactory<IMiddleware>(type));
                            break;
                        case BelogsToStateAttribute belogsTo:
                            reducers.Add(belogsTo.StateType, type);
                            break;
                        case DataSourceAttribute _:
                            factorys.Add((AdvancedDataSourceFactory)(_context?.ResolveOptional(type) ?? Activator.CreateInstance(type)));
                            break;
                        case ProcessorAttribute _:
                            processors.Add(type);
                            break;
                    }
                }
            }

            if (factorys.Count != 0)
            {
                factorys.Add((AdvancedDataSourceFactory)factory);
                factory = MergeFactory.Merge(factorys.ToArray());
            }

            foreach (var (type, key) in states)
            {
                if(type == null || type.BaseType?.IsGenericType != true || type.BaseType?.GetGenericTypeDefinition() != typeof(StateBase<>)) 
                    continue;

                var dataType = type.BaseType.GetGenericArguments()[0];
                var actualMethod = ConfigurateStateMethod.MakeGenericMethod(dataType);
                actualMethod.Invoke(this, new object?[] {type, builder, factory, reducers, key});
            }

            foreach (var processor in processors) 
                builder.Superviser.CreateAnonym(processor, $"Processor--{processor.Name}");
        }

        private void ConfigurateState<TData>(Type target, ManagerBuilder builder, IDataSourceFactory factory, GroupDictionary<Type, Type> reducerMap, string? key)
            where TData : class, IStateEntity
        {
            var config = builder.WithDataSource(factory.Create<TData>());

            if (!string.IsNullOrWhiteSpace(key))
                config.WithKey(key);

            config.WithStateType(target);

            if (!reducerMap.TryGetValue(target, out var reducers)) return;
            
            var methods = new Dictionary<Type, MethodInfo>();
            var validators = new Dictionary<Type, object>();

            foreach (var reducer in reducers)
            {
                foreach (var method in reducer.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if(!method.HasAttribute<ReducerAttribute>())
                        continue;

                    var parms = method.GetParameters();
                    if(parms.Length != 2)
                        continue;
                    if(!parms[0].ParameterType.IsGenericType)
                        continue;
                    if(parms[0].ParameterType.GetGenericTypeDefinition() != typeof(MutatingContext<>))
                        continue;
                    methods[parms[1].ParameterType] = method;
                }

                foreach (var property in reducer.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if(!property.HasAttribute<ValidatorAttribute>())
                        continue;

                    var potenialValidator = property.PropertyType;
                    if(!potenialValidator.IsAssignableTo<IValidator>())
                        continue;

                    var validatorType = potenialValidator.GetInterface(typeof(IValidator<>).Name);
                    if(validatorType == null)
                        continue;
                    var validator = property.GetValue(null);
                    if(validator == null)
                        continue;

                    validators[validatorType.GenericTypeArguments[0]] = validator;
                }
            }

            foreach (var (actionType, reducer) in methods)
            {
                Type? delegateType = null;

                //Sync Version
                var returnType = reducer.ReturnType;
                if (returnType == typeof(MutatingContext<TData>))
                    delegateType = typeof(Func<,,>).MakeGenericType(typeof(MutatingContext<TData>), actionType, typeof(MutatingContext<TData>));
                else if (returnType == typeof(ReducerResult<TData>))
                    delegateType = typeof(Func<,,>).MakeGenericType(typeof(MutatingContext<TData>), actionType, typeof(ReducerResult<TData>));
                else if (returnType.IsAssignableTo<TData>())
                    delegateType = typeof(Func<,,>).MakeGenericType(typeof(MutatingContext<TData>), actionType, typeof(TData));

                //AsyncVersion
                if (returnType == typeof(Task<MutatingContext<TData>>))
                    delegateType = typeof(Func<,,>).MakeGenericType(typeof(MutatingContext<TData>), actionType, typeof(Task<MutatingContext<TData>>));
                else if (returnType == typeof(Task<ReducerResult<TData>>))
                    delegateType = typeof(Func<,,>).MakeGenericType(typeof(MutatingContext<TData>), actionType, typeof(Task<ReducerResult<TData>>));
                else if (returnType.IsAssignableTo<Task<TData>>())
                    delegateType = typeof(Func<,,>).MakeGenericType(typeof(MutatingContext<TData>), actionType, typeof(Task<TData>));

                if (delegateType == null)
                    continue;

                var acrualDelegate = Delegate.CreateDelegate(delegateType, reducer);
                object? validator = null;
                if (validators.ContainsKey(actionType))
                    validator = validators[actionType];

                var constructedReducer = typeof(DelegateReducer<,>).MakeGenericType(actionType, typeof(TData));
                var reducerInstance = Activator.CreateInstance(constructedReducer, acrualDelegate, validator);

                config.WithReducer(() => (IReducer<TData>) reducerInstance);
            }
        }

        private static class ReducerBuilder
        {
            private static MethodInfo GenericBuilder = Reflex.MethodInfo(() => ReducerBuilder.Create<string, string>(null!)).GetGenericMethodDefinition();
            
            public static ReducerBuilderBase? Create<TData>(MethodInfo info, Type actionType) 
                => GenericBuilder.MakeGenericMethod(typeof(TData), actionType).Invoke(null, new object[] {info}) as ReducerBuilderBase;

            [UsedImplicitly]
            private static ReducerBuilderBase? Create<TData, TAction>(MethodInfo info)
            {
                var returnType = info.ReturnType;
                var parms = info.GetParameterTypes().ToArray();
                if (parms.Length != 2 && parms[1] != typeof(TAction))
                    return null;

                var parm = parms[0];

                //One to One
                if (returnType == typeof(IObservable<ReducerResult<TData>>) && parm == typeof(IObservable<MutatingContext<TData>>))
                    return new ReducerBuilder<TData, TAction>.OneToOneMap(info);
                
                //Observable Variants
                if (returnType == typeof(IObservable<MutatingContext<TData>>) && parm == typeof(IObservable<MutatingContext<TData>>))
                    return new ReducerBuilder<TData, TAction>.SameTypeMap(info);
                if (parm == typeof(IObservable<MutatingContext<TData>>) && returnType == typeof(IObservable<TData>))
                    return new ReducerBuilder<TData, TAction>.DataToDataMap(info);
                if (parm == typeof(IObservable<MutatingContext<TData>>) && returnType == typeof(IObservable<ReducerResult<TData>>))
                    return new ReducerBuilder<TData, TAction>.DataToContextMap(info);

                if (parm == typeof(IObservable<TData>) && returnType == typeof(IObservable<TData>))
                    return new ReducerBuilder<TData, TAction>.DataToDataMap(info);
                if (parm == typeof(IObservable<TData>) && returnType == typeof(IObservable<MutatingContext<TData>>))
                    return new ReducerBuilder<TData, TAction>.DataToContextMap(info);
                if (parm == typeof(IObservable<TData>) && returnType == typeof(IObservable<ReducerResult<TData>>))
                    return new ReducerBuilder<TData, TAction>.DataToResultMap(info);

                return null;
            }
        }

        private abstract class ReducerBuilderBase
        {
            protected delegate IObservable<ReducerResult<TData>> ReducerDel<TData, in TAction>(IObservable<MutatingContext<TData>> state, TAction action);

            protected static TDelegate CreateDelegate<TDelegate>(MethodInfo method)
                where TDelegate : Delegate => (TDelegate) Delegate.CreateDelegate(typeof(TDelegate), method);
        }

        private abstract class ReducerBuilder<TData, TAction> : ReducerBuilderBase
        {
            private readonly MethodInfo _info;
            private ReducerDel<TData, TAction>? _reducer;

            protected ReducerBuilder(MethodInfo info) => _info = info;

            protected abstract ReducerDel<TData, TAction> Build(MethodInfo info);

            public IObservable<ReducerResult<TData>> Reduce(IObservable<MutatingContext<TData>> state, TAction action)
            {
                _reducer ??= Build(_info);
                return _reducer(state, action);
            }

            public sealed class OneToOneMap : ReducerBuilder<TData, TAction>
            {
                public OneToOneMap(MethodInfo info) 
                    : base(info) { }

                protected override ReducerDel<TData, TAction> Build(MethodInfo info) => CreateDelegate<ReducerDel<TData, TAction>>(info);
            }
            public sealed class SameTypeMap : ReducerBuilder<TData, TAction>
            {
                public SameTypeMap([NotNull] MethodInfo info) : base(info) { }
                protected override ReducerDel<TData, TAction> Build(MethodInfo info)
                {
                    var del = CreateDelegate<Func<IObservable<MutatingContext<TData>>, TAction, IObservable<MutatingContext<TData>>>>(info);

                    return (state, action) => del(state, action).Select(ReducerResult.Sucess);
                }
            }
            public sealed class SameTypeMap : ReducerBuilder<TData, TAction>
            {
                public SameTypeMap([NotNull] MethodInfo info) : base(info) { }
                protected override ReducerDel<TData, TAction> Build(MethodInfo info)
                {
                    var del = CreateDelegate<Func<IObservable<MutatingContext<TData>>, TAction, IObservable<MutatingContext<TData>>>>(info);

                    return (state, action) => del(state, action).Select(ReducerResult.Sucess);
                }
            }
            public sealed class SameTypeMap : ReducerBuilder<TData, TAction>
            {
                public SameTypeMap([NotNull] MethodInfo info) : base(info) { }
                protected override ReducerDel<TData, TAction> Build(MethodInfo info)
                {
                    var del = CreateDelegate<Func<IObservable<MutatingContext<TData>>, TAction, IObservable<MutatingContext<TData>>>>(info);

                    return (state, action) => del(state, action).Select(ReducerResult.Sucess);
                }
            }
            public sealed class DataToContextMap : ReducerBuilder<TData, TAction>
            {
                public DataToContextMap([NotNull] MethodInfo info) : base(info) { }
                protected override ReducerDel<TData, TAction> Build(MethodInfo info)
                {
                    var del = CreateDelegate<Func<IObservable<TData>, TAction, IObservable<MutatingContext<TData>>>>(info);

                    return (state, action) => del(state.Select(c => c.Data), action).Select(ReducerResult.Sucess);
                }
            }
            public sealed class DataToResultMap : ReducerBuilder<TData, TAction>
            {
                public DataToResultMap([NotNull] MethodInfo info) : base(info) { }
                protected override ReducerDel<TData, TAction> Build(MethodInfo info)
                {
                    var del = CreateDelegate<Func<IObservable<TData>, TAction, IObservable<ReducerResult<TData>>>>(info);

                    return (state, action) => del(state.Select(c => c.Data), action);
                }
            }
            public sealed class DataToDataMap : ReducerBuilder<TData, TAction>
            {
                public DataToDataMap([NotNull] MethodInfo info) : base(info) { }
                protected override ReducerDel<TData, TAction> Build(MethodInfo info)
                {
                    var del = CreateDelegate<Func<IObservable<TData>, TAction, IObservable<TData>>>(info);

                    return (state, action) => del(state.Select(c => c.Data), action).Select(d => ReducerResult.Sucess(MutatingContext<TData>.New(d)));
                }
            }
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
        private sealed class DelegateReducer<TAction, TData> : Reducer<TAction, TData> 
            where TData : IStateEntity 
            where TAction : IStateAction
        {
            private readonly ReducerBuilder<TData, TAction> _builder;

            public DelegateReducer(ReducerBuilder<TData, TAction> builder, IValidator<TAction>? validation)
            {
                _builder = builder;
                Validator = validation;
            }
            

            public override IValidator<TAction>? Validator { get; }
            
            protected override IObservable<ReducerResult<TData>> Reduce(IObservable<MutatingContext<TData>> state, TAction action) 
                => _builder.Reduce(state, action);
        }
    }
}