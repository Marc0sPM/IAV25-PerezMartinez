using UnityEngine;
using System.Collections.Generic;
using UCM.IAV.Navegacion;

namespace UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Considerations/ForceComparison")]
    public class ForceComparison : Consideration
    {
        public override float Evaluate(Context context)
        {
            float myForce = context.GetData<float>("force");
            float targetForce = 0f;

            // Obtener la fuerza del objetivo (jugador)
            if (context.target != null)
            {
                var targetForceComp = context.target.GetComponent<Force>();
                if (targetForceComp != null)
                    targetForce = targetForceComp.normalizedForce;
            }

            if (targetForce <= 0f) return 1f; // Evitar división por 0

            // Si el minotauro está en un grupo, usar media + bonus
            int gId = context.agent.GetComponent<GroupComponent>().g_id;
            if (gId != -1 && MinoManager.Instance.minoGroups.ContainsKey(gId))
            {
                List<GameObject> group = MinoManager.Instance.minoGroups[gId];
                if (group.Count > 0)
                {
                    float totalForce = 0f;
                    foreach (var mino in group)
                    {
                        var force = mino.GetComponent<Force>();
                        if (force != null)
                            totalForce += force.normalizedForce;
                    }

                    float avgForce = totalForce / group.Count;
                    float bonus = Mathf.Sqrt(group.Count) / Force.maxForce;
                    myForce = avgForce + bonus / 2; 
                }
            }

            float ratio = myForce / targetForce;
            float response = Mathf.Pow(ratio, 2.6f); // más sensible a diferencias

            return Mathf.Clamp01(response);
        }
    }
}
