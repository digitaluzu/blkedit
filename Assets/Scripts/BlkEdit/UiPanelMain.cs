﻿using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelMain : Uzu.UiPanel
	{
		private const string MODE_BUTTON_ID = "ModeButton";
		private const string SAVE_BUTTON_ID = "SaveButton";
		private const string LOAD_BUTTON_ID = "LoadButton";
		private const string UNDO_BUTTON_ID = "UndoButton";
		private const string REDO_BUTTON_ID = "RedoButton";

		[SerializeField]
		private UILabel _modeButton;
		[SerializeField]
		private UIButton _undoButton;
		[SerializeField]
		private UIButton _redoButton;

		public enum Mode {
			Add,
			Remove,
		};

		public Mode CurrentMode {
			get { return _currentMode; }
		}

		private Mode _currentMode = Mode.Add;

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
				case MODE_BUTTON_ID:
				{
					const string addText = "[Add] / Erase";
					const string eraseText = "Add / [Erase]";

					if (_modeButton.text == addText) {
						_currentMode = Mode.Remove;
						_modeButton.text = eraseText;
					}
					else {
						_currentMode = Mode.Add;
						_modeButton.text = addText;
					}
				}
				break;

				case SAVE_BUTTON_ID:
				{
					DoSave ();
				}
				break;

				case LOAD_BUTTON_ID:
				{
					DoLoad ();
				}
				break;

			case UNDO_BUTTON_ID:
				{
					Main.CommandMgr.UndoCommand ();
				}
				break;

			case REDO_BUTTON_ID:
				{
					Main.CommandMgr.RedoCommand ();
				}
				break;
			}
		}

		public override void OnUpdate ()
		{
			if (Main.CommandMgr.HasCommandsToUndo) {
				if (!_undoButton.isEnabled) {
					_undoButton.isEnabled = true;
				}
			}
			else {
				if (_undoButton.isEnabled) {
					_undoButton.isEnabled = false;
				}
			}

			if (Main.CommandMgr.HasCommandsToRedo) {
				if (!_redoButton.isEnabled) {
					_redoButton.isEnabled = true;
				}
			}
			else {
				if (_redoButton.isEnabled) {
					_redoButton.isEnabled = false;
				}
			}
		}

		private const string FILENAME = "TestData/abc.txt";

		private void DoSave ()
		{
			Uzu.BlockExporter exporter = new Uzu.BlockExporter();
			Uzu.BlockPack pack = new Uzu.BlockPack ();
			pack._blockWorld = Main.BlockWorld;
			exporter.Export (FILENAME, pack);
		}

		private void DoLoad ()
		{
			Uzu.BlockImporter importer = new Uzu.BlockImporter ();
			Uzu.BlockPack pack = importer.Import (FILENAME);
			Main.BlockWorld = pack._blockWorld;

			// TODO:
			Grid grid = GameObject.Find ("Grid").GetComponent <Grid> ();
			grid.TODO_ForceRefreshWithCurrentWorld ();
		}
	}
}