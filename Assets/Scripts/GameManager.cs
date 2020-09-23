using CombatCore;
using UnityEngine;
using World;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
	#region STATIC_VARS

	private static GameManager instance;

	public static BossController Boss { get { return instance.boss; } }
    public static Controller Player { get { return instance.player; } }
    public static Arena Arena { get { return instance.arena; } }
    public static GameObject HeldShield
	{
		get { return instance.heldShield; }
		set { instance.heldShield = value; }
	}
    public static GameObject PlacedShield
	{
		get { return instance.placedShield; }
		set { instance.placedShield = value; }
	}
    public static GameObject ThrownShield
	{
		get { return instance.thrownShield; }
		set { instance.thrownShield = value; }
	}
    public static HUD HUD { get { return instance.hud; } }

	public static float TimeScale
	{
		get { return Time.timeScale; }
		set { Time.timeScale = value; Time.fixedDeltaTime = 0.02f * Time.timeScale; }
	}
	#endregion

	#region INSTANCE_VARS

	[SerializeField]
	private BossController boss = null;
	private Vector3 startingBossPosition;

	[SerializeField]
	private Controller player = null;
	private Vector3 startingPlayerPosition;

	[SerializeField]
	private Arena arena = null;

	[SerializeField]
	private GameObject heldShield = null;

	[SerializeField]
	private GameObject placedShield = null;

	[SerializeField]
	private GameObject thrownShield = null;

	[SerializeField]
	private HUD hud = null;
	#endregion

	#region STATIC_METHODS

	/// <summary>
	/// Resets all player-facing game components to starting values.
	/// </summary>
	public static void ResetGame()
	{
		if (instance == null)
		{
			Debug.LogError ("Cannot reset; no GameManager instance!");
			return;
		}

		//boss boy
		Boss.transform.position = instance.startingBossPosition;
		Entity bossData = Player.GetComponent<Entity> ();
		Entity.HealEntity (bossData, float.PositiveInfinity);
		bossData.ClearStatuses ();
        //TODO reset boss behavior

        Boss.ResetBoss();
		Debug.Log ("Reset boss");

		//player
		Player.transform.position = instance.startingPlayerPosition;
		Entity playerData = Player.GetComponent<Entity> ();
		Entity.HealEntity (playerData, float.PositiveInfinity);
		playerData.ClearStatuses ();
		Player.gameObject.SetActive (true);

		Debug.Log ("Reset player");

		//general game stuff
		TimeScale = 1f;
		instance.arena.enabled = true;

		Debug.Log ("Finished game reset");
	}
	#endregion

	#region INSTANCE_METHODS
	public void Awake()
    {
        if (Application.isPlaying)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

		if (instance == null)
		{
			instance = this;
			if(boss != null)
				startingBossPosition = boss.transform.position;
			if (player != null)
				startingPlayerPosition = player.transform.position;
		}
		else
		{
			Debug.Log("Extra GameManager in scene on \"" + gameObject.name + "\"");
#if UNITY_EDITOR
			UnityEditor.EditorGUIUtility.PingObject (gameObject);
#endif
		}
    }
	#endregion
}
