using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FocusStacking : MonoBehaviour
{
	//Image読み込み→Projection(省略可)→各画像でコントラストマップ計算+Maxコントラストカラーを探索(compute shader)

	//to do 二値化画像も利用したアルゴリズムに変更
	//あるピクセルについて、値が0の二値化画像がbackImgThreshold枚以上存在した場合
	//そのピクセルは背景とする

	public void StartFocusStacking(string[] imgsFolderPath, int convWindowSize = 9, int backImgThreshold = 5)
  {
    List<string> jpegImgPaths = new List<string>();

		foreach (string path in imgsFolderPath)
    if (path.EndsWith(".jpeg") || path.EndsWith(".JPG") || path.EndsWith(".JPEG") || path.EndsWith(".png"))
			jpegImgPaths.Add(path);
	
		StartCoroutine( CreateFocusStackImg(jpegImgPaths, convWindowSize) );
	}


  private IEnumerator CreateFocusStackImg(List<string> imgFilePaths, int convWindowSize)
  {
		if (imgFilePaths.Count == 0) yield break;

		Texture2D tex = ImageLoader.ReadTextureByFile(imgFilePaths[0]);
		Debug.Log(tex.format.ToString());

		//読み込み専用と書き込み専用を交互に使い分ける
		var contrastMaps = new RenderTexture[2];

		for (int i = 0; i < 2; ++i)
		{
			contrastMaps[i] = new RenderTexture( tex.width
																				 , tex.height
																				 , 0
																				 , RenderTextureFormat.RFloat
																				 );
			contrastMaps[i].enableRandomWrite = true;
			contrastMaps[i].Create();
		}

    var focusStackImg = new RenderTexture( tex.width
																				 , tex.height
																				 , 0
																				 , RenderTextureFormat.ARGBFloat
																				 );
		focusStackImg.enableRandomWrite = true;
		focusStackImg.Create();

		Object.Destroy(tex);
		tex = null;

		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();

    for (int i = 0; i < imgFilePaths.Count; ++i)
    {
			ComputeShader focusStackingShader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("FocusStacking"));

      int kernelFuncID = focusStackingShader.FindKernel("FocusStacking_MaxContrast");
      focusStackingShader.SetInt("LocalWinSize", convWindowSize);
      focusStackingShader.SetInt("TexWidth", focusStackImg.width);
      focusStackingShader.SetInt("TexHeight", focusStackImg.height);
      focusStackingShader.SetTexture(kernelFuncID, "ContrastMap", contrastMaps[i % 2]);
			focusStackingShader.SetTexture(kernelFuncID, "NewContrastMap", contrastMaps[(i + 1) % 2]);

			focusStackingShader.SetTexture(kernelFuncID, "FocusStackingImg", focusStackImg);

      Texture2D srcTex = ImageLoader.ReadTextureByFile(imgFilePaths[i]);
			focusStackingShader.SetTexture(kernelFuncID, "SrcTex", srcTex);

      focusStackingShader.Dispatch(kernelFuncID, srcTex.width / 2, srcTex.height / 2, 1);

			Destroy(srcTex);
			srcTex = null;
			Resources.UnloadUnusedAssets();

      yield return null;
		}

		sw.Stop();
		Debug.Log("focus stacking execute time");
		Debug.Log($"{sw.ElapsedMilliseconds}ミリ秒");
		SaveRendTexAsJPEG(focusStackImg, @"C:\Users\光\Desktop\angle0\test.jpeg", TextureFormat.RGBAFloat);

		focusStackImg.Release();
		contrastMaps[0].Release();
		contrastMaps[1].Release();
	}



	private void SaveRendTexAsJPEG(RenderTexture srcTex, string filePath, TextureFormat format)
	{
		var tex = CreateTex2DfromRendTex(srcTex, format);

		byte[] bytes = tex.EncodeToJPG();
		Object.Destroy(tex);

		File.WriteAllBytes(filePath, bytes);
	}

	private void SaveRendTexAsPNG(RenderTexture srcTex, string filePath, TextureFormat format)
	{
		var tex = CreateTex2DfromRendTex(srcTex, format);

		byte[] bytes = tex.EncodeToPNG();
		Object.Destroy(tex);

		File.WriteAllBytes(filePath, bytes);
	}

	private Texture2D CreateTex2DfromRendTex(RenderTexture rendTex, TextureFormat format)
	{
		var tex = new Texture2D(rendTex.width, rendTex.height, format, false);
		RenderTexture.active = rendTex;
		tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;

		return tex;
	}
}
