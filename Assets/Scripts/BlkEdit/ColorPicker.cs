using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	// TODO: should be managed by panel??
	public class ColorPicker : Uzu.BaseBehaviour
	{
		public Color32 ActiveColor {
			get { return _activeColor; }
		}

		[SerializeField]
		private GameObject[] _colors = null;
		private Color32 _activeColor;

		protected override void Awake ()
		{
			base.Awake ();

			for (int i = 0; i < _colors.Length; i++) {
				UIEventListener.Get (_colors[i]).onClick += OnColorClicked;
			}

			// Default.
			if (_colors.Length != 0) {
				_activeColor = GetColor (_colors [0]);
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
	}
}