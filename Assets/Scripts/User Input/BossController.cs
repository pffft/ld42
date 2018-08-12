using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {

    GameObject player;
    private float speedScale = 100f;

    private float rotationScale = 10f;
    private float xRot;
    private float yRot;
    private Quaternion newRotation;

    Camera mainCamera;
    private float followDistance = 10f;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        mainCamera.transform.position = player.transform.position + new Vector3(-10, 10, 0);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0 && mouseY != 0) {
            Quaternion rot = mainCamera.transform.rotation;

            xRot = xRot + (rotationScale * mouseX);
            yRot = Mathf.Clamp(yRot + (rotationScale * mouseY), -90f, 90f);

            newRotation = Quaternion.Euler(new Vector3(yRot, xRot, 0));
        }

        followDistance -= Input.GetAxis("Mouse ScrollWheel");
        if (followDistance < 0)
            followDistance = 0;


        //Vector3 forward = player.transform.position - mainCamera.transform.position;
        //mainCamera.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        mainCamera.transform.position = player.transform.position + (newRotation * new Vector3(0, 0, -followDistance));
        mainCamera.transform.rotation = newRotation;

        //movement
        Vector3 movementVector = Vector2.zero;

        bool up = Input.GetKey(KeyCode.W);
        bool left = Input.GetKey(KeyCode.A);
        bool down = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);

        if (up)
            movementVector += speedScale * Vector3.forward;
        if (left)
            movementVector += speedScale * Vector3.left;
        if (down)
            movementVector += speedScale * Vector3.back;
        if (right)
            movementVector += speedScale * Vector3.right;

        player.GetComponent<Rigidbody>().AddForce(movementVector);
        //player.GetComponent<Rigidbody>().velocity = movementVector;
	}
}
