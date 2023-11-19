using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private List<GameObject> itemDisplays;

    // Left to right, top down
    public void GenerateUI(List<ItemDefinition> items)
    {
        float leftBound = -this.transform.parent.position.x;
        float topBound = this.transform.parent.position.y;
        int line = 0;

        for (int i = 0; i < items.Count; i++)
        {
            float result = i % 7;
            if(result == 0)
            {
                line++;
            }

            GameObject newItem = Instantiate(itemSlot, this.transform);
            newItem.transform.localPosition = new Vector3(leftBound + ((result + 1) * 100), topBound - (line * 100), 0);
            newItem.GetComponent<ItemSlotScript>().FillItemSlot(items[i]);
            itemDisplays.Add(newItem);
        }
    }

    public void RemoveUI()  // Clear the slot when unpausing the game
    {
        foreach (GameObject slot in itemDisplays)
        {
            Destroy(slot);
        }

        itemDisplays.Clear();
    }
}
