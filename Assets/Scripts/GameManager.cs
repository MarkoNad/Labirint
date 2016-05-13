using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public float collectingTime;
    public float escapeTime;
    public int pickupCount;

    public Camera startScreenCamera;
    public GameObject[] startScreenCharacters;
    public GameObject[] players;
    public GameObject[] exits;
    public GameObject pickupManager;
    public GameObject bodyManager;

    public Text scoreText;
    public Text timeLeftText;
    public Text gameInfoText;

    private GameObject activePlayer;
    private int score;

    private const float DEFAULT_START_DELAY_TIME = 3;
    private const float DEFAULT_COLLECTING_TIME = 100;
    private const float DEFAULT_ESCAPE_TIME = 30;
    private const float DEFAULT_END_GAME_WALK_TIME = 3;
    private const int SECONDS_PER_ITEM = 5;

    private BodySourceManager source;
    private Body[] data = null;
    private Body trackedBody = null;

    private float startDelayTimeLeft;
    private float collectTimeLeft;
    private float escapeTimeLeft;
    private float endGameWalkTimeLeft;
    private bool escaped;

    private enum GameState {
        characterSelection,
        startDelay,
        collecting,
        escaping,
        end
    };

    private GameState gameState;



	void Start () {
        if (collectingTime <= 0) collectingTime = DEFAULT_COLLECTING_TIME;
        if (escapeTime <= 0) escapeTime = DEFAULT_ESCAPE_TIME;

        score = 0;

        startDelayTimeLeft = DEFAULT_START_DELAY_TIME;
        collectTimeLeft = collectingTime;
        escapeTimeLeft = escapeTime;
        endGameWalkTimeLeft = DEFAULT_END_GAME_WALK_TIME;

        gameState = GameState.characterSelection;

        timeLeftText.text = "";
        gameInfoText.text = "";

        escaped = false;

        initializeKinectData();
	}
	


	void Update () {
        if (Input.GetKeyDown("r")) SceneManager.LoadScene("MainScene");
        if (Input.GetKeyDown("q")) Application.Quit();

        switch(gameState) {
            case GameState.characterSelection:
                selectCharacter();
                break;
            case GameState.startDelay:
                startDelayTimeLeft -= Time.deltaTime;
                gameInfoText.text = "Game starts in " + (int)startDelayTimeLeft + " seconds";
                if (startDelayTimeLeft <= 0) switchToCollecting();                          
                break;
            case GameState.collecting:
                collectTimeLeft -= Time.deltaTime;
                scoreText.text = score + "/" + pickupCount + " items found";
                timeLeftText.text = "Search time left: " + (int)collectTimeLeft;
                if (collectTimeLeft <= 0 || score == pickupCount) switchToEscaping();
                break;
            case GameState.escaping:
                escapeTimeLeft -= Time.deltaTime;
                timeLeftText.text = "Escape time left: " + (int)escapeTimeLeft;
                if (escapeTimeLeft <= 0) switchToGameOver();
                if (escaped) switchToWin();
                break;
            case GameState.end:
                endGameWalkTimeLeft -= Time.deltaTime;
                if (endGameWalkTimeLeft <= 0) activePlayer.GetComponent<PlayerController>().setWalkingDisabled(true);
                break;
            default:
                print("Unknown game state");
                break;
        }
	}


    private void initializeKinectData() {
        if (bodyManager == null) {
            print("body source manager missing");
            return;
        }

        source = bodyManager.GetComponent<BodySourceManager>();

        if (source == null) {
            print("invalid body manager");
            return;
        }
    }

    private void manageKinectData() {
        if (data == null) {
            data = source.GetData();
        }

        int idx = -1;
        for (int i = 0; i < source.getBodyCount(); i++) {
            if (data == null) return;
            if (data[i].IsTracked) {
                idx = i;
            }
        }

        if (idx > -1) {
            trackedBody = data[idx];
        }
    }


    private void selectCharacter() {
        gameInfoText.text = "Select a character; rise left or right hand";

        manageKinectData();
        if (trackedBody == null) return;

        float leftHandHeight = trackedBody.Joints[JointType.HandLeft].Position.Y * 10;
        float rightHandHeight = trackedBody.Joints[JointType.HandRight].Position.Y * 10;

        if(rightHandHeight - leftHandHeight >= 5) {          
            switchToStartDelay("Nikola");          
        }

        if (leftHandHeight - rightHandHeight >= 5) {
            switchToStartDelay("Marko");
        }
    }

    private void switchToStartDelay(string activePlayerName) {
        startScreenCamera.gameObject.SetActive(false);
        foreach (GameObject player in players) {
            if (player.name.Equals(activePlayerName)) {
                activePlayer = player;
                activePlayer.SetActive(true);
                break;
            }
        }
        activePlayer.GetComponent<PlayerController>().setWalkingDisabled(true);
        deactivateStartScreenCharacters();
        gameState = GameState.startDelay;
    }

    private void deactivateStartScreenCharacters() {
        foreach(GameObject character in startScreenCharacters) {
            character.SetActive(false);
        }
    }

    private void switchToCollecting() {
        activePlayer.GetComponent<PlayerController>().setWalkingDisabled(false);
        gameInfoText.text = "";
        gameState = GameState.collecting;
    }

    private void switchToEscaping() {
        timeLeftText.text = "Escape time left: " + (int)escapeTimeLeft;
        escapeTimeLeft = escapeTime + score * SECONDS_PER_ITEM;
        deactivatePickups();
        openExit();
        gameState = GameState.escaping;
    }

    private void switchToGameOver() {
        timeLeftText.text = "Time has run out!";
        gameInfoText.text = "Game over!\nr - restart\nq - quit";
        gameState = GameState.end;
    }

    private void switchToWin() {
        timeLeftText.text = "";
        gameInfoText.text = "Level complete!\nr - restart\nq - quit";
        gameState = GameState.end;
    }

    private void deactivatePickups() {
        pickupManager.GetComponent<SpawnPointManager>().deactivatePickups();
    }

    private void openExit() {
        System.Random rand = new System.Random();
        int idx = rand.Next(0, exits.Length);
        exits[idx].SetActive(false);
    }

    public void setEscaped(bool escaped) {
        this.escaped = escaped;
    }

    public void increaseScore() {
        score++;
    }

}
