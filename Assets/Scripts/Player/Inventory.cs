using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    public List<Item> inventoryCollection;
    public Item activeItem;

    public GameObject activeItemHolder;
    public GameObject activeAmmoHolder;
    public GameObject holder;

    public GameObject prefabUI;
    public GameObject spawnPoint;

    private FirstPersonController player;

    private void Awake()
    {
        player = GetComponentInParent<FirstPersonController>();
    }

    public void SetActiveItem(ItemUIInteractable itemUIInteractable, Item item, bool isActive)
    {
        if (activeItem != null)
        {
            ItemUIInteractable currentItemUIInteractable =
                activeItemHolder.GetComponentInChildren<ItemUIInteractable>();
            Rigidbody2D currentRb = currentItemUIInteractable.gameObject.GetComponent<Rigidbody2D>();
            IsNotActiveItem(currentRb, currentItemUIInteractable);
        }
        Rigidbody2D rb = itemUIInteractable.gameObject.GetComponent<Rigidbody2D>();
        if (isActive)
        {
            IsNotActiveItem(rb, itemUIInteractable);
            return;
        }
        IsActiveItem(rb, itemUIInteractable, item);
    }

    private void IsNotActiveItem(Rigidbody2D rb, ItemUIInteractable itemUIInteractable)
    {
        RemoveSetActiveItem();
        itemUIInteractable.gameObject.transform.SetParent(holder.transform);
        itemUIInteractable.gameObject.transform.position = spawnPoint.transform.position;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = false;
        rb.gravityScale = 10;
    }
    
    private void IsActiveItem(Rigidbody2D rb, ItemUIInteractable itemUIInteractable, Item item)
    {
        SetToActiveItem(item);
        itemUIInteractable.gameObject.transform.SetParent(activeItemHolder.transform);
        itemUIInteractable.gameObject.transform.localPosition = Vector3.zero;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    public void addUIItem(Item item)
    {
        GameObject gameObject = Instantiate(prefabUI, spawnPoint.transform.position, quaternion.identity, holder.transform);
        gameObject.GetComponent<ItemUIInteractable>().item = item;
        gameObject.transform.localScale *= item.size;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-0.45f,0.45f)*500, -100);
    }
    public void removeUIItem(Item item)
    {
        RemoveSetActiveItem();
        inventoryCollection.Remove(item);
        GameObject selectedItemObject = item.itemObject;
        selectedItemObject.transform.position = transform.position+Vector3.down;
        player.Weight -= item.weight;
        selectedItemObject.transform.parent = null;
        selectedItemObject.SetActive(true);
        inventoryCollection.Remove(item);
    }

    public void SetToActiveItem(Item item)
    {
        if (activeItem != null)
        {
            RemoveSetActiveItem();
        }
        activeItem = item;
        item.itemObject.transform.SetParent(player.activeItemObject.transform);
        item.itemObject.transform.localPosition = Vector3.zero;
        activeItem.itemObject.transform.localRotation = Quaternion.identity;
        item.itemObject.SetActive(true);
        item.itemObject.TryGetComponent(out DropDownItem dropDownItem);
        dropDownItem.enabled = false;
        item.TryGetComponent(out Collider collider);
        collider.enabled = false;
    }

    private void RemoveSetActiveItem()
    {
        if (activeItem != null)
        {
            activeItem.itemObject.transform.SetParent(player.inventoryHolder.transform);
            activeItem.itemObject.transform.localPosition = Vector3.zero;
            activeItem.itemObject.transform.localRotation = Quaternion.identity;
            activeItem.itemObject.SetActive(false);
            activeItem.itemObject.TryGetComponent(out DropDownItem dropDownItemLastActive);
            dropDownItemLastActive.enabled = true;
            activeItem.TryGetComponent(out Collider colliderLastActive);
            colliderLastActive.enabled = true;
            activeItem = null;
        }
    }
}
