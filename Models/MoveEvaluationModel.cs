using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlockuMoveEvaluator.Models
{
    [Serializable]
    public class MoveEvaluationModel
    {
        public List<List<int>> StartingBoard { get; set; }
        public List<List<int>> NextPiece { get; set; }
        public Point SuggestedMovePoint { get; set; }
        public List<SuggestionEvaluationModel> Suggestions { get; set; } = new List<SuggestionEvaluationModel>();
    }
}
