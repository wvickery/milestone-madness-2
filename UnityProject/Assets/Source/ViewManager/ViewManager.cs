using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseViewTracker : CustomYieldInstruction
{
	public override bool keepWaiting
	{
		get
		{
			return !m_bDone;
		}
	}

	public BaseViewTracker(BaseView InTargetView)
	{
		m_TargetView = InTargetView;
	}

	public void SetComplete()
	{
		m_bDone = true;
	}

	public TViewType GetView<TViewType>()
		where TViewType : BaseView
	{
		return m_TargetView as TViewType;
	}

	private bool m_bDone = false;
	private BaseView m_TargetView;

}

public class ViewManager : MonoBehaviour
{
	#region Inspector Properties

	public List<BaseView> m_ViewPrefabs;

	#endregion

	#region API

	public BaseViewTracker ShowView<TViewType>(string PrefabName, bool bHidePrevious = true)
		where TViewType : BaseView
	{
		if (m_RootCanvas != null)
		{
			foreach (BaseView ViewPrefab in m_ViewPrefabs)
			{
				if (ViewPrefab.gameObject.name == PrefabName)
				{
					if (ViewPrefab is TViewType)
					{
						BaseView NewView = Instantiate<BaseView>(ViewPrefab, m_RootCanvas.transform);
						BaseViewTracker NewViewTracker = new BaseViewTracker(NewView);

						m_ShowQueue.Enqueue(NewViewTracker);

						return NewViewTracker;
					}
					else
					{
						throw new ArgumentException(String.Format("ShowView Type Mismatch.Prefab[{0}] Expected[{1}], Given[{2}]", PrefabName, typeof(TViewType).ToString(), ViewPrefab.GetType().ToString()));
					}
				}
			}
		}
		else
		{
			throw new NullReferenceException("Root Canvas is null.");
		}

		return null;
	}

	#endregion

	private Canvas m_RootCanvas = null;

	private Queue<BaseViewTracker> m_ShowQueue = new Queue<BaseViewTracker>();
	private Stack<BaseViewTracker> m_ViewStack = new Stack<BaseViewTracker>();
}
