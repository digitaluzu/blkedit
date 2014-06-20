using UnityEngine;
using System.Collections;

namespace Blk
{
	public class DialogBox : Uzu.BaseBehaviour
	{
		[SerializeField]
		private UILabel _bodyLabel;
		[SerializeField]
		private UILabel _yesLabel;
		[SerializeField]
		private UILabel _noLabel;

		private System.Action <string> _callback;
		private string _pressedButtonText;
		
		public bool IsVisible {
			get { return gameObject.activeSelf; }
		}
		
		public void Show (string body, string yes, string no, System.Action <string> callback)
		{
			// Call at beginning to make sure Awake is triggered first.
			gameObject.SetActive (true);
			
			_bodyLabel.text = body;
			_yesLabel.text = yes;
			
			// For 1-button objects.
			if (_noLabel != null) {
				_noLabel.text = no;
			}
			
			_callback = callback;
			_pressedButtonText = string.Empty;
		}
		
		private void OnButtonPress (GameObject go, bool isPressed)
		{
			if (isPressed) {
				_pressedButtonText = go.GetComponentInChildren <UILabel> ().text;
				
				gameObject.SetActive (false);

				// Trigger callback.
				if (_callback != null) {
					_callback (_pressedButtonText);
				}
			}
		}
		
		protected override void Awake ()
		{
			base.Awake ();
			
			{
				UIEventListener.Get (_yesLabel.cachedTransform.parent.gameObject).onPress = OnButtonPress;
				
				// For 1-button objects.
				if (_noLabel != null) {
					UIEventListener.Get (_noLabel.cachedTransform.parent.gameObject).onPress = OnButtonPress;
				}
			}
		}
	}
}
