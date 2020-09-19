using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hex;

public class HexTest : MonoBehaviour
{

    public GameObject hexPrefab;

    public GameObject mainArena;

    [SerializeField]
    [Range(0.01f, 100f)]
    public float mainRadius;

    // Start is called before the first frame update
    void Start()
    {
        if (hexPrefab == null) 
        {
            Debug.LogError("Hex prefab is null!");
            return;
        }

        float hexRadius = 2f;
        HexGrid grid = new HexGrid(hexRadius);
        RangeHex range = new RangeHex(VectorHex.zero, 25);
        foreach (VectorHex vec in range.GetRing()) 
        {
            Vector2 cart = grid.HexToCart(vec);
            GameObject obj = Instantiate(hexPrefab, new Vector3(cart.x, 0, cart.y), hexPrefab.transform.rotation);
            obj.transform.localScale = hexRadius * Vector3.one;
            obj.isStatic = true;
        }

        foreach (VectorHex vec in range.GetRing(24))
        {
            Vector2 cart = grid.HexToCart(vec);
            GameObject obj = Instantiate(hexPrefab, new Vector3(cart.x, 0, cart.y), hexPrefab.transform.rotation);
            obj.transform.localScale = hexRadius * Vector3.one;
            obj.isStatic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        mainArena.transform.localScale = new Vector3(mainRadius, 1.99f, mainRadius);
    }
}
