using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    public float maxHealth { get; }
    public float currentHealth  { get; set; }

    public void GetDamaged(int value);
    
    public void Death();
}
