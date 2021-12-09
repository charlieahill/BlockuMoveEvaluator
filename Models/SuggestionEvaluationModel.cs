using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlockuMoveEvaluator.Models
{
    [Serializable]
    public class SuggestionEvaluationModel
    {
        public Point Location { get; set; }
        public double improvementOnSpacesFactor { get; set; }
        public double numberOfHolesFactor { get; set; }
        public double TraditionalHoleSizeScoreFactor { get; set; }
        public double WeightedHoleSizeScoreFactor { get; set; }
        public double WeightedBlocksTouchingScoreFactor { get; set; }
        public double AveragePointsPerAreaFactor { get; set; }
        public int improvementOnSpaces { get; set; }
        public int numberOfHoles { get; set; }
        public List<int> HoleSizes { get; set; }
        public int TraditionalHoleSizesScore { get; set; }
        public int WeighedHoleSizesScore { get; set; }
        public int WeighedBlocksTouchingScore { get; set; }
        public double AveragePointsPerArea { get; set; }
        public int score { get; set; }
        public override string ToString()
        {
            return $"({Location.X}, {Location.Y}): {score}";
        }
    }
}
