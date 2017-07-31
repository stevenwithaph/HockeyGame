using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : MonoBehaviour {

    public float rotationSpeed = 10.0f;
    public float movementSpeed = 5.0f;

    public int currentTeam = 0;
    public int playerNum = 0;

    public int goals = 0;
    public int assists = 0;

    public GameObject stick;

    private Rigidbody2D body;
    private InputDevice input;

    private float desiredHeading = 0;
    private float currentHeading = 0;
	// Use this for initialization
	void Start () {
        this.body = GetComponent<Rigidbody2D>();
        this.body.centerOfMass = Vector2.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (this.playerNum < InputManager.Devices.Count) {
            this.input = InputManager.Devices[this.playerNum];
            Rotate();
            Movement();
        }
	}

    void Rotate() {
        
        if (input.RightStick.Vector.x != 0.0f || input.RightStick.Vector.y != 0.0f)
		{
            this.desiredHeading = Mathf.Atan2(input.RightStick.Vector.y, input.RightStick.Vector.x) * Mathf.Rad2Deg;
		}

		this.currentHeading = Mathf.LerpAngle(this.currentHeading, this.desiredHeading, Time.fixedDeltaTime * rotationSpeed);

		this.body.MoveRotation(this.currentHeading);
    }

    void Movement() {
        Vector2 direction = input.Direction;
        this.body.AddForce(direction * 15.0f, ForceMode2D.Impulse);
    }

    public void SetColor(Color color) {
        GetComponent<SpriteRenderer>().color = color;
    }
}
