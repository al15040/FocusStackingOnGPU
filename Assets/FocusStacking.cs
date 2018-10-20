using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FocusStacking 
{
  //Image読み込み→Projection(省略可)→各画像でコントラストマップ計算(compute shader)→Maxコントラストカラーを探索
  [SerializeField]
  private ComputeShader m_cs;

  private int m_convWindowSize;
  private string[] m_imgPaths;

  public FocusStacking(int convWindowSize = 9, string[] imgPaths = null )
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
    if (path.EndsWith(".jpeg") || path.EndsWith(".JPG") || path.EndsWith(".JPEG"))
      jpegImgPaths.Add(path);

    List<Texture> photos = new List<Texture>();
    foreach (string path in jpegImgPaths) photos.Add( ImageLoader.ReadTextureByFile(path) );

    List<RenderTexture> contrastMaps = CreateContrastMaps(photos);
  }


  private List<RenderTexture> CreateContrastMaps(List<Texture> photos)
  {
    var contrastMaps = new List<RenderTexture>();
    for (int i=0; i < photos.Count; ++i) 
    {
      contrastMaps.Add( new RenderTexture( photos[i].width, photos[i].height, 0, RenderTextureFormat.ARGB32) );
      contrastMaps[i].enableRandomWrite = true;
      contrastMaps[i].Create();
    }
  }

}
