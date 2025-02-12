﻿using UnityEngine;

namespace NeoFPS
{
	public interface IAdditiveTransformHandler : IMonoBehaviour
	{
		float springPositionMultiplier
		{
			get;
			set;
		}

		float springRotationMultiplier
		{
			get;
			set;
		}

		void ApplyAdditiveEffect (IAdditiveTransform add);
		void RemoveAdditiveEffect (IAdditiveTransform add);

		T GetAdditiveTransform<T> () where T: class, IAdditiveTransform;
		T[] GetAdditiveTransforms<T> () where T: class, IAdditiveTransform;
	}
}