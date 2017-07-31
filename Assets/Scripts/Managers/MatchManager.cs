using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

enum MatchState {
    STARTING,
    PLAYING,
    OVER,
}

enum Winner {
    NONE,
    TEAMONE,
    TEAMTWO,
}

public class MatchManager : MonoBehaviour {

    public static MatchManager instance;

    public int winScore = 1;

    public GameObject puck;
    public Transform[] spawnLocations;
    public Text scoreText;

    public GameObject scoreScreen;
    public GameObject scoreTableContainer;
    public GameObject scoreTableUI;
    public GameObject scoreTextUI;

    public Text winningTeamText;

    private GameObject currentScoreTable;

    private int teamOneScore = 0;
    private int teamTwoScore = 0;

    private Puck currentPuck;
    private MatchState state = MatchState.STARTING;
    private Winner winner = Winner.NONE;

    void Awake() {
        instance = this;
    }

    void Start() {
        state = MatchState.STARTING;
        HideText();
    }

    void Update() {
        if(state == MatchState.OVER) {
            for (int i = 0; i < InputManager.Devices.Count; i++) {
                InputDevice device = InputManager.Devices[i];
                if(device.AnyButton.WasPressed) {
                    state = MatchState.STARTING;
                    winner = Winner.NONE;
                    teamOneScore = 0;
                    teamTwoScore = 0;

                    Destroy(this.currentScoreTable.gameObject);
                    this.scoreScreen.SetActive(false);
                    GameManager.instance.ResetGame();
                }
            }
        }
    }
    //  take the net
    public void OnScore(string tag, PlayerController scorer, PlayerController assister) {
        if(tag == "TeamOneNet") {
            teamTwoScore++;
        } else {
            teamOneScore++;
        }

        if(scorer) {
            scorer.goals++;
        }
        if(assister) {
            scorer.assists++;
        }

        scoreText.text = teamOneScore.ToString() + " - " + teamTwoScore.ToString();
        ShowText();

        StartCoroutine(SpawnNewPuck());
    }

    public void StartGame() {
        state = MatchState.PLAYING;
        SpawnPuck();
    }

    private void ShowText()
    {
        scoreText.gameObject.SetActive(true);
    }

    private void HideText()
    {
        scoreText.gameObject.SetActive(false);
    }

    private void CheckForWinner()
    {
        if(teamOneScore == winScore) {
            winner = Winner.TEAMONE;
        } else if(teamTwoScore == winScore) {
            winner = Winner.TEAMTWO;
        }

        if(winner != Winner.NONE) {
            state = MatchState.OVER;
            this.CreateScoreScreen();
        }
    }

    private void SpawnPuck() {
        Vector3 spawnLocation = spawnLocations[Random.Range(0, spawnLocations.Length)].position;
        currentPuck = Instantiate(puck, spawnLocation, Quaternion.identity).GetComponent<Puck>();
    }

    private IEnumerator SpawnNewPuck() {
        yield return new WaitForSeconds(2);
        this.HideText();
        this.currentPuck.FadeAway();
        yield return new WaitForSeconds(2);
        CheckForWinner();
        if(this.state == MatchState.PLAYING) {
            this.SpawnPuck();   
        }
    }

    private void CreateScoreScreen() {
        scoreScreen.SetActive(true);
        string winningTeam = "";
        switch(winner) {
            case Winner.TEAMONE:
                winningTeam = "BLUE";
				break;
			case Winner.TEAMTWO:
				winningTeam = "GREEN";
				break;
        }

        winningTeamText.text = winningTeam + " WINS!";

        List<PlayerController> players = new List<PlayerController>();
        //  merge list of players
        players.AddRange(GameManager.instance.teams[0].players);
        players.AddRange(GameManager.instance.teams[1].players);

        currentScoreTable = Instantiate(this.scoreTableUI, Vector3.zero, Quaternion.identity, this.scoreTableContainer.transform);

        for (int i = 0; i < players.Count; i++) {
			Text playerName = Instantiate(this.scoreTextUI, Vector3.zero, Quaternion.identity, currentScoreTable.transform).GetComponent<Text>();
            playerName.text = "Player " + (players[i].playerNum+1).ToString();
            Text goals = Instantiate(this.scoreTextUI, Vector3.zero, Quaternion.identity, currentScoreTable.transform).GetComponent<Text>();
            goals.text = players[i].goals.ToString();
			Text assists = Instantiate(this.scoreTextUI, Vector3.zero, Quaternion.identity, currentScoreTable.transform).GetComponent<Text>();
            assists.text = players[i].assists.ToString();
        }

        currentScoreTable.transform.localPosition = Vector3.zero;
    }
}
