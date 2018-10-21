using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public static class ImageLoader
{
  public static Texture2D ReadTextureByFile(string filePath)
  {
    byte[] binaryTex = ReadByteByFile(filePath);

    Texture2D tex = new Texture2D(1, 1);
    tex.LoadImage(binaryTex);
		
    return tex;
  }

  private static byte[] ReadByteByFile(string filePath)
  {
    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
      BinaryReader bin = new BinaryReader(fileStream);
      byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);
      bin.Close();
      return values;
    }
  }
}
