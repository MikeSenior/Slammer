using UnityEngine;

namespace ParkitectMods.FlatRides.Slammer
{
	public class SlammerLoad : FlatRideLoader
	{
		protected override GameObject LoadRideModel()
		{
			//GameObject asset = GameObject.CreatePrimitive(PrimitiveType.Cube);
			GameObject asset = LoadAsset("Slammer");
			return asset;
		}

		protected override void InitializeRideData(GameObject asset)
		{
			RabbitHole rabbitHole = asset.AddComponent<RabbitHole>();
			rabbitHole.seatCount = 40;
			Slammer slammer = asset.AddComponent<Slammer>();

			BasicFlatRideSettings(slammer, "Slammer", 600, .8f, .2f, .1f, 6, 6);
		}

		protected override string GetAssetBundleName()
		{
			return "Slammer";
		}
	}
}

