using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FluentBehaviourTree.Nodes
{
    /// <summary>
    /// Runs child nodes in sequence, until one fails.
    /// </summary>
    [PublicAPI]
    public class SequenceNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        private string _name;

        /// <summary>
        /// List of child nodes.
        /// </summary>
        private readonly List<IBehaviourTreeNode> _children = new List<IBehaviourTreeNode>(); //todo: this could be optimized as a baked array.

        public SequenceNode(string name)
        {
            _name = name;
        }

        public BehaviourTreeStatus Tick(TimeData time) 
            => _children.Select(child => child.Tick(time)).FirstOrDefault(childStatus => childStatus != BehaviourTreeStatus.Success);

        /// <summary>
        /// Add a child to the sequence.
        /// </summary>
        public void AddChild(IBehaviourTreeNode child) 
            => _children.Add(child);
    }
}
