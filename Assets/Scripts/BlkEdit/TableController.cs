using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	[RequireComponent (typeof(UITable))]
	public class TableController : Uzu.BaseBehaviour
	{
		public delegate void OnButtonClickedDelegate (string id);
		public OnButtonClickedDelegate OnButtonClicked;

		[SerializeField]
		private Uzu.GameObjectPool _tableEntryPool;
		[SerializeField]
		private TableEntry _entryPrefab;

		private UITable _table;

		private Dictionary<string, TableEntry> _entries = new Dictionary<string, TableEntry>();

		public void AddEntry (string id)
		{
			// TODO: guard against duplicate id.... delete existing id entry if duplicate detected

			GameObject go = _tableEntryPool.Spawn (Vector3.zero);
			Transform xform = go.transform;

			xform.parent = CachedXform;
			xform.localScale = Vector3.one;

			// TODO:
			{
				TableEntry entry = go.GetComponent <TableEntry> ();
				entry.Text = id;
				entry.Id = id;
				entry.TEMP_TC = this;

				_entries.Add(id, entry);
			}

			_table.Reposition ();
//			_table.repositionNow = true;
		}

		// TODO:
		public void UpdateEntry (string id, Texture2D texture)
		{
			TableEntry entry;
			if (!_entries.TryGetValue (id, out entry)) {
				Debug.LogWarning ("Id not found: " + id);
				return;
			}

			Debug.Log ("Setting for id: " + id);

			// TODO: size (dimensions) of texture?
			{
				entry.Texture = texture;
			}
			
			_table.Reposition ();
			//			_table.repositionNow = true;
		}

		public void ClearEntries ()
		{
			_tableEntryPool.UnspawnAll ();
			_table.repositionNow = true;
//			_table.Reposition ();

			_entries.Clear ();
		}

		protected override void Awake ()
		{
			base.Awake ();

			_table = GetComponent <UITable> ();
		}
	}
}
