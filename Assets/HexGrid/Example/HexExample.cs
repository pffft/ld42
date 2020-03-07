using Hex;
using UnityEngine;

public class HexExample : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab = null;

    [SerializeField]
    private int radius = 10;

    [SerializeField]
    private Vector3 planeNormal = Vector3.up;

    private void Start()
    {
        HexGrid grid = new HexGrid(1f);
        GridPlane plane = new GridPlane(Vector3.back);
        RangeHex area = new RangeHex(VectorHex.zero, radius);

        Debug.Log($"{Vector2.one} -> {plane.Normal} = {plane[Vector2.one]}");

        //Vector3 prefabPostion;
        //foreach (VectorHex hexPos in area)
        //{
        //    prefabPostion = plane.Get3dPositionAt(grid.HexToCart(hexPos));
        //    Instantiate(tilePrefab?? new GameObject(), prefabPostion, Quaternion.identity);
        //}
    }
}
