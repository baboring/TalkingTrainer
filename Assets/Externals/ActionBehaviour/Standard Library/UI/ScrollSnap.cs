using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

namespace UI {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollSnap : UIBehaviour, IDragHandler, IEndDragHandler {
		
		[SerializeField] public int startingIndex = 0;
		[SerializeField] public bool wrapAround = false;
		[SerializeField] public float lerpTimeMilliSeconds = 200f;
		[SerializeField] public float triggerPercent = 5f;
		[Range(0f, 10f)] public float triggerAcceleration = 1f;
		[SerializeField] public RectTransform elementInScroll;
		[SerializeField] public CanvasGroup canvasView;    // touch group, in view

		public class OnLerpCompleteEvent : UnityEvent {}
		public OnLerpCompleteEvent onLerpComplete;
		public class OnReleaseEvent : UnityEvent<int> {}
		public OnReleaseEvent onRelease;

		int actualIndex;
		int cellIndex;
		ScrollRect scrollRect;
		RectTransform content;
		Vector2 cellSize;
		bool indexChangeTriggered = false;
		bool isLerping = false;
		DateTime lerpStartedAt;
		Vector2 releasedPosition;
		Vector2 targetPosition;

		public int CurrentIndex {
            get {
                int count = LayoutElementCount();
                int mod = actualIndex % count;
                return mod >= 0 ? mod : count + mod;
            }
        }
		Vector2 m_offset = Vector2.zero;


		protected override void Awake() {
			base.Awake();
			actualIndex = startingIndex;
			cellIndex = startingIndex;
			this.onLerpComplete = new OnLerpCompleteEvent();
			this.onRelease = new OnReleaseEvent();
			this.scrollRect = GetComponent<ScrollRect>();
			this.content = scrollRect.content;

			//this.cellSize = new Vector2(content.sizeDelta.x * content.localScale.x,content.sizeDelta.y * content.localScale.y);
			//content.anchoredPosition = new Vector2(-elementInScroll.rect.width * cellIndex, content.anchoredPosition.y);
			//int count = LayoutElementCount();
			//SetContentSize(count);

			//if(startingIndex < count) {
			//	MoveToIndex(startingIndex);
			//}
		}

		protected override void Start() {
			base.Start();

		}

        void OnEnable()
		{
			StartCoroutine(SetupCellSize());
		}

		IEnumerator SetupCellSize()
		{
			yield return new WaitForEndOfFrame();

			while(!CalcCellSize())
				yield return null;
		}

        bool CalcCellSize()
		{
			var elements = content.GetComponentsInChildren<LayoutElement>(false)
			                      .Where(e => e.transform.parent == content)
			                      .ToArray();
			if (elements.Length < 1)
				return false;
			if(elements.Length == 1)
			{
				var rectTrans = elements[0].GetComponent<RectTransform>();
                rectTrans.ForceUpdateRectTransforms();
				if (rectTrans.rect.size.magnitude == 0f)
                    return false;
				this.cellSize = rectTrans.rect.size;
			}
			else
			{
				var first = elements[0].GetComponent<RectTransform>();
                var second = elements[1].GetComponent<RectTransform>();
                first.ForceUpdateRectTransforms();
				if (first.rect.size.magnitude == 0f)
                    return false;

				this.cellSize.y = first.rect.size.y;
				this.cellSize.x = second.anchoredPosition.x - first.anchoredPosition.x;
			}

			m_offset = content.anchoredPosition;

			if(startingIndex < elements.Length) {
              MoveToIndex(startingIndex);
            }
			return true;
		}

		void LateUpdate() {

			if(isLerping) {
				LerpToElement();
				if(ShouldStopLerping()) {
					isLerping = false;
					canvasView.blocksRaycasts = true;
					onLerpComplete.Invoke();
					onLerpComplete.RemoveListener(WrapElementAround);
				}
			}
		}
		
		public void PushLayoutElement(LayoutElement element) {
			element.transform.SetParent(content.transform, false);
			SetContentSize(LayoutElementCount());
		}
		
		public void PopLayoutElement() {
			LayoutElement[] elements = content.GetComponentsInChildren<LayoutElement>();
			Destroy(elements[elements.Length - 1].gameObject);
			SetContentSize(LayoutElementCount() - 1);
			if(cellIndex == CalculateMaxIndex()) {
				cellIndex -= 1;
			}
		}
		
		public void UnshiftLayoutElement(LayoutElement element) {
			cellIndex += 1;
			element.transform.SetParent(content.transform, false);
			element.transform.SetAsFirstSibling();
			SetContentSize(LayoutElementCount());
			content.anchoredPosition = new Vector2(content.anchoredPosition.x - cellSize.x, content.anchoredPosition.y);
		}
		
