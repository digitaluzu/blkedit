using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class Main : Uzu.Main
	{
		public Material _mat;
		
		#region Overrided methods.
		protected override void OnMainBegin ()
		{
			_instance = (Main)Uzu.Main.Instance;
			
			// Singleton creation.
			{
			}
		}
		
		protected override void OnMainBegin2 ()
		{
			// Create block world.
			{
				Uzu.BlockWorldConfig config = new Uzu.BlockWorldConfig ();
				config.BlockSize = new Vector3 (1.0f, 1.0f, 1.0f);
				config.ChunkSizeInBlocks = Constants.CHUNK_SIZE_IN_BLOCKS;
				{
					Uzu.BlockDesc[] descs = new Uzu.BlockDesc[(int)Uzu.BlockType.MAX_COUNT];
					{
						Uzu.BlockDesc desc = new Uzu.BlockDesc ();
						descs [0] = desc;
					}
					{
						Uzu.BlockDesc desc = new Uzu.BlockDesc ();
						desc.Color = Color.red;
						desc.Material = _mat;
						descs [1] = desc;
					}
					config.BlockDescs = descs;
				}
				
				GameObject blockWorldGO = new GameObject ("UzuBlockWorld", typeof(Uzu.BlockWorld));
				_blockWorld = blockWorldGO.GetComponent<Uzu.BlockWorld> ();
				_blockWorld.Initialize (config);
			}
			
			// Create block world controller.
			{
				Uzu.BlockWorldControllerConfig config = new Uzu.BlockWorldControllerConfig ();
				config.TargetBlockWorld = _blockWorld;
				config.LoadedChunkCount = Constants.LOADED_CHUNK_COUNT;
				
				GameObject blockWorldControllerGO = new GameObject ("UzuBlockWorldController", typeof(Uzu.BlockWorldController));
				_blockWorldController = blockWorldControllerGO.GetComponent<Uzu.BlockWorldController> ();
				_blockWorldController.Initialize (config);
			}
		}
		
		private Uzu.BlockWorld _blockWorld;
		private Uzu.BlockWorldController _blockWorldController;
		
		public static Uzu.BlockWorld BlockWorld {
			get { return _instance._blockWorld; }
		}
		
		protected override void OnMainEnd ()
		{
			_instance = null;
		}
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