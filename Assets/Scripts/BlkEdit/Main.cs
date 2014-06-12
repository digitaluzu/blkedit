using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class Main : Uzu.Main
	{
		public static Uzu.UiPanelMgr PanelMgr {
			get { return _instance._panelMgr; }
		}

		public static BlkEdit.CommandMgr CommandMgr {
			get { return _instance._commandMgr; }
		}

		public static Uzu.BlockWorld BlockWorld {
			get { return _instance._blockWorld; }
		}

		public static GridController GridController {
			get { return _instance._gridController; }
		}
		
		#region Overrided methods.
		protected override void OnMainBegin ()
		{
			_instance = (Main)Uzu.Main.Instance;
			
			// Singleton creation.
			{
				_commandMgr = new BlkEdit.CommandMgr ();

				// Create block world.
				{
					Uzu.BlockWorldConfig config = new Uzu.BlockWorldConfig ();
					config.BlockSize = Constants.BLOCK_SIZE;
					config.ChunkSizeInBlocks = Constants.CHUNK_SIZE_IN_BLOCKS;
					config.MaxBlockTypeCount = (int)Uzu.BlockType.SYSTEM_DEFAULT_COUNT;
					{
						Uzu.BlockDesc[] descs = new Uzu.BlockDesc[(int)Uzu.BlockType.SYSTEM_DEFAULT_COUNT];
						{
							Uzu.BlockDesc desc = new Uzu.BlockDesc ();
							descs [0] = desc;
						}
						{
							Uzu.BlockDesc desc = new Uzu.BlockDesc ();
							desc.Material = _blockMaterial;
							descs [1] = desc;
						}
						config.BlockDescs = descs;
					}
					
					GameObject blockWorldGO = new GameObject ("UzuBlockWorld", typeof(Uzu.BlockWorld));
					_blockWorld = blockWorldGO.GetComponent<Uzu.BlockWorld> ();
					_blockWorld.Initialize (config);

					{
						_spinRegion.Target = _blockWorld.CachedXform;

						float centerPosX = (_blockWorld.Config.BlockSize.x * _blockWorld.Config.ChunkSizeInBlocks.x) * 0.5f;
						_spinRegion.RotationPoint = new Vector3 (centerPosX, 0.0f, 0.0f);
					}

					// Manually load.
					_blockWorld.LoadChunk (Uzu.VectorI3.zero);
				}
			}
		}
		
		protected override void OnMainBegin2 ()
		{
			//_panelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
			_panelMgr.ChangeCurrentPanel (PanelIds.PANEL_OPTIONS);
		}

		protected override void OnMainEnd ()
		{
			_instance = null;
		}

		private void Update ()
		{
			if (Input.GetKeyDown (KeyCode.C)) {
				StartCoroutine (TmpWebRequest());
			}
		}

		private System.Collections.IEnumerator TmpWebRequest ()
		{
#if false
			// Post:
			Debug.Log ("sending http request....");
			
			const string url = "http://localhost:3000/";
			
			WWWForm form = new WWWForm ();
			form.AddField ("name", "test ship");

			string path = Application.persistentDataPath + "/" + "abc.blk";
			byte[] data = Uzu.BlockIO.ReadFile (path);
			form.AddBinaryData("file", data);

			WWW www = new WWW(url, form);
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError (www.error);
			}
			else {
				Debug.Log ("success: " + www.text);
			}
#else
			// Get:
			const string url = "http://localhost:3000/query?id=0";

			WWW www = new WWW (url);
			yield return www;

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError (www.error);
			}
			else {
				Debug.Log ("success: ");

				{
					byte[] data = www.bytes;
					Uzu.BlockFormat.Data blockData = Uzu.BlockReader.Read (data);

					// TODO: ugly
					{
						Uzu.ChunkIterator it = Main.BlockWorld.GetActiveChunksIterator ();
						it.MoveNext ();
						
						Uzu.Chunk chunk = it.CurrentChunk;
						
						int count = blockData._states.Length;
						
						if (count != chunk.Blocks.Count) {
							Debug.LogError ("Invalid BlockFormat.Data! Size does not match chunk size.");
							return false;
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
#endif
		}

		[SerializeField]
		private GridController _gridController;
		[SerializeField]
		private SpinWithMouse _spinRegion;
		[SerializeField]
		private Uzu.UiPanelMgr _panelMgr;
		[SerializeField]
		private Material _blockMaterial;

		private BlkEdit.CommandMgr _commandMgr;
		private Uzu.BlockWorld _blockWorld;
		#endregion
		
		#region Implementation.	
		#region Singleton implementation.
		private static Main _instance;
	
		public static new Main Instance {
			get { return _instance; }
		}
		#endregion
		#endregion
	}
}