using UnityEngine;
using System.Collections;

namespace Blk
{
	public class GridController : Uzu.BaseBehaviour
	{
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

			if (_isPressed) {
				ProcessInput ();
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

			// TODO: temp
			if ((Main.PanelMgr.CurrentPanel as UiPanelMain).CurrentMode == UiPanelMain.Mode.Add) {
				Color32 color = _colorPicker.ActiveColor;

				// Only process if there is not already an entry at this coordinate,
				// or the color of the coordinate will actually change.
				if (!_grid.IsSet (coord) || !IsSameColor (_grid.GetColor (coord), color)) {
					CommandInterface cmd = new AddBlockCommand (_grid, coord, color);
					Main.CommandMgr.DoCommand (cmd);
				}
			}
			else {
				// Only process if there is an existing entry at this coord.
				if (_grid.IsSet (coord)) {
					CommandInterface cmd = new EraseBlockCommand (_grid, coord);
					Main.CommandMgr.DoCommand (cmd);
				}
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
			
			localPos += _grid.TEMP_GridPivotOffset;
			
			Uzu.VectorI2 cellCoord = new Uzu.VectorI2 (localPos.x / _grid.TEMP_CellSize.x, localPos.y / _grid.TEMP_CellSize.y);
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
