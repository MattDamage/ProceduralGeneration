using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    [Header("Dependencies")]
    public Transform Target;
    public GameObject MuzzleFlash;



    [Header("Stats")]
    public float MaxElavation;

    public float MaxAzimuth;

    public float Damage;

    public float RateOfFire;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
