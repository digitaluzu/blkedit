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

		private string _savePathBlockInfo;
		private string _savePathBlockData;
		private bool _needsSave;

		public bool NeedsSave {
			get { return _needsSave; }
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

			// Create directories for later writing.
			System.IO.Directory.CreateDirectory(FileUtil.LocalModelPath);
			System.IO.Directory.CreateDirectory(FileUtil.SavedModelPath);
		}

		public void Save ()
		{
			if (!_needsSave) {
				return;
			}

			if (string.IsNullOrEmpty(_savePathBlockData)) {
				string baseFilePath = System.IO.Path.Combine (FileUtil.LocalModelPath, FileUtil.GetNewFileName ());
				_savePathBlockInfo = baseFilePath + "." + BlkEdit.BlockInfo.Extension;
				_savePathBlockData = baseFilePath + "." + Uzu.BlockFormat.Extension;
			}

			// Block info.
			{
				BlkEdit.BlockInfo info = new BlkEdit.BlockInfo ();
				info.Id = BlkEdit.BlockInfo.GetNewId ();
				info.Name = "Foo Name";
				info.BlockDataPath = _savePathBlockData;
				info.ImagePath = "...";
				if (!BlkEdit.BlockInfo.Save (_savePathBlockInfo, info)) {
					Debug.LogError ("Error saving block info.");
					return;
				}
			}

			// TODO: screenshot

			// Block data.
			{
				Uzu.ChunkIterator it = Main.BlockWorld.GetActiveChunksIterator ();
				it.MoveNext ();
				
				Uzu.BlockContainer blocks = it.CurrentChunk.Blocks;
				byte[] data = Uzu.BlockWriter.Write (blocks);
				Uzu.BlockIO.WriteFile (_savePathBlockData, data);
			}

			_needsSave = false;
		}

		public void Load (BlkEdit.BlockInfo info)
		{
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
