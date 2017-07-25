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

	public void Track(BaseView InView)
	{
		m_TrackedView = InView;
	}

	public void End()
	{
		m_bDone = true;
	}

	public TViewType GetView<TViewType>()
		where TViewType : BaseView
	{
		return m_TrackedView as TViewType;
	}

	private bool m_bDone = false;
	private BaseView m_TrackedView = null;
}

public class ViewManager : MonoBehaviour
{
	#region Inspector Properties

	public List<BaseView> m_ViewPrefabs;

	#endregion

	#region API

	public BaseViewTracker OpenView<TViewType>(string PrefabName, bool bHidePrevious = true)
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
						BaseViewTracker NewViewTracker = new BaseViewTracker();

						m_ShowQueue.Enqueue(new ViewQueueEntry(ViewPrefab, NewViewTracker));

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

	#region Internal

	public void Awake()
	{
		for (int Index = 0; Index < transform.childCount; Index++)
		{
			Canvas ChildCanvas = transform.GetChild(Index).gameObject.GetComponent<Canvas>();

			if (ChildCanvas != null)
			{
				m_RootCanvas = ChildCanvas;
				break;
			}
		}
	}

	public void Update()
	{
		while (!m_bPushingView && (m_ShowQueue.Count > 0) && (m_RootCanvas != null))
		{
			ViewQueueEntry QueueEntry = m_ShowQueue.Dequeue();

			StartCoroutine(PushView(QueueEntry));
		}
	}

	private IEnumerator PushView(ViewQueueEntry QueueEntry)
	{
		if (!m_bPushingView && QueueEntry.IsValid())
		{
			m_bPushingView = true;

			if (m_ViewStack.Count > 0)
			{
				yield return StartCoroutine(m_ViewStack.Peek().GetView<BaseView>().Hide());
			}

			BaseView NewView = Instantiate<BaseView>(QueueEntry.PrefabType, m_RootCanvas.transform);

			QueueEntry.Tracker.Track(NewView);
			NewView.OnClosed += QueueEntry.Tracker.End;

			m_ViewStack.Push(QueueEntry.Tracker);

			yield return QueueEntry.Tracker.GetView<BaseView>().Open();

			m_bPushingView = false;
		}
	}

	private Canvas m_RootCanvas = null;

	struct ViewQueueEntry
	{
		public BaseView PrefabType;
		public BaseViewTracker Tracker;

		public ViewQueueEntry(BaseView InPrefabType, BaseViewTracker InTracker)
		{
			PrefabType = InPrefabType;
			Tracker = InTracker;
		}

		public bool IsValid()
		{
			return (PrefabType != null) && (Tracker != null);
		}
	}

	private Queue<ViewQueueEntry> m_ShowQueue = new Queue<ViewQueueEntry>();

	private bool m_bPushingView;

	private Stack<BaseViewTracker> m_ViewStack = new Stack<BaseViewTracker>();

	#endregion
}
