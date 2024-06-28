using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownItem : MonoBehaviour
{
    public Item item;
    public float slowdown;
    public LayerMask wallLayer;
    public float distanceBlock;

    public void Update()
    {
        if(!Physics.Raycast(transform.position,Vector3.down,distanceBlock,wallLayer)) 
            transform.position +=  item.weight*slowdown * Time.deltaTime*Vector3.down;
    }
}
