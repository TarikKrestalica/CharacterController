using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] public List<ItemDefinition> items;
    [SerializeField] private GameObject objectTemplate;
    public ItemDefinition currentlyEquippedItem;
    public void AddItem(ItemDefinition item)
    {
        items.Add(item);
    }

    public void RemoveItem(ItemDefinition item)
    { 
        items.Remove(item);
    }

    public void RunPickupLogic(ItemDefinition item)
    {
        if(items.Count > 0)  // If there was an item, remove its effects!
        {
            ItemDefinition previousItem = items[items.Count - 1];
            if(previousItem.name != "Heart")  // Add to the effects, not cancel and reapply
            {
                DeEquipItem(previousItem);
            } 
        }

        if (!IsThereExistingItem(item))
        {
            AddItem(item);
        }

        EquipItem(item);
        currentlyEquippedItem = item;
    }

    public ItemDefinition FindNextItemToTheRight()
    {
        int curIndex = items.IndexOf(currentlyEquippedItem);
        if(currentlyEquippedItem.name != "Heart")
        {
            DeEquipItem(items[curIndex]);
        }
       
        if (curIndex + 1 >= items.Count)
        {
            curIndex = 0;
        }
        else
        {
            ++curIndex;
        }
        currentlyEquippedItem = items[curIndex];
        return items[curIndex];
    }

    public ItemDefinition FindNextItemToTheLeft()
    {
        int curIndex = items.IndexOf(currentlyEquippedItem);
        if (currentlyEquippedItem.name != "Heart")
        {
            DeEquipItem(items[curIndex]);
        }

        if (curIndex - 1 < 0)
        {
            curIndex = items.Count - 1;
        }
        else
        {
            --curIndex;
        }

        currentlyEquippedItem = items[curIndex];
        return items[curIndex];
    }

    // Modify to new stats, set up previous values for dequiping, except health
    public void EquipItem(ItemDefinition item)
    {
        for (int i = 0; i < item.statUpdates.Count; i++)
        {
            StatType curType = item.statUpdates[i].statType;
            float value = item.statUpdates[i].currentValue;
            this.gameObject.GetComponent<StatSystem>().TryToUpdateStats(curType, value);
            Debug.Log("New value for " + curType + " is " + this.gameObject.GetComponent<StatSystem>().FindCurrentValue(curType));
        }

        if (!item.IsPickedUpForFirstTime())
        {
            item.SetFirstTimeStatus(true);
            item.amount = 1;
        }
        else
        {
            item.amount += 1;
        }
    }

    public void ReEquipItem(ItemDefinition item)
    {
        for (int i = 0; i < item.statUpdates.Count; i++)
        {
            StatType curType = item.statUpdates[i].statType;
            float value = item.statUpdates[i].currentValue;
            this.gameObject.GetComponent<StatSystem>().TryToUpdateStats(curType, value);
            Debug.Log("New value for " + curType + " is " + this.gameObject.GetComponent<StatSystem>().FindCurrentValue(curType));
        }

        if (item.tag == "Item")  // Visual cues for items should be kept
        {
            GameManager.playerController.itemHolder.sprite = item.sprite;
        }
    }

    public void DeEquipItem(ItemDefinition item)
    {
        for (int i = 0; i < item.statUpdates.Count; i++)
        {
            StatType curType = item.statUpdates[i].statType;
            float value = item.statUpdates[i].currentValue;
            this.gameObject.GetComponent<StatSystem>().TryToUpdateStats(curType, value * -1);
        }

        if (item.tag == "Item")  // Visual cues for items should be kept
        {
            GameManager.playerController.itemHolder.sprite = null;
        }
    }

    // Multiplying the boost by negative 1 will remove the item's effects
    public void UnequipItem(ItemDefinition item)
    {
        for (int i = 0; i < item.statUpdates.Count; i++)
        {
            StatType curType = item.statUpdates[i].statType;
            float value = item.statUpdates[i].currentValue;
            this.gameObject.GetComponent<StatSystem>().TryToUpdateStats(curType, value * -1);
        }

        item.amount -= 1;
        if (item.amount == 0)
        {
            item.SetFirstTimeStatus(false);
        }
    }
    public bool IsThereExistingItem(ItemDefinition item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == item.name)
                return true;
        }

        return false;
    }

    public void DropItem()
    {
        int curIndex = items.Count - 1;
        ItemDefinition mostRecentItem = items[curIndex];
        PopulateItemInGame(mostRecentItem);
        UnequipItem(mostRecentItem);

        // Nothing left
        if (mostRecentItem.amount < 1)
        {
            items.Remove(mostRecentItem);
            if (items.Count == 0)
            {
                currentlyEquippedItem = null;
            }
            else
            {
                currentlyEquippedItem = items[items.Count - 1];
            }
        }

        if (mostRecentItem.tag == "Item")  // Visual cues for items should be kept
        {
            GameManager.playerController.itemHolder.sprite = null;
        }

        
    }

    public void PopulateItemInGame(ItemDefinition itemDef)
    {
        // Knew the idea but needed the syntax to place it in front of the player : https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
        GameObject item = Instantiate(objectTemplate, this.gameObject.transform.position + this.gameObject.transform.right * 1.5f, Quaternion.identity, GameManager.environment.transform);
        item.GetComponent<ItemDisplay>().itemDef = itemDef;
        item.tag = itemDef.tag;
    }
}

