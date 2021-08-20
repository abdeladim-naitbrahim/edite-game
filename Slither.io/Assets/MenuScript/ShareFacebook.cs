using UnityEngine;
using System.Collections;
using Facebook.Unity;
using UnityEngine.UI;


public class ShareFacebook : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void Awake(){
	
		if (!FB.IsInitialized) {
		
			FB.Init();
		} else {
		
			FB.ActivateApp();
		
		}


	}

//	public void logIn(){
//		{
//			FB.LogInWithReadPermissions ();
//		}
//	}
//
//	private void OnLogIn(ILoginResult result){
//	
//		if (FB.IsLoggedIn) {
//		
//			AccessToken token = AccessToken.CurrentAccessToken;
//			userIdText.text = token.UserId;
//		} else {
//
//			Debug.Log ("Canceled Login");
//		
//		}
//	}

	public void Share(){

		/*FB.ShareLink (
			contentTitle:"Slither from Team CCMP",
			contentDescription:"Hey,join this game and play with me!",
			callback: OnShare);	*/
			string url="https://play.google.com/store/apps/details?id=air.com.hypah.io.slither";
			 Application.OpenURL("https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dair.com.hypah.io.slither");
			 //+" target="_blank"
	}

	private void OnShare(IShareResult result){
				
		if (result.Cancelled || !string.IsNullOrEmpty (result.Error)) {
		
			Debug.Log ("Share score error" + result.Error);
		} else if (!string.IsNullOrEmpty(result.PostId)){
		
			Debug.Log (result.PostId);
		} else
			Debug.Log("Share succeed!");
	}

}
