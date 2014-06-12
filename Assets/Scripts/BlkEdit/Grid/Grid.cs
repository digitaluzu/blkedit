using UnityEngine;
using System.Collections;

namespace Blk
{
	public class Grid : Uzu.BaseBehaviour, BlkEdit.GridInterface
	{
		public Vector2 CellSize {
			get { return _cellSize; }
		}

		public static int CoordToIndex (Uzu.VectorI2 dimensions, Uzu.VectorI2 coord)
		{
			return coord.y * dimensions.x + coord.x;
		}

		public bool IsSet (Uzu.VectorI2 coord)
		{
			return _currentLayer.IsSet (coord);
		}

		public void Unset (Uzu.VectorI2 coord)
		{
			_currentLayer.Unset (coord);

			// Sprite display.
			{
				int idx = CoordToIndex (Constants.GRID_DIMENSIONS, coord);
				GridCell cell = _cells [idx];
				cell.Sprite.enabled = false;
			}

			// Block world.
			{
				Uzu.VectorI3 blockIndex = new Uzu.VectorI3 (coord.x, coord.y, 0);
				Main.BlockWorld.SetBlockType (blockIndex, Uzu.BlockType.EMPTY);
			}
		}

		public Color32 GetColor (Uzu.VectorI2 coord)
		{
			return _currentLayer.GetColor (coord);
		}

		public void SetColor (Uzu.VectorI2 coord, Color32 color)
		{
			_currentLayer.SetColor (coord, color);

			// Sprite display.
			{
				int idx = CoordToIndex (Constants.GRID_DIMENSIONS, coord);
				GridCell cell = _cells [idx];
				cell.Sprite.color = color;
				cell.Sprite.enabled = true;
			}

			// Block world.
			{
				Uzu.VectorI3 blockIndex = new Uzu.VectorI3 (coord.x, coord.y, 0);
				Main.BlockWorld.SetBlockType (blockIndex, Uzu.BlockType.SOLID);
				Main.BlockWorld.SetBlockColor (blockIndex, color);
			}
		}

		#region Implementation.
		[SerializeField]
		private GameObject _gridCellPrefab;
		private GridLayer _currentLayer;
		private Uzu.FixedList <GridCell> _cells;

		private Vector2 _cellSize;

		protected override void Awake ()
		{
			base.Awake ();

			_currentLayer = new GridLayer (Constants.GRID_DIMENSIONS);

			// Allocate and initialize sprite resources.
			{
				UITexture gridSprite = GetComponent <UITexture> ();
				int cellSpriteDepth = gridSprite.depth - 1;
				_cellSize = new Vector2 (gridSprite.localSize.x / Constants.GRID_DIMENSIONS.x, gridSprite.localSize.y / Constants.GRID_DIMENSIONS.y);

				int totalCount = Uzu.VectorI2.ElementProduct (Constants.GRID_DIMENSIONS);
				_cells = new Uzu.FixedList<GridCell> (totalCount);

				for (int y = 0; y < Constants.GRID_DIMENSIONS.y; y++) {
					for (int x = 0; x < Constants.GRID_DIMENSIONS.x; x++) {
						Uzu.VectorI2 coord = new Uzu.VectorI2 (x, y);

						Vector3 pos = Uzu.Math.Vector2ToVector3 (coord * _cellSize);
						GameObject go = (GameObject)GameObject.Instantiate (_gridCellPrefab);
						Transform xform = go.transform;
						xform.parent = CachedXform;
						xform.localScale = Vector3.one;
						xform.localPosition = pos;
						GridCell cell = go.GetComponent <GridCell> ();

						UISprite sprite = cell.Sprite;
						sprite.depth = cellSpriteDepth;
						sprite.width = (int)_cellSize.x - 1;
						sprite.height = (int)_cellSize.y - 1;

						_cells.Add (cell);

						cell.Sprite.enabled = false;
					}
				}
			}
		}
		#endregion
	}
}