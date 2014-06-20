using UnityEngine;
using System.Collections;

namespace Blk
{
	public class TableEntry : Uzu.PooledBehaviour
	{
		[SerializeField]
		private UILabel _label;
		[SerializeField]
		private UITexture _texture;
		[SerializeField]
		private UIButton _button;
		[SerializeField]
		private UILabel _buttonLabel;

		public string Id {
			get; set;
		}

		public UIButton Button {
			get { return _button; }
		}

		public string ButtonText {
			get { return _buttonLabel.text; }
			set { _buttonLabel.text = value; }
		}

		public string Text {
			set { _label.text = value; }
		}

		public Texture2D Texture {
			set { _texture.mainTexture = value; }
		}
	}
}
