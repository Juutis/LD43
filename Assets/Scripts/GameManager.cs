using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public List<GameObject> throwables;

    float initialGW = 33;

    public float mojo = 0, godsWrath = 0;

    public RectTransform mojoBar, godBar;

    public GameObject gamePrefab;
    GameObject game;

    public GameObject startScreen;
    public GameObject tutorialScreen;
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public GameObject winScreen;
    public GameObject uiBars;

    enum STATE
    {
        START,
        TUTORIAL,
        RUN,
        PAUSE,
        WIN,
        LOSE
    };

    STATE state = STATE.START;

    // Use this for initialization
    void Start ()
    {
        gamePrefab.SetActive(false);
        uiBars.SetActive(false);
        startScreen.SetActive(true);
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            if (state == STATE.RUN)
            {
                state = STATE.PAUSE;
                game.SetActive(false);
                pauseScreen.SetActive(true);
            }
            else if (state == STATE.PAUSE || state == STATE.WIN || state == STATE.LOSE)
            {
                Application.Quit();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (state == STATE.PAUSE || state == STATE.WIN || state == STATE.LOSE)
            {
                restart();
            }
        }
        
        if (state == STATE.TUTORIAL && Input.anyKeyDown)
        {
            restart();
        }

        if (state == STATE.START && Input.anyKeyDown)
        {
            state = STATE.TUTORIAL;
            startScreen.SetActive(false);
            tutorialScreen.SetActive(true);
        }

        if (state == STATE.PAUSE && Input.GetKeyDown(KeyCode.Return))
        {
            pauseScreen.SetActive(false);
            game.SetActive(true);
            state = STATE.RUN;
        }

        if (state == STATE.RUN)
        {
            godsWrath += 0.5f * Time.deltaTime;
        }

        if (godsWrath > 100)
        {
            godsWrath = 100;
            lose();
        }
        if (godsWrath < 0)
        {
            godsWrath = 0;
            win();
        }

        if (mojo > 100) mojo = 100;
        if (mojo < 0) mojo = 0;

        mojoBar.localScale = new Vector3(mojoBar.localScale.x, mojo / 100, mojoBar.localScale.z);
        godBar.localScale = new Vector3(godBar.localScale.x, godsWrath / 100, godBar.localScale.z);
    }

    public void lose()
    {
        game.SetActive(false);
        loseScreen.SetActive(true);
        state = STATE.LOSE;
    }

    public void win()
    {
        game.SetActive(false);
        winScreen.SetActive(true);
        state = STATE.WIN;
    }

    public void restart()
    {
        if (game != null) {
            Destroy(game);
        }
        godsWrath = initialGW;
        mojo = 0;
        game = Instantiate(gamePrefab);
        game.SetActive(true);
        throwables.Clear();
        uiBars.SetActive(true);

        pauseScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        startScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        state = STATE.RUN;
    }
}
