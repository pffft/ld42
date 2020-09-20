using CombatCore;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HUD : MonoBehaviour
{
    [SerializeField]
    private Entity player = null;
    
    [SerializeField]    
    private Entity shield = null;

    [Header("Shield")]
    [SerializeField]
    private Image shieldIndicator = null;

    [SerializeField]
    private Image shieldPointer = null;

    [SerializeField]
    private Color[] shieldIndicatorColors = new Color[2];

    [SerializeField]
    private Color unavailableShieldIndicatorColor = Color.gray;
    public Color UnavailableShieldIndicatorColor
    {
        get { return unavailableShieldIndicatorColor; }
        set { unavailableShieldIndicatorColor = value; }
    }

    [SerializeField]
    private Color shieldPointerColor;
    public Color ShieldPointerColor
    {
        get { return shieldPointerColor; }
        set { shieldPointerColor = value; }
    }

    public bool shieldAvailable = true;

    [Header ("Dash")]
    [SerializeField]
    private Image dashIndicator = null;

    [SerializeField]
    private Color dashIndicatorColor = Color.cyan;
    public Color DashIndicatorColor
    {
        get { return dashIndicatorColor; }
        set { dashIndicatorColor = value; }
    }

    [SerializeField]
    private Vector3 followOffset = new Vector3(0f, 0f, 0.1f);

    public void Update()
    {
        if (dashIndicator != null)
        {
            dashIndicator.color = dashIndicatorColor;
        }

        if (shieldIndicator != null && shield != null)
        {
            if (shieldAvailable)
            {
                shieldIndicator.color = Color.Lerp (shieldIndicatorColors[1], shieldIndicatorColors[0], shield.ShieldPerc);
                if (shieldPointer != null)
                {
                    shieldPointer.color = Color.clear;
                }
            }
            else
            {
                shieldIndicator.color = unavailableShieldIndicatorColor;
                if (shieldPointer != null)
                {
                    shieldPointer.color = shieldPointerColor;
                    shieldPointer.fillAmount = 1f - (5f / Vector3.Distance (transform.position, GameManager.ThrownShield.transform.position));
                    Quaternion q = Quaternion.LookRotation ((GameManager.ThrownShield.transform.position - transform.position).normalized, Vector3.forward);
                    shieldPointer.transform.rotation = Quaternion.Euler (
                        shieldPointer.transform.rotation.eulerAngles.x, 
                        q.eulerAngles.y, 
                        shieldPointer.transform.rotation.eulerAngles.z);
                }
            }

            shieldIndicator.fillAmount = shield.ShieldPerc;
        }
    }

    public void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position + followOffset;

            if (dashIndicator != null)
            {
                dashIndicator.transform.position = player.GetComponent<Controller> ().GetDashTargetPoint () + Vector3.up * 0.01f;
            }
        }
    }
}
