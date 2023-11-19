using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum overview, refreshing the concept: https://youtu.be/3p0OJErAbEI?si=Xy2WhwR-qT5gAh2R
public enum StatType
{
    NULL = 0,
    Health,
    Mana,
    Stamina,
    JumpPower,
    WalkSpeed,
    CrouchedSpeed,
    DashSpeed,
    CurrentSpeed,
    Damage
}

[System.Serializable]
public class Stat 
{
    public StatType statType;
    public float currentValue;
    public float initialValue;
    public float maxValue;
    public float minValue;
}
