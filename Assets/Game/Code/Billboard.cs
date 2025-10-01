using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
	public class Billboard : MonoBehaviour
	{
        [SerializeField, ReadOnly] Camera mainCam;
		[SerializeField] bool x;
		[SerializeField] bool y;
		[SerializeField] bool z;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            Vector3 targetRot = mainCam.transform.rotation.eulerAngles;
            Vector3 finalRot = transform.rotation.eulerAngles;

            if (x) finalRot.x = targetRot.x;
            if (y) finalRot.y = targetRot.y;
            if (x) finalRot.z = targetRot.z;

            transform.rotation = Quaternion.Euler(finalRot);
        }
    }
}