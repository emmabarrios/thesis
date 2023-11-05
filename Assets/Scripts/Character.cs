using System;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private float health;
    [SerializeField] private float stamina;
    [SerializeField] private float attack;
    [SerializeField] private float defense;



    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
    public float Attack { get { return attack; } set { attack = value; } }
    public float Defense { get { return defense; } set { defense = value; } }
    

}
