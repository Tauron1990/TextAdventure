using System;
using JetBrains.Annotations;

namespace FluentBehaviourTree.Nodes
{
    /// <summary>
    /// A behaviour tree leaf node for running an action.
    /// </summary>
    [PublicAPI]
    public class ActionNode : IBehaviourTreeNode
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        private string _name;

        /// <summary>
        /// Function to invoke for the action.
        /// </summary>
        private Func<TimeData, BehaviourTreeStatus> _fn;
        

        public ActionNode(string name, Func<TimeData, BehaviourTreeStatus> fn)
        {
            _name=name;
            _fn=fn;
        }

        public BehaviourTreeStatus Tick(TimeData time) 
            => _fn(time);
    }
}
