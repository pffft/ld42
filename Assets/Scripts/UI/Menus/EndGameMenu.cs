using CombatCore;
using GameUI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
	[SerializeField]
	private float menuAppearanceDelay = 3f;

	private Entity player;

	public void Start()
	{
        player = GameManager.Player.GetComponent<Entity> ();
		player.died += GameEnd;

		Menu.GetMenu ("End Game Menu").CloseImmediate ();
	}

	private void GameEnd()
	{
		StartCoroutine (BringUpMenu ());
	}

	private IEnumerator BringUpMenu()
	{
		//TODO way to interrupt this?
		yield return new WaitForSecondsRealtime (menuAppearanceDelay);
		MenuManager.GetInstance ().NavigateTo (Menu.GetMenu ("End Game Menu"));
	}

	public void RestartGame()
	{
		GameManager.ResetGame ();
		MenuManager.GetInstance ().NavigateTo (null);
	}

	public void ExitGame()
	{
		SceneManager.LoadScene ("Main");
	}
}
