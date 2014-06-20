using UnityEngine;
using System.Collections.Generic;

namespace Blk
{
	public class UiPanelMyDataView : UiPanelBaseView
	{
		protected override string TransitionPanelIdOnClose {
			get { return PanelIds.PANEL_OPTIONS; }
		}

		private List <BlkEdit.BlockInfo> _infos;

		public override void OnActivate ()
		{
			Main.ScrollViewController.AttachToPanelAndShow (this);
			Main.ScrollViewController.WindowTitleText = "My Models";
			Main.ScrollViewController.NoDataText = "No Models";
			Main.ScrollViewController.DisabledEntryId = Main.WorkspaceController.ActiveBlockInfoId;
			Main.ScrollViewController.EntryButtonText = "Edit";
			Main.ScrollViewController.OnTableEntryButtonClicked += OnTableEntryButtonClicked;

			_infos = WorkspaceController.GetMyBlockInfos ();
			Main.ScrollViewController.AppendEntries (_infos);
		}

		public override void OnDeactivate ()
		{
			Main.ScrollViewController.OnTableEntryButtonClicked -= OnTableEntryButtonClicked;
		}

		private void OnTableEntryButtonClicked (string id, string buttonText)
		{
			// TODO: saving?
//			if (Main.WorkspaceController.NeedsSave) {
//				Main.DialogBox2.Show ("Save changes?", "Yes", "No",
//				                      (buttonText) => {
//					if (buttonText == "Yes") {
//						DoSave ();
//					}
//					
//					BlkEdit.BlockInfo info;
//					if (GetInfo (id, out info)) {
//						Main.WorkspaceController.LoadForEditing (info);
//						DoClose ();
//					}
//				});
//				return;
//			}

			BlkEdit.BlockInfo info;
			if (GetBlockInfoById (id, _infos, out info)) {
				Main.WorkspaceController.LoadForEditing (info);
				Main.PanelMgr.ChangeCurrentPanel (PanelIds.PANEL_CANVAS);
			}
		}
	}
}
