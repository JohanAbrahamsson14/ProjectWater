using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AddItem : MonoBehaviour
{
    public GameObject prefab;
    public GameObject spawnPoint;
    public Transform holder;
    public Inventory inventory;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.inventoryCollection.Add(prefab.GetComponent<ItemUIInteractable>().item);
            
            GameObject gameObject = Instantiate(prefab, spawnPoint.transform.position, quaternion.identity, holder);
            gameObject.transform.localScale *= Random.Range(0.7f, 1.4f);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-0.45f,0.45f)*500, -100);
        }
    }
}
