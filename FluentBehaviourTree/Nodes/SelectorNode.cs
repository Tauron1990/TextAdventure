using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FluentBehaviourTree.Nodes
{
    /// <summary>
    /// Selects the first node that succeeds. Tries successive nodes until it finds one that doesn't fail.
    /// </summary>
    [PublicAPI]
    public class SelectorNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        private string _name;

        /// <summary>
        /// List of child nodes.
        /// </summary>
        private readonly List<IBehaviourTreeNode> _children = new List<IBehaviourTreeNode>(); //todo: optimization, bake this to an array.

        public SelectorNode(string name) 
            => _name = name;

        public BehaviourTreeStatus Tick(TimeData time)
        {
            foreach (var childStatus in _children.Select(child => child.Tick(time)).Where(childStatus => childStatus != BehaviourTreeStatus.Failure))
                return childStatus;

            return BehaviourTreeStatus.Failure;
        }

        /// <summary>
        /// Add a child node to the selector.
        /// </summary>
        public void AddChild(IBehaviourTreeNode child) => _children.Add(child);
    }
}
