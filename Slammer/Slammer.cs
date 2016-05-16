namespace ParkitectMods.FlatRides.Slammer
{
	public class Slammer : FlatRide
	{
		public new enum State
		{
			Stopped,
			Running,
            Rising,
            Spinning,
            Lowering
		}

		[Serialized]
		public State CurrentState;
		[Serialized]
		public float Time;

		public override void Start()
		{
			guestsCanRaiseArms = false;
			CurrentState = State.Stopped;
			base.Start();
		}

		public override void onStartRide()
		{
			base.onStartRide();
			CurrentState = State.Running;
		}

		public override void tick(StationController stationController)
		{
			if (CurrentState == State.Running)
			{
				Time += UnityEngine.Time.deltaTime;

				if (Time > 60)
				{
					CurrentState = State.Stopped;
					Time = 0;
				}
			}
		}

		public override bool shouldLetGuestsIn(StationController stationController)
		{
			return CurrentState == State.Stopped;
		}

		public override bool shouldLetGuestsOut()
		{
			return CurrentState == State.Stopped;
		}
	}
}
