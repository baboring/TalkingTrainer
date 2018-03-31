using UnityEngine;
using System.Collections;

// 재활용을 하기위한 Initalize를 호출해줘야한다.

namespace Common.Utilities {
		
	public class SelfDestoryTimer : MonoBehaviour
	{

		public bool useLiveTime = true;
		public float liveTime = 10.0f;

		protected float orgLiveTime;

		//public 
		public virtual void Awake()
		{
			orgLiveTime = liveTime;
		}


		public virtual void Initialize()
		{
			liveTime = orgLiveTime;
			useLiveTime = true;

		}

		// Use this for initialization
		public virtual void Start()
		{
			
		}

		// Update is called once per frame
		public virtual void Update()
		{
			if (useLiveTime == true)
			{
				liveTime -= Time.deltaTime;
			}

			// 임펙트 삭제 처리
			if (liveTime <= 0.0f)
			{
				DestroyOjbect();
			}
		}

		public virtual void DestroyOjbect()
		{
			// @cloud
			// ObjectPool에 다시 복구하지 않는경우(메모리해제)
			Destroy(gameObject);

			// ObjectPool에 다시 돌려서 재사용하는경우
			//ObjectPool.Instance.PoolObject(gameObject);
		}

	}

}