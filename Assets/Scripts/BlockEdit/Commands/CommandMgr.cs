using UnityEngine;
using System.Collections.Generic;

namespace BlkEdit
{
	/// <summary>
	/// Manages execution of commands.
	/// </summary>
	public class CommandMgr
	{
		public delegate void OnCommandExecutedDelegate (BlkEdit.CommandInterface cmd);
		public event OnCommandExecutedDelegate OnCommandExecuted;

		public bool CanUndo {
			get {
				return
					_undoCommands.Count != 0 &&
					!IsExecutingGroup();
			}
		}

		public bool CanRedo {
			get {
				return
					_redoCommands.Count != 0 &&
					!IsExecutingGroup ();
			}
		}

		public void ClearCommandHistory ()
		{
			_undoCommands.Clear ();
			_redoCommands.Clear ();
		}

		/// <summary>
		/// Execute a command.
		/// </summary>
		public void DoCommand (CommandInterface cmd)
		{
			// New command was executed, so we must clear the
			// redo buffer since we aren't supporting any type of
			// undo tree or branching.
			_redoCommands.Clear ();

			// Add to existing group?
			if (IsExecutingGroup ()) {
				GroupCommand topCommand = (GroupCommand)_undoCommands.Peek ();
				topCommand.AddChild (cmd);
			}
			else {
				_undoCommands.Push (cmd);
			}

			TriggerCallback (cmd);
			cmd.Do ();
		}

		/// <summary>
		/// Undo the last executed command.
		/// </summary>
		public void UndoCommand ()
		{
			if (CanUndo) {
				CommandInterface cmd = _undoCommands.Pop ();
				_redoCommands.Push (cmd);

				TriggerCallback (cmd);
				cmd.Undo ();
			}
		}

		/// <summary>
		/// Re-execute the next command.
		/// </summary>
		public void RedoCommand ()
		{
			if (CanRedo) {
				CommandInterface cmd = _redoCommands.Pop ();
				_undoCommands.Push (cmd);

				TriggerCallback (cmd);
				cmd.Do ();
			}
		}

		public void BeginGroupCommand ()
		{
			if (IsExecutingGroup ()) {
				Debug.LogWarning ("Already executing a group command.");
				return;
			}

			GroupCommand cmd = new GroupCommand ();
			cmd.IsActive = true;

			_undoCommands.Push (cmd);
		}

		public void EndGroupCommand ()
		{
			if (!IsExecutingGroup ()) {
				Debug.LogWarning ("No group command has been started.");
				return;
			}

			GroupCommand cmd = (GroupCommand)_undoCommands.Peek ();

			// Remove empty command groups.
			if (cmd.ChildCommandCount == 0) {
				_undoCommands.Pop ();
			}
			else {
				cmd.IsActive = false;
			}
		}

		#region Implementation.
		private Stack <CommandInterface> _undoCommands = new Stack<CommandInterface> ();
		private Stack <CommandInterface> _redoCommands = new Stack<CommandInterface> ();

		private bool IsExecutingGroup ()
		{
			if (_undoCommands.Count > 0) {
				GroupCommand topCmd = _undoCommands.Peek () as GroupCommand;
				if (topCmd != null &&
				    topCmd.IsActive) {
					return true;
				}
			}

			return false;
		}

		private void TriggerCallback (CommandInterface cmd)
		{
			if (OnCommandExecuted != null) {
				OnCommandExecuted (cmd);
			}
		}

		private class GroupCommand : CommandInterface
		{
			public bool IsActive {
				get { return _isActive; }
				set { _isActive = value; }
			}

			public int ChildCommandCount {
				get { return _childCmds.Count; }
			}

			private bool _isActive;
			private List <CommandInterface> _childCmds = new List<CommandInterface> ();

			public void AddChild (CommandInterface cmd)
			{
				_childCmds.Add (cmd);
			}

			public void Do ()
			{
				for (int i = 0; i < _childCmds.Count; i++) {
					_childCmds [i].Do ();
				}
			}
			
			public void Undo ()
			{
				for (int i = _childCmds.Count - 1; i >= 0; i--) {
					_childCmds [i].Undo ();
				}
			}
		}
		#endregion
	}
}
