using UnityEngine;
using System.Collections;

namespace Blk
{
	public class TableEntry : Uzu.PooledBehaviour
	{
		public string Text {
			set { _label.text = value; }
		}

		public Texture2D Texture {
			set { _texture.mainTexture = value; }
		}

		[SerializeField]
		private UILabel _label;
		[SerializeField]
		private UITexture _texture;
	}
}
