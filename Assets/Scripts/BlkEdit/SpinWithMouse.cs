using UnityEngine;

namespace Blk
{
	/// <summary>
	/// TODO: allow block world to spin with mouse.
	/// 	- needs to handle touch + momentum + ...
	/// </summary>
	public class SpinWithMouse : Uzu.BaseBehaviour
	{
		[SerializeField]
		private Transform _target;
		[SerializeField]
		private float _speed = 1.0f;
		private Vector3 _rotationPoint;

		public Transform Target {
			get { return _target; }
			set { _target = value; }
		}

		public float Speed {
			get { return _speed; }
			set { _speed = value; }
		}

		public Vector3 RotationPoint {
			get { return _rotationPoint; }
			set { _rotationPoint = value; }
		}
		
		private void OnDrag (Vector2 delta)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

///			blockWorld.CachedXform.RotateAround(centerPos, Vector3.up, spinSpeed);
			_target.RotateAround (_rotationPoint, Vector3.up, -0.5f * delta.x * _speed);

//			_target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * _speed, 0f) * _target.localRotation;
		}
	}
}