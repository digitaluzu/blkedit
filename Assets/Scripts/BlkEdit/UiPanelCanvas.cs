using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelCanvas : Uzu.UiPanel
	{
		private const string UNDO_BUTTON_ID = "Button-Undo";
		private const string REDO_BUTTON_ID = "Button-Redo";
		private const string BUTTON_ID_OPTIONS = "Button-Options";

		[SerializeField]
		private UISprite _activeColor;
		[SerializeField]
		private ColorPicker _colorPicker;

		public override void OnActivate ()
		{

		}

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case UNDO_BUTTON_ID:
				Main.CommandMgr.UndoCommand ();
				break;

			case REDO_BUTTON_ID:
				Main.CommandMgr.RedoCommand ();
				break;

			case BUTTON_ID_OPTIONS:
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_OPTIONS);
				break;
			}
		}

		public override void OnUpdate ()
		{
			/*
			Color newColor = _colorPicker.ActiveColor;
			if (_currentMode == Mode.Add) {
				newColor.a = 1.0f;
			}
			else {
				newColor.a = 0.25f;
			}
			_activeColor.color = newColor;
			*/
		}
	}
}