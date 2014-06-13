using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
	public string NextScene = "";
	
	void OnMouseEnter()
	{
		renderer.material.color = Color.green;
	}
	
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}
	
	void OnMouseDown()
	{
		//print("wazaaaaaa");
        if (!NextScene.Equals(""))
            Application.LoadLevel(NextScene);
        /*if (this.name.Equals("Return"))
        {
            Pause.SetActive(false);
            RootInGame.SetActive(true);
        }*/
        if (this.name.Equals("Button_Quit"))
        {
            Application.Quit();
        }
	}
}
