using System;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float strenght = 1f;
    [SerializeField] private float dexterity = 1f;

    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
    public float Strenght { get { return strenght; } set { strenght = value; } }
    public float Dexterity { get { return dexterity; } set { dexterity = value; } }

}
