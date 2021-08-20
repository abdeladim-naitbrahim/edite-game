using UnityEngine;
using UnityEngine.Advertisements;

public class OpenAds : MonoBehaviour
{
	public void ShowAd()
	{
		GoogleMobileAdsDemoScript.mobile_script.ShowInterstitialAd();
	}
}

