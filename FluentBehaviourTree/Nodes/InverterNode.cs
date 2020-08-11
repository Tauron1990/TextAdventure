using System;
using JetBrains.Annotations;

namespace FluentBehaviourTree.Nodes
{
    /// <summary>
    /// Decorator node that inverts the success/failure of its child.
    /// </summary>
    [PublicAPI]
    public class InverterNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        private string _name;

        /// <summary>
        /// The child to be inverted.
        /// </summary>
        private IBehaviourTreeNode _childNode;

        public InverterNode(string name)
        {
            _name = name;
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {
            if (_childNode == null)
                throw new ApplicationException("InverterNode must have a child node!");

            var result = _childNode.Tick(time);
            switch (result)
            {
                case BehaviourTreeStatus.Failure:
                    return BehaviourTreeStatus.Success;
                case BehaviourTreeStatus.Success:
                    return BehaviourTreeStatus.Failure;
                default:
                    return result;
            }
        }

        /// <summary>
        /// Add a child to the parent node.
        /// </summary>
        public void AddChild(IBehaviourTreeNode child)
        {
            if (_childNode != null)
                throw new ApplicationException("Can't add more than a single child to InverterNode!");

            _childNode = child;
        }
    }
}
