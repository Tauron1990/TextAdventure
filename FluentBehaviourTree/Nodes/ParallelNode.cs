using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FluentBehaviourTree.Nodes
{
    /// <summary>
    /// Runs childs nodes in parallel.
    /// </summary>
    [PublicAPI]
    public class ParallelNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        private string _name;

        /// <summary>
        /// List of child nodes.
        /// </summary>
        private List<IBehaviourTreeNode> _children = new List<IBehaviourTreeNode>();

        /// <summary>
        /// Number of child failures required to terminate with failure.
        /// </summary>
        private int _numRequiredToFail;

        /// <summary>
        /// Number of child successess require to terminate with success.
        /// </summary>
        private int _numRequiredToSucceed;

        public ParallelNode(string name, int numRequiredToFail, int numRequiredToSucceed)
        {
            _name = name;
            _numRequiredToFail = numRequiredToFail;
            _numRequiredToSucceed = numRequiredToSucceed;
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {
            var numChildrenSuceeded = 0;
            var numChildrenFailed = 0;

            foreach (var childStatus in _children.Select(child => child.Tick(time)))
            {
                switch (childStatus)
                {
                    case BehaviourTreeStatus.Success: ++numChildrenSuceeded; break;
                    case BehaviourTreeStatus.Failure: ++numChildrenFailed; break;
                }
            }

            if (_numRequiredToSucceed > 0 && numChildrenSuceeded >= _numRequiredToSucceed)
                return BehaviourTreeStatus.Success;

            if (_numRequiredToFail > 0 && numChildrenFailed >= _numRequiredToFail)
                return BehaviourTreeStatus.Failure;

            return BehaviourTreeStatus.Running;
        }

        public void AddChild(IBehaviourTreeNode child) 
            => _children.Add(child);
    }
}
