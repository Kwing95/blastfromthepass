using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineText : MonoBehaviour
{
	// get textmesh stats from parent
	private TextMesh parent_mesh;
	private TextMesh my_mesh;

    // Start is called before the first frame update
    void Start()
    {
		parent_mesh = transform.parent.GetComponent<TextMesh>();
		my_mesh = GetComponent<TextMesh>();
		my_mesh.fontSize = parent_mesh.fontSize;
		my_mesh.characterSize = parent_mesh.characterSize;
		my_mesh.text = parent_mesh.text;

		// set mesh renderers
		MeshRenderer temp_parent = transform.parent.GetComponent<MeshRenderer>();
		temp_parent.sortingLayerName = "HUD";
		temp_parent.sortingOrder = -2;
		temp_parent = GetComponent<MeshRenderer>();
		temp_parent.sortingLayerName = "HUD";
		temp_parent.sortingOrder = -1;
	}
}
