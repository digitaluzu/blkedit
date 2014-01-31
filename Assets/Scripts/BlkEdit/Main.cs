using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class Main : Uzu.Main
	{
		public Material _mat;

		public static ColorPicker ColorPicker {
			get { return _instance._colorPicker; }
		}

		public static Uzu.GameObjectPool GridCellPool {
			get { return _instance._gridCellPool; }
		}
		
		#region Overrided methods.
		protected override void OnMainBegin ()
		{
			_instance = (Main)Uzu.Main.Instance;
			
			// Singleton creation.
			{
				// Create block world.
				{
					Uzu.BlockWorldConfig config = new Uzu.BlockWorldConfig ();
					config.BlockSize = Constants.BLOCK_SIZE;
					config.ChunkSizeInBlocks = Constants.CHUNK_SIZE_IN_BLOCKS;
					config.MaxBlockTypeCount = (int)BlockType.MAX_COUNT;
					{
						Uzu.BlockDesc[] descs = new Uzu.BlockDesc[(int)BlockType.MAX_COUNT];
						{
							Uzu.BlockDesc desc = new Uzu.BlockDesc ();
							descs [0] = desc;
						}
						{
							Uzu.BlockDesc desc = new Uzu.BlockDesc ();
							desc.Material = _mat;
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
		}
		
		protected override void OnMainBegin2 ()
		{

		}

		protected override void OnMainEnd ()
		{
			_instance = null;
		}

		[SerializeField]
		private ColorPicker _colorPicker;
		[SerializeField]
		private Uzu.GameObjectPool _gridCellPool;
		[SerializeField]
		private SpinWithMouse _spinRegion;

		private Uzu.BlockWorld _blockWorld;
		private Uzu.BlockWorldController _blockWorldController;
		
		public static Uzu.BlockWorld BlockWorld {
			get { return _instance._blockWorld; }
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