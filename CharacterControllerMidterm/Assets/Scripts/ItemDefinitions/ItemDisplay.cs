using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    public ItemDefinition itemDef;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.playerController.playerInventory.items.Contains(itemDef))  // Not in inventory!
        { 
            itemDef.amount = 0;
            itemDef.pickedUpForFirstTime = false;
        }

        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemDef.sprite;
    }
}
