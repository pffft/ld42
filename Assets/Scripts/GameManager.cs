using CombatCore;
using UnityEngine;
using World;

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
	private HUD hud;
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

		//general game stuff
		TimeScale = 1f;

		//boss boy
		Boss.transform.position = instance.startingBossPosition;
		Entity bossData = Player.GetComponent<Entity> ();
		Entity.HealEntity (bossData, float.PositiveInfinity);
		bossData.ClearStatuses ();
		//TODO reset boss behavior

		Debug.Log ("Reset boss");

		//player (just nuke humpty-dumpty; we can't put him back together again) 
		//TODO

		Debug.Log ("Reset player");

		Debug.Log ("Finished game reset");
	}
	#endregion

	#region INSTANCE_METHODS
	public void Awake()
    {
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
