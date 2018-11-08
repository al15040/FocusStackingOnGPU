using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public static class ImageWriter
{
	public static void SaveRendTexAsJPEG(RenderTexture srcTex, string filePath, TextureFormat format)
	{
		var tex = CreateTex2DfromRendTex(srcTex, format);

		byte[] bytes = tex.EncodeToJPG();
		Object.Destroy(tex);

		File.WriteAllBytes(filePath, bytes);
	}

	public static void SaveRendTexAsPNG(RenderTexture srcTex, string filePath, TextureFormat format)
	{
		var tex = CreateTex2DfromRendTex(srcTex, format);

		byte[] bytes = tex.EncodeToPNG();
		Object.Destroy(tex);

		File.WriteAllBytes(filePath, bytes);
	}

	public static Texture2D CreateTex2DfromRendTex(RenderTexture rendTex, TextureFormat format)
	{
		var tex = new Texture2D(rendTex.width, rendTex.height, format, false);
		RenderTexture.active = rendTex;
		tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;

		return tex;
	}

}
