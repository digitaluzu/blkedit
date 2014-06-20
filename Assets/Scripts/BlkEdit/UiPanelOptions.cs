using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class UiPanelOptions : UiPanelBaseView
	{
		private const string BUTTON_ID_SAVE = "Button-Save";
		private const string BUTTON_ID_MY_DATA = "Button-MyData";
		private const string BUTTON_ID_SAVED_DATA = "Button-SavedData";
		private const string BUTTON_ID_SEARCH = "Button-Search";

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

			case BUTTON_ID_SAVED_DATA:
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_SAVED_DATA_VIEW);
				break;

			case BUTTON_ID_SEARCH:
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_SEARCH);
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
	}
}