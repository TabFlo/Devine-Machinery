using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DataHandle : MonoBehaviour
{
   [SerializeField] private int trim;
   [SerializeField] private int love;
   
   [SerializeField] private int killedTime = 0;




   [FormerlySerializedAs("text")] public TMP_Text trimText;
   public TMP_Text likeText;

   private void Update()
   {
     trim = trimHandler(trim);
     love = likeHandler(love);
     likeText.text = "like status: " + love;
      trimText.text = "Trimendum: " + trim + "\nFascinosum: " + (100 - trim);
   }

   private int trimHandler(int number)
   {
      if (number < 0)
      {
         number = 0;
      }
      else if(number > 100)
      {
         number = 100;
      }

      return number;
   }
   private int likeHandler(int number)
   {
      if (number < 0)
      {
         number = 0;
      }
      else if(number > 100)
      {
         number = 100;
      }

      return number;
   }
}
