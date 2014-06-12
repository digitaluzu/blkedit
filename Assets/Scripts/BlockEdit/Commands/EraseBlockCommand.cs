using UnityEngine;
using System.Collections;

namespace BlkEdit
{
	public class EraseBlockCommand : CommandInterface
	{
		private Grid _grid;
		private Uzu.VectorI2 _coord;

		private bool _prevState;
		private Color32 _prevColor;

		public EraseBlockCommand (Grid grid, Uzu.VectorI2 coord)
		{
			_grid = grid;
			_coord = coord;
		}

		public void Do ()
		{
			_prevState = _grid.IsSet (_coord);
			_prevColor = _grid.GetColor (_coord);

			_grid.Unset (_coord);
		}

		public void Undo ()
		{
			if (_prevState) {
				_grid.SetColor (_coord, _prevColor);
			}
		}
	}
}
