using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour {

    public float magnitude = 70f;
    public float angleAround = 0f;
    private float angleUpRaw = 0f;
    public float angleUp = 50f;

    public float aroundScale = 20f;
    public float upScale = 0.05f;

    private static Vector3 cameraPos;

	// Use this for initialization
	void Start () {
        Quaternion angle = Quaternion.AngleAxis(angleUp, Vector3.right);
        Quaternion angle2 = Quaternion.AngleAxis(angleAround, Vector3.up);

        cameraPos = angle * (Vector3.back * magnitude);
	}
	
	// Update is called once per frame
	void Update ()
    {
        angleAround = Mathf.Repeat(angleAround + (Time.deltaTime * aroundScale), 360f);
        angleUpRaw = Mathf.Repeat(angleUpRaw + (Time.deltaTime * upScale), Mathf.PI * 2);
        angleUp = (15f * Mathf.Sin(angleUpRaw)) + 45f;
        //angleUp = 85f;

        Quaternion angle = Quaternion.AngleAxis(angleUp, Vector3.right);
        Quaternion angle2 = Quaternion.AngleAxis(angleAround, Vector3.up);

        cameraPos = angle2 * angle * (Vector3.back * magnitude);

        transform.position = cameraPos;
        Camera.main.transform.LookAt(Vector3.zero);
    }
}
