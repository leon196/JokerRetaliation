﻿/*
                |    |              _.-7
                |\.-.|             ( ,(_
                | a a|              \\  \,
                ) ["||          _.--' \  \\
             .-'  '-''-..____.-'    ___)  )\
            F   _/-``-.__;-.-.--`--' . .' \_L_
           |   l  {~~} ,_\  '.'.      ` __.' )\
           (    -.;___,;  | '- _       :__.'( /
           | -.__ _/_.'.-'      '-._ .'      \\
           |     .'   |  -- _                 '\,
           |  \ /--,--{ .    '---.__.       .'  .'
           J  ;/ __;__]. '.-.            .-' )_/
           J  (-.     '\'. '. '-._.-.-'--._ /
           |  |  '. .' | \'. '.    ._       \
           |   \   T   |  \  '. '._  '-._    '.
           F   J   |   |  '.    .  '._   '-,_.--`
           F   \   \   F .  \    '.   '.  /
          J     \  |  J   \  '.   '.    '/
          J      '.L__|    .   \    '    |
          |   .    \  |     \   '.   '. /
          |    '    '.|      |    ,-.  (
          F   | ' ___  ',._   .  /   '. \
          F   (.'`|| (-._\ '.  \-      '-\
          \ .-'  ( L `._ '\ '._ (
     snd  /'  |  /  '-._\      ''\
              `-'
*/
using UnityEngine;
public class Batman : MonoBehaviour
{
	public Sprite spriteDead;
	private SpriteRenderer spriteRenderer;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Push()
	{
		spriteRenderer.sprite = spriteDead;
	}
}
