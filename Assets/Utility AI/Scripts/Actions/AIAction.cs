using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UtilityAI {
    public abstract class AIAction : ScriptableObject
    {
        /// <summary>
        /// Tag used to identify the target object.
        /// </summary>
        public string targetTag;

        /// <summary>
        /// Optional initialization logic for the action.
        /// </summary>
        /// <param name="context">The context in which the action is initialized.</param>
        public virtual void Initialize(Context context)
        { 
        }

        /// <summary>
        /// A consideration object used to evaluate the utility of the action.
        /// </summary>
        public Consideration consideration;

        /// <summary>
        /// Calculates the utility of the action based on the provided context.
        /// </summary>
        /// <param name="context">The context in which the utility is calculated.</param>
        /// <returns>The calculated utility value.</returns>
        public float CalculateUtility(Context context) => consideration.Evaluate(context);

        /// <summary>
        /// Executes the action. Must be implemented by derived classes.
        /// </summary>
        /// <param name="context">The context in which the action is executed.</param>
        public abstract void Execute(Context context);
    }
}
