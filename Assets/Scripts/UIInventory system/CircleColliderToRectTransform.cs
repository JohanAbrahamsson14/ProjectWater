using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleColliderToRectTransform : MonoBehaviour
{
    private void Awake()
    {
        gameObject.GetComponent<CircleCollider2D>().radius =
            (gameObject.GetComponent<RectTransform>().sizeDelta.x +
            gameObject.GetComponent<RectTransform>().sizeDelta.y)/4;
    }
}
