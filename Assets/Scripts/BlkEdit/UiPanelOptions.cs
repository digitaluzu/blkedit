using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class UiPanelOptions : UiPanelBaseView
	{
		private const string BUTTON_ID_SAVE = "Button-Save";
		private const string BUTTON_ID_MY_DATA = "Button-Load";
		private const string BUTTON_ID_CLOSE = "Button-Close";
		private const string BUTTON_ID_SEARCH = "Button-Search";
		private const string BUTTON_ID_SAVED = "Button-Saved";

		private const string BUTTON_ID_MODE_ADD = "Button-ModeAdd";
		private const string BUTTON_ID_MODE_ERASE = "Button-ModeErase";
		private const string BUTTON_ID_MODE_EYEDROP = "Button-ModeEyedrop";

		[SerializeField]
		private UIButton _saveButton;

		protected override string TransitionPanelIdOnClose {
			get { return PanelIds.PANEL_CANVAS; }
		}

		public override void OnActivate ()
		{
			RefreshIcons ();
		}

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case BUTTON_ID_SAVE:
				DoSave ();
				break;

			case BUTTON_ID_MY_DATA:
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_MY_DATA_VIEW);
				break;

			case BUTTON_ID_SEARCH:
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_SEARCH);
				break;

			case BUTTON_ID_SAVED:
				DoShowSavedData ();
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

			default:
				base.OnClick (widget);
				break;
			}
		}

		private void DoSelectTool (GridController.Mode mode)
		{
			Main.GridController.CurrentMode = mode;
			DoClose ();
		}

		private void RefreshIcons ()
		{

		}

		private void DoSave ()
		{
			Main.WorkspaceController.Save ();
		}

		public override void OnUpdate ()
		{
			if (Main.WorkspaceController.NeedsSave && !_saveButton.isEnabled) {
				_saveButton.isEnabled = true;
			}
			else if (!Main.WorkspaceController.NeedsSave && _saveButton.isEnabled) {
				_saveButton.isEnabled = false;
			}
		}

		private void DoShowSavedData ()
		{
			/*
			ShowScrollView (ScrollViewMode.SavedData);
			_tableController.ClearEntries ();
			
			_infos = WorkspaceController.GetSavedBlockInfos ();
			for (int i = 0; i < _infos.Count; i++) {
				_tableController.AddEntry (_infos [i]);
			}

			// TODO: if no infos, show NO DATA label
			
			// Disable the currently active entry.
			if (Main.WorkspaceController.HasActiveBlockInfo) {
				string currentId = Main.WorkspaceController.ActiveBlockInfoId;
				_tableController.DisableEntry (currentId);
			}*/
		}

		private void DoSearchOnline ()
		{
			/*
			_scrollViewObject.SetActive (true);

			_tableController.ClearEntries ();

			// TODO: move to awake or setup function..
			Main.HttpRequestHandler.OnGetMostRecentEntries = OnGetMostRecentEntries;
			Main.HttpRequestHandler.OnGetImage = OnGetImage;

			Main.HttpRequestHandler.GetMostRecentEntries();
			*/
		}

		private void OnGetMostRecentEntries (HttpRequestHandler.DataInfo data)
		{
			Debug.Log ("OnGetMostRecentEntries");

		//	_tableController.AddEntry (data.id);

		//	Main.HttpRequestHandler.GetImage (data.id, data.imageURL);
		}

		private void OnGetImage (string id, Texture2D texture)
		{
			Debug.Log ("OnGetImage");

//			_tableController.UpdateEntry (id, texture);
		}
	}
}