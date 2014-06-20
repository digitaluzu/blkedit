using UnityEngine;
using System.Collections;

namespace Blk
{
	public class TableEntry : Uzu.PooledBehaviour
	{
		public string Id {
			get; set;
		}

		public string Text {
			set { _label.text = value; }
		}

		public Texture2D Texture {
			set { _texture.mainTexture = value; }
		}

		public TableController OwnerController {
			get; set;
		}

		public void OnAddedToTable ()
		{
			_button.isEnabled = true;
		}

		public void Disable ()
		{
			_button.isEnabled = false;
		}

		[SerializeField]
		private UILabel _label;
		[SerializeField]
		private UITexture _texture;
		[SerializeField]
		private UIButton _button;

		[SerializeField]
		private GameObject _buttonObject;

		protected override void Awake ()
		{
			base.Awake ();

			UIEventListener.Get(_buttonObject).onClick += OnButtonClicked;
		}

		private void OnButtonClicked (GameObject go)
		{
			OwnerController.OnButtonClicked (Id);
		}
	}
}
