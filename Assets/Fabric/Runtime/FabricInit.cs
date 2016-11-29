namespace Fabric.Internal
{
	using UnityEngine;
	using System.Collections;
	
	public class FabricInit : MonoBehaviour
	{

        public bool DebugMode = false;

        void Start() {
            if(DebugMode) {
                int j = 0;
                int i = 34/j;
                Debug.Log("Crashalytics Error = "+i);
            }
        }

	}
}
