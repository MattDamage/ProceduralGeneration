using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raft : MonoBehaviour {


    public Transform Rudder;

    public float WaterLevel;


	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.I))
        {
            transform.Translate(-transform.right * Time.deltaTime) ;


        }
        if (Input.GetKey(KeyCode.J))
        {
            Rudder.Rotate(Vector3.down * 20 * Time.deltaTime);
            transform.Rotate(Vector3.down * 20 * Time.deltaTime);


        }
        if (Input.GetKey(KeyCode.L))
        {
            Rudder.Rotate(Vector3.up * 20 *  Time.deltaTime);
            transform.Rotate(Vector3.up * 20 *  Time.deltaTime);

        }


	}
}
