using System;
using JetBrains.Annotations;
using Tauron.Application.Workflow;

namespace Tauron.Application.ActorWorkflow
{
    public delegate StepId LambdaExecution<TContext>(LambdaStep<TContext> step, TContext context);

    public delegate void LambdaFinish<in TContext>(TContext context);

    [PublicAPI]
    public sealed class LambdaStep<TContext> : IStep<TContext>, IHasTimeout
    {
        private readonly LambdaExecution<TContext>? _onExecute;
        private readonly LambdaExecution<TContext>? _onNextElement;
        private readonly LambdaFinish<TContext>? _onFinish;

        public string? ErrorMessage { get; set; }


        public LambdaStep(
            LambdaExecution<TContext>? onExecute = default,
            LambdaExecution<TContext>? onNextElement = default,
            LambdaFinish<TContext>? onFinish = default, TimeSpan? timeout = default)
        {
            Timeout = timeout;
            _onExecute = onExecute;
            _onNextElement = onNextElement;
            _onFinish = onFinish;
        }

        public StepId OnExecute(TContext context) => _onExecute?.Invoke(this, context) ?? StepId.None;

        public StepId NextElement(TContext context) => _onNextElement?.Invoke(this, context) ?? StepId.None;

        public void OnExecuteFinish(TContext context) => _onFinish?.Invoke(context);

        public TimeSpan? Timeout { get; }

        public void SetError(string error)
            => ErrorMessage = error;
    }

    [PublicAPI]
    public sealed class LambdaStepConfiguration<TContext>
    {
        private LambdaExecution<TContext>? _onExecute;
        private LambdaExecution<TContext>? _onNextElement;
        private LambdaFinish<TContext>? _onFinish;
        private TimeSpan? _timeout;

        public void OnExecute(LambdaExecution<TContext> func) 
            => _onExecute = _onExecute.Combine(func);

        public void OnNextElement(LambdaExecution<TContext> func) 
            => _onNextElement = _onNextElement.Combine(func);

        public void OnExecute(Func<TContext, StepId> func)
            => OnExecute((_, context) => func(context));

        public void OnNextElement(Func<TContext, StepId> func)
            => OnNextElement((_, context) => func(context));

        public void OnFinish(LambdaFinish<TContext> func) 
            => _onFinish = _onFinish.Combine(func);

        public void WithTimeout(TimeSpan? timeout)
            => _timeout = timeout;


        public LambdaStep<TContext> Build() 
            => new(_onExecute, _onNextElement, _onFinish, _timeout);
    }
}