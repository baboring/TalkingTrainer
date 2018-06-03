/********************************************************************
 created:	2014/03/14
 filename:	Pool.cs
 author:		Benjamin
 purpose:	[]
*********************************************************************/

using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Utilities
{
	public class Pool<T>
	{
		private readonly List<T> items = new List<T>();
		private readonly Queue<T> freeItems = new Queue<T>();

		private readonly Func<T> createItemAction;

		public Pool(Func<T> createItemAction)
		{
			Assert.IsNotNull(createItemAction, "pool object is null");
			this.createItemAction = createItemAction;
		}

		public Pool(Func<T> createItemAction, int initSize)
		{
			Assert.IsNotNull(createItemAction, "pool object is null");
			this.createItemAction = createItemAction;

			freeItems = new Queue<T>(initSize);
		}

		public void FlagFreeItem(T item)
		{
			freeItems.Enqueue(item);
		}

		public T GetFreeItem()
		{
			if (freeItems.Count == 0) {
				T item = createItemAction();
				items.Add(item);

				return item;
			}

			return freeItems.Dequeue();
		}

		public List<T> Items
		{
			get { return items; }
		}

		public void Clear() {
			items.Clear();
			freeItems.Clear();
		}
	}
}