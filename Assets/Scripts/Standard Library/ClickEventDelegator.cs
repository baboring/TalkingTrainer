/********************************************************************
	created:	2014/09/17 
	filename:	ButtonEventDelegator.cs
	author:		ehalshbest
	purpose:	[]
*********************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TTrainer
{
	//[RequireComponent(typeof(BoxCollider2D))]
	public class ClickEventDelegator : MonoBehaviour
	{
		public event Action<ClickEventDelegator> ClickedHandler = null;

		void Awake()
		{
		}

		void OnClick()
		{
			if( null != ClickedHandler ) {
				ClickedHandler(this);
			};
		}
	}	// __end ButtonEventDelegator 

}