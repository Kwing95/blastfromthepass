using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLayer : MonoBehaviour
{
	// give meshrenderers a public sprite layer
	private MeshRenderer mr;
	public int sprite_order = 1;
	public string layer_name = "Default";

    // Start is called before the first frame update
    void Start()
    {
		mr = GetComponent<MeshRenderer>();
		mr.sortingLayerName = layer_name;
		mr.sortingOrder = sprite_order;
    }
}
