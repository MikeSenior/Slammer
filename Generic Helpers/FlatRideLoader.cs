using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ParkitectMods.FlatRides
{
	public abstract class FlatRideLoader : MonoBehaviour
	{
		private List<BuildableObject> _sceneryObjects = new List<BuildableObject>();

		public string Path;
		public string Identifier;
		public FlatRide FlatRideComponent;

		protected abstract GameObject LoadRideModel();
		protected abstract void InitializeRideData(GameObject ride);
		protected abstract string GetAssetBundleName();

		public void LoadFlatRide()
		{
			GameObject asset = LoadRideModel();
			asset.AddComponent<Waypoints>();
			SetWaypoints(asset, false);
			asset.transform.position = new Vector3(0, 999, 0);

			InitializeRideData(asset);

			BuildableObject buildableObject = asset.GetComponent<BuildableObject>();
			buildableObject.dontSerialize = true;
			buildableObject.isPreview = true;

			FlatRide flatRide = asset.GetComponent<FlatRide>();
			AssetManager.Instance.registerObject(flatRide);

			AddBoundingBox(asset, flatRide.xSize, flatRide.zSize);
		}

		public void BasicFlatRideSettings(FlatRide flatRide, string name, float price, float excitement, float intensity, float nausea, int x, int Z)
		{
			_sceneryObjects.Add(flatRide);
			flatRide.entranceGO = AssetManager.Instance.attractionEntranceGO;
			flatRide.exitGO = AssetManager.Instance.attractionExitGO;
			flatRide.entranceExitBuilderGO = AssetManager.Instance.flatRideEntranceExitBuilderGO;
			flatRide.price = price;
			flatRide.excitementRating = excitement;
			flatRide.intensityRating = intensity;
			flatRide.nauseaRating = nausea;
			flatRide.categoryTag = "Attractions/Flat Ride";
			flatRide.setDisplayName(name);
			flatRide.xSize = x;
			flatRide.zSize = Z;
		}

		public GameObject LoadAsset(string PrefabName)
		{
			try
			{
				GameObject asset = new GameObject();

				char dsc = System.IO.Path.DirectorySeparatorChar;

				using (WWW www = new WWW("file://" + Path + dsc + "AssetBundles" + dsc + GetAssetBundleName()))
				{
					if (www.error != null)
						throw new Exception("Loading had an error:" + www.error);

					AssetBundle bundle = www.assetBundle;

					try
					{
						asset = Instantiate(bundle.LoadAsset(PrefabName)) as GameObject;

						return asset;
					}
					catch (Exception e)
					{
						LogException(e);
						//return null;
					}
					finally
					{
						bundle.Unload(false);
					}
					return null;
				}
			}
			catch (Exception e)
			{
				LogException(e);
				return null;
			}
		}

		public void SetWaypoints(GameObject asset, bool debug)
		{
			Waypoints points = asset.GetComponent<Waypoints>();
			float spacingAmount = 1.0f;

			Debug.Log(asset.GetComponentsInChildren<BoxCollider>().Length);

			Dictionary<KeyValuePair<float, float>, Waypoint> waypoints = new Dictionary<KeyValuePair<float, float>, Waypoint>();

			for (float x = -2.5f; x <= 2.5f; x += spacingAmount)
			{
				for (float y = -2.5f; y <= 2.5f; y += spacingAmount)
				{
					Vector3 localPosition = new Vector3(x, 0, y);
					if (Physics.Raycast(asset.transform.position + localPosition + (Vector3.up * 3.0f), Vector3.down, 3.0f))
					{
						continue;
					}
					Waypoint wp = new Waypoint();
					wp.localPosition = localPosition;
					wp.isRabbitHoleGoal = x < 1.0f && y < 1.0f && x > -1.0f && y > -1.0f;
					waypoints.Add(new KeyValuePair<float, float>(x, y), wp);

					points.waypoints.Add(wp);
				}
			}

			foreach (KeyValuePair<KeyValuePair<float, float>, Waypoint> pair in waypoints)
			{
				bool outer = false;
				for (float x = -spacingAmount; x <= spacingAmount; x += spacingAmount)
				{
					for (float y = -spacingAmount; y <= spacingAmount; y += spacingAmount)
					{
						if (x == 0 && y == 0)
						{
							continue;
						}

						Waypoint otherWp;

						waypoints.TryGetValue(
							new KeyValuePair<float, float>(pair.Value.localPosition.x + x, pair.Value.localPosition.z + y),
							out otherWp);

						if (otherWp != null)
						{
							pair.Value.connectedTo.Add(points.waypoints.FindIndex(a => a == otherWp));
						}
						else
						{
							outer = true;
						}
					}
				}

				pair.Value.isOuter = outer;
			}

			if (debug)
			{
				foreach (Waypoint waypoint in points.waypoints)
				{
					Vector3 worldPosition = waypoint.getWorldPosition(asset.transform);
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					Renderer cubeRenderer = cube.GetComponent<Renderer>();
					if (waypoint.isRabbitHoleGoal)
					{
						cubeRenderer.material.color = Color.green;
					}
					else if (waypoint.isOuter)
					{
						cubeRenderer.material.color = Color.yellow;
					}
					else
					{
						cubeRenderer.material.color = Color.red;
					}
					cube.transform.parent = asset.transform;
					cube.transform.position = worldPosition + (Vector3.up * 2.0f);
				}
			}
		}

		public void AddBoundingBox(GameObject asset, float x, float z)
		{
			BoundingBox bb = asset.AddComponent<BoundingBox>();
			bb.isStatic = false;
			bb.layers = BoundingVolume.Layers.Buildvolume;
			Bounds b = new Bounds();
			b.center = new Vector3(0, 1, 0);
			b.size = new Vector3(x - .01f, 2, z - .01f);
			bb.setBounds(b);
			bb.isStatic = true;
		}

		/*public void SetColors(GameObject asset, Color[] c)
		{
			CustomColors cc = asset.AddComponent<CustomColors>();
			cc.setColors(c);

			foreach (Material material in AssetManager.Instance.objectMaterials)
			{
				if (material.name == "CustomColorsDiffuse")
				{
					asset.GetComponentInChildren<Renderer>().sharedMaterial = material;

					// Go through all child objects and recolor		
					Renderer[] renderCollection;
					renderCollection = asset.GetComponentsInChildren<Renderer>();

					foreach (Renderer render in renderCollection)
					{
						render.sharedMaterial = material;
					}

					break;
				}
			}
		}*/

		private void LogException(Exception e)
		{
			StreamWriter sw = File.AppendText(Path + @"/mod.log");

			sw.WriteLine(e);

			sw.Flush();

			sw.Close();
		}

		public void UnloadScenery()
		{
			foreach (BuildableObject deco in _sceneryObjects)
			{
				AssetManager.Instance.unregisterObject(deco);
				DestroyImmediate(deco.gameObject);
			}
		}

		/*public Color ConvertColor(int r, int g, int b)
		{
			return new Color(r / 255f, g / 255f, b / 255f);
		}*/
	}
}

