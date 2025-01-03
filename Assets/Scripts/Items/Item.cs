using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{

    // UI Managment
    [SerializeField] public string itemName;
    [SerializeField] public Sprite icon;

    public bool isStackable = false;
    // Attributes
    public string type;

    public virtual void Initialize(GameObject player)
    {
        return;
    }

    void Start()
    {
        
    }

    

    public string GetType()
    {
        return this.type;
    }


    public virtual void Use()
    {

    }

}
