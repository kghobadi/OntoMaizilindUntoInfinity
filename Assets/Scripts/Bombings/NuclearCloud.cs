using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearCloud : MonoBehaviour {
    DebugTime debugTime;
    PostProcessor pp;
    GameObject player;
    MoveTowards mover;
    Transform consumptionPoint;
    
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
        if(debugTime.gameTime > 150f)
        {
            //consume player if nobody else has
            if(other.gameObject.tag == "Player")
            {
                if(mover.moving == false)
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
