/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace Pong.Framework.Game.States
{
    internal class GameState : IState<GameState>
    {
        public bool BallCollidedLastFrame { get; set; }
        public bool Starting { get; set; } = true;
        public int StartTimer { get; set; } = 180;

        public ScoreState ScoreState { get; set; } = new ScoreState();
        public VelocityState BallVelocityState { get; set; } = new VelocityState();
        public PositionState BallPositionState { get; set; } = new PositionState();
        public PositionState PaddlePositionState { get; set; } = new PositionState();

        public double CurrentTime;

        public GameState()
        {
            this.Reset();
        }

        public bool All = true;

        public void SetState(GameState state)
        {
            if (this.All)
            {
                this.BallCollidedLastFrame = state.BallCollidedLastFrame;
                this.Starting = state.Starting;
                this.StartTimer = state.StartTimer;

                this.BallVelocityState.SetState(state.BallVelocityState);
                this.ScoreState.SetState(state.ScoreState);
                this.BallPositionState.SetState(state.BallPositionState);
            }
            this.PaddlePositionState.SetState(state.PaddlePositionState);
        }

        public void Reset()
        {
            this.BallCollidedLastFrame = false;
            this.Starting = true;
            this.StartTimer = 180;

            this.ScoreState.Reset();
            this.BallVelocityState.Reset();
            this.BallPositionState.Reset();
            this.PaddlePositionState.Reset();
        }
    }
}
