using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_Distance : MonoBehaviour
{
	// public particle stats
	public GameObject spark_prefab;
	public Vector3 offset;
	public int particles_per_second = 50;

	// private mode
	private int[] proportions = new int[] { 1, 2, 3 };
	private int prop_sum;
	private int[] color_priorities = new int[] { 1, 2, 3 };
	private float[] mean_lifespans = new float[] { 1f, 1f, 0.5f };
	private float[] std_lifespans;
	private float y_vel = 2f;
	private float x_std = .1f;
	private float x_range = .5f;
	private int particles_this_frame;

	// temp ref to particle
	private GameObject temp_part;
	private float part_xvel;
	private float[] part_lifespans = new float[3]; // hardcoded 3 colors

	// rng
	private System.Random rand = new System.Random(); // default seed

	// particle pooling
	private int pool_size = 450; // hard coded number
	private GameObject[] spark_pool;
	private int next_index = 0;
	public int spark_count = 0;

	// calculate prop_sum
	private void Start()
	{
		// init pool
		spark_pool = new GameObject[pool_size];
		for (int a = 0; a < pool_size; a++)
		{
			spark_pool[a] = Instantiate(spark_prefab, transform.position, Quaternion.identity);
			spark_pool[a].GetComponent<Spark_Distance>().parent_script = this;
			spark_pool[a].transform.parent = transform;
		}

		// init other stuff
		prop_sum = 0;
		foreach (int p in proportions) { prop_sum += p; }
		std_lifespans = new float[mean_lifespans.Length];
		for (int a = 0; a < mean_lifespans.Length; a++)
		{
			std_lifespans[a] = mean_lifespans[a] / 2.0f;
		}
	}

	// Update is called once per frame
	void Update()
	{
		particles_this_frame = Mathf.FloorToInt(particles_per_second * Time.deltaTime);
		if (particles_this_frame <= 0) particles_this_frame = 1;
		if (particles_this_frame + spark_count > pool_size * .95f)
		{
			Debug.Log("OVER LIMIT!: " + particles_this_frame + " || " + spark_count);
			return;
		}
		for (int a = 0; a < particles_this_frame; a++)
		{
			// generate particle
			temp_part = spark_pool[next_index++ % pool_size];
			temp_part.transform.position = transform.position + offset;
			temp_part.transform.position += Vector3.right * bounded_gaussian(0, x_std, -x_range, x_range);
			temp_part.transform.position += Vector3.up * Random.Range(-.1f, .1f);
			// try spawning in a circle
			// Vector3 circle = Random.insideUnitCircle;
			// temp_part.transform.position += circle * 1;

			// calculate stats
			for (int b = 0; b < part_lifespans.Length; b++)
			{
				part_lifespans[b] = gaussian_float(mean_lifespans[b], std_lifespans[b]);
			}
			part_xvel = 0f;
			// y_vel = gaussian_float(.7f, .1f);
			temp_part.GetComponent<Spark_Distance>().init_stats(part_lifespans, part_xvel, y_vel);
		}
	}

	// select a random color according to the color proportions
	private int select_color()
	{
		int rand_num = Random.Range(0, prop_sum);
		int index = 0;
		while (rand_num - proportions[index] > 0)
		{
			rand_num -= proportions[index];
			index++;
		}
		return index;
	}

	// get a range_bounded gaussian
	private float bounded_gaussian(float mean, float sdev, float min, float max)
	{
		float temp = min - 1;
		int loop_breaker = 0;
		while ((temp < min || temp > max) && loop_breaker++ < 100)
		{
			temp = gaussian_float(mean, sdev);
		}
		return temp;
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
