using JetBrains.Annotations;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Interface for behaviour tree nodes.
    /// </summary>
    [PublicAPI]
    public interface IParentBehaviourTreeNode : IBehaviourTreeNode
    {
        /// <summary>
        /// Add a child to the parent node.
        /// </summary>
        void AddChild(IBehaviourTreeNode child);
    }
}
