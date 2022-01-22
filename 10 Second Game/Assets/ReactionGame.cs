using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ReactionGame : MonoBehaviour
{
    public GameObject reactionImage;
    public GameObject startButton;
    public GameObject easyButton;
    public GameObject mediumButton;
    public GameObject hardButton;
    public GameObject exitButton;
    public GameObject infoButton;
    private float reactionspeed, reactionTime, startTime, randomDelayBeforeMeasuring, displayTime;
    private bool clockIsTicking, timerCanBeStopped, BeginClicked;
    public Text gameText;
    public Text streakText;
    public int streak;
    public AudioSource VictorySound;
    public AudioSource DefeatSound;
    public AudioSource ReactSound;
    public AudioSource InfoSound;
    public AudioSource ClickSound;
    public Animator playerAnimation;
    public Animator enemyAnimation;
    
    // Start is called before the first frame update
    void Start()
    {
        reactionImage.GetComponent<Renderer>().enabled = false;
        startButton.GetComponent<Renderer>().enabled = true;
        easyButton.GetComponent<Renderer>().enabled = true;
        mediumButton.GetComponent<Renderer>().enabled = true;
        hardButton.GetComponent<Renderer>().enabled = true;
        exitButton.GetComponent<Renderer>().enabled = true;
        infoButton.GetComponent<Renderer>().enabled = true;
        reactionspeed = 0.3f;
        reactionTime = 0f;
        startTime = 0f;
        displayTime = 0f;
        randomDelayBeforeMeasuring = 0f;
        clockIsTicking = false;
        timerCanBeStopped = true;
        BeginClicked = false;
        gameText.text = null;
        streak = 0;
        streakText.text = "Streak: " + streak.ToString();
        playerAnimation.SetBool("gameIsBeingPlayed", false);
        playerAnimation.SetBool("mouseClick", false);
        enemyAnimation.SetBool("gameIsBeingPlayed", false);
        enemyAnimation.SetBool("playerFail", false);
        
        //Display Start button, when start button is clicked, begin game.
        //When the game begins, there is a hidden image, a random time between 2 and 6 seconds is determiend for when the image is to be shown
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            ClickSound.Play();
            Application.Quit();
        }
        reactionTime = Time.time - startTime;
        if ((Time.time - displayTime) >= 2.0f){
            gameText.text = null;
            startButton.GetComponent<Renderer>().enabled = true;
            easyButton.GetComponent<Renderer>().enabled = true;
            mediumButton.GetComponent<Renderer>().enabled = true;
            hardButton.GetComponent<Renderer>().enabled = true;
            exitButton.GetComponent<Renderer>().enabled = true;
            infoButton.GetComponent<Renderer>().enabled = true;
        }
        if (clockIsTicking && timerCanBeStopped){
            gameText.text = reactionTime.ToString("N3");
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D Begin = startButton.GetComponent<Collider2D>();
            Collider2D Easy = easyButton.GetComponent<Collider2D>();
            Collider2D Medium = mediumButton.GetComponent<Collider2D>();
            Collider2D Hard = hardButton.GetComponent<Collider2D>();
            Collider2D Exit = exitButton.GetComponent<Collider2D>();
            Collider2D Info = infoButton.GetComponent<Collider2D>();

            if (Begin.OverlapPoint(wp))
            {
                playerAnimation.SetBool("mouseClick", false);
                playerAnimation.SetBool("gameIsBeingPlayed", true);
                enemyAnimation.SetBool("gameIsBeingPlayed", true);
                enemyAnimation.SetBool("playerFail", false);
                ClickSound.Play();
                startButton.GetComponent<Renderer>().enabled = false;
                BeginClicked = true;
                gameText.color = Color.white;
                displayTime = Time.time+5.1f;
                DefeatSound.Stop();
                VictorySound.Stop();
                
            }
            else if (Easy.OverlapPoint(wp))
            {
                ClickSound.Play();
                reactionspeed = 0.40f;
                gameText.color = Color.white;
                displayTime = Time.time;
                gameText.text = "Easy Difficulty Selected. \n Reaction Speed Goal set to 0.4 seconds. Good Luck!";
            }
            else if (Medium.OverlapPoint(wp))
            {
                ClickSound.Play();
                reactionspeed = 0.30f;
                gameText.color = Color.white;
                displayTime = Time.time;
                gameText.text = "Medium Difficulty Selected. \n Reaction Speed Goal set to 0.3 seconds. Good Luck!";
            }
            else if (Hard.OverlapPoint(wp))
            {
                ClickSound.Play();
                reactionspeed = 0.20f;
                gameText.color = Color.white;
                displayTime = Time.time;
                gameText.text = "Hard Difficulty Selected. \n Reaction Speed Goal set to 0.2 seconds. Good Luck!";
            }
            else if (Info.OverlapPoint(wp))
            {
                
                InfoSound.Play();
                gameText.color = Color.white;
                displayTime = Time.time;
                gameText.text = "Wait for the ! to appear. React by pressing the Left Mouse Button. \n  Don't be too quick, or too slow... Good Luck!";

                // gameText.text = null;
            }
            else if (Exit.OverlapPoint(wp))
            {
                ClickSound.Play();
                Application.Quit();
            }
        }

        if (BeginClicked == true && Input.GetMouseButtonDown(0))
        {
            easyButton.GetComponent<Renderer>().enabled = false;
            mediumButton.GetComponent<Renderer>().enabled = false;
            hardButton.GetComponent<Renderer>().enabled = false;
            exitButton.GetComponent<Renderer>().enabled = false;
            infoButton.GetComponent<Renderer>().enabled = false;
            if(!clockIsTicking)
            {
                StartCoroutine("StartMeasuring");
                gameText.text = "Wait for the ! to appear.";
                clockIsTicking = true;
                timerCanBeStopped = false;
            }
            else if (clockIsTicking && timerCanBeStopped)
            {
                StopCoroutine("StartMeasuring");
                reactionTime = Time.time - startTime;
                displayTime = Time.time;
                if (reactionTime>reactionspeed){
                    gameText.color = Color.red;
                    enemyAnimation.SetBool("playerFail", true);
                    gameText.text = "Reaction time: \n" + reactionTime.ToString("N3") + "sec\n" + "Too Slow! Click Begin to Try again.";
                    DefeatSound.Play();
                    clockIsTicking = false;
                    reactionImage.GetComponent<Renderer>().enabled = false;
                    BeginClicked = false;
                    playerAnimation.SetBool("gameIsBeingPlayed", false);
                    enemyAnimation.SetBool("gameIsBeingPlayed", false);
                    ReactSound.Stop();
                    streak = 0;
                    streakText.text = "Streak: " + streak.ToString();
                }
                else if (reactionTime<reactionspeed){
                    gameText.color = Color.green;
                    playerAnimation.SetBool("mouseClick", true);
                    gameText.text = "Reaction time: \n" + reactionTime.ToString("N3") + "sec\n" + "Nice Job! Click Begin to Try again.";
                    VictorySound.Play();
                    clockIsTicking = false;
                    reactionImage.GetComponent<Renderer>().enabled = false;
                    BeginClicked = false;
                    playerAnimation.SetBool("gameIsBeingPlayed", false);
                    enemyAnimation.SetBool("gameIsBeingPlayed", false);
                    ReactSound.Stop();
                    streak += 1;
                    streakText.text = "Streak: " + streak.ToString();
                    
                }
            }
            else if (clockIsTicking && !timerCanBeStopped)
            {
                StopCoroutine("StartMeasuring");
                reactionTime = 0f;
                displayTime = Time.time;
                clockIsTicking = false;
                timerCanBeStopped = true;
                gameText.color = Color.red;
                enemyAnimation.SetBool("playerFail", true);
                gameText.text = "Too early\n" + "Click Begin to Try again.";
                DefeatSound.Play();
                reactionImage.GetComponent<Renderer>().enabled = false;
                BeginClicked = false;
                playerAnimation.SetBool("gameIsBeingPlayed", false);
                enemyAnimation.SetBool("gameIsBeingPlayed", false);
                ReactSound.Stop();
                streak = 0;
                streakText.text = "Streak: " + streak.ToString();
            }
        }
        //Display start button 
        //When the image appears, begin a timer, once timer reaches reaction speed, stop timer, freeze controls
    }

    private IEnumerator StartMeasuring()
    {
        randomDelayBeforeMeasuring = Random.Range(1.5f, 5f);
        yield return new WaitForSeconds(randomDelayBeforeMeasuring);
        ReactSound.Play();
        reactionImage.GetComponent<Renderer>().enabled = true;
        startTime = Time.time;
        clockIsTicking = true;
        timerCanBeStopped = true;
    }
}

