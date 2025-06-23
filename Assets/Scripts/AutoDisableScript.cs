using UnityEngine;

public class AutoDisableScript : MonoBehaviour
{
    public float TimeOut;
    private float remainingTime;

	void OnEnable ()
	{
	    remainingTime = TimeOut;
	}
	
	void Update ()
	{
	    if (remainingTime < 0) gameObject.SetActive(false);
	    else remainingTime -= Time.deltaTime;
	}
}
