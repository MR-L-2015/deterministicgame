﻿using NUnit.Framework;

public class TestGameStep {

	public class GameStepEngineMock : DeterministicGameLogic
	{
		public float lastDt;
		public int lastFrame;

		#region GameStepEngine implementation
		public void Update (float dt, int frame)
		{
			lastDt = dt;
			lastFrame = frame;
		}
		#endregion
	}

	[Test]
	public void TestFixedGameStepTwoUpdates()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();

		gameFixedUpdate.FixedStepTime = 0.05f;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (0.01f);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (0));

		gameFixedUpdate.Update (0.045f);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepGreaterThanFixedTime()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();

		gameFixedUpdate.FixedStepTime = 0.05f;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (0.051f);

		Assert.That (gameFixedUpdate.CurrentGameFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestFixedGameStepEngineCalled()
	{
		GameFixedUpdate gameFixedUpdate = new GameFixedUpdate ();
		GameStepEngineMock gameStepEngine = new GameStepEngineMock ();

		gameFixedUpdate.SetGameLogic (gameStepEngine);

		gameFixedUpdate.FixedStepTime = 0.05f;

		gameFixedUpdate.Init ();

		gameFixedUpdate.Update (0.078f);

		Assert.That (gameStepEngine.lastDt, Is.EqualTo (0.05f));
		Assert.That (gameStepEngine.lastFrame, Is.EqualTo (0));
	}

	[Test]
	public void TestLockstepImplementation()
	{
		IntervalDeterministicGameLogic lockstepImplementation = new IntervalDeterministicGameLogic ();

		lockstepImplementation.GameFramesPerLockstepFrame = 2;

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		lockstepImplementation.Update (0.01f, 0);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		try {
			lockstepImplementation.Update (0.01f, 2);
		} catch {
			Assert.Pass ();
			return;
		}

		Assert.Fail ();

		// Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestLockstepImplementation2()
	{
		IntervalDeterministicGameLogic lockstepImplementation = new IntervalDeterministicGameLogic ();

		lockstepImplementation.GameFramesPerLockstepFrame = 2;

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (0));

		lockstepImplementation.Update (0.01f, 0);
		lockstepImplementation.Update (0.01f, 1);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (1));

		lockstepImplementation.Update (0.01f, 2);
		lockstepImplementation.Update (0.01f, 3);

		Assert.That (lockstepImplementation.CurrentLockstepFrame, Is.EqualTo (2));
	}

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){

//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new GameStepEngineMock ();

		CommandsList pendingCommands = new CommandsList ();
		pendingCommands.IsReady = false;

		LockstepGameLogic lockstepGameLogic = new LockstepGameLogic (gameLogic, pendingCommands);

		lockstepGameLogic.GameFramesPerLockstep = 1;

		lockstepGameLogic.Update (0.1f, 0);
		lockstepGameLogic.Update (0.1f, 1);
		lockstepGameLogic.Update (0.1f, 2);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

		pendingCommands.IsReady = true;

		lockstepGameLogic.Update (0.1f, 3);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
	}
}