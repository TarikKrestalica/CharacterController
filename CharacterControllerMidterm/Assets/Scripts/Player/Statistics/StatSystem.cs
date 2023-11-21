using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StatSystem : MonoBehaviour
{
    public List<Stat> statTypes = new List<Stat>();
    private Dictionary<StatType, StatFunctionalityUpdate> statFunctionalities = new Dictionary<StatType, StatFunctionalityUpdate>();
    public delegate void StatFunctionalityUpdate(StatType statType, float aValue);

    private void Start()
    {
        StartCoroutine(SetUpStats());
    }

    IEnumerator SetUpStats()
    {
        yield return new WaitForEndOfFrame();
        foreach (Stat stat in statTypes)  // Should be called after adding all callbacks!
        {
            SetStat(stat.statType, stat.initialValue);
        }
    }

    public void LowerStat(StatType statType, float aValue)
    {
        for(int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                if (statTypes[i].currentValue - aValue < statTypes[i].minValue)
                    continue;

                statTypes[i].currentValue -= aValue;
                if (!statFunctionalities.ContainsKey(statType))  // Nothing to do!
                {
                    return;
                }

                statFunctionalities[statType].Invoke(statType, statTypes[i].currentValue);
            }
        }
    }

    // Set current values to initial values
    public void SetStat(StatType statType, float aValue)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                statTypes[i].currentValue = aValue;
                if (!statFunctionalities.ContainsKey(statType))  // Nothing to do!
                {
                    Debug.LogError(statType + " has no functionalities");
                    return;
                }

                statFunctionalities[statType].Invoke(statType, statTypes[i].currentValue);
            }
        }
    }
 
    // Add some functionality and assign it to the appropriate stat type to be triggered
    public void AddCallBack(StatType statType, StatFunctionalityUpdate Func)
    {
        if (!statFunctionalities.ContainsKey(statType))
        {
            statFunctionalities[statType] = Func;
        }
        else
        {
            statFunctionalities[statType] += Func;
        }
    }

    public void AddStat(StatType statType, float aValue)  // Add stat boost, used when I collect hearts and stars
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                if (statTypes[i].currentValue + aValue > statTypes[i].maxValue)
                    return;

                statTypes[i].currentValue += aValue;

                if(!statFunctionalities.ContainsKey(statType))  // Nothing to do!
                {
                    Debug.LogError(statType + " has no functionalities");
                    return;
                }
                statFunctionalities[statType].Invoke(statType, statTypes[i].currentValue);
            }
        }
    }

    public float FindInitialValue(StatType statType)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                return statTypes[i].initialValue;
            }
        }
        return 0;
    }

    public float FindCurrentValue(StatType statType)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                return statTypes[i].currentValue;
            }
        }
        return 0;
    }

    public float FindMaxValue(StatType statType)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                return statTypes[i].maxValue;
            }
        }
        return 0;
    }

    public float FindMinValue(StatType statType)
    {
        for (int i = 0; i < statTypes.Count; i++)
        {
            if (statTypes[i].statType == statType)
            {
                return statTypes[i].minValue;
            }
        }
        return 0;
    }

    public bool StatInCollection(StatType statType)
    {
        foreach(Stat type in statTypes)
        {
            if (type.statType == statType)
                return true;
        }

        return false;
    }
    
    public void TryToUpdateStats(StatType statType, float boost)
    {
        // Retrieve current value and bounds
        float currentValue = FindCurrentValue(statType);
        float maxValue = FindMaxValue(statType);
        float minValue = FindMinValue(statType);

        float partialUpdate = 0;

        // Will the boost go over the bound?
        if (currentValue + boost >= minValue && currentValue + boost < maxValue)
        {
            AddStat(statType, boost);
        }
        else if(currentValue + boost > maxValue)
        {
            partialUpdate = maxValue - currentValue;
            AddStat(statType, partialUpdate);

        }
        else if(currentValue + boost < minValue)
        {
            partialUpdate = currentValue - minValue;
            LowerStat(statType, partialUpdate);
        }
    }
}
