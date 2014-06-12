using UnityEngine;
using System.Collections;

namespace BlkEdit
{
	public interface GridInterface
	{
		bool IsSet (Uzu.VectorI2 coord);
		void Unset (Uzu.VectorI2 coord);

		Color32 GetColor (Uzu.VectorI2 coord);
		void SetColor (Uzu.VectorI2 coord, Color32 color);
	}
}