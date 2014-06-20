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

		public void AddEntry (BlkEdit.BlockInfo info)
		{
			if (_entries.ContainsKey (info.Id)) {
				Debug.LogWarning ("Duplicate id detected: " + info.Id);
				return;
			}

			// Spawn an entry.
			GameObject go = _tableEntryPool.Spawn (Vector3.zero);
			TableEntry entry = go.GetComponent <TableEntry> ();
			entry.CachedXform.parent = CachedXform;
			entry.CachedXform.localScale = Vector3.one;

			// Set it up.
			{
				entry.Text = info.Name;
				entry.Id = info.Id;
				entry.OwnerController = this;
			}

			_entries.Add (info.Id, entry);
			entry.OnAddedToTable ();

			// Refresh table immediately to prevent flicker.
			_table.Reposition ();
		}

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

			_table.repositionNow = true;
		}

		public void DisableEntry (string id)
		{
			TableEntry entry;
			if (!_entries.TryGetValue (id, out entry)) {
				Debug.LogWarning ("Id not found: " + id);
				return;
			}

			entry.Disable ();
		}

		public void ClearEntries ()
		{
			_tableEntryPool.UnspawnAll ();
			_entries.Clear ();
			_table.repositionNow = true;
		}

		protected override void Awake ()
		{
			base.Awake ();

			_table = GetComponent <UITable> ();
		}
	}
}
