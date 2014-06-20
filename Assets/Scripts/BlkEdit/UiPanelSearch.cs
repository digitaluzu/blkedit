using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelSearch : UiPanelBaseView
	{
		private const string BUTTON_ID_SEARCH_BY_NAME = "Button-Search-ByName";
		private const string BUTTON_ID_SEARCH_MOST_DOWNLOADED = "Button-SearchMostDownloaded";
		private const string BUTTON_ID_SEARCH_MOST_LIKED = "Button-SearchMostLiked";
		private const string BUTTON_ID_SEARCH_MOST_RECENT = "Button-SearchMostRecent";

		protected override string TransitionPanelIdOnClose {
			get { return PanelIds.PANEL_OPTIONS; }
		}

		public override void OnActivate ()
		{

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

			default:
				base.OnClick (widget);
				break;
			}
		}

		protected override void Awake ()
		{
			base.Awake ();

			Main.HttpRequestHandler.OnGetMostRecentEntries = OnGetMostRecentEntries;
//			Main.HttpRequestHandler.OnGetImage = OnGetImage;
		}

		private void DoSearchMostRecent ()
		{
			Main.ScrollViewController.AttachToPanel (this);
			Main.ScrollViewController.WindowTitleText = "Most Recent";
			Main.ScrollViewController.NoDataText = "No Models";
//			Main.ScrollViewController.DisabledEntryId = Main.WorkspaceController.ActiveBlockInfoId;
			Main.ScrollViewController.EntryButtonText = "Get";
//			Main.ScrollViewController.OnTableEntryButtonClicked += OnTableEntryButtonClicked;

			Main.HttpRequestHandler.GetMostRecentEntries();
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