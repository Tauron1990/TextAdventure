using JetBrains.Annotations;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Represents time. Used to pass time values to behaviour tree nodes.
    /// </summary>
    [PublicAPI]
    public struct TimeData
    {
        public TimeData(float deltaTime) 
            => DeltaTime = deltaTime;

        public float DeltaTime { get; }
    }
}
