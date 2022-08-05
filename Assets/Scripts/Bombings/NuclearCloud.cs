using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearCloud : MonoBehaviour 
{
    DebugTime debugTime;
    PostProcessor pp;
    GameObject player;
    MoveTowards mover;
    Transform consumptionPoint;
    public float consumeTime = 100f;
    
	void Awake () {
        debugTime = FindObjectOfType<DebugTime>();
        pp = FindObjectOfType<PostProcessor>();
        player = GameObject.FindGameObjectWithTag("Player");
        mover = player.GetComponent<MoveTowards>();
        consumptionPoint = transform.GetChild(1);
	}

    private void OnTriggerStay(Collider other)
    {
        //towards the end
        if(debugTime.gameTime > consumeTime)
        {
            //consume player if nobody else has
            if(other.gameObject.CompareTag("Player"))
            {
                if(mover.moving == false) // this is sometimes null, maybe bc of how player is referenced
                {
                    ConsumePlayer();
                }
            }
        }
    }

    public void ConsumePlayer()
    {
        mover.MoveTo(consumptionPoint.position, mover.moveSpeed);
    }
}
