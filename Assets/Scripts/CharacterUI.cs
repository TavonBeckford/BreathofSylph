using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{

	public GameObject characterWindowUI;  // The entire UI

	[SerializeField] CinemachineFreeLook freeLookCamera;   //grab camera object

	public bool isCharacterWindowOpen = false;


	void Start()
	{
		
	}

	void Update()
	{
		// Check to see if we should open/close the inventory
		if (Input.GetKeyDown(KeyCode.C))
		{

			if (isCharacterWindowOpen)
			{

				freeLookCamera.m_XAxis.m_MaxSpeed = 450;
				freeLookCamera.m_YAxis.m_MaxSpeed = 4;
				characterWindowUI.SetActive(false);
				isCharacterWindowOpen = false;
			}
			else
			{
				freeLookCamera.m_XAxis.m_MaxSpeed = 0;
				freeLookCamera.m_YAxis.m_MaxSpeed = 0;
				characterWindowUI.SetActive(true);
				isCharacterWindowOpen = true;
			}
		}

	}
}
