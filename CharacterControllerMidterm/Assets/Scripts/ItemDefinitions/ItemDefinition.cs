using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Review and help with the implementation: https://youtu.be/aPXvoWVabPY?si=1AWhj9oeQvfGqudt
public class ItemDefinition : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int amount;
    public string tag;
    public bool pickedUpForFirstTime;

    public List<Stat> statUpdates;

    // Do we have the type of stat that needs to be modified?
    public bool StatExists(StatType type)
    {
        for(int i = 0; i < statUpdates.Count; i++)
        {
            if (statUpdates[i].statType == type)
                return true;
        }
        return false;
    }

    public float GetStatValue(StatType type)
    {
        for(int i = 0; i < statUpdates.Count; i++)
        {
            if (statUpdates[i].statType == type)  // Apply value
            {
                return statUpdates[i].currentValue;
            }
        }
        return 0;  // Default case
    }

    public void SetFirstTimeStatus(bool toggle)
    {
        pickedUpForFirstTime = toggle;
    }

    public bool IsPickedUpForFirstTime()
    {
        return pickedUpForFirstTime;
    }
}
