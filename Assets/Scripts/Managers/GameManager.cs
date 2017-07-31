using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

[System.Serializable]
public class Team {
    public Color color;
    public List<PlayerController> players;
    public string tag;

    public void AddPlayer(PlayerController player) {
        this.players.Add(player);   
        player.SetColor(this.color);
        player.stick.gameObject.tag = this.tag;
    }
}

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Team[] teams;

    public GameObject playerJoinUI;
    public GameObject playerJoinContainer;

    public GameObject player;

    public GameObject begin;

    private List<PlayerJoin> playerJoins;

	// Use this for initialization
	void Awake() {
        GameManager.instance = this;
	}

    void Start() {
        playerJoins = new List<PlayerJoin>();
        for (int i = 0; i < 4; i++) {
            PlayerJoin playerJoin = Instantiate(playerJoinUI, Vector3.zero, Quaternion.identity, playerJoinContainer.transform).GetComponent<PlayerJoin>();
            playerJoin.playerNum = i;
            playerJoins.Add(playerJoin);
        }
    }

    public void ResetGame() {
        for (int i = 0; i < teams.Length; i++) {
            for (int j = 0; j < teams[i].players.Count; j++) {
                Destroy(teams[i].players[j].gameObject);
            }

            teams[i].players = new List<PlayerController>();
        }

        for (int i = 0; i < playerJoins.Count; i++) {
            playerJoins[i].ReSelect();
            playerJoinContainer.SetActive(true);
        }
    }

    public void BeginGame() {
        for (int i = 0; i < this.playerJoins.Count; i++) {
            if(this.playerJoins[i].joinState == JoinState.SELECT) {
                return;
            }
        }

        MatchManager.instance.StartGame();
        this.begin.SetActive(false);
        this.playerJoinContainer.SetActive(false);
    }

    void Update() {
        Debug.Log(Input.GetJoystickNames().Length);
    }

    public PlayerController AddPlayer(int team) {
        PlayerController newPlayer = Instantiate(this.player, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        this.teams[team].AddPlayer(newPlayer);

        this.IsAnyPlayerReady();

        return newPlayer;
    }

    public void RemovePlayer(int team, PlayerController player) {
        this.teams[team].players.Remove(player);
        DestroyImmediate(player.gameObject);

        this.IsAnyPlayerReady();
    }

    private void IsAnyPlayerReady() {
        bool isReady = false;
		for (int i = 0; i < this.playerJoins.Count; i++) {
            if (playerJoins[i].joinState == JoinState.READY) {
                isReady = true;
                break;
			}
		}

        this.begin.SetActive(isReady);
    }
}
