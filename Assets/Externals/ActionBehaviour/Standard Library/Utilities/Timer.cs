using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utilities {


	public class Timer : MonoSingleton<Timer> {

		/// <summary>
		/// Timer 
		/// </summary>
		void decreaseTimeRemaining() {
			foreach (var t in lstIimer)
				t.Elapsed(100);
			lstIimer.RemoveAll(item => item.IsTimeOver);
		}

		protected override void Awake() {
			InvokeRepeating("decreaseTimeRemaining", 1, 0.1f);
		}

		class TIMER {
			public int id;
			public Action<int> onListener;
			public int remainTime;

			public bool IsTimeOver { get { return (remainTime <= 0); } }
			public bool Elapsed(int time) {
				remainTime -= time;
				//Debug.Log(string.Format("time {0} {1}",id,remainTime));
				if (IsTimeOver) {
					onListener(id);
					onListener = null;
					return true;
				}
				return false;
			}
		}
		List<TIMER> lstIimer = new List<TIMER>();

		static int time_uid = 0;
		public static int SetTimer(int msElapse, Action<int> listener) {

			instance.lstIimer.Add(new TIMER() {
				id = ++time_uid,
				remainTime = msElapse,
				onListener = listener
			});

			return time_uid;
		}

		// cancel timer
		public static void CancelTimer(int id) {
			int idx = instance.lstIimer.FindIndex(va => va.id == id);
			if(idx > -1)
				instance.lstIimer.RemoveAt(idx);
		}

		
	}
}