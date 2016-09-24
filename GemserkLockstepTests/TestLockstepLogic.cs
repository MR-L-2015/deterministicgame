﻿using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;


public class TestLockstepLogic {

	[Test]
	public void LockstepTurnShouldNotAdvanceIfWaitingForActions(){

//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.GameFramesPerLockstep = 1;
		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady (0).ReturnsForAnyArgs (false);

//		LockstepGameLogic lockstepGameLogic = new LockstepGameLogic (gameLogic, pendingCommands);

		lockstepGameLogic.GameFramesPerLockstep = 1;

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (0));

		lockstepLogic.IsReady (0).ReturnsForAnyArgs (true);

		lockstepGameLogic.Update (0.1f);

		Assert.That (gameLogic.lastFrame, Is.EqualTo (1));
	}

	[Test]
	public void TestNextLockstepFrame(){

		//		var gameLogic = NSubstitute.Substitute.For<DeterministicGameLogic> ();

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.GameFramesPerLockstep = 4;
		lockstepGameLogic.FixedStepTime = 0.1f;

		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady (0).Returns (true);

		Assert.That (lockstepGameLogic.GetNextLockstepFrame (0), Is.EqualTo (8));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (1), Is.EqualTo (8));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (2), Is.EqualTo (8));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (4), Is.EqualTo (12));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (5), Is.EqualTo (12));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (6), Is.EqualTo (12));
		Assert.That (lockstepGameLogic.GetNextLockstepFrame (8), Is.EqualTo (16));
	}

	[Test]
	public void LockstepLogicShouldNotProcessAgainIfNoFixedGameStep(){

		var gameLogic = new TestGameStep.GameStepEngineMock ();

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);
		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.GameFramesPerLockstep = 2;
		lockstepGameLogic.SetGameLogic (gameLogic);

		lockstepLogic.IsReady (0).ReturnsForAnyArgs (true);

		lockstepGameLogic.Update (0.1f);
		lockstepGameLogic.Update (0.1f);

		Assert.That (lockstepGameLogic.IsLockstepTurn(), Is.True);

		lockstepGameLogic.Update (0.002f);

		Assert.That (lockstepGameLogic.IsLockstepTurn(), Is.False);
	}

	[Test]
	public void TestGetLockstepFrameForCurrentFrame(){

		var lockstepLogic = NSubstitute.Substitute.For<LockstepLogic> ();

		LockstepFixedUpdate lockstepGameLogic = new LockstepFixedUpdate (lockstepLogic);

		lockstepGameLogic.FixedStepTime = 0.1f;
		lockstepGameLogic.GameFramesPerLockstep = 4;

		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (0), Is.EqualTo (false));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (1), Is.EqualTo (false));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (2), Is.EqualTo (false));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (3), Is.EqualTo (true));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (7), Is.EqualTo (true));
		Assert.That (lockstepGameLogic.IsLastFrameForNextLockstep (8), Is.EqualTo (false));
	}

		
}
