using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launcher : MonoBehaviour {

    public GameObject prefab;
    public int minLaunches = 1;
    public int maxLaunches = 5;
    public float minInterval = 5;
    public float maxInterval = 10;
    public float minFirstLaunch = 5;
    public float maxFirstLaunch = 10;


    float nextLaunch;

    public AudioClip sound1, sound2;
    AudioSource audio;
    public GameObject particles;

    // Use this for initialization
    void Start ()
    {
        nextLaunch = Time.time + Random.Range(minFirstLaunch, maxFirstLaunch);
        audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (nextLaunch < Time.time)
        {
            Launch();
            nextLaunch = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    public void Launch()
    {
        int amount = Random.Range(minLaunches, maxLaunches + 1);
        for (var i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab, transform.parent);
            obj.transform.position = transform.position;
        }

        if (sound1 != null)
        {
            audio.clip = sound1;
            audio.Play();
        }

        if (sound2 != null)
        {
            audio.clip = sound2;
            audio.Play();
        }

        if (particles != null)
        {
            Instantiate(particles);
        }
    }
}
