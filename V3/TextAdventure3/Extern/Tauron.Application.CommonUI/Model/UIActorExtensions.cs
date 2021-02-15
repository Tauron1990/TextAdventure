using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using JetBrains.Annotations;
using Tauron.Akka;
using Tauron.Application.CommonUI.Helper;

namespace Tauron.Application.CommonUI.Model
{
    [PublicAPI]
    public static class UIActorExtensions
    {
        public static IDisposable LinkProperty<TData>(this ObservableObject source, IActorProperty<TData> target,
            Expression<Func<TData>> property)
        {
            return new CompositeDisposable
                   {
                       source.WhenAny(property).Where(d => !Equals(target.Value, d)).Subscribe(d => target.Value = d)
                   };
        }

        public static UIModel<TModel> RegisterViewModel<TModel>(this UiActor actor, string name,
            IViewModel<TModel>? model = null)
            where TModel : class
        {
            model ??= actor.LifetimeScope.Resolve<IViewModel<TModel>>();

            if (!model.IsInitialized)
                model.InitModel(ObservableActor.ExposedContext, name);

            return new UIModel<TModel>(
                actor.RegisterProperty<IViewModel<TModel>>(name).WithDefaultValue(model).Property);
        }

        public static FluentCollectionPropertyRegistration<TData> RegisterUiCollection<TData>(this UiActor actor,
            string name)
        {
            actor.ThrowIsSeald();
            return new FluentCollectionPropertyRegistration<TData>(name, actor);
        }
    }
}