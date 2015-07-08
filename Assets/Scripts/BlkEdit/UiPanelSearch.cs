using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	// TODO: cache results for certain time period w/o doing repeated HTTP requests
	//	- for example, 1-2 minutes for 'recent'
	//  - 5 - 10 min for 'most downloaded'.. etc.
	public class UiPanelSearch : Uzu.UiPanel
	{
		private const string BUTTON_ID_CLOSE = "Button-Close";
		private const string BUTTON_ID_SEARCH_BY_NAME = "Button-Search-ByName";
		private const string BUTTON_ID_SEARCH_MOST_DOWNLOADED = "Button-SearchMostDownloaded";
		private const string BUTTON_ID_SEARCH_MOST_LIKED = "Button-SearchMostLiked";
		private const string BUTTON_ID_SEARCH_MOST_RECENT = "Button-SearchMostRecent";

		private bool _isScrollViewOpen;

		private List <BlkEdit.BlockInfo> _infos;

		public override void OnActivate ()
		{
			_isScrollViewOpen = false;

			// Setup callbacks.
			{
				Main.ScrollViewController.OnTableEntryButtonClicked += OnTableEntryButtonClicked;

				Main.HttpRequestHandler.OnError += OnHttpError;
				Main.HttpRequestHandler.OnGetMostRecentEntries += OnGetMostRecentEntries;
	//			Main.HttpRequestHandler.OnGetImage += OnGetImage;
				Main.HttpRequestHandler.OnGetBlockData += OnGetBlockData;
			}

			_infos = WorkspaceController.GetSavedBlockInfos ();
		}

		public override void OnDeactivate ()
		{
			// Remove callbacks.
			{
				Main.ScrollViewController.OnTableEntryButtonClicked -= OnTableEntryButtonClicked;

				Main.HttpRequestHandler.OnError -= OnHttpError;
				Main.HttpRequestHandler.OnGetMostRecentEntries -= OnGetMostRecentEntries;
			}

			Main.HttpRequestHandler.StopAllRequests ();
		}

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case BUTTON_ID_SEARCH_BY_NAME:
				break;

			case BUTTON_ID_SEARCH_MOST_DOWNLOADED:
				break;

			case BUTTON_ID_SEARCH_MOST_LIKED:
				break;

			case BUTTON_ID_SEARCH_MOST_RECENT:
				DoSearchMostRecent ();
				break;

			case BUTTON_ID_CLOSE:
				DoClose ();
				break;
			}
		}

		private void DoSearchByName ()
		{
			_isScrollViewOpen = true;
		}

		private void DoSearchMostDownloaded ()
		{
			_isScrollViewOpen = true;
		}

		private void DoSearchMostLiked ()
		{
			_isScrollViewOpen = true;
		}

		private void DoSearchMostRecent ()
		{
			_isScrollViewOpen = true;

			Main.ScrollViewController.AttachToPanelAndShow (this);
			Main.ScrollViewController.WindowTitleText = "Most Recent";
			Main.ScrollViewController.NoDataText = "No Models";
			Main.ScrollViewController.DisabledEntryId = Main.WorkspaceController.ActiveBlockInfoId;
			Main.ScrollViewController.EntryButtonDefaultText = "Get";

			Main.HttpRequestHandler.GetMostRecentEntries();
		}

		private void DoClose ()
		{
			if (_isScrollViewOpen) {
				Main.ScrollViewController.Hide ();
				_isScrollViewOpen = false;
			}
			else {
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_OPTIONS);
			}
		}

		private void OnTableEntryButtonClicked (string id, string buttonText)
		{
			if (buttonText == "Get") {
				Main.ScrollViewController.SetEntryButtonText (id, "...");
				Main.HttpRequestHandler.GetBlockData (id);
			}
			else if (buttonText == "View") {
				BlkEdit.BlockInfo info;
				GetInfo (id, out info);
				Main.WorkspaceController.LoadForViewing (info);
			}
		}

		private void OnHttpError (int errorCode, string errorText)
		{
			Debug.LogError ("HTTP Error: " + errorCode + " - " + errorText);
		}

		private void OnGetMostRecentEntries (BlkEdit.BlockInfo info)
		{
			Main.ScrollViewController.AppendEntry (info);

			// TODO: updating if version is not the same?
			if (HasSavedInfo (info.Id)) {
				Main.ScrollViewController.SetEntryButtonText (info.Id, "View");
			}
			else {
				Main.ScrollViewController.SetEntryButtonText (info.Id, "Get");
			}

			_infos.Add (info);

			//	Main.HttpRequestHandler.GetImage (data.id, data.imageURL);
		}

		private void OnGetBlockData (string id, byte[] data)
		{
			Debug.Log ("Get block data: " + id);
			Debug.Log ("Length: " + data.Length);

			// TODO: verify data integrity in HTTP request? (magic #) before returning
			Uzu.BlockFormat.Data blockData = Uzu.BlockReader.Read (data);

			// get path... write to path... store path in blki... write blki
			string path = System.IO.Path.Combine (FileUtil.SavedModelPath, id);
			string blockDataPath = path + "." + Uzu.BlockFormat.Extension;
			string blockInfoPath = path + "." + BlkEdit.BlockInfo.Extension;
			Uzu.BlockIO.WriteFile (blockDataPath, data);

			BlkEdit.BlockInfo info;
			if (!GetInfo (id, out info)) {
				Debug.LogError ("omg");
				return;
			}

			// TODO: BlockiNfo is a struct, so this won't change the actual instance...
			info.BlockDataPath = blockDataPath;

			BlkEdit.BlockInfo.Save (blockInfoPath, info);

			Main.ScrollViewController.SetEntryButtonText (id, "View");
		}

		private bool GetInfo (string id, out BlkEdit.BlockInfo info)
		{
			for (int i = 0; i < _infos.Count; i++) {
				info = _infos [i];
				if (info.Id == id) {
					return true;
				}
			}

			info = new BlkEdit.BlockInfo ();
			return false;
		}

		private bool HasSavedInfo (string id)
		{
			for (int i = 0; i < _infos.Count; i++) {
				BlkEdit.BlockInfo info = _infos [i];
				if (info.Id == id) {
					return true;
				}
			}

			return false;
		}
	}
}