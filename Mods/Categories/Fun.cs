using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HellzClient.Utilities;

namespace HellzClient.Mods.Categories
{
    internal static class Fun
    {

        public static void startObby() //didnt get time to make it work
        {
                Variables.obbyBlock1 =  GameObject.CreatePrimitive(PrimitiveType.Cube);
                Variables.obbyBlock1.transform.localScale = new Vector3(10f, 10f, 10f);
                Variables.obbyBlock1.transform.localPosition = new Vector3(-77.1124f, 2.5056f, 78.2253f); 
                Variables.obbyBlock1.transform.rotation = Quaternion.identity;
                Variables.obbyBlock1.GetComponent<Renderer>().material.color = Color.red;

        }

        public static void destroyObby()
        {
            Variables.obbyBlock1?.Destroy();
        }


    }
}
