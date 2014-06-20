using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class WorkspaceController
	{
		public static List <BlkEdit.BlockInfo> GetLocalBlockInfos ()
		{
			List <string> paths = new List<string> ();
			
			// Get the paths.
			{
				System.IO.DirectoryInfo levelDirInfo = new System.IO.DirectoryInfo (FileUtil.LocalModelPath);
				System.IO.FileInfo[] files = levelDirInfo.GetFiles ("*." + BlkEdit.BlockInfo.Extension);
				for (int i = 0; i < files.Length; i++) {
					paths.Add (files [i].FullName);
				}
			}

			List <BlkEdit.BlockInfo> infos = new List<BlkEdit.BlockInfo> (paths.Count);

			// Create infos.
			{
				for (int i = 0; i < paths.Count; i++) {
					BlkEdit.BlockInfo info;
					if (BlkEdit.BlockInfo.Load (paths [i], out info)) {
						infos.Add (info);
					}
					else {
						Debug.Log ("Unable to load BlockInfo at: " + paths [i]);
					}
				}
			}

			// TODO: sort infos based on: name? modified data? ...

			return infos;
		}

		private BlkEdit.BlockInfo? _activeBlockInfo;
		private string _savePathBlockInfo;
		private bool _needsSave;

		public bool NeedsSave {
			get { return _needsSave; }
		}

		public bool HasActiveBlockInfo {
			get { return _activeBlockInfo.HasValue; }
		}

		public string ActiveBlockInfoId {
			get {
				if (_activeBlockInfo.HasValue) {
					return _activeBlockInfo.Value.Id;
				}

				return string.Empty;
			}
		}

		public WorkspaceController ()
		{
			Main.CommandMgr.OnCommandExecuted += OnCommandExecuted;
		}

		private void OnCommandExecuted (BlkEdit.CommandInterface cmd)
		{
			_needsSave = true;
		}

		public void New ()
		{
			_needsSave = false;
			_activeBlockInfo = null;

			// Create directories for later writing.
			System.IO.Directory.CreateDirectory(FileUtil.LocalModelPath);
			System.IO.Directory.CreateDirectory(FileUtil.SavedModelPath);
		}

		public void Save ()
		{
			if (!_needsSave) {
				return;
			}

			// Generate a new info if this is the first save.
			if (!HasActiveBlockInfo) {
				string baseFilePath = System.IO.Path.Combine (FileUtil.LocalModelPath, FileUtil.GetNewFileName ());
				_savePathBlockInfo = baseFilePath + "." + BlkEdit.BlockInfo.Extension;

				BlkEdit.BlockInfo info = new BlkEdit.BlockInfo ();
				info.Id = BlkEdit.BlockInfo.GetNewId ();
				info.Name = "Foo Name";
				info.BlockDataPath = baseFilePath + "." + Uzu.BlockFormat.Extension;
				info.ImagePath = "...";
				if (!BlkEdit.BlockInfo.Save (_savePathBlockInfo, info)) {
					Debug.LogError ("Error saving block info.");
					return;
				}

				_activeBlockInfo = info;
			}

			// TODO: screenshot

			// Block data.
			{
				Uzu.ChunkIterator it = Main.BlockWorld.GetActiveChunksIterator ();
				it.MoveNext ();
				
				Uzu.BlockContainer blocks = it.CurrentChunk.Blocks;
				byte[] data = Uzu.BlockWriter.Write (blocks);
				Uzu.BlockIO.WriteFile (_activeBlockInfo.Value.BlockDataPath, data);
			}

			_needsSave = false;
		}

		public void LoadForEditing (BlkEdit.BlockInfo info)
		{
			_activeBlockInfo = info;

			{
				byte[] data = Uzu.BlockIO.ReadFile (info.BlockDataPath);
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
}
