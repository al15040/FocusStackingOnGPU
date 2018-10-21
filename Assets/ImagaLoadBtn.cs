using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class ImagaLoadBtn : MonoBehaviour 
{
	public Material simple_material_;
	public ComputeShader cs_;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public void OnClick()
	{
		Texture2D tex = (Texture2D)ImageLoader.ReadTextureByFile("C:\\Users\\光\\Pictures\\Screenshots\\rinze.png");

		int kernelFuncID = cs_.FindKernel("LaplacianFilter");

		RenderTexture buffer = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.ARGBFloat);
		buffer.enableRandomWrite = true;
		buffer.Create();

		cs_.SetTexture(kernelFuncID, "buffer", buffer);

		cs_.SetTexture(kernelFuncID, "srcTex", tex);
		cs_.SetInt("texWidth", tex.width);
		cs_.SetInt("texHeight", tex.height);

		cs_.Dispatch(kernelFuncID, tex.width, tex.height, 1);

		simple_material_.SetTexture("_MainTex", buffer);

		Texture[] texes = new Texture[10];
		Parallel.For(0, 10, i => texes[i] = new Texture2D(1,1));
	}

}
