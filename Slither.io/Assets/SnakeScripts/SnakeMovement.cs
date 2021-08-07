using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEngine.Advertisements;

public class SnakeMovement : MonoBehaviour
{
	public SpriteRenderer character;
	bool energiget=false;
	bool camerascaled=false;
	public SphereCollider sphereCollider;
    public Vector3 lastposition;
    public float time = 0;
    public float rotacof;
	Vector2 oldemouse;
	Vector2 newdemouse;
    //	##### added by Yue Chen #####
	private int moveWay ;    // It determines how to control the movement of snake, gained from initial interface
	private int skinID ;     // It determines the skin of the snake, gained from initial interface
	private string nickName;

    public List<Transform> bodyParts = new List<Transform>();   // Records the location information of body parts of the snake
    public List<GameObject> Robots = new List<GameObject>();    // Records the information of Robots
    public float defaultwalk=7;
    public float snakeWalkSpeed = 7; // Called in SnakeMove()
    public float snakeRunSpeed = 11;  // Called in SnakeRun()
    private bool isRunning; // Called in SnakeRun()
    //public float runBodyPartSmoothTime = 0.1f; // // Called in SnakeRun()
   // private float cameraSmoothTime = 0.13f;  // Called in CameraFollowSnake()

	public float runBodyPartSmoothTime = 0.1f; // // Called in SnakeRun()
	private float cameraSmoothTime = 0.13f;  // Called in CameraFollowSnake()

    public Transform addBodyPart;   // Called in OnCollisionEnter(), it is the thing added behind the snake after eating food

    //Called in SizeUp(), determine after eating how much food the snake will grow its size
    private int[] sizeUpArray = {2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 5096, 10192, 20348, 999999};
    //Called in OnCollisionEnter(), determine after eating how many food points will the snake add a body part
    private int[] foodUpArray = { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 6, 10 };
    public int foodUpCounter=0;
    public int foodCounter;    // Called in OnCollisionEnter(), the number of food the snake eats so far
    public int curSizeLevel;    // Called in OnCollisionEnter()
    public Vector3 curSize = Vector3.one;  //Called in SizeUp()
    private float growRate = 0.1f;   //Called in OnCollisionEnter(), how much to grow snake size
    private float cameraGrowRate = 0.03f;    // Called in OnCollisionEnter(), when snake gets larger, camera size gets larger as well
    private float bodyPartSmoothTime = 0.2f; //Called in OnCollisionEnter(), the same value as in SnakeBodyActions.cs
    private float bodySmoothTime; // Called in SetBodySizeAndSmoothTime()

    //	##### added by Yue Chen #####
    public Text countText;
    public int length;
	
    // use this for initialization
    void Start() {
        GenerateFoodBeforeBegin();
        GenerateStoneBeforeBegin();
        GenerateRobotBeforeBegin();
		//	##### added by Yue Chen #####
		moveWay = PlayerPrefs.GetInt("moveWayID",1);    // It determines how to control the movement of snake, gained from initial interface
		// It determines the skin of the snake, gained from initial interface
		skinID = PlayerPrefs.GetInt("skinID",1);
		nickName = PlayerPrefs.GetString("nickname","");
    }
	
