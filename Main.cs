using UnityEngine;

namespace ParkitectMods.FlatRides.Slammer
{
	public class Main : IMod
	{
		private GameObject _go;

		public void onEnabled()
		{
			_go = new GameObject();

			_go.AddComponent<SlammerLoad>();
			_go.GetComponent<SlammerLoad>().Path = Path;
			_go.GetComponent<SlammerLoad>().Identifier = Identifier;
			_go.GetComponent<SlammerLoad>().LoadFlatRide();
		}

		public void onDisabled()
		{
			_go.GetComponent<SlammerLoad>().UnloadScenery();

			Object.Destroy(_go);
		}

		/// <summary>
		///     Gets the name of this instance.
		/// </summary>
		public string Name
		{
			get
			{
				return "Slammer";
			}
		}

		/// <summary>
		///     Gets the description of this instance.
		/// </summary>
		public string Description
		{
			get
			{
				return "An S&S Fly Squat Clone based on Slammer @ Thorpe park";
			}
		}

		private string _identifier;
		/// <summary>
		///     Gets an unique identifier of this mod.
		/// <summary>
		public string Identifier
		{
			get
			{
				return _identifier;
			}
			set
			{
				_identifier = value;
			}
		}

		private string _path;
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}
	}
}