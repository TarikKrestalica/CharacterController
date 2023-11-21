using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    private void Start()
    {
        float health = GameManager.playerController.playerStats.FindCurrentValue(StatType.Health);
        Debug.Log("Hit");
        GameManager.playerController.playerStats.AddCallBack(StatType.Health, UpdateHealthBar);  // Should be executed before getting and setting the stats!
        //UpdateHealthBar(StatType.Health, health);
    }

    private void UpdateHealthBar(StatType statType, float aValue) 
    {
        healthBar.value.text = aValue.ToString();
    }

}
