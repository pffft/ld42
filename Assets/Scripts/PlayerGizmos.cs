using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerGizmos : MonoBehaviour
{

    private bool shouldDraw = false;
    private float actualAngle = 45f;
    private Vector3 target = Vector3.zero;

    public void DrawShotgun(bool shouldDraw, float actualAngle, Vector3 target) 
    {
        this.shouldDraw = shouldDraw;
        this.actualAngle = actualAngle;
        this.target = target;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        if (!shouldDraw) 
        {
            return;
        }

        //Vector3 towardsBoss = GameManager.Boss.transform.position - GameManager.Player.transform.position;
        Handles.color = Color.magenta;
        if (Mathf.Approximately(actualAngle, 0)) 
        {
            Handles.DrawWireCube(GameManager.Player.transform.position + target, 2 * Vector3.one);
        }
        else
        {
            Handles.DrawWireArc(
                GameManager.Player.transform.position, 
                Vector3.up,
                Quaternion.AngleAxis(-actualAngle / 2.0f, Vector3.up) * target, 
                actualAngle, 
                10
            );
        }
    }
}
