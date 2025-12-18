using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Relief Database")]
public class ReliefDatabase : ScriptableObject
{
    public List<ReliefData> reliefs = new();

    public ReliefData GetRelief(float height, float temperature)
    {
        ReliefData chosen = null;
        int highestPriority = int.MinValue;

        foreach (var relief in reliefs)
        {
            if (height < relief.minHeight || height > relief.maxHeight)
                continue;

            if (temperature < relief.minTemperature ||
                temperature > relief.maxTemperature)
                continue;

            if (relief.priority > highestPriority)
            {
                highestPriority = relief.priority;
                chosen = relief;
            }
        }

        return chosen;
    }
}
