using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    public float moveSpeed = 20;
    public GameObject model;
    public GameObject cursor;

    Camera mainCamera;
    GameManager gameManager;

    Vector3 lookDirection;
    Vector3 desiredMoveDirection;
    Animator anim;

    GameObject throwableGO, handle;
    Throwable throwable;

    Rigidbody rigidBody;
    Collider collider;

    int groundMask;
    int defaultLayer;
    int hurtingLayer;
    int groundLayer;

    bool hurting = false;
    float hurtingSince;

    public AudioClip hurtSound, hurlSound, grabSound;
    AudioSource audio;

    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        anim = model.GetComponent<Animator>();
        GameObject gmObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gmObj.GetComponent<GameManager>();

        handle = GameObject.FindGameObjectWithTag("ThrowableHandle");

        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        groundMask = LayerMask.GetMask("Ground");
        defaultLayer = LayerMask.NameToLayer("PlayerDefault");
        hurtingLayer = LayerMask.NameToLayer("OnlyStationary");
        groundLayer = LayerMask.NameToLayer("Ground");
        audio = GetComponent<AudioSource>();
    }

    void playSound(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {

        anim.ResetTrigger("pickUp");
        anim.ResetTrigger("throw");

        var mousePos = getMousePos();

        if (Input.GetAxis("Fire1") != 0 && throwableGO == null)
        {
            anim.SetTrigger("pickUp");

            GameObject target = findNearestThrowable(mousePos);
            if (target != null)
            {
                throwableGO = target;
                throwable = throwableGO.GetComponent<Throwable>();
                throwable.grab(collider);
                anim.SetBool("carrying", true);

                Vector3 dirToTarget = target.transform.position - transform.position;
                dirToTarget.y = 0;
                lookDirection = dirToTarget;
                model.transform.rotation = Quaternion.LookRotation(dirToTarget);
                playSound(grabSound);
            }
        }

        if (Input.GetAxis("Fire2") != 0)
        {
            if (throwableGO != null)
            {
                model.transform.rotation = Quaternion.LookRotation(mousePos);
                anim.SetTrigger("throw");
                anim.SetBool("carrying", false);
                lookDirection = mousePos;
                throwable.throwTowards(mousePos);
                throwableGO = null;
                playSound(hurlSound);
            }
        }

        if (desiredMoveDirection != Vector3.zero)
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }

        if (throwableGO != null)
        {
            throwableGO.transform.position = handle.transform.position;
        }

        updateCamera();

        if (Time.time > hurtingSince + 1)
        {
            anim.SetBool("hurting", false);
            gameObject.layer = defaultLayer;
            hurting = false;
        }

        if (cursor != null) {
            cursor.transform.position = mousePos;
        }
    }

    void updateCamera()
    {
        Vector3 targetPoint = new Vector3(0, -15, 0);
        Vector3 camPos = (transform.position - targetPoint).normalized;
        camPos = transform.position + camPos * 80;
        mainCamera.transform.position = camPos;

        mainCamera.transform.rotation = Quaternion.LookRotation(targetPoint - camPos);
    }

    void FixedUpdate()
    {
        if (transform.position.y < -20)
        {
            gameManager.lose();
        }

        //reading the input:
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        //camera forward and right vectors:
        var up = mainCamera.transform.up;
        var right = mainCamera.transform.right;
        
        up.y = 0f;
        right.y = 0f;
        up.Normalize();
        right.Normalize();
        
        desiredMoveDirection = up * verticalAxis + right * horizontalAxis;

        Vector3 velocityY = new Vector3(0, rigidBody.velocity.y, 0);

        if (!hurting)
        {
            rigidBody.velocity = desiredMoveDirection * moveSpeed + velocityY;
        }
        Vector3 heading = rigidBody.velocity;
        heading.y = 0;
        heading = heading.normalized * 2;
        Ray ray = new Ray(transform.position + heading + new Vector3(0, 100, 0), Vector3.down);
        if (!Physics.Raycast(ray, 1000, groundMask))
        {
            rigidBody.velocity = velocityY;
        }

        if (desiredMoveDirection != Vector3.zero)
        {
            lookDirection = desiredMoveDirection;
        }
        if (lookDirection != Vector3.zero)
        {
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.LookRotation(lookDirection), 0.5f);
        }
    }

    Vector3 getMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        float mousePosY = mousePos.y;
        var mousePosW = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePosY, 10));
        var camPos = mainCamera.transform.position;

        var dir = mousePosW - camPos;

        return camPos + camPos.y / Mathf.Abs(dir.y) * dir;
    }

    GameObject findNearestThrowable(Vector3 position)
    {
        GameObject nearest = null;
        float nearestDist = 0;
        foreach (GameObject go in gameManager.throwables)
        {
            if (go == null)
            {
                continue;
            }
            float dist = Vector3.Distance(go.transform.position, position);
            float distPlayer = Vector3.Distance(go.transform.position, transform.position);
            if (distPlayer > 3)
            {
                continue;
            }

            if (nearest == null)
            {
                nearest = go;
                nearestDist = dist;
                continue;
            }
            if (dist < nearestDist)
            {
                nearest = go;
                nearestDist = dist;
            }
        }
        return nearest;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == groundLayer)
        {
            if (hurting)
            {
                hurting = false;
                anim.SetBool("hurting", false);
            }
        }
    }

    public void hurt()
    {
        if (throwableGO != null)
        {
            anim.SetBool("carrying", false);
            throwable.drop();
            throwableGO = null;
        }
        
        if (!hurting)
        {
            Vector3 force = Quaternion.AngleAxis(Random.value * 360, Vector3.up) * Vector3.forward + new Vector3(0, 0.25f, 0);
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
            rigidBody.AddForce(force.normalized * 300, ForceMode.Impulse);

            hurting = true;
            hurtingSince = Time.time;
            anim.SetBool("hurting", true);
            gameObject.layer = hurtingLayer;
            gameManager.mojo -= 40;
            playSound(hurtSound);
        }
    }
}
