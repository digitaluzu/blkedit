using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class UiPanelSavedDataView : UiPanelBaseView
	{
		protected override string TransitionPanelIdOnClose {
			get { return PanelIds.PANEL_OPTIONS; }
		}
		
		private List <BlkEdit.BlockInfo> _infos;
		
		public override void OnActivate ()
		{
			Main.ScrollViewController.AttachToPanelAndShow (this);
			Main.ScrollViewController.WindowTitleText = "Saved Models";
			Main.ScrollViewController.NoDataText = "No Models";
			Main.ScrollViewController.DisabledEntryId = Main.WorkspaceController.ActiveBlockInfoId;
			Main.ScrollViewController.EntryButtonText = "View";
			Main.ScrollViewController.OnTableEntryButtonClicked += OnTableEntryButtonClicked;
			
			_infos = WorkspaceController.GetSavedBlockInfos ();
			Main.ScrollViewController.AppendEntries (_infos);
		}
		
		public override void OnDeactivate ()
		{
			Main.ScrollViewController.OnTableEntryButtonClicked -= OnTableEntryButtonClicked;
		}
		
		private void OnTableEntryButtonClicked (string id, string buttonText)
		{			
			BlkEdit.BlockInfo info;
			if (GetBlockInfoById (id, _infos, out info)) {
				Main.WorkspaceController.LoadForViewing (info);
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
			}
		}
	}
}
