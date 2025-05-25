using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{ 
    [RequireComponent(typeof(NavMeshAgent), typeof(Sensor))]
    public class Brain : MonoBehaviour
    {
        public List<AIAction> actions;
        public Context context;


        #region Components 
        // Add any additional components that are needed for the considerations as Health, Energy, etc.
        public Force force;
        #endregion


        void Awake()
        {
            try
            {
                context = new Context(this);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create Context: {ex.Message}");
            }

            force = GetComponent<Force>();

            foreach (var action in actions)
            {
                action.Initialize(context);
            }
        }

        void Update()
        {
            UpdateContext(); 
            AIAction bestAction = EvaluateActions();
            if(context == null)
            {
                Debug.LogError("Context is null");
            }
            if (bestAction != null)
            {
                bestAction.Execute(context);
            }

        }

        void UpdateContext() {

            #region UpdateData
            // Update the context data with the info of the initialized componentes before.
            // Example: context.SetData<T>("health", healthComponent.normalizedHealth);
            context.SetData<float>("force", force.normalizedForce);
            #endregion
        }

        AIAction EvaluateActions()
        {
            AIAction bestAction = null;
            float bestUtility = float.MinValue;
            foreach (var action in actions)
            {
                float utility = action.CalculateUtility(context);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestAction = action;
                }
            }
            return bestAction;
        }
    }
}