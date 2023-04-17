using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class StencilVolume : MonoBehaviour
{
	public bool isSphere = false;

	Material stencilMaterial;
	Renderer thisRenderer = null;
	Camera bufferCam;
	UnityEngine.Rendering.CommandBuffer buffer;
	Bounds bounds = new Bounds();
	string commandName = "";

	bool ContainsCamera()
	{
		bool _inside = false;
		Camera _cam = Camera.main;
		#if UNITY_EDITOR
		_cam = Camera.current;
		// if (!Application.isPlaying)
		// {
		// 	if (UnityEditor.SceneView.lastActiveSceneView != null)
		// 		_cam = UnityEditor.SceneView.lastActiveSceneView.camera;
		// }
		#endif
		if (_cam == null) return false;

		Vector3 _localPos = transform.InverseTransformPoint(_cam.transform.position);
		if (isSphere)
		{
			_inside = _localPos.magnitude < 0.5f;
		}
		else
		{
			_inside = Mathf.Abs(_localPos.x) < 0.5f && Mathf.Abs(_localPos.y) < 0.5f && Mathf.Abs(_localPos.z) < 0.5f;
		}

		return _inside;
	}

	  #if UNITY_EDITOR
    private void OnValidate() {
        if (UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating || UnityEditor.BuildPipeline.isBuildingPlayer)
        {
            return;
        }
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
		if (Application.isPlaying)
		{
			return;
		}
		thisRenderer = null;
    }
    #endif

	public void Update()
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (thisRenderer == null) return;
			
			bool _error = thisRenderer.sharedMaterial == null;
			if (_error)
			{
				Abort();
				return;
			}
			string s = thisRenderer.sharedMaterial.GetTag("StencilVolume", false, "False");
			if (s != "true")
			{
				Abort();
				return;
			}
		}
		#endif		
	}

	void Abort()
	{
		thisRenderer.enabled = true;
		GameObject.DestroyImmediate(this);
	}

	void OnEnable()
    {
		if (string.IsNullOrEmpty(commandName))
			commandName = "StencilVolume: " + gameObject.name;
		
        Camera.onPreRender -= ApplyCommandBuffer;
        Camera.onPostRender -= RemoveCommandBuffer;
        Camera.onPreRender += ApplyCommandBuffer;
        Camera.onPostRender += RemoveCommandBuffer;
    }

    void OnDisable()
    {
        Camera.onPreRender -= ApplyCommandBuffer;
        Camera.onPostRender -= RemoveCommandBuffer;
    }

	void CreateCommandBuffer(Camera cam)
    {
        if (buffer == null || buffer.sizeInBytes == 0)
        {
            buffer = new UnityEngine.Rendering.CommandBuffer();
            buffer.name = commandName;
        }
        else
        {
            buffer.Clear();
        }
        
		if (thisRenderer != null && thisRenderer.sharedMaterial != null)
		{
			if (ContainsCamera())
			{
				buffer.DrawRenderer(thisRenderer, stencilMaterial, 0, 0);
				buffer.DrawRenderer(thisRenderer, stencilMaterial, 0, 3);
				buffer.DrawRenderer(thisRenderer, stencilMaterial, 0, 2);
			}
			else
			{
				buffer.DrawRenderer(thisRenderer, stencilMaterial, 0, 0);
				buffer.DrawRenderer(thisRenderer, stencilMaterial, 0, 1);
				buffer.DrawRenderer(thisRenderer, stencilMaterial, 0, 2);
			}
		}
          

        //UpdateShadow();
    }

    void RemoveCommandBuffer(Camera cam)
    {
        if (bufferCam != null && buffer != null)
        {
            bufferCam.RemoveCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterForwardOpaque, buffer);
            bufferCam = null;
        }
    }

	static Plane[] planes = new Plane[6];
	static int planesForFrame = -1;

	// #if UNITY_EDITOR
	// Camera _previewSceneCamera = null;
	// #endif

    void ApplyCommandBuffer(Camera cam)
    {
		if (cam.cullingMask == 0)
			return;
        // #if UNITY_EDITOR
        // // hack to avoid rendering in the inspector preview window
        // if (_previewSceneCamera == null)
		// {
		// 	if (cam.gameObject.name == "Preview Scene Camera")
		// 		_previewSceneCamera = cam;
		// }
		// if (_previewSceneCamera == cam)
        // 	return;
		
        // #endif

		if ((cam.cullingMask & (1 << 0)) <= 0)
			return;

        if (thisRenderer == null)
        {
            thisRenderer = GetComponent<Renderer>();
			stencilMaterial = thisRenderer.sharedMaterial;
        }

		//if (bounds.center == Vector3.zero && bounds.size == Vector3.zero)
		{
			bounds = TransformBounds(transform, new Bounds(Vector3.zero, Vector3.one));
		}

		{
			GeometryUtility.CalculateFrustumPlanes(cam, planes);
			//planesForFrame = Time.frameCount;
		}

		if (!GeometryUtility.TestPlanesAABB(planes, bounds))
		{
			return;
		}

		// if (Vector3.Distance(transform.position, cam.transform.position) > cam.farClipPlane)
		// {
		// 	return;
		// }

		thisRenderer.enabled = false;        

        if (bufferCam != null)
        {
            if(bufferCam == cam)
                return;
            else
                RemoveCommandBuffer(cam);
        }

        CreateCommandBuffer(cam);
        if (buffer == null)
            return;

        bufferCam = cam;
        bufferCam.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterForwardOpaque, buffer);
    }

	public Bounds TransformBounds( Transform _transform, Bounds _localBounds )
	{
		var center = _transform.TransformPoint(_localBounds.center);

		// transform the local extents' axes
		var extents = _localBounds.extents;
		var axisX = _transform.TransformVector(extents.x, 0, 0);
		var axisY = _transform.TransformVector(0, extents.y, 0);
		var axisZ = _transform.TransformVector(0, 0, extents.z);

		// sum their absolute value to get the world extents
		extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
		extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
		extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

		return new Bounds { center = center, extents = extents };
	}

	#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		var _old = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;

		if (isSphere)
		{
			Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
		}
		else
		{
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}

		Gizmos.matrix = _old;

		//Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
	#endif
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(StencilVolume))]
public class StencilVolumeEditor : UnityEditor.Editor
{
    public bool HasFrameBounds()
    {
        return true;
    }
 
    public Bounds OnGetFrameBounds()
    {
        StencilVolume script = (StencilVolume) target;
        Bounds bounds = new Bounds(script.transform.position, script.transform.localPosition.magnitude * Vector3.one);
        return bounds;
    }
}
#endif