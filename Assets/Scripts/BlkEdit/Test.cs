using UnityEngine;
using System.Collections;

namespace Blk
{
	public class Test : MonoBehaviour
	{
		public Camera _cam;
		public GameObject _prefab;
		
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
			if (Input.GetMouseButton(0)) {
				int xCount = 14;
				int xSize = Screen.width / xCount;
				int ySize = Screen.height / 2 / xCount;
				
				Vector3 pos = Input.mousePosition;
				
				// to screen
				int x = (int)(pos.x / xSize);
				int y = (int)(pos.y / ySize);
				
				Vector3 sPos = new Vector3(x * xSize, y * ySize, 0) - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
				
				GameObject go = (GameObject)GameObject.Instantiate(_prefab);
				go.transform.parent = _prefab.transform.parent;
				go.transform.localScale = new Vector3(20, 16, 0);
				go.SetActive(true);
				go.transform.localPosition = new Vector3(sPos.x, sPos.y, 0);
				
				
				Uzu.VectorI3 idx = new Uzu.VectorI3(x, y, 0);
				Main.BlockWorld.SetBlockType(idx, Uzu.BlockType.SOLID);
				Main.BlockWorld.SetBlockColor(idx, Color.red);
			}
		}
	}
}