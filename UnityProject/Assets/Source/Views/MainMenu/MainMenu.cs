using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : BaseView
{
	public Text m_Title;

	protected override void PreShow(bool bFirstTime)
	{
		m_Title.color = new Color(1, 1, 1, 0);
	}

	protected override IEnumerator AnimateIn()
	{
		yield return m_Title.DOFade(1.0f, 2.0f).WaitForCompletion();
	}
}
