using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

public class MosqueTrigger : MonoBehaviour
{
   /// <summary>
   /// Simply causes npcs to stop having physics collisions. 
   /// </summary>
   /// <param name="other"></param>
   private void OnTriggerEnter(Collider other)
   {
      Movement npc = other.gameObject.GetComponent<Movement>();
      if (npc)
      {
         npc.DisableAiCollision();
      }
   }
}
