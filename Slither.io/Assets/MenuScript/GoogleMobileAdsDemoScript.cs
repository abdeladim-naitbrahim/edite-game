using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class GoogleMobileAdsDemoScript : MonoBehaviour
{
	public static GoogleMobileAdsDemoScript mobile_script;
    public static int intermax=0;
    public static int internum=0;
    public GameObject gdprpanel;
    string gdpr = "0";
    public static bool stargdpr = false;
    public string privacilink;
    public bool reward_load_editor = true;
    public GameObject rewardpanel;
    public static bool intersetialshowed = false;
    public static int rewarddednum = 0;
    public static string IdAdmob;
    public static string RewardAdmob;
	public static string BannerAdmob;
	public static string IntertisialdAdmob;
    public Animation conexiontext;
    public static bool isconcted=true;
    public static int conexion_int = 0;
    public static bool reward_loaded = false;
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private float deltaTime = 0.0f;
    private static string outputMessage = string.Empty;   
    public static bool earned=false;
    public static bool closrewardflag = false;
    public static float RequestTime=1;
    public static bool rewardload = false;
    public static string OutputMessage
    {
        set { outputMessage = value; }
    }
    void Awake()
    {        
        GameObject[] objs = GameObject.FindGameObjectsWithTag("myPlugins");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
		DontDestroyOnLoad(this.gameObject);
		mobile_script=this;
    }
    public void Start()
    {



        // Gets called when the player opens the notification.



#if UNITY_ANDROID
        string appId = IdAdmob;
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#else
        string appId = "unexpected_platform";
#endif

        MobileAds.SetiOSAppPauseOnBackground(true);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
        gdpr = PlayerPrefs.GetString("gdpr", "1");
        if (gdpr == "1")
        {
            gdprpanel.active = true;
        }
        else
            debut();
    }
	private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
           
        });
    }
    public void agree()
    {
        gdpr = "0";
        PlayerPrefs.SetString("gdpr", "0");
        notnow();
    }
    public void notnow()
    {
        gdprpanel.active = false;
        debut();
    }
    // debut();
    public void privaci()
    {
        Application.OpenURL(privacilink);
    }
    void strg()
    {
        stargdpr = true;
    }
    public void Update()
    {
        // Calculate simple moving average for time to render screen. 0.1 factor used as smoothing
        // value.
        //reward_loaded = this.rewardedAd.IsLoaded();

        /* if (!rewardload && this.rewardedAd.IsLoaded() || !rewardload && this.rewardedAd.IsLoaded())
             Androidtoole.rewardload(this.rewardedAd.IsLoaded());
             rewardload = this.rewardedAd.IsLoaded();*/
        //if (rewardload)
        // {
        try
        { if (this.rewardedAd == null)
            {}//Androidtoole.rewardload(false);
        else
		{}
            //Androidtoole.rewardload(this.rewardedAd.IsLoaded());
        }
        catch(Exception e)
            {
            //Androidtoole.rewardload(false);
        }
           // rewardload = false; }

        if (closrewardflag)
        {
            closrewardflag = false;
            if (earned)
            {
                earned = false;
                //Androidtoole.rewardrewarded(rewarddednum);
            }
            else
            {
                earned = false;
                //Androidtoole.rewardnorewarded(rewarddednum);
            }
        }
        this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
        if (conexion_int >= 10)
        {
           /* if(GameObject.Find("myPlugins").GetComponent<Canvas>.enabled)
                conexiontext.gameObject.active=false;*/
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                isconcted = false;
                //Debug.Log("Error. Check internet connection!");
            }
            else
            {
                isconcted = true;
            }
        }
        else
            conexion_int = conexion_int+1;
    }

    public void debut()
    {
        this.CreateAndLoadRewardedAd();
        this.RequestInterstitial();
    }
    // Returns an ad request with custom ad targeting.
    public AdRequest CreateAdRequest(){
    
        return new AdRequest.Builder()            
            .AddKeyword("game")
            .AddExtra("npa", gdpr)
            .Build();
    }

    public void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = BannerAdmob;
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Register for ad events.
        this.bannerView.OnAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        this.bannerView.OnAdOpening += this.HandleAdOpened;
        this.bannerView.OnAdClosed += this.HandleAdClosed;
        //this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

        // Load a banner ad.
        this.bannerView.LoadAd(this.CreateAdRequest());
    }

    public void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = IntertisialdAdmob;
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
       // this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }

    public void destroybaner()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }

    public void CreateAndLoadRewardedAd()
    {
        bool rewardnonul = !(this.rewardedAd == null);

        if (rewardnonul)
            rewardnonul = rewardnonul & (this.rewardedAd.IsLoaded());
        if (!rewardnonul)
        {

           
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = RewardAdmob;
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif
        // Create new rewarded ad instance.
        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = this.CreateAdRequest();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }
    else
            MonoBehaviour.print("reward ads is arleady loaded");
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            if (internum >= intermax)
            {
                internum = 0;
                this.interstitial.Show();
				Debug.Log("interstitial.Show()");
            }
            else
            {
                internum = internum + 1;
            }
            
        }
        else

        { 
        MonoBehaviour.print("Interstitial is not ready yet");
           
        }
    }


    public void ShowRewardedAd()
    {
        if (!isconcted)
        {

            MonoBehaviour.print("you must conect internet to continue");
            MonoBehaviour.print("Rewarded ad is not ready yet");
            conexiontext.gameObject.active = true;
            conexiontext.Rewind();
            conexiontext.Play();
        }
        else if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
#if UNITY_EDITOR
            if (reward_load_editor)
            {
                Time.timeScale = 0;
                rewardpanel.active = true;
            }
#endif
        }

        else
        {
           
            //Androidtoole.rewardnoconexion(rewarddednum);
        }
    }

    public void rewardbutton()
    {
        earned = true;
    }
    public void closebutton()
    {
        closrewardflag = true;
        Time.timeScale = 1;
        rewardpanel.active = false;
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
       // MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
        RequestBanner();
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
       // MonoBehaviour.print(
         //   "HandleInterstitialFailedToLoad event received with message: " + args.Message);
        RequestInterstitial();
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialOpened event received");
        
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialClosed event received");
        
        RequestInterstitial();
        intersetialshowed = true;
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLeftApplication event received");
    }

    #endregion

    #region RewardedAd callback handlers

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        rewardload = true;
    }

    public void HandleRewardedAdFailedToLoad(object sender, EventArgs args)
    {
       // MonoBehaviour.print(
     //       "HandleRewardedAdFailedToLoad event received with message: " + args.Message);
        CreateAndLoadRewardedAd();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: " + args.Message);       
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        CreateAndLoadRewardedAd();
        closrewardflag = true;
        rewardload = false;



    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
     earned = true;
        
       
    string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }

    #endregion
}
