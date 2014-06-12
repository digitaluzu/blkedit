using UnityEngine;
using System.Collections;

namespace Blk
{
	[RequireComponent (typeof(BlkEdit.Grid))]
	public class GridController : Uzu.BaseBehaviour
	{
		public enum Mode {
			AddBlocks,
			EraseBlocks,
			EyeDropper,
			Count,
		};

		public Mode CurrentMode {
			get; set;
		}

		public void ClearGrid ()
		{
			Uzu.VectorI2 xy = Constants.GRID_DIMENSIONS;

			for (int x = 0; x < xy.x; x++) {
				for (int y = 0; y < xy.y; y++) {
					Uzu.VectorI2 coord = new Uzu.VectorI2 (x, y);
					_grid.Unset (coord);
				}
			}
		}

		public void RebuildGrid (Uzu.BlockFormat.Data data)
		{
			Uzu.VectorI2 xy = Constants.GRID_DIMENSIONS;

			if (Uzu.VectorI2.ElementProduct (xy) != data._states.Length) {
				Debug.LogError ("Invalid BlockFormat.Data! Size does not match chunk size.");
				return;
			}

			int cnt = 0;
			for (int x = 0; x < xy.x; x++) {
				for (int y = 0; y < xy.y; y++) {
					Uzu.VectorI2 coord = new Uzu.VectorI2 (x, y);
					if (data._states [cnt]) {
						_grid.SetColor (coord, data._colors [cnt].ToColor32 ());
					}
					else {
						_grid.Unset (coord);
					}

					cnt++;
				}
			}
		}

		#region Implementation.
		[SerializeField]
		private ColorPicker _colorPicker;
		private BlkEdit.Grid _grid;
		private Camera _uiCamera;
		private bool _isPressed;

		[SerializeField]
		private GameObject _gridCellPrefab;
		private Uzu.FixedList <GridCell> _cells;
		private Vector2 _cellSize;

		private void OnPress (bool pressed)
		{
			_isPressed = pressed;

			// TODO: cleanup

			if (_isPressed) {
				if (CurrentMode == Mode.AddBlocks ||
				    CurrentMode == Mode.EraseBlocks) {
					Blk.Main.CommandMgr.BeginGroupCommand ();
				}

				ProcessInput ();
			}
			else {
				if (CurrentMode == Mode.AddBlocks ||
				    CurrentMode == Mode.EraseBlocks) {
					Blk.Main.CommandMgr.EndGroupCommand ();
				}
			}
		}

		private void Update ()
		{
			// Drag handling.
			if (_isPressed && UICamera.hoveredObject == this.gameObject) {
				ProcessInput ();
			}
		}

		private void ProcessInput ()
		{
			// Calculate cell coordinates.
			Uzu.VectorI2 coord = ScreenPosToCoord (UICamera.lastTouchPosition);

			if (CurrentMode == Mode.AddBlocks) {
				Color32 color = _colorPicker.ActiveColor;

				// Only process if there is not already an entry at this coordinate,
				// or the color of the coordinate will actually change.
				if (!_grid.IsSet (coord) || !IsSameColor (_grid.GetColor (coord), color)) {
					BlkEdit.CommandInterface cmd = new BlkEdit.AddBlockCommand (_grid, coord, color);
					Main.CommandMgr.DoCommand (cmd);
				}
			}
			else if (CurrentMode == Mode.EraseBlocks) {
				// Only process if there is an existing entry at this coord.
				if (_grid.IsSet (coord)) {
					BlkEdit.CommandInterface cmd = new BlkEdit.EraseBlockCommand (_grid, coord);
					Main.CommandMgr.DoCommand (cmd);
				}
			}
			else if (CurrentMode == Mode.EyeDropper) {
				if (_grid.IsSet (coord)) {
					Color32 color = _grid.GetColor (coord);
					_colorPicker.ActiveColor = color;
				}
			}
			else {
				Debug.LogWarning ("Unhandled mode: " + CurrentMode);
			}
		}

		private bool IsSameColor (Color32 c0, Color32 c1)
		{
			return c0.r == c1.r &&
				c0.g == c1.g &&
				c0.b == c1.b &&
				c0.a == c1.a;
		}

		private Uzu.VectorI2 ScreenPosToCoord (Vector3 screenPos)
		{
			Vector3 worldPos = _uiCamera.ScreenToWorldPoint (screenPos);
			Vector3 localPos = CachedXform.worldToLocalMatrix.MultiplyPoint3x4 (worldPos);
			
			Uzu.VectorI2 cellCoord = new Uzu.VectorI2 (localPos.x / _cellSize.x, localPos.y / _cellSize.y);
			cellCoord = Uzu.VectorI2.Clamp (cellCoord, Uzu.VectorI2.zero, Constants.GRID_DIMENSIONS - Uzu.VectorI2.one);
			return cellCoord;
		}

		private static int CoordToIndex (Uzu.VectorI2 dimensions, Uzu.VectorI2 coord)
		{
			return coord.y * dimensions.x + coord.x;
		}

		private void OnGridCellSet (Uzu.VectorI2 coord, Color32 color)
		{
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

		private void OnGridCellUnset (Uzu.VectorI2 coord)
		{
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

		protected override void Awake ()
		{
			base.Awake ();

			{
				_grid = GetComponent <BlkEdit.Grid> ();

				BlkEdit.GridConfig config = new BlkEdit.GridConfig ();
				config.Dimensions = new Uzu.VectorI3(Constants.GRID_DIMENSIONS.x, Constants.GRID_DIMENSIONS.y, 1);
				_grid.Initialize (config);

				// Callbacks.
				_grid.OnGridCellSet += OnGridCellSet;
				_grid.OnGridCellUnset += OnGridCellUnset;
			}

			_uiCamera = NGUITools.FindCameraForLayer (this.gameObject.layer);

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
