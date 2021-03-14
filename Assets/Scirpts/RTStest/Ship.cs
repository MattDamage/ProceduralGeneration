using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ShipType{
    Dreadnought,Destroyer, Frigate, Gunboat 


}

public class Ship : MonoBehaviour {

    [Header("Targets")]
    public Transform denstination;
    public Transform Target;
    public NavMeshAgent nav;

    public GameObject DeathParticle;

    public GameObject Damageparticle;

    [Header("Stats")]
    public float Health;
    public float Speed;
    public float Damage;
    public float Firerate;
    public float RotateSpeed;
    public float AgroRange;
    [Header("Shooting")]
    public GameObject Bullet;
    public GameObject[] Turrets;


    bool move;


    public void Start()
    {

        nav = GetComponent<NavMeshAgent>();

        SetTarget(denstination);
    }

    public void Update()
    {
     
        if(Health <= 0) {
            Instantiate(DeathParticle,transform.position,transform.rotation);
            Destroy(this.gameObject);

        }

        if (move)
            MoveTowardsDestination();

    }

    public void GetDamaged(float damage, Vector3 HitPos) {
        Health -= damage;
        Instantiate(DeathParticle, transform.position, transform.rotation);



    }

    public void MoveTowardsDestination() {

      

        Vector3 targetDir = denstination.position - transform.position;

       
        float step = Speed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        Debug.DrawRay(transform.position, newDir, Color.red);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);

    }

    public void SetTarget(Transform Destination) {
        if (Destination != null)
        {
            denstination = Destination;
            move = true;
        } else {
            move = false;

        }
       



    }
    public void Shoot( ) {




    }


	
}
