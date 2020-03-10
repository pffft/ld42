using Hex;
using Hex.Converters;
using UnityEngine;

public class HexExample : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab = null;

    [SerializeField]
    private int radius = 10;

    private void Start()
    {
        HexGrid grid = new HexGrid();
        RangeHex area = new RangeHex(VectorHex.zero, radius);
        HexToCartXZ xzSpace = new HexToCartXZ(grid);

        Vector3 prefabPostion;
        foreach (VectorHex hexPos in area)
        {
            prefabPostion = xzSpace.convertTo(hexPos);
            Instantiate(tilePrefab ?? new GameObject(), prefabPostion, Quaternion.identity);
        }
    }
}
