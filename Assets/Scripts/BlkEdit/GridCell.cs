using UnityEngine;
using System.Collections;

namespace Blk
{
	public class GridCell : Uzu.BaseBehaviour
	{
		public UISprite Sprite {
			get { return _sprite; }
		}

		public void SetColor (Color32 color)
		{
			_sprite.color = color;
		}

		private UISprite _sprite;

		protected override void Awake ()
		{
			base.Awake ();

			_sprite = GetComponent <UISprite> ();
		}
	}
}