using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreateConvFilter
{

	public static Texture2D CreateGaussianFilter(double sigma, int filterSize)
	{
		Texture2D filter = new Texture2D(filterSize, filterSize, TextureFormat.RGBAFloat, false);

		float normalizeCoef = 0.0f;

		for (int y = -filterSize / 2; y <= filterSize / 2; ++y)
		for (int x = -filterSize / 2; x <= filterSize / 2; ++x)
			normalizeCoef += (float)Gaussian2D(sigma, x, y);

		Debug.Log("normalize");

		Debug.Log(normalizeCoef);
		float debug = 0.0f;
		for ( int y = -filterSize / 2; y <= filterSize / 2; ++y )
		for ( int x = -filterSize / 2; x <= filterSize / 2; ++x )
		{
				float value = (float)Gaussian2D(sigma, x, y) / normalizeCoef;
				filter.SetPixel(x + filterSize / 2, y + filterSize / 2, new Color(value, 0, 0));
				Debug.Log(x.ToString() + "," + y.ToString());
				Debug.Log(value);
				debug += value;
			}

		Debug.Log("debug : " + debug.ToString());
		return filter;
	}


	private static double Gaussian2D(double sigma, double x, double y)
	{
		return ( System.Math.Exp( -( x*x + y*y ) / (2.0*sigma*sigma) ) ) / (2.0*sigma*sigma*System.Math.PI) ;
	}

}
