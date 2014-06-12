using UnityEngine;
using System.Collections;

namespace Blk
{
	[RequireComponent (typeof(UITable))]
	public class TableController : Uzu.BaseBehaviour
	{
		[SerializeField]
		private Uzu.GameObjectPool _tableEntryPool;
		[SerializeField]
		private TableEntry _entryPrefab;

		private UITable _table;

		public void AddEntry (string text)
		{
			GameObject go = _tableEntryPool.Spawn (Vector3.zero);
			Transform xform = go.transform;

			xform.parent = CachedXform;
			xform.localScale = Vector3.one;

			{
				TableEntry entry = go.GetComponent <TableEntry> ();
				entry.Text = text;
			}

			_table.Reposition ();
//			_table.repositionNow = true;
		}

		protected override void Awake ()
		{
			base.Awake ();

			_table = GetComponent <UITable> ();
		}
	}
}