		public void ShiftLayoutElement() {
			Destroy(GetComponentInChildren<LayoutElement>().gameObject);
			SetContentSize(LayoutElementCount() - 1);
			cellIndex -= 1;
			content.anchoredPosition = new Vector2(content.anchoredPosition.x + cellSize.x, content.anchoredPosition.y);
		}
		
		public int LayoutElementCount() {
			return content.GetComponentsInChildren<LayoutElement> (false)
				.Count (e => e.transform.parent == content);
		}
		

		public void OnDrag(PointerEventData data) {
			float dx = data.delta.x;
			float dt = Time.deltaTime * 1000f;
			float acceleration = Mathf.Abs(dx / dt);
			if(acceleration > triggerAcceleration && acceleration != Mathf.Infinity) {
				indexChangeTriggered = true;
			}
		}

		public void OnEndDrag(PointerEventData data) {
			if(IndexShouldChangeFromDrag(data)) {
				int direction = (data.pressPosition.x - data.position.x) > 0f ? 1 : -1;
				SnapToIndex(cellIndex + direction);
			} else {
				StartLerping();
			}
		}

		public void SnapToNext() {
			SnapToIndex(cellIndex + 1);
		}

		public void SnapToPrev() {
			SnapToIndex(cellIndex - 1);
		}

		public void SnapToIndex(int newCellIndex) {
			int maxIndex = CalculateMaxIndex();
			if(wrapAround && maxIndex > 0) {
				actualIndex += newCellIndex - cellIndex;
				cellIndex = newCellIndex;
				onLerpComplete.AddListener(WrapElementAround);
			} else {
				// when it's the same it means it tried to go out of bounds
				if(newCellIndex >= 0 && newCellIndex <= maxIndex) {
					actualIndex += newCellIndex - cellIndex;
					cellIndex = newCellIndex;
				}
			}
			onRelease.Invoke(cellIndex);
			StartLerping();
		}

		public void MoveToIndex(int newCellIndex) {
			int maxIndex = CalculateMaxIndex();
			if(newCellIndex >= 0 && newCellIndex <= maxIndex) {
				actualIndex += newCellIndex - cellIndex;
				cellIndex = newCellIndex;
			}
			onRelease.Invoke(cellIndex);	
			content.anchoredPosition = CalculateTargetPoisition(cellIndex);
		}

		void StartLerping() {
			releasedPosition = content.anchoredPosition;
			targetPosition = CalculateTargetPoisition(cellIndex);
			lerpStartedAt = DateTime.Now;
			canvasView.blocksRaycasts = false;
			isLerping = true;
		}

		int CalculateMaxIndex() {
			int cellPerFrame = Mathf.FloorToInt(scrollRect.GetComponent<RectTransform>().rect.size.x / cellSize.x);
			return LayoutElementCount() - cellPerFrame;
		}

		bool IndexShouldChangeFromDrag(PointerEventData data) {
			// acceleration was above threshold
			if(indexChangeTriggered) {
				indexChangeTriggered = false;
				return true;
			}
			// dragged beyond trigger threshold
			var offset = scrollRect.content.anchoredPosition.x + cellIndex * cellSize.x;
			var normalizedOffset = Mathf.Abs(offset / cellSize.x);
			return normalizedOffset * 100f > triggerPercent;
		}

		void LerpToElement() {
			float t = (float)((DateTime.Now - lerpStartedAt).TotalMilliseconds / lerpTimeMilliSeconds);
			float newX = Mathf.Lerp(releasedPosition.x, targetPosition.x, t);
			content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);
		}

		void WrapElementAround() {
			if(cellIndex <= 0) {
				var elements = content.GetComponentsInChildren<LayoutElement>();
				elements[elements.Length - 1].transform.SetAsFirstSibling();
				cellIndex += 1;
				content.anchoredPosition = new Vector2(content.anchoredPosition.x - cellSize.x, content.anchoredPosition.y);
			} else if(cellIndex >= CalculateMaxIndex()) {
				var element = content.GetComponentInChildren<LayoutElement>();
				element.transform.SetAsLastSibling();
				cellIndex -= 1;
				content.anchoredPosition = new Vector2(content.anchoredPosition.x + cellSize.x, content.anchoredPosition.y);
			}
		}
		
		void SetContentSize(int elementCount) {
			content.sizeDelta = new Vector2(cellSize.x * elementCount, content.rect.height);
		}
		
		Vector2 CalculateTargetPoisition(int index) {
			return new Vector2(-cellSize.x * index + m_offset.x, m_offset.y);
		}

		bool ShouldStopLerping() {
			return Mathf.Abs(content.anchoredPosition.x - targetPosition.x) < 0.001;
		}
	}
}