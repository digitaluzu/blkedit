using UnityEngine;
using System.Collections;

namespace BlkEdit
{
	public class AddBlockCommand : CommandInterface
	{
		private Grid _grid;
		private Uzu.VectorI2 _coord;
		private Color32 _newColor;

		private bool _prevState;
		private Color32 _prevColor;

		public AddBlockCommand (Grid grid, Uzu.VectorI2 coord, Color32 color)
		{
			_grid = grid;
			_coord = coord;
			_newColor = color;
		}

		public void Do ()
		{
			_prevState = _grid.IsSet (_coord);
			_prevColor = _grid.GetColor (_coord);

			_grid.SetColor (_coord, _newColor);
		}

		public void Undo ()
		{
			if (_prevState) {
				_grid.SetColor (_coord, _prevColor);
			}
			else {
				_grid.Unset (_coord);
			}
		}
	}
}
