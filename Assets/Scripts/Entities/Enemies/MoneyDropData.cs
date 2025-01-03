using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDropData
{
    public GameObject MoneyPrefab { get; set; }
    public int DropAmount { get; set; }
    public Scrap MoneyItem { get; set; }
    public Transform ParentTransform { get; set; }
    public Vector3 DropPosition { get; set; }
    public float DropDuration { get; set; }
    public float DropMinForce { get; set; }
    public float DropMaxForce { get; set; }
    public float DropAmountVariation { get; set; }
}