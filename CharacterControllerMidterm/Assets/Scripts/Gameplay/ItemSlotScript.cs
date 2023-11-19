using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotScript : MonoBehaviour
{
    [SerializeField] private new TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Image itemDisplay;

    public string GetName()
    {
        return name.text;
    }

    public int GetCount()
    {
        return int.Parse(count.text);
    }

    public void FillItemSlot(ItemDefinition item)
    {
        name.text = item.name;
        count.text = item.amount.ToString();
        itemDisplay.sprite = item.sprite;
    }

    public void SetCount(int num)
    {
        count.text = num.ToString();
    }

}
