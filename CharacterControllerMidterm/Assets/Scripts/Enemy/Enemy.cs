using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text value;
    private float increaseFactor = 25f;  // Move the description up for all the stats
    private string text;

    StatSystem statSystem;
    // Start is called before the first frame update
    void Awake()
    {
        text = value.text;
        statSystem = GetComponent<StatSystem>();
        if (statSystem == null)
        {
            return;
        }

        statSystem.AddCallBack(StatType.Health, UpdateBar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateBar(StatType statType, float aValue)
    {
        value.text = aValue.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (GameManager.playerController.playerInventory.currentlyEquippedItem.name != "Sword")
            {
                return;
            }

            // Get sword, amount of damage it will take
            ItemDefinition sword = GameManager.playerController.playerInventory.currentlyEquippedItem;
            float hit = 0;
            if (sword.StatExists(StatType.Damage))
            {
                hit = sword.GetStatValue(StatType.Damage);
            }

            foreach(Stat stat in statSystem.statTypes)
            {
                if (statSystem.StatInCollection(stat.statType))
                {
                    statSystem.LowerStat(stat.statType, hit);
                }

                if (stat.statType != StatType.Health)
                    return;

                if (statSystem.FindCurrentValue(StatType.Health) <= statSystem.FindMinValue(StatType.Health))
                    Destroy(this.gameObject);
            }
        }
    }
}
