/* *************************************************
*  Created:  2018-1-28 19:46:40
*  File:     BaseNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

namespace ActionBehaviour
{

    public enum ActionState
    {
        None = 0,
        Success,
        Fail,
        Running,
        Error = -1,

    }
    public interface IBaseNode {
        
        ActionState OnUpdate();
    }

	// Base class
    public abstract class BaseNode : MonoBehaviour,IBaseNode {

        protected ActionState _curr_state;
        protected ActionState _prev_state;

		// state
        public ActionState state {
            get { return _curr_state; }
             
            protected set
            {
                if(_curr_state != value) {
                    _prev_state = _curr_state;
                    _curr_state = value;
                    // notify state changed
                    if (null != onChangedState)
                        onChangedState(_curr_state,_prev_state);
                }
            }
        }

        // delegates
        public System.Action<ActionState,ActionState> onChangedState;


        // Hierarchy functions
        protected virtual void OnReset() {}


        // Interface methods
        public void Reset() { 
            _curr_state = ActionState.None; 
            _prev_state = ActionState.None; 

            OnReset(); 
        }

		// ready
		public abstract ActionState OnUpdate();

		// default core function
		public virtual ActionState Execute() {

			// start up 
            OnReset();

			// update 
            if(ActionState.Running == state || ActionState.None == state) 
				state = OnUpdate();

			return state;
		}
	}

    // Aync proc
    public class AsyncProcessing
    {
        public event System.Action<AsyncProcessing> onCompleted;
        bool _isDone;
        public bool isDone
        {
            get
            {
                return _isDone;
            }
            set
            {
                _isDone = value;
                if (_isDone && null != onCompleted)
                {
                    onCompleted(this);
                    backgroundWorking = null;
                }
            }
        }

        public Coroutine backgroundWorking;
        public AsyncProcessing(Coroutine backgroundWorking)
        {
            this.backgroundWorking = backgroundWorking;
            _isDone = false;
        }
    }

}