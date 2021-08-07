using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class efect : MonoBehaviour 
{
    public string[] names;
	public int panelIndex;
	public int valuelIndex;
    public static List<efect> instance;
	public Image image;
	public RectTransform bare;
	public  float size;
    public  float time;
    public const  float defaulttime= 20;
    private void FixedUpdate()
    {
        setsize(bare,size* time/defaulttime);
        time -= Time.deltaTime;
        if (time < 0)
		{
			time = 0;
			endefect();
		}

    }
	public static efect getefect(int i)
	{
		int j=instance.Count-1;
		
		for(int k=0;k<instance.Count-1;k++ )
		{
			efect x = instance[k];
			if(x.gameObject.active && x.valuelIndex==i)
				return instance[k];
			else if(!x.gameObject.active && j==instance.Count-1)
				j=k;
		}
		return instance[j];
	}
    void Awake()
    {
       // names = new string[] { };
	   
	   if(instance==null)
		   instance=new List<efect>();
	   else if(instance.Count>=3)
		   instance.Clear();
	    panelIndex=instance.Count;
        instance.Add(this);
		size=bare.sizeDelta.x;
		//Debug.Log(instance.names[0]);
		gameObject.active=false;
	}
	public static void setefect(int index)
	{
		getefect(index).setefectfinal(index);
	}
	public void setefectfinal(int index)
	{
		valuelIndex=index;
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
		panelIndex=-1;
    }

    }