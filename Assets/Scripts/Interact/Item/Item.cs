using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public GameObject itemObject;
    public float size;
    public float weight;

    public void Start()
    {
        if (itemObject == null)
        {
            itemObject = GetComponentInParent<GameObject>();
        }
    }

    public void Action(FirstPersonController player)
    {
        player.inventory.inventoryCollection.Add(this);
        player.inventory.addUIItem(this);
        player.Weight += weight;
        itemObject.transform.SetParent(player.inventoryHolder.transform);
        itemObject.SetActive(false);
    }
}
