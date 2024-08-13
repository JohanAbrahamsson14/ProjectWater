using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> inventoryCollection;
    public Item activeItem;

    public GameObject activeItemHolder;
    public GameObject activeAmmoHolder;
    public GameObject holder;

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
        activeItem = null;
        itemUIInteractable.gameObject.transform.SetParent(holder.transform);
        itemUIInteractable.gameObject.transform.localPosition = Vector3.zero;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = false;
        rb.gravityScale = 10;
    }
    
    private void IsActiveItem(Rigidbody2D rb, ItemUIInteractable itemUIInteractable, Item item)
    {
        activeItem = item;
        itemUIInteractable.gameObject.transform.SetParent(activeItemHolder.transform);
        itemUIInteractable.gameObject.transform.localPosition = Vector3.zero;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }
}
