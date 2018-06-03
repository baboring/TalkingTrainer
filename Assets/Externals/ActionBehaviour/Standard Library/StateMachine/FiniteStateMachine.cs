/********************************************************************
	created:	2014/12/11
	filename:	ButtonHandler.cs
	author:		Benjamin
	purpose:	[FSM]
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TTrainer
{
	public delegate void Callback();

	/// State Transition Class
	public class StateTransition<T> : System.IEquatable<StateTransition<T>>
	{
		// Public variables
		// ----------------------------------------

		// Protected variables
		// ----------------------------------------
		protected T mInitState;
		protected T mEndState;

		// Public functions
		// ----------------------------------------
		public StateTransition() { }
		public StateTransition(T init, T end) { mInitState = init; mEndState = end; }

		public bool Equals(StateTransition<T> other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return mInitState.Equals(other.GetInitState()) && mEndState.Equals(other.GetEndState());
		}

		public override int GetHashCode()
		{
			if ((mInitState == null || mEndState == null))
				return 0;

			unchecked {
				int hash = 17;
				hash = hash * 23 + mInitState.GetHashCode();
				hash = hash * 23 + mEndState.GetHashCode();
				return hash;
			}
		}

		public T GetInitState() { return mInitState; }
		public T GetEndState() { return mEndState; }
	}


	/// A generic Finite state machine
	public class FiniteStateMachine<T>
	{
		// Public variables
		// ----------------------------------------

		// Protected variables
		// ----------------------------------------
		protected T mState;
		protected T mPrevState;
		protected IEnumerable currTransEnumerator;
		protected bool mbLocked = false;

		protected Dictionary<StateTransition<T>, System.Delegate> mTransExit;
		protected Dictionary<StateTransition<T>, System.Delegate> mTransExcute;
		protected Dictionary<StateTransition<T>, System.Delegate> mTransEnter;

		protected Dictionary<StateTransition<T>, IEnumerable> mTransEnumerator;	// 코루틴 전용

		StateTransition<T> currTransExcute = null;
		// Public functions
		// ----------------------------------------
		public FiniteStateMachine()
		{
			mTransExcute = new Dictionary<StateTransition<T>, System.Delegate>();
			mTransExit = new Dictionary<StateTransition<T>, System.Delegate>();
			mTransEnter = new Dictionary<StateTransition<T>, System.Delegate>();
			mTransEnumerator = new Dictionary<StateTransition<T>, IEnumerable>();
		}

		// 처음 시작 상태 설정
		public void Initialize(T state) { 
			mState = state;
			mPrevState = state;
		}

		// 상태 전이 등록
		public void AddTransition(T init, T end,
			Callback _callback_excute,
			Callback _callback_exit = null,
			Callback _callback_enter = null,
			IEnumerable _routine = null)
		{
			StateTransition<T> tr = new StateTransition<T>(init, end);

			if (mTransExcute.ContainsKey(tr)) {
				Debug.LogErrorFormat("[FSM] Transition: {0} - {1} exists already." , tr.GetInitState(),tr.GetEndState());
				return;
			}

			mTransEnter.Add(tr, _callback_enter);
			mTransExcute.Add(tr, _callback_excute);
			mTransExit.Add(tr, _callback_exit);
			mTransEnumerator.Add(tr, _routine);

			//Debug.Log("[FSM] Added transition " + mTransExcute.Count + ": " + tr.GetInitState() + " - " + tr.GetEndState() + ", Callback: " + _callback_excute);
		}

		// Cheking Function
		public bool IsNextConfirm(T nextState) {
			if (mbLocked)
				return false;

			// Check if the transition is valid
			return mTransExcute.ContainsKey(new StateTransition<T>(mState, nextState));
		}

		// Advece to next state
		public bool Advance(T nextState) {
			if (mbLocked)
				return false;

			// Check if the transition is valid
			StateTransition<T> transition = new StateTransition<T>(mState, nextState);
			System.Delegate _delegate;
			if (!mTransExcute.TryGetValue(transition, out _delegate)) // new StateTransition(mState, nextState)
			{
				Debug.LogErrorFormat("[FSM] Cannot advance from {0}  to {1} state", mState, nextState);
				return false;
			}

			// Do Exit prev state
			_DoCallback(mTransExit, new StateTransition<T>(mPrevState,mState));

			//Debug.Log(ColorType.cyan, "[FSM] Advancing to " + nextState + " from " + mState);

			// Change state
			mPrevState = mState;
			mState = nextState;
			this.currTransExcute = transition;

			// pick up current one
			mTransEnumerator.TryGetValue(transition, out currTransEnumerator);


			// Do Enter State
			_DoCallback(mTransEnter, transition);

			return true;
		}

		// do callback
		static void _DoCallback(Dictionary<StateTransition<T>, System.Delegate> dicTrans, StateTransition<T> trans)
		{
			if (null == trans || null == dicTrans)
				return;

			System.Delegate _delegate;
			if (dicTrans.TryGetValue(trans, out _delegate)) {
				Callback _callback = _delegate as Callback;
				if (null != _callback)
					_callback();
			}
		}

		// for coroutine
		public void DoCororutine(MonoBehaviour _mono)
		{
			if (null != currTransEnumerator)
				_mono.StartCoroutine(currTransEnumerator.GetEnumerator());
		}
		
		// command  
		public IEnumerator routine
		{
			get { return currTransEnumerator.GetEnumerator(); }
		}

		// Call this to prevent the state machine from leaving this state
		public void Lock() { mbLocked = true; }

		public void Unlock()
		{
			mbLocked = false;
			Advance(mPrevState);
		}

		public T GetState() { return mState; }
		public T GetPrevState() { return mPrevState; }

		// 주기적으로 실행 할 것...
		public void Update() {
			_DoCallback(mTransExcute, currTransExcute);
		}
	}

}
