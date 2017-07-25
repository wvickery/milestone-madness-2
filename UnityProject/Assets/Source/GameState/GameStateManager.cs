using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	private ViewManager m_ViewManager = null;

	void Start ()
	{
		DontDestroyOnLoad(this);

		m_ViewManager = FindObjectOfType<ViewManager>();

		StartCoroutine(StartUp());
	}
	
	void Update ()
	{
		
	}

	IEnumerator StartUp()
	{
		if (m_ViewManager != null)
		{
			yield return m_ViewManager.OpenView<LoadingScreenView>("LoadingScreen");
			yield return m_ViewManager.OpenView<MainMenu>("MainMenu");
		}
	}
}
