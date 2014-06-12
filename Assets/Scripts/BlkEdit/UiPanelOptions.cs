using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelOptions : Uzu.UiPanel
	{
		private const string BUTTON_ID_SAVE = "Button-Save";
		private const string BUTTON_ID_LOAD = "Button-Load";
		private const string BUTTON_ID_CLOSE = "Button-Close";
		private const string BUTTON_ID_SEARCH = "Button-Search";

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
			}
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

		private void DoClose ()
		{
			Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
		}

		[SerializeField]
		private GameObject _searchOnlineObject;
		[SerializeField]
		private TableController _tableController;

		private void DoSearchOnline ()
		{
			_searchOnlineObject.SetActive (true);

			_tableController.AddEntry ("hello1");
		}
	}
}