using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGen : MonoBehaviour {

    private GameObject arenaObjPrefab;
    private GameObject blockPrefab;

    private List<Transform> initialTransforms;
    private List<Vector3> initialPositions;
    private List<Vector3> finalPositions;

    public float scale = 1f;
    private Vector3 offset = new Vector3(50, 0, 50);

    public int Depth = 28;
    public int MaxWidth = 8;

	// Use this for initialization
	void Start () {
        arenaObjPrefab = Resources.Load<GameObject>("Models/City_Roundabout_Arena");
        blockPrefab = Resources.Load<GameObject>("Prefabs/City_Block_1");

        initialTransforms = new List<Transform>();
        initialPositions = new List<Vector3>();
        finalPositions = new List<Vector3>();

        GameObject arenaBlock = GameObject.Instantiate(arenaObjPrefab);
        initialTransforms.Add(arenaBlock.transform);
        //GameObject.Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);

        for (int depth = -1; depth < Depth; depth++)
        {
            float maxWidth = Mathf.Max(MaxWidth, scale * depth);
            for (int width = (int)(-maxWidth); width <= maxWidth; width++)
            {
                if ((width == -1 || width == 0) && (depth == -1 || depth == 0))
                {
                    continue;
                }
                GameObject newObj = GameObject.Instantiate(blockPrefab, offset + new Vector3(200f * width, 0f, 200f * depth), Quaternion.AngleAxis(Random.Range(0, 3) * 90f, Vector3.up));
                initialTransforms.Add(newObj.transform);
            }
        }

        MoveDown();
	}

    private Vector3 finalOffset = new Vector3(0, -1000, 0);
    bool shouldMove = true;
    private float currTime = 0f;
    private float maxTime = 5f;

    public void MoveDown()
    {
        shouldMove = true;
        //GameManager.Boss.Add(new Moves.Basic.MoveCamera());
    }

	// Update is called once per frame
	void Update () {
        if (Time.time > 10f)
        {
            if (shouldMove)
            {
                foreach (Transform t in initialTransforms)
                {
                    initialPositions.Add(t.position);
                    finalPositions.Add(t.position + finalOffset);
                }
                shouldMove = false;
            }
            currTime += Time.deltaTime;
            for (int i = 0; i < initialTransforms.Count; i++)
            {
                Transform t = initialTransforms[i];
                t.position = Vector3.Lerp(initialPositions[i], finalPositions[i], currTime / maxTime);
            }
            GameObject.Find("PlayerCamera").GetComponent<CameraController>().MinZoom = Mathf.Lerp(20, 120, currTime * 2 / maxTime);
        }
	}
}
