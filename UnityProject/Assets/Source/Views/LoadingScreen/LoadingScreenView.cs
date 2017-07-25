using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreenView : BaseView
{
	public Image m_Base;
	public Image m_Barrel;

	protected override IEnumerator AnimateIn()
	{
		yield return m_Barrel.rectTransform.DORotate(new Vector3(0, 0, 90), 1.0f).WaitForCompletion();
		yield return new WaitForSeconds(0.25f);
		yield return m_Barrel.rectTransform.DORotate(new Vector3(0, 0, 180), 1.0f).WaitForCompletion();
		yield return new WaitForSeconds(0.25f);
		yield return m_Barrel.rectTransform.DORotate(new Vector3(0, 0, 270), 1.0f).WaitForCompletion();
		yield return new WaitForSeconds(0.25f);
		yield return m_Barrel.rectTransform.DORotate(new Vector3(0, 0, 0), 1.0f).WaitForCompletion();

		Close();
	}

	protected override IEnumerator AnimateOut()
	{
		m_Base.DOFade(0, 2.0f);
		yield return m_Barrel.DOFade(0, 2.0f).WaitForCompletion();
	}
}
