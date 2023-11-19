using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ItemDefinition/Sword")]
public class SwordDefinition : ItemDefinition
{
    public float damage;
    public float attackSpeed;
}
