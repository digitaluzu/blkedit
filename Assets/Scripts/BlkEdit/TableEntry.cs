using UnityEngine;
using System.Collections;

namespace Blk
{
	public class TableEntry : Uzu.PooledBehaviour
	{
		[SerializeField]
		private UILabel _nameLabel;
		[SerializeField]
		private UILabel _idLabel;
		[SerializeField]
		private UITexture _texture;
		[SerializeField]
		private UIButton _button;
		[SerializeField]
		private UILabel _buttonLabel;

		public string Id {
			get { return _idLabel.text; }
			set { _idLabel.text = value; }
		}

		public UIButton Button {
			get { return _button; }
		}

		public string ButtonText {
			get { return _buttonLabel.text; }
			set { _buttonLabel.text = value; }
		}

		public string NameText {
			set { _nameLabel.text = value; }
		}

		public Texture2D Texture {
			set { _texture.mainTexture = value; }
		}
	}
}
