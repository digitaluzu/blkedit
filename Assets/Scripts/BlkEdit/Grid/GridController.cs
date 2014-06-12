using UnityEngine;
using System.Collections;

namespace Blk
{
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
		private Grid _grid;
		private Camera _uiCamera;
		private bool _isPressed;

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
			
			Uzu.VectorI2 cellCoord = new Uzu.VectorI2 (localPos.x / _grid.CellSize.x, localPos.y / _grid.CellSize.y);
			cellCoord = Uzu.VectorI2.Clamp (cellCoord, Uzu.VectorI2.zero, Constants.GRID_DIMENSIONS - Uzu.VectorI2.one);
			return cellCoord;
		}

		protected override void Awake ()
		{
			base.Awake ();

			_grid = GetComponent <Grid> ();
			_uiCamera = NGUITools.FindCameraForLayer (this.gameObject.layer);
		}
		#endregion
	}
}
