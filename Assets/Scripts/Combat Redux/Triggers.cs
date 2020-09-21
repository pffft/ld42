using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Combat.AbilityCaster;

public class Triggers : MonoBehaviour
{
    public void PlayerShoot(BoolRef br) => br.Value = Input.GetKey(KeyCode.G);
}
