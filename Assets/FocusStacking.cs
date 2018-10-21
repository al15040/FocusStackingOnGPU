using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

public class FocusStacking 
{
  //Image読み込み→Projection(省略可)→各画像でコントラストマップ計算(compute shader)→Maxコントラストカラーを探索
  private ComputeShader m_focusStackingShader;

  private int m_convWindowSize;
  private string[] m_imgPaths;

  public FocusStacking(string[] imgPaths, int convWindowSize = 9)
  {
    m_convWindowSize = convWindowSize;
    m_imgPaths = new string[imgPaths.Length];
    System.Array.Copy(imgPaths, m_imgPaths, imgPaths.Length);
	}


	public void StartFocusStacking(string[] imgsFolderPath, int convWindowSize = 9)
  {
    List<string> jpegImgPaths = new List<string>();
		//JPEGのみ
		foreach (string path in imgsFolderPath)
    if (path.EndsWith(".jpeg") || path.EndsWith(".JPG") || path.EndsWith(".JPEG") || path.EndsWith(".png"))
      jpegImgPaths.Add(path);

    List<Texture> photos = new List<Texture>();
		Debug.Log("img loading time");
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		foreach (string path in jpegImgPaths) photos.Add( ImageLoader.ReadTextureByFile(path) );
		sw.Stop();
		Debug.Log($"{sw.ElapsedMilliseconds}ミリ秒");

		CreateFocusStackImg(photos);
  }


  private void CreateFocusStackImg(List<Texture> photos)
  {
		var contrastMap   = new RenderTexture( photos[0].width
																				 , photos[0].height
																				 , 0
																				 , RenderTextureFormat.RFloat
																				 );
		contrastMap.enableRandomWrite = true;
		contrastMap.Create(); 

		var focusStackImg = new RenderTexture( photos[0].width
																				 , photos[0].height
																				 , 0
																				 , RenderTextureFormat.ARGBFloat
																				 );
		focusStackImg.enableRandomWrite = true;
		focusStackImg.Create();

		Debug.Log("focus stacking execute time");
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		for (int i = 0; i < photos.Count; ++i)
		{
			m_focusStackingShader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("FocusStacking"));

			int kernelFuncID = m_focusStackingShader.FindKernel("FocusStacking_MaxContrast");
			m_focusStackingShader.SetInt("LocalWinSize", m_convWindowSize);
			m_focusStackingShader.SetInt("TexWidth", photos[0].width);
			m_focusStackingShader.SetInt("TexHeight", photos[0].height);
			m_focusStackingShader.SetTexture(kernelFuncID, "ContrastMap", contrastMap);
			m_focusStackingShader.SetTexture(kernelFuncID, "FocusStackingImg", focusStackImg);

			m_focusStackingShader.SetTexture(kernelFuncID, "SrcTex", photos[i]);

			m_focusStackingShader.Dispatch(kernelFuncID, photos[i].width / 2, photos[i].height / 2, 1);
		}

		sw.Stop();
		Debug.Log($"{sw.ElapsedMilliseconds}ミリ秒");

		SaveImgAsJPEG(focusStackImg, "C:\\Users\\光\\Desktop\\angle01\\test.JPG");
	}

	private void SaveImgAsJPEG(Texture srcTex, string filePath)
	{
		var tex = new Texture2D(srcTex.width, srcTex.height, TextureFormat.RGBAFloat, false);
		RenderTexture.active = (RenderTexture)srcTex;
		tex.ReadPixels(new Rect(0, 0, srcTex.width, srcTex.height), 0, 0);
		tex.Apply();

		byte[] bytes = tex.EncodeToJPG();
		Object.Destroy(tex);

		File.WriteAllBytes(filePath, bytes);
	}

}
