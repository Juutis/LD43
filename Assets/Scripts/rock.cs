using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock : MonoBehaviour {

    Rigidbody rigidBody;
    Throwable throwable;

    int groundLayer;
    player player;


    // Use this for initialization
    void Start ()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        rigidBody = GetComponent<Rigidbody>();
        throwable = GetComponent<Throwable>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        Launch();
    }

    public void Launch()
    {
        Vector3 force = Quaternion.AngleAxis(Random.value * 360, Vector3.up) * Vector3.forward + new Vector3(0, 3f + Random.value*5, 0);
        rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
        rigidBody.AddForce(force.normalized * (1000 + Random.value*500), ForceMode.Impulse);
        throwable.flying = true;
    }
	
	// Update is called once per frame
	void Update () {

        if (transform.position.y < -20)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!throwable.activated)
        {

            if (collision.gameObject.tag == "Player")
            {
                player.hurt();
                rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
                rigidBody.angularVelocity = Vector3.zero;
                throwable.flying = false;
            }
            if (collision.collider.gameObject.layer == groundLayer)
            {
                throwable.Activate();
                rigidBody.velocity = new Vector3(0, 0, 0);
                rigidBody.angularVelocity = Vector3.zero;
                throwable.flying = false;
            }
        }
    }
}
