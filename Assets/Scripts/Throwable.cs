using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public GameObject model;

    public bool activated = false;
    public bool isRock = false;

    public bool grabbed = false;
    public float height = 0;
    float lastHeight = 0;
    float hDelta = 0;

    GameManager gameManager;
    Rigidbody rigidBody;
    Collider collider;
    Collider playerCollider;

    public bool flying = false;

    int projectileLayer;
    int defaultLayer;
    int groundMask;

    float flyTimer;


    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = obj.GetComponent<GameManager>();
        rigidBody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();

        if (isRock)
        {
            defaultLayer = LayerMask.NameToLayer("StationaryObject");
        }
        else
        {
            defaultLayer = LayerMask.NameToLayer("OnlyStationary");
        }
        projectileLayer = LayerMask.NameToLayer("Projectile");
        groundMask = LayerMask.GetMask("Ground");

        if (activated)
        {
            Activate();
        }
    }

    public void Activate()
    {
        activated = true;
        gameManager.throwables.Add(gameObject);
        gameObject.layer = defaultLayer;
        rigidBody.isKinematic = true;
        rigidBody.velocity = Vector3.zero;
    }

    public void throwTowards(Vector3 position)
    {
        flying = true;
        grabbed = false;
        rigidBody.isKinematic = false;
        rigidBody.detectCollisions = true;
        var dir = position - transform.position;
        dir.y = 0;
        rigidBody.AddForce((dir.normalized + new Vector3(0, 0.5f, 0)) * 400, ForceMode.Impulse);
        gameObject.layer = projectileLayer;
        flyTimer = Time.time + 0.5f;
    }

    private void Update()
    {

    }

    void FixedUpdate()
    {
        if (activated && !grabbed && rigidBody.velocity.magnitude < 0.01 && flyTimer < Time.time)
        {
            rigidBody.isKinematic = true;
            gameObject.layer = defaultLayer;
            rigidBody.velocity = Vector3.zero;
        }
    }

    public void grab(Collider playerCollider)
    {
        this.playerCollider = playerCollider;
        rigidBody.isKinematic = true;
        rigidBody.detectCollisions = false;
        grabbed = true;
        gameObject.layer = projectileLayer;
    }

    public void drop()
    {
        flying = false;
        grabbed = false;
        rigidBody.isKinematic = false;
        rigidBody.detectCollisions = true;
        flyTimer = Time.time + 0.5f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (activated)
        {
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
            rigidBody.angularVelocity = Vector3.zero;

            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Ray ray = new Ray(transform.position + new Vector3(0, 100, 0), Vector3.down);
                if (Physics.Raycast(ray, 1000, groundMask))
                {
                    rigidBody.isKinematic = true;
                    gameObject.layer = defaultLayer;
                    rigidBody.velocity = Vector3.zero;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (gameObject != null && gameManager != null)
        {
            gameManager.throwables.Remove(gameObject);
        }
    }
}
