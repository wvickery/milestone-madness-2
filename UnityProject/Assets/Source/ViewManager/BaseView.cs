using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseView : MonoBehaviour
{
	#region API
	public void Open(ViewManager InViewManager)
	{
		if (m_ViewManager == null)
		{
			m_ViewManager = InViewManager;
			StartCoroutine(Show());
		}
	}

	public void Close()
	{
		if (m_ViewManager != null)
		{
			StartCoroutine(Hide());
			m_ViewManager = null;
		}
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

	private ViewManager m_ViewManager = null;

	private int m_ShowCount = 0;
	private bool m_bShown = false;
}
