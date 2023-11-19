using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    private void Start()
    {
        GameManager.playerController.playerStats.AddCallBack(StatType.Health, UpdateHealthBar);
    }

    private void UpdateHealthBar(StatType statType, float aValue) 
    {
        healthBar.value.text = aValue.ToString();
    }

}
