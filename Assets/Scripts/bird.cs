using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bird : MonoBehaviour
{
    float moveSpeed = 20;

    public GameObject model;
    player player;

    Vector3 lookDirection, desiredMoveDirection;
    int groundMask;
    int pitMask;
    Rigidbody rigidBody;
    Animator anim;
    Collider collider;

    bool dead = false;

    GameManager gameManager;

    public AudioClip hurtSound;
    AudioSource audio;

    bool tracking;
    float trackingUntil;
    float trackingReset;



    // Use this for initialization
    void Start ()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = obj.GetComponent<GameManager>();

        anim = model.GetComponent<Animator>();

        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        groundMask = LayerMask.GetMask("Ground");
        pitMask = LayerMask.GetMask("Pit");

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    void playSound(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            if (!tracking)
            {
                if (trackingReset < Time.time)
                {
                    tracking = true;
                }
            }
            else
            {
                if (Vector3.Distance(player.transform.position, transform.position) > 150)
                {
                    trackingUntil = Time.time + 10;
                }

                if (trackingUntil < Time.time)
                {
                    tracking = false;
                    trackingReset = Time.time + 10f;
                }
            }

            if (tracking)
            {
                desiredMoveDirection = player.transform.position - transform.position;
                desiredMoveDirection.y = 0;
            }
            else
            {

            }

            if (desiredMoveDirection != Vector3.zero)
            {
                lookDirection = Vector3.Slerp(lookDirection, desiredMoveDirection, 1.5f * Time.deltaTime);
            }
            if (lookDirection != Vector3.zero)
            {
                model.transform.rotation = Quaternion.LookRotation(lookDirection);
            }

            rigidBody.velocity = lookDirection.normalized * moveSpeed;
        }

        if (transform.position.y < -20)
        {
            Ray ray = new Ray(transform.position + new Vector3(0, 100, 0), Vector3.down);
            if (Physics.Raycast(ray, 1000, pitMask))
            {
                gameManager.godsWrath -= (15 + gameManager.mojo) * 0.05f;
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.hurt();
        }
        else if (collision.gameObject.tag == "Rock")
        {
            Throwable t = collision.gameObject.GetComponent<Throwable>();
            if (t.flying)
            {
                t.flying = false;
                die();
            }
        }
    }

    public void die()
    {
        dead = true;
        anim.SetTrigger("die");
        rigidBody.detectCollisions = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        Throwable t = GetComponent<Throwable>();
        t.Activate();
        t.drop();
        gameManager.mojo += 8;
        playSound(hurtSound);
    }
}
