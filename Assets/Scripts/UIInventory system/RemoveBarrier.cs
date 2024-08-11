using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RemoveBarrier : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    private void OnCollisionEnter2D(Collision2D other)
    {
        gameObjects.Add(other.gameObject);
        if (other.gameObject.CompareTag("Item"))
        {
            Destroy(other.gameObject);
        }
    }
}
