using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class ColorPicker : Uzu.BaseBehaviour
	{
		public Color32 ActiveColor {
			get { return _activeColor; }
		}

		#region Implementation.
		private GameObject[] _colorObjects;
		private Color32 _activeColor;

		protected override void Awake ()
		{
			base.Awake ();

			// Initialize the objects.
			{
				_colorObjects = new GameObject[CachedXform.childCount];
				for (int i = 0; i < _colorObjects.Length; i++) {
					GameObject go = CachedXform.GetChild (i).gameObject;
					_colorObjects [i] = go;
					UIEventListener.Get (go).onClick += OnColorClicked;
				}
			}

			// Default.
			if (_colorObjects.Length != 0) {
				_activeColor = GetColor (_colorObjects [0]);
			}
		}

		private void OnColorClicked (GameObject go)
		{
			_activeColor = GetColor (go);
		}

		private Color32 GetColor (GameObject go)
		{
			UISprite sprite = go.GetComponent <UISprite> ();
			if (sprite != null) {
				return sprite.color;
			}

			return new Color32 (255, 255, 255, 255);
		}
		#endregion
	}
}
