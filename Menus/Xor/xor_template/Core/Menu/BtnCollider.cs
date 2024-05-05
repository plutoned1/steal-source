using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ark.Core.Menu
{
    public class BtnCollider : MonoBehaviour
    {
        public string relatedText = "";

        private void OnTriggerEnter(Collider collider)
        {
            if (collider == Main.menuPointer.GetComponent<Collider>())
            {
                if (Time.frameCount >= Main.framePressCooldown + 30)
                {
                    Main.Toggle(relatedText);
                    Main.framePressCooldown = Time.frameCount;
                }
            }
        }
    }
}
