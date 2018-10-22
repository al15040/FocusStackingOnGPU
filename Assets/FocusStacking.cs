using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

public class FocusStacking : MonoBehaviour
{
  //Image読み込み→Projection(省略可)→各画像でコントラストマップ計算(compute shader)→Maxコントラストカラーを探索
  private ComputeShader m_focusStackingShader;

  private int m_convWindowSize;
  private string[] m_imgPaths;

	/*
  public FocusStacking(string[] imgPaths, int convWindowSize = 9)
  {
    m_convWindowSize = convWindowSize;
    m_imgPaths = new string[imgPaths.Length];
    System.Array.Copy(imgPaths, m_imgPaths, imgPaths.Length);
	}
	*/

	public void StartFocusStacking(string[] imgsFolderPath, int convWindowSize = 9)
  {
    List<string> jpegImgPaths = new List<string>();

    foreach (string path in imgsFolderPath)
      if (path.EndsWith(".jpeg") || path.EndsWith(".JPG") || path.EndsWith(".JPEG") || path.EndsWith(".png"))
      {
        Debug.Log(path);
        jpegImgPaths.Add(path);
      }

		StartCoroutine("CreateFocusStackImg", jpegImgPaths);
	}


  private IEnumerator CreateFocusStackImg(List<string> imgFilePaths)
  {
		Texture2D tex = ImageLoader.ReadTextureByFile(imgFilePaths[0]);

		var contrastMap   = new RenderTexture( tex.width
																				 , tex.height
																				 , 0
																				 , RenderTextureFormat.RFloat
																				 );
		contrastMap.enableRandomWrite = true;
		contrastMap.Create();


    var focusStackImg = new RenderTexture( tex.width
																				 , tex.height
																				 , 0
																				 , RenderTextureFormat.ARGBFloat
																				 );
		focusStackImg.enableRandomWrite = true;
		focusStackImg.Create();

		Object.Destroy(tex);
		tex = null;

		Debug.Log("focus stacking execute time");
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		for (int i = 0; i < imgFilePaths.Count; ++i)
		{
			m_focusStackingShader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("FocusStacking"));

			int kernelFuncID = m_focusStackingShader.FindKernel("FocusStacking_MaxContrast");
			m_focusStackingShader.SetInt("LocalWinSize", m_convWindowSize);
			m_focusStackingShader.SetInt("TexWidth", focusStackImg.width);
			m_focusStackingShader.SetInt("TexHeight", focusStackImg.height);
			m_focusStackingShader.SetTexture(kernelFuncID, "ContrastMap", contrastMap);
			m_focusStackingShader.SetTexture(kernelFuncID, "FocusStackingImg", focusStackImg);

			Texture2D srcTex = ImageLoader.ReadTextureByFile(imgFilePaths[i]);
			m_focusStackingShader.SetTexture(kernelFuncID, "SrcTex", srcTex);

      m_focusStackingShader.Dispatch(kernelFuncID, srcTex.width / 2, srcTex.height / 2, 1);

			MonoBehaviour.Destroy(srcTex);
			MonoBehaviour.Destroy(m_focusStackingShader);
			srcTex = null;
			m_focusStackingShader = null;
			Resources.UnloadUnusedAssets();
      
      yield return null;
		}

		sw.Stop();
		Debug.Log($"{sw.ElapsedMilliseconds}ミリ秒");
    
		SaveImgAsJPEG(focusStackImg, "C:\\Users\\H_Shionozaki\\Desktop\\angle0\\test.jpeg", TextureFormat.RGBAFloat);
	}

	private void SaveImgAsJPEG(RenderTexture srcTex, string filePath, TextureFormat format)
	{
		var tex = new Texture2D(srcTex.width, srcTex.height, format, false);
		RenderTexture.active = srcTex;
		tex.ReadPixels(new Rect(0, 0, srcTex.width, srcTex.height), 0, 0);
		tex.Apply();

		byte[] bytes = tex.EncodeToJPG();
		Object.Destroy(tex);

		File.WriteAllBytes(filePath, bytes);
	}

}
