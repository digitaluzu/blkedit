using UnityEngine;
using System.Collections;

namespace Blk
{
	public class GridController : Uzu.BaseBehaviour
	{
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
				Color32 color = Main.ColorPicker.ActiveColor;
				CommandInterface cmd = new AddBlockCommand (_grid, coord, color);
				Main.CommandMgr.DoCommand (cmd);
			}
			else {
				CommandInterface cmd = new EraseBlockCommand (_grid, coord);
				Main.CommandMgr.DoCommand (cmd);
			}
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
	}
}
