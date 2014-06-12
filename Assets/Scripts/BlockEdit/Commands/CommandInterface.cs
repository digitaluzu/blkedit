using UnityEngine;
using System.Collections;

namespace BlkEdit
{
	public interface CommandInterface
	{
		/// <summary>
		/// Execute the command.
		/// </summary>
		void Do ();

		/// <summary>
		/// Undo the command.
		/// </summary>
		void Undo ();
	}
}
