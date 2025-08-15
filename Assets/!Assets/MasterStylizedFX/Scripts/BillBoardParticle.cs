using UnityEngine;
using System.Collections;
namespace MasterStylizedExplosion
{
    [ExecuteInEditMode]
    public class BillBoardParticles : MonoBehaviour
    {

        public bool bTurnOver = false;

        void OnWillRenderObject()
        {
            if (Camera.main)
            {
                if (bTurnOver)
                    transform.forward = Camera.main.transform.forward;
                else
                    transform.forward = -Camera.main.transform.forward;
            }
        }
    }
}