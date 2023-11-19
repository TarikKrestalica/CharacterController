using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurItemSlotScript : MonoBehaviour
{
    [SerializeField] private new TextMeshProUGUI name;
    [SerializeField] private Image itemSprite;

    private void Start()
    {
        // Load up the last currently equipped item to effectively make transition
        if (GameManager.player.GetComponent<PlayerController>().playerInventory.items.Count >= 1)
        {
            int indexOfLastPickedItem = GameManager.playerController.playerInventory.items.Count - 1;
            ItemDefinition lastItem = GameManager.playerController.playerInventory.items[indexOfLastPickedItem];
            GameObject.FindGameObjectWithTag("CurItemDisplay").GetComponent<CurItemSlotScript>().UpdateCurItemSlot(lastItem);
        }
    }

    public void UpdateCurItemSlot(ItemDefinition item)
    {
        name.text = item.name;
        itemSprite.sprite = item.sprite;
    }

    public void RemoveCurItemSlot()
    {
        name.text = "Name:";
        itemSprite.sprite = null;
    }
}
