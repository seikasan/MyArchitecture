using MyArchitecture.Core;

namespace MyArchitecture.Feature.Rhythm
{
    public partial class RhythmScoreModel : Model
    {
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int MaxCombo { get; private set; }
        public double Accuracy { get; private set; }
        public double Gauge { get; private set; }
        public int PerfectCount { get; private set; }
        public int GreatCount { get; private set; }
        public int GoodCount { get; private set; }
        public int BadCount { get; private set; }
        public int MissCount { get; private set; }
        public int TotalJudged => Snapshot().TotalJudged;

        public void ResetScore()
        {
            ApplySnapshot(RhythmScoreSnapshot.Empty());
        }

        public void ApplySnapshot(RhythmScoreSnapshot snapshot)
        {
            Score = snapshot.Score;
            Combo = snapshot.Combo;
            MaxCombo = snapshot.MaxCombo;
            Accuracy = snapshot.Accuracy;
            Gauge = snapshot.Gauge;
            PerfectCount = snapshot.PerfectCount;
            GreatCount = snapshot.GreatCount;
            GoodCount = snapshot.GoodCount;
            BadCount = snapshot.BadCount;
            MissCount = snapshot.MissCount;
        }

        public RhythmScoreSnapshot Snapshot()
        {
            return new RhythmScoreSnapshot(
                Score,
                Combo,
                MaxCombo,
                Accuracy,
                Gauge,
                PerfectCount,
                GreatCount,
                GoodCount,
                BadCount,
                MissCount);
        }
    }
}
