using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseView : MonoBehaviour
{
	#region API
	public delegate void OnClosedDelegate();
	public OnClosedDelegate OnClosed;

	public Coroutine Open()
	{
		if (!m_bOpened)
		{
			m_bOpened = true;
			return StartCoroutine(Show());
		}

		return null;
	}

	public Coroutine Close()
	{
		if (m_bOpened)
		{
			return StartCoroutine(CloseCoroutine());
		}

		return null;
	}

	public IEnumerator Show(bool bInstant = false)
	{
		if (!m_bShown)
		{
			m_ShowCount++;
			PreShow(m_ShowCount == 1);

			if (!bInstant)
			{
				yield return StartCoroutine(AnimateIn());
			}

			m_bShown = true;
			OnShow();
		}
	}

	public IEnumerator Hide(bool bInstant = false)
	{ 
		if (m_bShown)
		{
			OnHide();

			if (!bInstant)
			{
				yield return StartCoroutine(AnimateOut());
			}

			m_bShown = false;
			PostHide();
		}
	}
	#endregion

	#region View Interface
	protected virtual void PreShow(bool bFirstTime) { }
	protected virtual IEnumerator AnimateIn() { yield return null; }
	protected virtual void OnShow() { }

	protected virtual void OnHide() { }
	protected virtual void PostHide() { }

	protected virtual IEnumerator AnimateOut() { yield return null; }
	#endregion

	#region Internal
	private int m_ShowCount = 0;

	private bool m_bOpened = false;
	private bool m_bShown = false;

	private IEnumerator CloseCoroutine()
	{
		if (m_bOpened)
		{
			yield return StartCoroutine(Hide());

			m_bOpened = false;

			if (OnClosed != null)
			{
				OnClosed();
			}
		}
	}

	#endregion
}
