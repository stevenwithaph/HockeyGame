using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public enum JoinState {
    JOIN,
    SELECT,
    READY,
}

public class PlayerJoin : MonoBehaviour {

    public JoinState joinState = JoinState.JOIN;
    public int playerNum = 0;

    public GameObject joinUI;
    public GameObject selectUI;
    public GameObject readyUI;
    public Image teamColor;

    private InputDevice input;
    private PlayerController player;
    private int currentTeam = 0;

	// Use this for initialization
	void Start () {
        teamColor.color = GameManager.instance.teams[this.currentTeam].color;
	}
	
	// Update is called once per frame
	void Update () {
        if(this.playerNum < InputManager.Devices.Count) {
			this.input = InputManager.Devices[this.playerNum];
			switch (joinState)
			{
				case JoinState.JOIN:
					JoinUpdate();
					break;
				case JoinState.SELECT:
					SelectUpdate();
					break;
				case JoinState.READY:
					ReadyUpdate();
					break;
			}   
        }
	}

    // Actions
    void JoinGame() {
        this.joinState = JoinState.SELECT;

		this.selectUI.SetActive(true);
		this.joinUI.SetActive(false);
    }

    void LeaveGame() {
        this.joinState = JoinState.JOIN;

		this.selectUI.SetActive(false);
		this.joinUI.SetActive(true);
    }

	void JoinTeam() {
        this.joinState = JoinState.READY;

		this.player = GameManager.instance.AddPlayer(this.currentTeam);
        this.player.playerNum = this.playerNum;

        Vector3 playerPosition = Camera.main.ScreenToWorldPoint(this.selectUI.transform.position);
        playerPosition.z = 0;
        this.player.transform.position = playerPosition;

		this.readyUI.SetActive(true);
		this.selectUI.SetActive(false);
	}

	void LeaveTeam() {
		this.joinState = JoinState.SELECT;

		GameManager.instance.RemovePlayer(this.currentTeam, this.player);

		this.readyUI.SetActive(false);
		this.selectUI.SetActive(true);
	}

    void NextTeam() {
        this.currentTeam = (this.currentTeam + 1) % 2;
        this.teamColor.color = GameManager.instance.teams[this.currentTeam].color;
    }

    public void ReSelect() {
        if(this.joinState == JoinState.READY) {
            this.selectUI.SetActive(true);
            this.readyUI.SetActive(false);
            this.joinState = JoinState.SELECT;
        }
    }

    //  poor mans fsm
    void JoinUpdate() {
        if (input.Action1.WasPressed) {
            this.JoinGame();
        }
    }

    void SelectUpdate() {
        if (input.Direction.Left.WasPressed || input.Direction.Right.WasPressed) {
            this.NextTeam();
        }

        if (input.Action1.WasPressed) {
            this.JoinTeam();
        }

		if (input.Action2.WasPressed) {
            this.LeaveGame();
		}
    }

    void ReadyUpdate() {
        if (input.MenuWasPressed) {
            GameManager.instance.BeginGame();
        }

		if (input.Action2.WasPressed) {
            this.LeaveTeam();
		}
    }
}
