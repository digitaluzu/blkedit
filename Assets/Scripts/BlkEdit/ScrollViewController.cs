using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class ScrollViewController : Uzu.BaseBehaviour
	{
		public delegate void OnTableEntryButtonClickedDelegate (string id, string buttonText);
		public event OnTableEntryButtonClickedDelegate OnTableEntryButtonClicked;

		public string WindowTitleText {
			get { return _windowTitleLabel.text; }
			set { _windowTitleLabel.text = value; }
		}

		public string NoDataText {
			get { return _noDataLabel.text; }
			set { _noDataLabel.text = value; }
		}

		public string DisabledEntryId {
			get; set;
		}

		public string EntryButtonText {
			get; set;
		}

		public void AttachToPanel (Uzu.UiPanel panel)
		{
			this.gameObject.SetActive (true);
			
			CachedXform.parent = panel.CachedXform;
			
			// Clear when attaching to a new panel.
			ClearEntries ();

			RequestRefresh ();
		}
		
		public void AppendEntries (List <BlkEdit.BlockInfo> infos)
		{
			for (int i = 0; i < infos.Count; i++) {
				AppendEntry (infos [i]);
			}

			RequestRefresh ();
		}

		public void AppendEntry (BlkEdit.BlockInfo info)
		{
			if (_entries.ContainsKey (info.Id)) {
				Debug.LogWarning ("Duplicate id detected: " + info.Id);
				return;
			}
			
			// Spawn an entry.
			GameObject go = _tableEntryPool.Spawn (Vector3.zero);
			TableEntry entry = go.GetComponent <TableEntry> ();
			entry.CachedXform.parent = _tableXform;
			entry.CachedXform.localScale = Vector3.one;
			
			// Set it up.
			{
				entry.Text = info.Name;
				entry.Id = info.Id;
				entry.ButtonText = EntryButtonText;
				entry.Button.isEnabled = true;
				UIEventListener.Get(entry.Button.gameObject).onClick += OnTableEntryButtonClickedImpl;
			}
			
			_entries.Add (info.Id, entry);

			RequestRefresh ();
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

			RequestRefresh ();
		}
		
		public void ClearEntries ()
		{
			_tableEntryPool.UnspawnAll ();
			_entries.Clear ();

			RequestRefresh ();
		}

		#region Implementation.
		[SerializeField]
		private UITable _table;
		[SerializeField]
		private Uzu.GameObjectPool _tableEntryPool;
		[SerializeField]
		private TableEntry _entryPrefab;

		[SerializeField]
		private UILabel _windowTitleLabel;
		[SerializeField]
		private UILabel _noDataLabel;

		private Transform _tableXform;
		private Dictionary<string, TableEntry> _entries = new Dictionary<string, TableEntry>();
		private bool _needsRefresh;

		private void RequestRefresh ()
		{
			_needsRefresh = true;
		}

		private void Update ()
		{
			if (!_needsRefresh) {
				return;
			}

			_needsRefresh = false;

			{
				if (_entries.Count == 0) {
					_noDataLabel.gameObject.SetActive (true);
				}
				else {
					_noDataLabel.gameObject.SetActive (false);
				}
			}

			{
				TableEntry entry;
				if (_entries.TryGetValue (DisabledEntryId, out entry)) {
					entry.Button.isEnabled = false;
				}
			}

			_table.Reposition ();
		}

		private void OnTableEntryButtonClickedImpl (GameObject go)
		{
			TableEntry entry = go.GetComponentInParent <TableEntry> ();

			if (OnTableEntryButtonClicked != null) {
				OnTableEntryButtonClicked (entry.Id, entry.ButtonText);
			}
			else {
				Debug.LogWarning ("No callback specified.");
			}
		}

		protected override void Awake ()
		{
			base.Awake ();

			_tableXform = _table.transform;
		}
		#endregion
	}
}
