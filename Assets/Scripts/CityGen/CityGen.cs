using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGen : MonoBehaviour {

    private GameObject arenaObjPrefab;
    private GameObject blockPrefab;

    public float scale = 1f;

	// Use this for initialization
	void Start () {
        arenaObjPrefab = Resources.Load<GameObject>("Models/City_Roundabout_Arena");
        blockPrefab = Resources.Load<GameObject>("Prefabs/City_Block_1");


        //GameObject.Instantiate(arenaObjPrefab);
        //GameObject.Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
        for (int depth = 0; depth < 28; depth++)
        {
            float maxWidth = Mathf.Max(8, scale * depth);
            for (int width = (int)(-maxWidth); width <= maxWidth; width++)
            {
                GameObject newObj = GameObject.Instantiate(blockPrefab, new Vector3(100f * width, 0f, 100f * depth), Quaternion.AngleAxis(Random.Range(0, 3) * 90f, Vector3.up));
                newObj.isStatic = true;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
