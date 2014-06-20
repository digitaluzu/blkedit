using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public abstract class UiPanelBaseView : Uzu.UiPanel
	{
		private const string BUTTON_ID_CLOSE = "Button-Close";

		/// <summary>
		/// The panel to be transitioned to when the close button is pressed.
		/// </summary>
		protected abstract string TransitionPanelIdOnClose {
			get;
		}
		
		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case BUTTON_ID_CLOSE:
				DoClose ();
				break;
			}
		}
		
		protected void DoClose ()
		{
			Main.PanelMgr.ChangeCurrentPanel (TransitionPanelIdOnClose);
		}

		protected static bool GetBlockInfoById (string id, List <BlkEdit.BlockInfo> infos, out BlkEdit.BlockInfo outInfo)
		{
			for (int i = 0; i < infos.Count; i++) {
				BlkEdit.BlockInfo info = infos [i];
				if (info.Id == id) {
					outInfo = info;
					return true;
				}
			}
			
			outInfo = new BlkEdit.BlockInfo ();
			return false;
		}
	}
}
