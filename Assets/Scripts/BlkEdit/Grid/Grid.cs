using UnityEngine;
using System.Collections;

namespace Blk
{
	public class Grid : Uzu.BaseBehaviour
	{
		private static readonly Uzu.VectorI2 DIMENSIONS = new Uzu.VectorI2 (16, 16);

		// TODO: temp
		[SerializeField]
		private UiPanelMain _panel;

		private Camera _uiCamera;
		private bool _isPressed;
		private UITexture _gridSprite;

		private Vector2 _cellSize;
		private Vector3 _gridPivotOffset;
		private int _cellSpriteDepth;

		private GridLayer _layer;

		protected override void Awake ()
		{
			base.Awake ();

			_uiCamera = NGUITools.FindCameraForLayer (this.gameObject.layer);
			_gridSprite = GetComponent <UITexture> ();
			_cellSpriteDepth = _gridSprite.depth - 1;

			_layer = new GridLayer (DIMENSIONS);
		}

		private void Update ()
		{
			{
				_cellSize = new Vector2 (_gridSprite.localSize.x / DIMENSIONS.x, _gridSprite.localSize.y / DIMENSIONS.y);

				{
					Vector2 pivot = _gridSprite.pivotOffset;
					_gridPivotOffset = new Vector3 (pivot.x * _gridSprite.width, pivot.y * _gridSprite.height, 0.0f);
				}
			}

			// Drag handling.
			if (_isPressed && UICamera.hoveredObject == this.gameObject) {
				ProcessInput ();
			}
		}

		public void TODO_ForceRefreshWithCurrentWorld ()
		{
			// for each non-empty idx of world, place cell of appropriate color
			Uzu.BlockWorld blockWorld = Main.BlockWorld;

			// clear all contents
			{
				_layer.ClearAll ();
			}

			for (int x = 0; x < blockWorld.Config.ChunkSizeInBlocks.x; x++) {
				for (int y = 0; y < blockWorld.Config.ChunkSizeInBlocks.y; y++) {
					Uzu.VectorI3 idx = new Uzu.VectorI3 (x, y, 0);
					Uzu.BlockType blockType = blockWorld.GetBlockType (idx);
					if (blockType != Uzu.BlockType.EMPTY) {
						Color32 color = blockWorld.GetBlockColor (idx);

						Uzu.VectorI2 cellCoord = new Uzu.VectorI2 (idx.x, idx.y);
						PlaceAtCell (cellCoord, color);
					}
				}
			}
		}

		private void OnPress (bool pressed)
		{
			_isPressed = pressed;

			if (_isPressed) {
				ProcessInput ();
			}
		}

		private void ProcessInput ()
		{
			// Calculate cell coordinates.
			Uzu.VectorI2 cellCoord;
			{
				Vector3 touchScreenPos = UICamera.lastTouchPosition;
				Vector3 touchWorldPos = _uiCamera.ScreenToWorldPoint (touchScreenPos);
				Vector3 touchLocalPos = CachedXform.worldToLocalMatrix.MultiplyPoint3x4 (touchWorldPos);

				touchLocalPos += _gridPivotOffset;

				cellCoord = new Uzu.VectorI2 (touchLocalPos.x / _cellSize.x, touchLocalPos.y / _cellSize.y);
				cellCoord = Uzu.VectorI2.Clamp (cellCoord, Uzu.VectorI2.zero, DIMENSIONS - Uzu.VectorI2.one);
			}

			Color32 color = Main.ColorPicker.ActiveColor;
			PlaceAtCell (cellCoord, color);
		}

		private void PlaceAtCell (Uzu.VectorI2 cellCoord, Color32 color)
		{
			GridCell cell = _layer.GetCell (cellCoord);

			// Create new cell if one hasn't already been placed.
			if (cell == null) {
				Vector3 cellPos = Uzu.Math.Vector2ToVector3 (cellCoord * _cellSize) - _gridPivotOffset;
				GameObject go = Main.GridCellPool.Spawn (cellPos);
				cell = go.GetComponent <GridCell> ();

				UISprite sprite = cell.Sprite;
				sprite.depth = _cellSpriteDepth;
				sprite.width = (int)_cellSize.x;
				sprite.height = (int)_cellSize.y;

				_layer.SetCell (cellCoord, cell);
			}

			bool isAdd = (_panel.CurrentMode == UiPanelMain.Mode.Add) ? true : false;

			if (isAdd) {
				cell.SetColor (color);
					
				{
					Uzu.VectorI3 blockIndex = new Uzu.VectorI3 (cellCoord.x, cellCoord.y, 0);
					Main.BlockWorld.SetBlockType(blockIndex, (Uzu.BlockType)BlockType.SOLID);
					Main.BlockWorld.SetBlockColor(blockIndex, color);
				}
			}
			else {
				cell.Unspawn ();
				_layer.SetCell (cellCoord, null);

				{
					Uzu.VectorI3 blockIndex = new Uzu.VectorI3 (cellCoord.x, cellCoord.y, 0);
					Main.BlockWorld.SetBlockType(blockIndex, Uzu.BlockType.EMPTY);
				}
			}
		}
	}
}