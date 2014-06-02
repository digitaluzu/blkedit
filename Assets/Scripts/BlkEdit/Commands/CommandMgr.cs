using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	/// <summary>
	/// Manages execution of commands.
	/// </summary>
	public class CommandMgr
	{
		public bool HasCommandsToUndo {
			get { return _undoCommands.Count != 0; }
		}

		public bool HasCommandsToRedo {
			get { return _redoCommands.Count != 0; }
		}

		/// <summary>
		/// Execute a command.
		/// </summary>
		public void DoCommand (CommandInterface cmd)
		{
			_undoCommands.Push (cmd);

			cmd.Do ();
		}

		/// <summary>
		/// Undo the last executed command.
		/// </summary>
		public void UndoCommand ()
		{
			if (_undoCommands.Count > 0) {
				CommandInterface cmd = _undoCommands.Pop ();
				_redoCommands.Push (cmd);

				cmd.Do ();
			}
		}

		/// <summary>
		/// Re-execute the next command.
		/// </summary>
		public void RedoCommand ()
		{
			if (_redoCommands.Count > 0) {
				CommandInterface cmd = _redoCommands.Pop ();
				_undoCommands.Push (cmd);

				cmd.Undo ();
			}
		}

		#region Implementation.
		private Stack <CommandInterface> _undoCommands = new Stack<CommandInterface> ();
		private Stack <CommandInterface> _redoCommands = new Stack<CommandInterface> ();
		#endregion
	}
}
