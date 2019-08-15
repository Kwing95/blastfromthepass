using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Projection : MonoBehaviour
{
	// physics prediction vars
	private Vector3 accel;
	private float timestep = .05f; // seconds
	private float time_max = 1.5f; // seconds

    // Start is called before the first frame update
    void Start()
    {
		accel = new Vector3(0, -9.81f, 0);
    }

	// call to return a list of Vector3 projectile positions
	public List<Vector3> get_prediction(Vector3 start_pos, Vector3 start_vel)
	{
		//Debug.Log("Projection Velocity: " + start_vel);
		// set up vars
		List<Vector3> preds = new List<Vector3>();
		Vector3 curr_pos = start_pos;
		Vector3 curr_vel = start_vel;
		preds.Add(curr_pos);
		float time_count = 0f;

		// begin prediction
		while (time_count <= time_max)
		{
			curr_pos += curr_vel * timestep;
			curr_vel += accel * timestep;
			preds.Add(curr_pos);
			time_count += timestep;
		}
		return preds;
	}

    public List<Vector3> quick_prediction(Vector3 start, Vector3 end)
    {
        bool projectingUp = start.y < end.y;
        bool projectingRight = start.x < end.x;

        List<Vector3> predictions = new List<Vector3>();
        Vector3 direction = Vector3.Normalize(end - start);
        Vector3 nextDot = start;

        while((nextDot.y < end.y == projectingUp) && (nextDot.x < end.x == projectingRight))
        {
            nextDot += direction;
            predictions.Add(nextDot);
            if(predictions.Count > 100)
                break;
        }
		// remove the last dot
		predictions.RemoveAt(predictions.Count - 1);
        return predictions;
    }

}
