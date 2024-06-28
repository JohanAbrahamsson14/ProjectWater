using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public Transform closed;
    private Vector3 closedValuesPos = Vector3.zero;
    private Quaternion closedValuesRot = Quaternion.identity;
    public Transform open;
    private Vector3 openValuesPos = Vector3.zero;
    private Quaternion openValuesRot = Quaternion.identity;
    public GameObject doorOpperation;
    public bool isOpen;

    public void Start()
    {
        closedValuesPos = closed.position; 
        closedValuesRot = closed.rotation; 
        openValuesPos = open.position; 
        openValuesRot = open.rotation;
    }

    public void Action(FirstPersonController player)
    {
        isOpen = !isOpen;
        doorOpperation.transform.position = isOpen ? openValuesPos : closedValuesPos;
        doorOpperation.transform.rotation = isOpen ? openValuesRot : closedValuesRot;
    }
}
