/* *************************************************
*  Created:  7/23/2017, 11:22:10 AM
*  File:     StateTransitionTable.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTrainer
{
	public interface IState {
		void Enter(Entity e);
		void Execute(Entity e);
		void Exit(Entity e);
	}

	// 기본적인 상태 전이 테이블
	abstract public class StateTransitionTable {
		protected Dictionary<object, IState> table = new Dictionary<object, IState>();

		public void SetState(object evt, IState state) {
			table.Add(evt, state);
		}

		public IState GetState(object evt) {
			IState state;
			if(table.TryGetValue(evt,out state))
				return state;
			return null;
		}
	}

	// 기본적인 상태 이벤트
	abstract public class Entity {
		protected StateTransitionTable transitionTable = null;
		public IState currentState {get; private set;}
		protected object currentEvent;

		public void UpdateState() {
			if (currentState != null)
				currentState.Execute(this);
			else
				System.Diagnostics.Trace.WriteLine("zero state");
		}

		public object Event {
			set {
				if(null != value)
					Debug.Log("FSM Event:"+value.ToString());
				currentEvent = value;
				if (value == null) {
					currentState.Exit(this);
					currentState = null;
					return;
				}

				IState i = transitionTable.GetState(value);
				if (i != null) {
					if (currentState != null)
						currentState.Exit(this);

					currentState = i;
					currentState.Enter(this);
				}
			}

			get {
				return currentEvent;
			}
		}
	}
}
