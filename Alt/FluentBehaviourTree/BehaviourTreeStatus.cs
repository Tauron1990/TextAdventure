using JetBrains.Annotations;

namespace FluentBehaviourTree
{
    /// <summary>
    /// The return type when invoking behaviour tree nodes.
    /// </summary>
    [PublicAPI]
    public enum BehaviourTreeStatus
    {
        Success,
        Failure,
        Running
    }
}
