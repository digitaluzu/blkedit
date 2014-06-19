using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class UiPanelOptions : Uzu.UiPanel
	{
		private const string BUTTON_ID_SAVE = "Button-Save";
		private const string BUTTON_ID_LOAD = "Button-Load";
		private const string BUTTON_ID_CLOSE = "Button-Close";
		private const string BUTTON_ID_SEARCH = "Button-Search";

		private const string BUTTON_ID_MODE_ADD = "Button-ModeAdd";
		private const string BUTTON_ID_MODE_ERASE = "Button-ModeErase";
		private const string BUTTON_ID_MODE_EYEDROP = "Button-ModeEyedrop";

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case BUTTON_ID_SAVE:
				DoSave ();
				break;

			case BUTTON_ID_LOAD:
				DoLoad ();
				break;

			case BUTTON_ID_CLOSE:
				DoClose ();
				break;

			case BUTTON_ID_SEARCH:
				DoSearchOnline ();
				break;

			case BUTTON_ID_MODE_ADD:
				DoSelectTool (GridController.Mode.AddBlocks);
				break;

			case BUTTON_ID_MODE_ERASE:
				DoSelectTool (GridController.Mode.EraseBlocks);
				break;

			case BUTTON_ID_MODE_EYEDROP:
				DoSelectTool (GridController.Mode.EyeDropper);
				break;
			}
		}

		private void DoSave ()
		{
			Main.WorkspaceController.Save ();
		}

		private void DoSelectTool (GridController.Mode mode)
		{
			Main.GridController.CurrentMode = mode;
			DoClose ();
		}

		private void DoClose ()
		{
			// TODO: hmmm...
			if (_searchOnlineObject.activeSelf) {
				_searchOnlineObject.SetActive (false);
				return;
			}

			Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
		}

		[SerializeField]
		private GameObject _searchOnlineObject;
		[SerializeField]
		private TableController _tableController;

		private List <BlkEdit.BlockInfo> _infos;

		private void DoLoad ()
		{
			_searchOnlineObject.SetActive (true);
			_tableController.ClearEntries ();

			_tableController.OnButtonClicked = OnButtonClicked;

			_infos = WorkspaceController.GetLocalBlockInfos ();
			for (int i = 0; i < _infos.Count; i++) {
				_tableController.AddEntry (_infos [i].Id);
			}
		}

		private void OnButtonClicked (string id)
		{
			Debug.Log ("BUTTON CLICKED: " + id);

			for (int i = 0; i < _infos.Count; i++) {
				if (_infos [i].Id == id) {
					Main.WorkspaceController.Load (_infos [i]);

					DoClose ();
					break;
				}
			}
		}

		private void DoSearchOnline ()
		{
			_searchOnlineObject.SetActive (true);

			_tableController.ClearEntries ();

			// TODO: move to awake or setup function..
			Main.HttpRequestHandler.OnGetMostRecentEntries = OnGetMostRecentEntries;
			Main.HttpRequestHandler.OnGetImage = OnGetImage;

			Main.HttpRequestHandler.GetMostRecentEntries();
		}

		private void OnGetMostRecentEntries (HttpRequestHandler.DataInfo data)
		{
			Debug.Log ("OnGetMostRecentEntries");

			_tableController.AddEntry (data.id);

			Main.HttpRequestHandler.GetImage (data.id, data.imageURL);
		}

		private void OnGetImage (string id, Texture2D texture)
		{
			Debug.Log ("OnGetImage");

			_tableController.UpdateEntry (id, texture);
		}
	}
}