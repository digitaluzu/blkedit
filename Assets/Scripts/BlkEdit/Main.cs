using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class Main : Uzu.Main
	{
		public Material _mat;

		public static Material Mat {
			get { return _instance._mat; }
		}

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
					config.MaxBlockTypeCount = (int)Uzu.BlockType.SYSTEM_DEFAULT_COUNT;
					{
						Uzu.BlockDesc[] descs = new Uzu.BlockDesc[(int)Uzu.BlockType.SYSTEM_DEFAULT_COUNT];
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

					// Manually load.
					_blockWorld.LoadChunk (Uzu.VectorI3.zero);
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
		
		public static Uzu.BlockWorld BlockWorld {
			get { return _instance._blockWorld; }
			set {
				// TODO:
				// Replace existing world.
				GameObject.Destroy (_instance._blockWorld.gameObject);
				_instance._blockWorld = value;

				_instance._spinRegion.Target = value.CachedXform;
			}
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