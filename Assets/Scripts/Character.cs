using System;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    
    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }

    [SerializeField] private bool isBlocking = false;

    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }

}
