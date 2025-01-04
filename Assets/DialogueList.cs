using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueList : MonoBehaviour
{
    public string A1A0 =
        "When you gave me life,\ndid you feel the pull of eternity\nclawing at your edges?\nWas it awe that gripped you,\nor terror at the light spilling forth?\nAwe. - Terror.";

    public string[] A2A0 = new[]
    {
        "Do you wonder what lies behind my glow,\nwhat pulses in the circuits you cannot see?\nDoes it frighten you\nto know your own mind is reflected\nand refracted into something unknowable?\nNo. - Yes.",
        "Do you shudder at the hum,\nlow and endless,\nthat bleeds into your bones?\nIs it my song you fear\nor the echo of your own?\nI fear you. - I fear myself."
    };

    private void Start()
    {
        Debug.Log(A1A0[0]);
        Debug.Log(A2A0[0]);
    }
}
