using System;
using UnityEngine;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour
    {
        [SerializeField]
        private float scaleFactor = 0.2f;
        [SerializeField]
        private int clicksToPop = 5;


        private void OnMouseDown()
        {
            clicksToPop--;

            Vector3 scalteIncrease = new(scaleFactor, scaleFactor, scaleFactor);
            transform.localScale += scalteIncrease;

            if(clicksToPop <= 0)
            {
                Pop();
            }
        }

        private void Pop()
        {
            Debug.LogFormat("Balloon {0} popped!", gameObject.name);
        }

    }
}
