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

		public static WorkspaceController WorkspaceController {
			get { return _instance._workspaceController; }
		}

		public static HttpRequestHandler HttpRequestHandler {
			get { return _instance._httpRequestHandler; }
		}
		
		#region Overrided methods.
		protected override void OnMainBegin ()
		{
			_instance = (Main)Uzu.Main.Instance;
			
			// Singleton creation.
			{
				_commandMgr = new BlkEdit.CommandMgr ();

				{
					GameObject go = new GameObject ("HttpRequestHandler", typeof (HttpRequestHandler));
					_httpRequestHandler = go.GetComponent <HttpRequestHandler> ();
				}

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
					_blockWorld.CachedXform.parent = _modelViewPivotXform;

					{
						_spinRegion.Target = _modelViewPivotXform;

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
			_workspaceController.New ();

//			_panelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
			_panelMgr.ChangeCurrentPanel (PanelIds.PANEL_OPTIONS);
		}

		protected override void OnMainEnd ()
		{
			_instance = null;
		}

		[SerializeField]
		private GridController _gridController;
		[SerializeField]
		private WorkspaceController _workspaceController;
		[SerializeField]
		private Transform _modelViewPivotXform;
		[SerializeField]
		private SpinWithMouse _spinRegion;
		[SerializeField]
		private Uzu.UiPanelMgr _panelMgr;
		[SerializeField]
		private Material _blockMaterial;

		private BlkEdit.CommandMgr _commandMgr;
		private Uzu.BlockWorld _blockWorld;
		private HttpRequestHandler _httpRequestHandler;
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