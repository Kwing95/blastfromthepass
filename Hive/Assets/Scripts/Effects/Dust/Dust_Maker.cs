using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Dust_Maker : MonoBehaviour
{
	// dust particle prefab
	public GameObject dust;

	// rng
	private System.Random rand = new System.Random(); // default seed

	// generate a puff of particles
	public void generate_puff(Vector3 offset, int num_particles, float x_mean, float x_sdev, float y_mean, float y_sdev, float life_mean, float life_sdev)
	{
		GameObject temp;
		Vector2 vel;
		float x_vel, y_vel, lifetime;
		for (int a=0; a<num_particles; a++)
		{
			temp = Instantiate(dust, transform.position + offset, Quaternion.identity);
			x_vel = gaussian_float(x_mean, x_sdev);
			y_vel = gaussian_float(y_mean, y_sdev);
			lifetime = gaussian_float(life_mean, life_sdev);
			vel = new Vector2(x_vel, y_vel);
			temp.GetComponent<Dust_Control>().init_dust(vel, lifetime);
		}
	}

	// get a random gaussian float centered on mean with sdev
	private float gaussian_float(float mean, float sdev)
	{
		float temp = marsaglia_polar();
		temp *= sdev;
		temp += mean;
		return temp;
	}

	// use Marsaglia's Polar Method
	// centered on mean = 0, sdev = 1
	private float marsaglia_polar()
	{
		float mean, sdev, logCheck;
		do
		{
			mean = 2.0f * (float)rand.NextDouble() - 1.0f;
			sdev = 2.0f * (float)rand.NextDouble() - 1.0f;
			logCheck = mean * mean + sdev * sdev;
		} while (logCheck >= 1.0f);
		return mean * Mathf.Sqrt(-2.0f * Mathf.Log(logCheck) / logCheck);
	}
}
