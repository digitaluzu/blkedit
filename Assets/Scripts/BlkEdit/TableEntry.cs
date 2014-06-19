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

		public string Id {
			get; set;
		}

		public TableController TEMP_TC {
			get; set;
		}

		[SerializeField]
		private UILabel _label;
		[SerializeField]
		private UITexture _texture;

		[SerializeField]
		private GameObject _buttonObject;

		protected override void Awake ()
		{
			base.Awake ();

			UIEventListener.Get(_buttonObject).onClick += OnButtonClicked;
		}

		private void OnButtonClicked (GameObject go)
		{
			TEMP_TC.OnButtonClicked (Id);
		}
	}
}
