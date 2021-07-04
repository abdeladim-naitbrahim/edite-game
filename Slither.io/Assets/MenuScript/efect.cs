using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class efect : MonoBehaviour 
{
    public string[] names;
    public static efect instance;
	public Image image;
	public RectTransform bare;
	public static float size;
    public static float time;
    public const  float defaulttime= 20;
    private void FixedUpdate()
    {
        setsize(bare,size* time/defaulttime);
        time -= Time.deltaTime;
        if (time < 0)
		{time = 0;
		endefect();
		}

    }

    void Awake()
    {
       // names = new string[] { };
	   
	   //if(!instance)
		   Debug.Log("go");
        instance =this;
		size=bare.sizeDelta.x;
		Debug.Log(instance.names[0]);
		gameObject.active=false;
	}
	public void setefect(int index)
	{
		time=defaulttime;
        gameObject.active = true;
        setsize(bare, size);
        image.sprite = Resources.Load<Sprite>(names[index]);
	}
    public void setsize(RectTransform bar,float y)
    {
        bar.sizeDelta = new Vector2(y, bar.sizeDelta.y);
    }
    public void endefect()
    {
        gameObject.active = false;
    }

    }