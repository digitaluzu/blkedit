using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelMain : Uzu.UiPanel
	{
		private const string MODE_BUTTON_ID = "ModeButton";

		[SerializeField]
		private UILabel _modeButton;

		public enum Mode {
			Add,
			Remove,
		};

		public Mode CurrentMode {
			get { return _currentMode; }
		}

		private Mode _currentMode = Mode.Add;

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
				case MODE_BUTTON_ID:
				{
					const string addText = "[Add] / Erase";
					const string eraseText = "Add / [Erase]";

					if (_modeButton.text == addText) {
						_currentMode = Mode.Remove;
						_modeButton.text = eraseText;
					}
					else {
						_currentMode = Mode.Add;
						_modeButton.text = addText;
					}
				}
				break;
			}
		}
		
		public override void OnActivate ()
		{
		}
		
		public override void OnDeactivate ()
		{
		}
			
		private void OnPressStart (Uzu.UiWidget widget)
		{
			Debug.Log(widget.name);
		}
		
		public override void OnUpdate ()
		{		
	#if UNITY_EDITOR
	
	#endif // UNITY_EDITOR
		}
	}
}