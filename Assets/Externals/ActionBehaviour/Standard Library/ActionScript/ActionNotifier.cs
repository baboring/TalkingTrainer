using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Common.ActionScript {

	using Common.Utilities;

	public class ActionNotifier : MonoSingleton<ActionNotifier> {

		protected Dictionary<string, Operation> events = new Dictionary<string, Operation>();

		// register
		static public bool Register(string Name, ActionHandler handler) {
			Operation op;
			if(!instance.events.TryGetValue(Name, out op)) {
				op = new Operation();
				instance.events.Add(Name,op);
			}
			op.handler += handler;
			return true;
		}

		// Unregister
		static public bool Unregister(string Name, ActionHandler handler) {
			Operation op;
			if(!instance.events.TryGetValue(Name, out op))
				return false;
			op.handler -= handler;
			return true;
		}


		static public bool Invoke(string Name, object arg) {
			Operation op;
			if(!instance.events.TryGetValue(Name, out op)) 
				return false;
			op.Action(arg);
			return true;			
		}

	}

	public delegate void ActionHandler(object arg);  
  
    public class Operation  
    {  
        public event ActionHandler handler;  
  
        public void Action(object arg)  
        {  
            if (handler != null)  
            {  
                handler(arg);  
                Debug.Log(arg);   
            }  
            else  
            {  
                Debug.Log("Not Registered");   
            }  
        }  
    }  
		
}