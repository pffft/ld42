using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Combat.AbilityBehaviors 
{
    public class PlayerDashBehavior : AbilityBehavior
    {
        public override bool Start(GameObject blackboard, Ability ability) 
        {
            AbilityCaster.BoolRef br = new AbilityCaster.BoolRef();

            // We can't dash when we're aiming or shooting.
            blackboard.GetComponent<Controller>().PlayerShoot(br);
            if (br)
            {
                return false;
            }
            return true;
        }

        public override IEnumerator Update()
        {
            // TODO implement dashing
            yield return null;
        }
        public override void Finish(Result result)
        {

        }
        
        public Vector3 GetMousePos() 
        {
            Vector3 facePos = Vector3.zero;

            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float dist;
            if (plane.Raycast(camRay, out dist))
            {
                facePos = camRay.origin + (dist * camRay.direction);
                Vector3 dir;
                if ((dir = (facePos - GameManager.Player.transform.position)).magnitude > 15f)
                {
                    facePos = GameManager.Player.transform.position + (dir.normalized * 15f);
                }
            }

            return facePos;
        }

    }
}