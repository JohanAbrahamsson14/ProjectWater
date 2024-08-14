using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIInteractable : MonoBehaviour, IInteractableMouse
{
    [SerializeField] public Item item;
    public void Interact(InteractorMouse interactor, Inventory inventory, bool isPrimary)
    {
        if (!isPrimary)
        {
            SecondaryInteract(interactor, inventory);
            return;
        }

        inventory.SetActiveItem(this, item, inventory.activeItem == item);
    }

    public void SecondaryInteract(InteractorMouse interactor, Inventory inventory)
    {
        inventory.removeUIItem(item);
        Destroy(gameObject);
    }
}
