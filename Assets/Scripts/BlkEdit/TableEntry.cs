using UnityEngine;
using System.Collections;

namespace Blk
{
	public class TableEntry : Uzu.PooledBehaviour
	{
		public string Text {
			set { _label.text = value; }
		}

		[SerializeField]
		private UILabel _label;
	}
}
