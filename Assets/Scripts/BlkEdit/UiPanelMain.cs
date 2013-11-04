using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelMain : Uzu.UiPanel
	{
		public override void OnPress (Uzu.UiWidget widget, bool isPressed)
		{
			if (isPressed) {
				OnPressStart (widget);
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