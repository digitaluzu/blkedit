using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelSearch : Uzu.UiPanel
	{
		private const string BUTTON_ID_CLOSE = "Button-Close";
		private const string BUTTON_ID_SEARCH_BY_NAME = "Button-Search-ByName";
		private const string BUTTON_ID_SEARCH_MOST_DOWNLOADED = "Button-SearchMostDownloaded";
		private const string BUTTON_ID_SEARCH_MOST_LIKED = "Button-SearchMostLiked";
		private const string BUTTON_ID_SEARCH_MOST_RECENT = "Button-SearchMostRecent";

		private bool _isScrollViewOpen;

		public override void OnActivate ()
		{
			_isScrollViewOpen = false;
		}

		public override void OnDeactivate ()
		{
			// Stop all requests.
			Main.HttpRequestHandler.StopAllCoroutines ();
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

		protected override void Awake ()
		{
			base.Awake ();

			Main.HttpRequestHandler.OnGetMostRecentEntries = OnGetMostRecentEntries;
//			Main.HttpRequestHandler.OnGetImage = OnGetImage;
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
//			Main.ScrollViewController.DisabledEntryId = Main.WorkspaceController.ActiveBlockInfoId;
			Main.ScrollViewController.EntryButtonText = "Get";
//			Main.ScrollViewController.OnTableEntryButtonClicked += OnTableEntryButtonClicked;

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

		private void OnGetMostRecentEntries (HttpRequestHandler.DataInfo data)
		{
			Debug.Log (data.id);

			BlkEdit.BlockInfo info = new BlkEdit.BlockInfo ();
			info.Id = data.id;
			info.Name = data.name;

			Main.ScrollViewController.AppendEntry (info);
			
			//	_tableController.AddEntry (data.id);
			
			//	Main.HttpRequestHandler.GetImage (data.id, data.imageURL);
		}
	}
}