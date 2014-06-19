using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class ColorPicker : Uzu.BaseBehaviour
	{
		public Color32 ActiveColor {
			get { return _activeColor; }
			set {
				_activeColor = value;
				_activeColorTile.color = _activeColor;
			}
		}

		#region Implementation.
		private static readonly Uzu.VectorI2 COLOR_COUNT = new Uzu.VectorI2 (6, 7);
		private const float COLOR_PADDING = 2.0f;

		[SerializeField]
		private GameObject _colorTilePrefab;
		[SerializeField]
		private UISprite _activeColorTile;
		private Uzu.FixedList <GameObject> _colorTiles;

		[SerializeField]
		private Color32 _activeColor;

		protected override void Awake ()
		{
			base.Awake ();

			int totalColorCount = Uzu.VectorI2.ElementProduct (COLOR_COUNT);

			// Initialize color grid.
			{
				_colorTiles = new Uzu.FixedList<GameObject> (totalColorCount);

				Vector3 tileDimension;
				{
					UISprite sprite = _colorTilePrefab.GetComponent <UISprite> ();
					tileDimension = new Vector2 (sprite.width, sprite.height);
				}

				for (int y = 0; y < COLOR_COUNT.y; y++) {
					for (int x = 0; x < COLOR_COUNT.x; x++) {
						GameObject colorTile = (GameObject)GameObject.Instantiate (_colorTilePrefab);
						Transform xform = colorTile.transform;
						xform.parent = CachedXform;

						Vector3 padding = new Vector3 (x *  COLOR_PADDING, -y * COLOR_PADDING, 0.0f);
						xform.localPosition = new Vector3 (x * tileDimension.x, -y * tileDimension.y, 0.0f) + padding;
						xform.localScale = Vector3.one;
						_colorTiles.Add (colorTile);

						// Set up callback.
						UIEventListener.Get (colorTile).onClick += OnColorClicked;
					}
				}
			}

			// Fill in colors.
			{
				// Color reference:
				// - http://www.oracle.com/webfolder/ux/middleware/richclient/index.html?/webfolder/ux/middleware/richclient/guidelines5/inputColor.html
				Color32[] colors = {
					new Color32(255, 182, 193, 255),
					new Color32(219, 112, 147, 255),
					new Color32(255, 0, 255, 255),
					new Color32(255, 20, 147, 255),
					new Color32(208, 32, 144, 255),
					new Color32(139, 0, 139, 255),

					new Color32(245, 222, 179, 255),
					new Color32(244, 164, 96, 255),
					new Color32(255, 99, 71, 255),
					new Color32(255, 0, 0, 255),
					new Color32(210, 105, 30, 255),
					new Color32(128, 0, 0, 255),

					new Color32(225, 228, 181, 255),
					new Color32(255, 160, 122, 255),
					new Color32(250, 128, 114, 255),
					new Color32(205, 92, 92, 255),
					new Color32(165, 42, 42, 255),
					new Color32(178, 34, 34, 255),

					new Color32(250, 250, 210, 255),
					new Color32(238, 232, 170, 255),
					new Color32(255, 255, 0, 255),
					new Color32(255, 165, 0, 255),
					new Color32(255, 140, 0, 255),
					new Color32(184, 134, 11, 255),

					new Color32(0, 255, 127, 255),
					new Color32(60, 179, 113, 255),
					new Color32(46, 139, 87, 255),
					new Color32(0, 128, 0, 255),
					new Color32(107, 142, 35, 255),
					new Color32(0, 100, 0, 255),

					new Color32(173, 216, 230, 255),
					new Color32(135, 206, 235, 255),
					new Color32(0, 191, 255, 255),
					new Color32(65, 105, 225, 255),
					new Color32(0, 0, 255, 255),
					new Color32(25, 25, 112, 255),

					new Color32(255, 255, 255, 255),
					new Color32(248, 248, 255, 255),
					new Color32(220, 220, 220, 255),
					new Color32(167, 167, 167, 255),
					new Color32(100, 100, 100, 255),
					new Color32(0, 0, 0, 255),
				};

#if UNITY_EDITOR
				if (colors.Length != totalColorCount) {
					Debug.LogError ("Color count does not match.");
				}
#endif

				for (int i = 0; i < _colorTiles.Count; i++) {
					_colorTiles [i].GetComponent <UISprite> ().color = colors [i];
				}
			}

			// Default.
			ActiveColor = Color.white;
		}

		private void OnColorClicked (GameObject go)
		{
			ActiveColor = GetColor (go);
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
