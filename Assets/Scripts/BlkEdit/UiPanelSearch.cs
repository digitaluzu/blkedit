using UnityEngine;
using System.Collections;

namespace Blk
{
	public class UiPanelSearch : Uzu.UiPanel
	{
		private const string BUTTON_ID_CLOSE = "Button-Close";
		private const string BUTTON_ID_SEARCH_BY_NAME = "Button-Search-ByName";
		private const string BUTTON_ID_SEARCH_MOST_RECENT = "Button-Search-MostRecent";

		public override void OnClick (Uzu.UiWidget widget)
		{
			switch (widget.name)
			{
			case BUTTON_ID_CLOSE:
				DoClose ();
				break;
			}
		}

		private void DoClose ()
		{
			Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_OPTIONS);
		}
	}
}