    // update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonDown(0))
		{
			//oldemouse;
		}
		if(Input.GetMouseButton(0))
		{
			
			//newdemouse;
		}
        ChooseControlMethod(moveWay);
		ColorSnake(skinID);
        GenerateFoodAndItem();
        SnakeRun();
		SetScore (length);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void FixedUpdate()
    {
        /*time += Time.deltaTime;
        if (time > bodyParts[0].GetComponent<SnakeBodyActions>().smoothTime)*/
        // If the body part is the first one, then it follows the head
        {
            lastposition = transform.position;
            // time = 0;
        }
        SnakeMove();
        SetBodySizeAndSmoothTime();
        CameraFollowSnake();
        SnakeGlowing(isRunning);
        SnakeMoveAdjust();
        //	##### added by Yue Chen #####
        countText.text = "G o o d  j o b  !  " + nickName + "\nY o u r  L e n g t h  :  " + length;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        foreach (Transform kl in bodyParts )
        {
            kl.GetComponent<SnakeBodyActions>().muUpdate();
        }
    }
        /* When the head encounters an object, figure out what to do*/
        void OnTriggerEnter(Collider obj)
    {
        if (obj.transform.tag == "Food")
        {
            length++;
            Destroy(obj.gameObject);
            curAmountOfFood--;
            if (foodUpCounter + 1 == foodUpArray[curSizeLevel])
            {
                if (SizeUp(foodCounter) == false)
                {
                    foodCounter++;
                    // The contents in 'if' shouldn't be exectued in logic as we always have several body parts
                    Vector3 currentPos;
                    if (bodyParts.Count == 0)
                    {
                        currentPos = transform.position;
                    }
                    else
                    {
                        currentPos = bodyParts[bodyParts.Count - 1].position;
                    }
                    Transform newPart = Instantiate(addBodyPart, currentPos, Quaternion.identity) as Transform;
                    newPart.parent = GameObject.Find("SnakeBodies").transform;
                    bodyParts.Add(newPart);
                }
                else
                {
                    curSize += Vector3.one * growRate;
                    bodyPartSmoothTime += 0.01f;
                    transform.localScale = curSize;
                    // Scale up camera
                    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                    camera.GetComponent<Camera>().orthographicSize += camera.GetComponent<Camera>().orthographicSize * cameraGrowRate;
                }
                foodUpCounter = 0;
            }
            else
            {
                foodUpCounter++;
            }
        }
        //	##### added by Morgan #####
        else if (obj.transform.tag == "Item")
        {
            //if (obj.transform.GetComponent<ParticleSystem>().startColor == new Color32(255, 0, 255, 255))
            if (obj.name.StartsWith("Energy"))            
			{
                Destroy(obj.gameObject);
                curAmountOfItem--;
                snakeWalkSpeed = snakeRunSpeed;
                StartCoroutine("speedUpTime");
            }
            /*if (obj.transform.GetComponent<ParticleSystem>().startColor == new Color32(0, 255, 0, 255))
			else if (obj.name.StartsWith("Poison"))
            {
                Destroy(obj.gameObject);
                curAmountOfItem--;
                if (bodyParts.Count > 4)
                { 
                    isRunning = true;
                    StartCoroutine("punishTime");
                }
            }*/
			else if (obj.name.StartsWith("magnitic"))
            {
                Destroy(obj.gameObject);
                sphereCollider.radius=2;
				sphereCollider.center=new Vector3(0,3,0);
                StartCoroutine("magnetic");
            }
            else if (obj.name.StartsWith("scalecamera"))
            {
				Destroy(obj.gameObject);                
                StartCoroutine("scalecamera");
            }                        
        }
        else if (obj.transform.tag == "Boundary")
        {
            while (bodyParts.Count > 0)
            {
                int lastIndex = bodyParts.Count - 1;
                Transform lastBodyPart = bodyParts[lastIndex].transform;
                bodyParts.RemoveAt(lastIndex);
                GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], lastBodyPart.position, Quaternion.identity) as GameObject;
                newFood.transform.parent = GameObject.Find("Foods").transform;
                Destroy(lastBodyPart.gameObject);
            }
            GameObject head = GameObject.FindGameObjectWithTag("Player");
            Destroy(head);
            //	##### added by Yue Chen #####
            if (PlayerPrefs.GetInt("removeAds", 0) == 0)
            {
                ShowAd();
            }
            SceneManager.LoadScene("Menu");

        }
        else if ((obj.transform.tag == "Body") || (obj.transform.tag == "Robot"))
        {
            while (bodyParts.Count > 0)
            {
                int lastIndex = bodyParts.Count - 1;
                Transform lastBodyPart = bodyParts[lastIndex].transform;
                bodyParts.RemoveAt(lastIndex);
                GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], lastBodyPart.position, Quaternion.identity) as GameObject;
                newFood.transform.parent = GameObject.Find("Foods").transform;
                Destroy(lastBodyPart.gameObject);
            }
            GameObject head = GameObject.FindGameObjectWithTag("Player");

            Destroy(head);
            //	##### added by Yue Chen #####

            if (PlayerPrefs.GetInt("removeAds", 0) == 0)
            {
                ShowAd();
            }
            SceneManager.LoadScene("Menu");
        }
        else if (obj.transform.tag == "Snake")
        {
            Vector3 temp = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1);
            gameObject.transform.position = temp;
        }
    }

    //	##### added by Morgan #####
    void speedUpTime()
    {
		energiget=true;
		CancelInvoke("antispeedUpTime");
        efect.setefect(0);
        Invoke( "antispeedUpTime",efect.defaulttime);

    }
	void antispeedUpTime()
    {
		energiget=false;
        snakeWalkSpeed = defaultwalk;
    }
    /*IEnumerator punishTime()
    {
        efect.setefect(1);        
        yield return new WaitForSeconds(efect.defaulttime);
        isRunning = false;
		snakeWalkSpeed = 3.5f;     
    }*/
	void magnetic()
    {
		CancelInvoke("antimagnetic");
        efect.setefect(1);        
        Invoke( "antimagnetic",efect.defaulttime);    
    }
	void antimagnetic()
    {
		sphereCollider.radius=1;
sphereCollider.center=new Vector3(0,1,0);		
    }
    IEnumerator scalecamera()
    {		
		CancelInvoke("invokantiscalecamera");
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
		efect.setefect(2);
		if(!camerascaled)		
        for (int i = 0; i < 20; i++)
        {      
			camerascaled=true;      
            camera.GetComponent<Camera>().orthographicSize += (0.05f*5);
            yield return new WaitForSeconds(0.05f);
        }		
		//yield return new WaitForSeconds(efect.defaulttime-1);
			Invoke( "invokantiscalecamera",efect.defaulttime-1);   	
    }
	void invokantiscalecamera()
	{
		StartCoroutine("antiscalecamera");
	}
    IEnumerator antiscalecamera()
    {		
		camerascaled=false;	
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        for (int i = 0; i < 20; i++)
        {
            camera.GetComponent<Camera>().orthographicSize -= (0.05f * 5);
			//if(!camerascaled)
            yield return new WaitForSeconds(0.05f);
        }
				
    }
    /* When losing body parts, snake size down*/

    void SnakeScaleChange()
	{
        if (curSizeLevel > 0 && foodCounter <= sizeUpArray[curSizeLevel - 1])
		{
            curSizeLevel--;
            curSize -= Vector3.one * growRate;
            bodyPartSmoothTime -= 0.01f;
            transform.localScale = curSize;
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<Camera>().orthographicSize -= camera.GetComponent<Camera>().orthographicSize * cameraGrowRate;
        }
    }

    /* Figure out whether snake size increases after eating*/
    bool SizeUp(int x)
    {
        if (x == sizeUpArray[curSizeLevel]) {
            curSizeLevel++;
            return true;
        }
        return false;
    }

    /* Set the size and smooth time of snake body parts every frame*/
    void SetBodySizeAndSmoothTime()
    {
        transform.localScale = curSize;
        if (snakeWalkSpeed >= snakeRunSpeed)
        {
            bodySmoothTime = runBodyPartSmoothTime;
        }
        else
        {
            bodySmoothTime = bodyPartSmoothTime;
        }
        foreach (Transform part in bodyParts)
        {
            part.localScale = curSize;
            part.GetComponent<SnakeBodyActions>().smoothTime = bodySmoothTime;
        }
    }

    public int curAmountOfRobot, maxAmountOfRobot = 30;  // The max amount of robots in the map
    public GameObject[] robotGenerateTarget;     // Store the objects of robot snakes

    // create robots
    public void GenerateRobotBeforeBegin(bool debut=true)
    {
        int i = 0;
		if(debut)
        while (i < 20)
        {
            int r = Random.Range(0, 2);
            Vector3 robotPos;
            if (r == 0)
            {
                robotPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else
            {
                robotPos = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0);
            }
            GameObject newRobot = Instantiate(robotGenerateTarget[Random.Range(0, robotGenerateTarget.Length)],
                robotPos, Quaternion.identity) as GameObject;
            newRobot.name = "Robot" + i;
            Robots.Add(newRobot);
            newRobot.GetComponent<RobotAction>().SkinId = Random.Range(1, 4);
			newRobot.GetComponent<RobotAction>().snakmouv=this;
            newRobot.transform.parent = GameObject.Find("Robots").transform;
            curAmountOfRobot++;
            i++;
        }
		else
		{
			int r = Random.Range(0, 2);
            Vector3 robotPos;
            if (r == 0)
            {
                robotPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else
            {
                robotPos = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0);
            }
            GameObject newRobot = Instantiate(robotGenerateTarget[Random.Range(0, robotGenerateTarget.Length)],
                robotPos, Quaternion.identity) as GameObject;
            newRobot.name = "Robot" + Robots.Count;
            Robots.Add(newRobot);
            newRobot.GetComponent<RobotAction>().SkinId = Random.Range(1, 4);
			newRobot.GetComponent<RobotAction>().snakmouv=this;
            newRobot.transform.parent = GameObject.Find("Robots").transform;
            curAmountOfRobot++;
		}
    }

    /* Gernate 200 food points before game start*/
    void GenerateFoodBeforeBegin()
    {
        int i = 0;
        while (i < 50)
        {
            int r = Random.Range(0, 2);
            Vector3 foodPos;
            //if (r == 0)
				if (false)
            {
                foodPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else
            {
                foodPos = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), 0);
            }
			int im=Random.Range(1, 4);
			for(int j=0;j<im;j++) 
            {GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], foodPos+0.6f*new Vector3(Mathf.Cos(6.24f*j/im),Mathf.Sin(6.24f*j/im),0), Quaternion.identity) as GameObject;
			newFood.transform.parent = GameObject.Find("Foods").transform;
            curAmountOfFood++;
		i++;}
        }
    }

    void GenerateStoneBeforeBegin()
    {
        /*int i = 0;
        while (i < 400)
        {
            Vector3 stonePos = new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), 0);
            GameObject newStone = Instantiate(stone, stonePos, Quaternion.identity) as GameObject;
            newStone.transform.parent = GameObject.Find("Stones").transform;
            curAmountOfFood++;
            i++;
        }*/
    }

    /* Generate food points every few seconds until there are enough points on the map*/
    public int curAmountOfFood, maxAmountOfFood = 600;  // The max amount of food in the map
    public int curAmountOfItem, maxAmountOfItem = 60;  // The max amount of item in the map
    public float foodGenerateEveryXSecond = 2;   // Generate a food point every 3 seconds
    public GameObject[] foodGenerateTarget;     // Store the objects of food points
    public GameObject[] itemGenerateTarget;     // Store the objects of item
    public GameObject stone; // the object of stone
    void GenerateFoodAndItem()
    {
        StartCoroutine("RunGenerateFoodAndItem", foodGenerateEveryXSecond);
    }
    IEnumerator RunGenerateFoodAndItem(float time)
    {
        yield return new WaitForSeconds(time);
        StopCoroutine("RunGenerateFoodAndItem");
		int k=Random.Range(0, 20);
        if (k>2 && curAmountOfFood < maxAmountOfFood)
        {
            int r = Random.Range(0, 4);
            Vector3 foodPos;
            if (r == 0)
            {
                foodPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else if (r <= 1)
            {
                foodPos = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), 0);
            }
            else if (r <= 2)
            {
                foodPos = new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), 0);
            }
            else
            {
                foodPos = new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), 0);
            }
			int im=Random.Range(1, 4);
			for(int j=0;j<im;j++) 
            {GameObject newFood = Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], foodPos+0.6f*new Vector3(Mathf.Cos(6.24f*j/im),Mathf.Sin(6.24f*j/im),0), Quaternion.identity) as GameObject;
            newFood.transform.parent = GameObject.Find("Foods").transform;
		curAmountOfFood++;}
        }
        else if (k<=2 && curAmountOfItem < maxAmountOfItem)
        {
            int r = Random.Range(0, 4);
            Vector3 itemPos;
            if (r == 0)
            {
                itemPos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);
            }
            else if (r <= 1)
            {
                itemPos = new Vector3(Random.Range(-60, 60), Random.Range(-60, 60), 0);
            }
            else if (r <= 2)
            {
                itemPos = new Vector3(Random.Range(-90, 90), Random.Range(-90, 90), 0);
            }
            else
            {
                itemPos = new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), 0);
            }
            GameObject newItem = Instantiate(itemGenerateTarget[Random.Range(0, itemGenerateTarget.Length)], itemPos, Quaternion.identity) as GameObject;
            newItem.transform.parent = GameObject.Find("Items").transform;
            curAmountOfItem++;
        }
    }

    /* Make the snake head move forward all the time*/
    void SnakeMove()
    {
        transform.position += transform.forward * snakeWalkSpeed * Time.deltaTime;
    }
    /* Make the camera follow the snake when it moves*/
    void CameraFollowSnake()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 velocity = Vector3.zero;
        // Reach from current position to target position smoothly
        camera.position = Vector3.SmoothDamp(camera.position,
            new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10)
            , ref velocity, cameraSmoothTime);
    }

    /* Make the snake run when it should run, and lose parts*/
    private float t1;
    private float t2;
    void SnakeRun() {
        if (bodyParts.Count > 5)
        {
            if (Input.GetMouseButtonDown(0))
            {
                t2 = Time.realtimeSinceStartup;
                if (t2 - t1 < 0.2)
                {
                    isRunning = true;
                    snakeWalkSpeed = snakeRunSpeed;
                }
                t1 = t2;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isRunning = false;
				if(!energiget)
                snakeWalkSpeed = defaultwalk;
			
            }
        }
        else {
            isRunning = false;         
			snakeWalkSpeed = defaultwalk;
        }
        if (isRunning == true)
        {
            StartCoroutine("LosingBodyParts");
        }
        
    }
    IEnumerator LosingBodyParts() {
        yield return new WaitForSeconds(0.8f);  // Every 0.8 second lose one body part
        StopCoroutine("LosingBodyParts");
        int lastIndex = bodyParts.Count - 1;
        Transform lastBodyPart = bodyParts[lastIndex].transform;
        bodyParts.RemoveAt(lastIndex);
        Instantiate(foodGenerateTarget[Random.Range(0, foodGenerateTarget.Length)], lastBodyPart.position, Quaternion.identity);
        Destroy(lastBodyPart.gameObject);
        curAmountOfFood++;
        foodCounter--;
		length--;
        SnakeScaleChange();
    }
    /* If snake is running, then glowing*/
    void SnakeGlowing(bool isRunning) {
        foreach (Transform part in bodyParts) {
            part.Find("Glowing").gameObject.SetActive(isRunning);
        }
    }

    /* Choose the way of snake moving*/
    void ChooseControlMethod(int id)
    {
        switch (id) {
            case 1: MouseControlSnake();
                break;
            case 2: StickControl();
                break;
            case 3: DirectionControl();
                break;
        }
    }
    /* Sanke moves toward finger*/
    private Vector3 pointInWorld, mousePosition, direction;
    private float radius = 20.0f;
    void MouseControlSnake()
    {
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Store the first obj touched by ray
        Physics.Raycast(ray, out hit, 50.0f); // The third parameter is the max distance
        mousePosition = new Vector3(hit.point.x, hit.point.y, 0)- transform.position;
		if(mousePosition.magnitude>1)
        {
			direction = Vector3.Slerp(direction, mousePosition, Time.deltaTime * rotacof);
			direction.z = 0;
			pointInWorld = direction.normalized * radius + transform.position;
			pointInWorld.z=0;
			transform.LookAt(pointInWorld);
		}
    }
    /* Use virtual joystick to control the direction of snake*/
    void StickControl() { }
    /* Slide finger to control the direction of snake*/
    void DirectionControl() { }

    void SnakeMoveAdjust()
    {
        Vector3 temp = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
        gameObject.transform.position = temp;
    }

    /* Choose the skin of snake*/
    public Material blue, red, orange;
    void ColorSnake(int id) 
	{
        switch (id%3)
        {
            case 1: BlueAndWhite(); break;
            case 2: RedAndWhite(); break;
            case 3: OrangeAndWhite();break;
        }
		character.sprite= Resources.Load<Sprite>("c"+id.ToString());
    }
    void BlueAndWhite()
    {
        for (int i = 0; i < bodyParts.Count; i++) {
            if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = blue;
				bodyParts[i].GetComponent<SnakeBodyActions>().sr. color=Color.blue;
            }
        }
    }
    void RedAndWhite()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = red;
				bodyParts[i].GetComponent<SnakeBodyActions>().sr. color=Color.red;
            }
        }
    }
    void OrangeAndWhite()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (i % 2 == 0)
            {
                bodyParts[i].GetComponent<Renderer>().material = orange;
				bodyParts[i].GetComponent<SnakeBodyActions>().sr. color=Color.green;
            }
        }
    }

	// added by Yue Chen
	public void ShowAd()
	{
//		if (Advertisement.IsReady ()) {
//			Advertisement.Show ();
//		} else {
//			Advertisement.Initialize ("1165124");
//		}


		/*if (!Advertisement.IsReady())
		{
		Debug.Log("Ads not ready for default placement");
		return;
		}*/

		//Advertisement.Show();
		GoogleMobileAdsDemoScript.mobile_script.ShowInterstitial();

	}

	public void SetScore(int curScore){
		int bestScore;
		PlayerPrefs.SetInt ("FinalScore",curScore );
		bestScore = PlayerPrefs.GetInt ("BestScore", 0);
		if (bestScore < curScore) {
			PlayerPrefs.SetInt ("BestScore", curScore);
		}
	}


}
