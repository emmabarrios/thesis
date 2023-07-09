using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour, IAttributes
{

    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float poise;
    
    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
    public float Poise { get { return poise; } set { poise = value; } }

    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool isBusy = false;
    [SerializeField] private bool isParryPerformed = false;
    [SerializeField] private bool isAttackPerformed = false;

    public bool IsBusy { get { return isBusy; } set { isBusy = value; } }
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }
    public bool IsParryPerformed { get { return isParryPerformed; } set { isParryPerformed = value; } }
    public bool IsAttackPerformed { get { return isAttackPerformed; } set { isAttackPerformed = value; } }

    public void ApplyItemAttributes(Item item) {
        throw new NotImplementedException();
    }

    public void RemoveItemAttributes(Item item) {
        throw new NotImplementedException();
    }
}
