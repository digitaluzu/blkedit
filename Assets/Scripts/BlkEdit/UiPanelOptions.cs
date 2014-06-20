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

		[SerializeField]
		private GameObject _scrollViewObject;
		[SerializeField]
		private TableController _tableController;
		[SerializeField]
		private UIButton _saveButton;
		[SerializeField]
		private DialogBox _dialogBox2;

		[SerializeField]
		private UIInput _titleNameInput;
		
		private List <BlkEdit.BlockInfo> _infos;

		protected override void Awake ()
		{
			base.Awake ();

			_tableController.OnButtonClicked = OnScrollViewButtonClicked;
			_titleNameInput.onValidate = TitleNameValidator;
		}

		public override void OnActivate ()
		{
			HideScrollView ();

			RefreshIcons ();
		}

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case BUTTON_ID_SAVE:
				DoSave ();
				break;

			case BUTTON_ID_LOAD:
				DoShowLocalData ();
				break;

			case BUTTON_ID_CLOSE:
				if (IsScrollViewVisible ()) {
					HideScrollView ();
				}
				else {
					DoClose ();
				}
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

		private void DoSelectTool (GridController.Mode mode)
		{
			Main.GridController.CurrentMode = mode;
			DoClose ();
		}

		private void DoClose ()
		{
			HideScrollView ();
			Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
		}

		private bool IsScrollViewVisible ()
		{
			return _scrollViewObject.activeSelf;
		}

		private void ShowScrollView (ScrollViewMode mode)
		{
			_currentScrollViewMode = mode;
			_scrollViewObject.SetActive (true);
		}

		private void HideScrollView ()
		{
			_scrollViewObject.SetActive (false);
		}

		private void RefreshIcons ()
		{
			_saveButton.isEnabled = Main.WorkspaceController.NeedsSave;
		}

		private void DoSave ()
		{
			Main.WorkspaceController.Save ();
		}

		private enum ScrollViewMode {
			None,
			LocalData,
		}

		private ScrollViewMode _currentScrollViewMode;
		
		private void DoShowLocalData ()
		{
			ShowScrollView (ScrollViewMode.LocalData);
			_tableController.ClearEntries ();

			_infos = WorkspaceController.GetLocalBlockInfos ();
			for (int i = 0; i < _infos.Count; i++) {
				_tableController.AddEntry (_infos [i]);
			}

			// Disable the currently active entry.
			if (Main.WorkspaceController.HasActiveBlockInfo) {
				string currentId = Main.WorkspaceController.ActiveBlockInfoId;
				_tableController.DisableEntry (currentId);
			}
		}

		private void OnScrollViewButtonClicked (string id)
		{
			if (Main.WorkspaceController.NeedsSave) {
				_dialogBox2.Show ("Save changes?", "Yes", "No",
				                  (buttonText) => {
					if (buttonText == "Yes") {
						Main.WorkspaceController.Save ();
					}

					BlkEdit.BlockInfo info;
					if (GetInfo (id, out info)) {
						Main.WorkspaceController.LoadForEditing (info);
						DoClose ();
					}
				});
				return;
			}

			{
				BlkEdit.BlockInfo info;
				if (GetInfo (id, out info)) {
					Main.WorkspaceController.LoadForEditing (info);
					DoClose ();
				}
			}
		}

		private char TitleNameValidator (string text, int pos, char ch)
		{
			char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
			char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
			
			if (ch >= 'a' && ch <= 'z')
			{
				// Space followed by a letter -- make sure it's capitalized
				if (lastChar == ' ') return (char)(ch - 'a' + 'A');
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				// Uppercase letters are only allowed after spaces (and apostrophes)
				if (lastChar != ' ' && lastChar != '\'') return (char)(ch - 'A' + 'a');
				return ch;
			}
			else if (ch == '\'')
			{
				// Don't allow more than one apostrophe
				if (lastChar != ' ' && lastChar != '\'' && nextChar != '\'' && !text.Contains("'")) return ch;
			}
			else if (ch == ' ')
			{
				// Don't allow more than one space in a row
				if (lastChar != ' ' && lastChar != '\'' && nextChar != ' ' && nextChar != '\'') return ch;
			}
			else if (ch >= '0' && ch <= '9')
			{
				return ch;
			}

			return (char)0;
		}

		private bool GetInfo (string id, out BlkEdit.BlockInfo outInfo)
		{
			for (int i = 0; i < _infos.Count; i++) {
				BlkEdit.BlockInfo info = _infos [i];
				if (info.Id == id) {
					outInfo = info;
					return true;
				}
			}

			outInfo = new BlkEdit.BlockInfo ();
			return false;
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

			_tableController.UpdateEntry (id, texture);
		}
	}
}