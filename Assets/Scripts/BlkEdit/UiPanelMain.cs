using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelMain : Uzu.UiPanel
	{
		private const string MODE_BUTTON_ID = "ModeButton";
		private const string SAVE_BUTTON_ID = "SaveButton";
		private const string LOAD_BUTTON_ID = "LoadButton";
		private const string UNDO_BUTTON_ID = "Button-Undo";
		private const string REDO_BUTTON_ID = "Button-Redo";

		[SerializeField]
		private UILabel _modeButton;
		[SerializeField]
		private UIButton _undoButton;
		[SerializeField]
		private UIButton _redoButton;

		[SerializeField]
		private UISprite _activeColor;
		[SerializeField]
		private ColorPicker _colorPicker;

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

			Color newColor = _colorPicker.ActiveColor;
			if (_currentMode == Mode.Add) {
				newColor.a = 1.0f;
			}
			else {
				newColor.a = 0.25f;
			}
			_activeColor.color = newColor;
		}

		private string GetTempFilePath ()
		{
			return Application.persistentDataPath + "/" + "abc.blk";
		}

		private void DoSave ()
		{
			Uzu.ChunkIterator it = Main.BlockWorld.GetActiveChunksIterator ();
			it.MoveNext ();

			Uzu.BlockContainer blocks = it.CurrentChunk.Blocks;
			byte[] data = Uzu.BlockWriter.Write (blocks);
			Uzu.BlockIO.WriteFile (GetTempFilePath (), data);
		}

		private void DoLoad ()
		{
			byte[] data = Uzu.BlockIO.ReadFile (GetTempFilePath ());
			Uzu.BlockFormat.Data blockData = Uzu.BlockReader.Read (data);

			// TODO: ugly
			{
				Uzu.ChunkIterator it = Main.BlockWorld.GetActiveChunksIterator ();
				it.MoveNext ();

				Uzu.Chunk chunk = it.CurrentChunk;

				int count = blockData._states.Length;
				
				if (count != chunk.Blocks.Count) {
					Debug.LogError ("Invalid BlockFormat.Data! Size does not match chunk size.");
					return;
				}
				
				for (int i = 0; i < count; i++) {
					if (blockData._states [i]) {
						chunk.Blocks [i].Type = Uzu.BlockType.SOLID;
						chunk.Blocks [i].Color = blockData._colors [i].ToColor32 ();
					}
					else {
						chunk.Blocks [i].Type = Uzu.BlockType.EMPTY;
					}
				}

				chunk.RequestRebuild ();
			}

			// Refresh the grid.
			Main.GridController.RebuildGrid (blockData);
		}
	}
}