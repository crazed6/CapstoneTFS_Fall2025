using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterStylizedExplosions
{
    public class AutoRotate : MonoBehaviour
    {
        public Vector3 RotateAngle;
        private void Update()
        {
            transform.Rotate(RotateAngle * Time.deltaTime, Space.Self);
        }
    }
}
