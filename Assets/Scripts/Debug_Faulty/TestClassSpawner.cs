using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClassSpawner : MonoBehaviour {

	public Texture2D src;
	public SpriteRenderer gridRend;
	// public int[,] fakeGrid;

	void Start () {
		// fakeGrid = new int[20,20];
		// print(fakeGrid.Length);
		gridRend = gameObject.AddComponent<SpriteRenderer>();

		Texture2D tex = new Texture2D(
			src.width,
			src.height) ;
		Color[] colorArray = new Color[tex.width * tex.height];

		Color[]srcArray = src.GetPixels();
		
		// int offset = 0;
		int maxSrcPixel = tex.width + (tex.height * tex.width);

		for (int x = 0; x < tex.width; x++)
		{
			for (int y = 0; y < tex.height; y++)
			{
                int pixelIndex = x + (y * tex.width);
                Color srcPixel = srcArray[pixelIndex];
				colorArray[pixelIndex] = srcArray[pixelIndex];
                
                // if (pixelIndex >= maxSrcPixel){
				// 	offset += 1;
				// 	print(offset);
					
				// }
			}
		}
		tex.SetPixels(colorArray);
		tex.Apply();

		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Point;

		Sprite newSprite = Sprite.Create(tex, new Rect (0, 0, tex.width,
		tex.height), Vector2.one * 0.5f);

		gridRend.sprite = newSprite;
	}
	
	void Update () {
		
	}
}
