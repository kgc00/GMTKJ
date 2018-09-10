using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClass
{
    public Texture2D texture;
    public Sprite nodeSprite;
    public SpriteRenderer nodeRenderer;
	public Vector3 worldPosition;

	public TestClass(Texture2D _texture, Sprite _nodeSprite,
	SpriteRenderer _nodeRenderer, Vector3 _worldPosition){
		texture = _texture;
		nodeSprite = _nodeSprite;
		nodeRenderer = _nodeRenderer;
		worldPosition = _worldPosition;
	}
	public TestClass(Sprite _nodeSprite){
		nodeSprite = _nodeSprite;
	}
}
