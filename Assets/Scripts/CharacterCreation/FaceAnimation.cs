using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// The animator of the npc Faces, expressions, and reactions.
/// </summary>
public class FaceAnimation : AnimationHandler
{
	private SkinnedMeshRenderer face;
	private NPC.Controller npcController;
	public NPC.Controller NpcController 
	{ 
		get
		{
			return npcController;
		}
		set 
		{ 
			npcController = value;
		}
	}

	//Should add Reactions to the animator.
	//Will set it up like Idle, except some may not loop. 
	//Pass in an int for the state in a blendTree. 
	
	[Tooltip("For bodies with multiple skinned meshes")]
	[SerializeField] private bool getBodyArray;
	private SkinnedMeshRenderer[] bodyParts;
	
	public bool faceShiftEnding;
	public float faceShiftTimer = 0.35f;
	
	public List<Material> normalFace= new List<Material>();
	public List<Material> screaming= new List<Material>();
	public bool randomizeFace;
	public int faceIndex = 0;
	public bool manualSetFace;

	protected override void Awake()
	{
		base.Awake();
		face = GetComponent<SkinnedMeshRenderer>();
		if (getBodyArray)
		{
			bodyParts = transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
		}
	}

	private void Start()
	{
		//randomize face?
		if (randomizeFace)
		{
			faceIndex = Random.Range(0, normalFace.Count);
			
			SetNormalFace();
		}
	}

	/// <summary>
	/// Updates body/face material. 
	/// </summary>
	/// <param name="newFace"></param>
	public void SetFace(Material newFace)
	{
		Material[] mats = face.materials;
		
		for (int i = 0; i < mats.Length; i++)
		{
			mats[i] = newFace;
		}

		face.materials = mats;

		//Set all body parts to this mat 
		if (getBodyArray)
		{
			foreach (var body in bodyParts)
			{
				body.materials = mats;
			}
		}
	}

	/// <summary>
	/// Sets normal face using my face index. 
	/// </summary>
	public void SetNormalFace()
	{
		//face change
		if (normalFace[faceIndex])
		{
			SetFace(normalFace[faceIndex]);
		}
	}
	
	/// <summary>
	/// Sets screaming face using my face index. 
	/// </summary>
	public void SetScreamingFace()
	{
		//face change
		if (screaming[faceIndex])
		{
			SetFace(screaming[faceIndex]);
		}
	}
	
	public void AddNormalFace(Material matFace)
	{
		if(!normalFace.Contains(matFace))
			normalFace.Add(matFace);
	}

	public void AddScreamingFace(Material matFace)
	{	
		if(!screaming.Contains(matFace))
			screaming.Add(matFace);
	}


	#region Face Shifting Effect

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Spirit"))
		{
			BeginFaceShifting();
		}
	}
	
	void BeginFaceShifting()
	{
		if (faceShiftEnding)
		{
			return;
		}
            
		faceShiftEnding = true;
		//disable the face animator.
		if (Animator || !manualSetFace)
		{
			Animator.enabled= false;
		}
		//disable animator.
		StartCoroutine(FaceShift());
	}

	IEnumerator FaceShift()
	{
		while (faceShiftEnding)
		{
			//randomize face index
			faceIndex = Random.Range(0, normalFace.Count);
			//face change
			if (normalFace[faceIndex])
			{
				SetFace(normalFace[faceIndex]);
			}
                
			yield return new WaitForSeconds(faceShiftTimer);
		}
	}

	#endregion
	
	private void OnDisable()
	{
		faceShiftEnding = false; 
	}
}
