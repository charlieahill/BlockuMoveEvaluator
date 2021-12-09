using BlockuMoveEvaluator.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlockuMoveEvaluator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MoveEvaluationModel Moves { get; set; } = new MoveEvaluationModel();
        public int CurrentDisplyIndex { get; set; }
        public string CurrentFilename { get; set; }
        public List<SuggestionEvaluationModel> Comparison { get; set; } = new List<SuggestionEvaluationModel>();

        public MainWindow()
        {
            InitializeComponent();
            ResetDisplay();
        }

        private void ResetDisplay()
        {
            SetBlock(0, 0, Brushes.SkyBlue);
            SetBlock(0, 1, Brushes.White);
            SetBlock(0, 2, Brushes.SkyBlue);
            SetBlock(1, 0, Brushes.White);
            SetBlock(1, 1, Brushes.SkyBlue);
            SetBlock(1, 2, Brushes.White);
            SetBlock(2, 0, Brushes.SkyBlue);
            SetBlock(2, 1, Brushes.White); 
            SetBlock(2, 2, Brushes.SkyBlue);
        }

        private void SetBlock(int x, int y, SolidColorBrush borderColor)
        {
            for (int i = 3*x; i < 3*x + 3; i++)
            {
                for (int j = 3*y; j < 3*y + 3; j++)
                {
                    ((Border)DisplayGrid.Children[ConvertTo1D(i, j)]).Background = borderColor;
                }
            }
        }

        private int ConvertTo1D(int i, int j)
        {
            return (j * 9) + i;
        }

        private Point ConvertTo2D(int value)
        {
            return new Point(Math.Floor(value / 9.0), value % 9);
        }

        private void DrawDisplay(List<List<int>> baseValues, List<List<int>> additionalValues, Point additionalValuesOffset, int numberOfSuggestions)
        {
            ResetDisplay();

            if (baseValues == null)
                return;

            //draw the fixed items
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (baseValues[i][j] == 1)
                        ((Border)DisplayGrid.Children[ConvertTo1D(j, i)]).Background = Brushes.Black;
                }
            }

            //draw the additional items
            for (int i = 0; i < additionalValues.Count(); i++)
            {
                for (int j = 0; j < additionalValues[0].Count(); j++)
                {
                    if (additionalValues[i][j] == 1)
                    {
                        if (((Border)DisplayGrid.Children[ConvertTo1D(i + (int)additionalValuesOffset.Y, j + (int)additionalValuesOffset.X)]).Background == Brushes.SlateBlue)
                            MessageBox.Show("Overlay Error.");
                        else
                            ((Border)DisplayGrid.Children[ConvertTo1D(i + (int)additionalValuesOffset.Y, j + (int)additionalValuesOffset.X)]).Background = Brushes.Red;
                    }
                }
            }

            UpdateStatisticalDataLabels(numberOfSuggestions, Moves.Suggestions[CurrentDisplyIndex]);
        }

        private void UpdateStatisticalDataLabels(int numberOfSuggestions, SuggestionEvaluationModel Suggestion)
        {
            //Suggestion shuffler
            DisplayedSuggestionIndex.Text = $"{CurrentDisplyIndex} / {numberOfSuggestions}";

            //Location
            LocationPoint.Text = $"Location ({Suggestion.Location.X}, {Suggestion.Location.Y})";

            //Improvement on spaces
            ImprovementOnSpaces1.Text = $"({(Suggestion.improvementOnSpaces * Suggestion.improvementOnSpacesFactor).TwoDP()})";
            ImprovementOnSpaces3.Text = $"({Suggestion.improvementOnSpaces.TwoDP()} x {Suggestion.improvementOnSpacesFactor.TwoDP()})";

            //Number of holes (hole count)
            NumberOfHoles1.Text = $"({(Suggestion.numberOfHoles * Suggestion.numberOfHolesFactor).TwoDP()})";
            NumberOfHoles3.Text = $"({Suggestion.numberOfHoles.TwoDP()} x {Suggestion.numberOfHolesFactor.TwoDP()})";

            //Traditional hole score
            TraditionalHoles1.Text = $"({(Suggestion.TraditionalHoleSizesScore * Suggestion.TraditionalHoleSizeScoreFactor).TwoDP()})";
            TraditionalHoles3.Text = $"({Suggestion.TraditionalHoleSizesScore.TwoDP()} x {Suggestion.TraditionalHoleSizeScoreFactor.TwoDP()})";

            //Weighted hole score
            WeightedHoles1.Text = $"({(Suggestion.WeighedHoleSizesScore * Suggestion.WeightedHoleSizeScoreFactor).TwoDP()})";
            WeightedHoles3.Text = $"({Suggestion.WeighedHoleSizesScore.TwoDP()} x {Suggestion.WeightedHoleSizeScoreFactor.TwoDP()})";

            //Weighted touching blocks
            WeightedBlocks1.Text = $"({(Suggestion.WeighedBlocksTouchingScore * Suggestion.WeightedBlocksTouchingScoreFactor).TwoDP()})";
            WeightedBlocks3.Text = $"({Suggestion.WeighedBlocksTouchingScore.TwoDP()} x {Suggestion.WeightedBlocksTouchingScoreFactor.TwoDP()})";

            //Average points per area
            AvgPointsPerArea1.Text = $"({(Suggestion.AveragePointsPerArea * Suggestion.AveragePointsPerAreaFactor).TwoDP()})";
            AvgPointsPerArea3.Text = $"({Suggestion.AveragePointsPerArea.TwoDP()} x {Suggestion.AveragePointsPerAreaFactor.TwoDP()})";

            //total score
            TotalScore1.Text = $"{Suggestion.score.TwoDP()}";
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true)
                return;

            CurrentFilename = openFileDialog.FileName;
            Moves = SaveLoad.LoadSuggestionFromXML(CurrentFilename);
            CurrentDisplyIndex = GetIndexOfSuggestion(Moves.Suggestions, Moves.SuggestedMovePoint);
            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, Moves.SuggestedMovePoint, Moves.Suggestions.Count());
            RefreshComparisonsList();

            Title = $"MoveViz - {System.IO.Path.GetFileName(CurrentFilename)}";
            RefreshComparisonsList(true);
            PreviousFile_Button.IsEnabled = true;
            NextFile_Button.IsEnabled = true;
        }

        private void RefreshComparisonsList(bool ClearComparisonList = false)
        {
            if (ClearComparisonList)
                Comparison = new List<SuggestionEvaluationModel>();

            ComparisonList.ItemsSource = null;
            ComparisonList.ItemsSource = Comparison;
        }

        private int GetIndexOfSuggestion(List<SuggestionEvaluationModel> suggestions, Point suggestedMovePoint)
        {
            int index = 0;

            foreach (SuggestionEvaluationModel suggestion in suggestions)
            {
                if (suggestion.Location.X == suggestedMovePoint.X && suggestion.Location.Y == suggestedMovePoint.Y)
                    return index;

                index++;
            }

            MessageBox.Show("This move is not allowed.");
            return -1;
        }

        private void PreviousSuggestion_Click(object sender, RoutedEventArgs e)
        {
            if (Moves.Suggestions.Count == 0)
                return;

            CurrentDisplyIndex--;

            if (CurrentDisplyIndex == -1)
                CurrentDisplyIndex = Moves.Suggestions.Count - 1;

            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, Moves.Suggestions[CurrentDisplyIndex].Location, Moves.Suggestions.Count());
        }

        private void NextSuggestion_Click(object sender, RoutedEventArgs e)
        {
            if (Moves.Suggestions.Count == 0)
                return;

            CurrentDisplyIndex++;

            if (CurrentDisplyIndex == Moves.Suggestions.Count)
                CurrentDisplyIndex = 0;

            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, Moves.Suggestions[CurrentDisplyIndex].Location, Moves.Suggestions.Count());

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point ClickPoint = ConvertTo2D(int.Parse(((Border)sender).Tag.ToString()));

            if (ClickPoint.Y + Moves.NextPiece.Count() > 9 || ClickPoint.X + Moves.NextPiece[0].Count > 9)
            {
                MessageBox.Show("This move is not a legal move.");
                return;
            }

            int suggestionIndex = GetIndexOfSuggestion(Moves.Suggestions, ClickPoint);

            if (suggestionIndex == -1)
                return;

            CurrentDisplyIndex = suggestionIndex;

            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, ClickPoint, Moves.Suggestions.Count());
        }

        private void LoadPreviousFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(IncreaseFileName(CurrentFilename, -1)))
            {
                MessageBox.Show("No more moves found for this game.");
                return;
            }

            CurrentFilename = IncreaseFileName(CurrentFilename, -1);

            Moves = SaveLoad.LoadSuggestionFromXML(CurrentFilename);
            CurrentDisplyIndex = GetIndexOfSuggestion(Moves.Suggestions, Moves.SuggestedMovePoint);
            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, Moves.SuggestedMovePoint, Moves.Suggestions.Count());
            Title = $"MoveViz - {System.IO.Path.GetFileName(CurrentFilename)}";
            RefreshComparisonsList(true);
        }

        private void LoadNextFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(IncreaseFileName(CurrentFilename, 1)))
            {
                MessageBox.Show("No more moves found for this game.");
                return;
            }

            CurrentFilename = IncreaseFileName(CurrentFilename, 1);
            
            Moves = SaveLoad.LoadSuggestionFromXML(CurrentFilename);
            CurrentDisplyIndex = GetIndexOfSuggestion(Moves.Suggestions, Moves.SuggestedMovePoint);
            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, Moves.SuggestedMovePoint, Moves.Suggestions.Count());
            Title = $"MoveViz - {System.IO.Path.GetFileName(CurrentFilename)}";
            RefreshComparisonsList(true);
        }

        private string IncreaseFileName(string filename, int increment)
        {
            string directory = System.IO.Path.GetDirectoryName(filename);

            filename = filename.Substring(directory.Length);

            int preambleLength = 20;
            int extensionLength = 4;
            string preamble = filename.Substring(0, preambleLength);

            int currentVal = int.Parse(filename.Substring(preambleLength, filename.Length - preambleLength - extensionLength));

            string extension = System.IO.Path.GetExtension(filename);

            return $"{directory}{preamble}{currentVal + increment}{extension}";
        }

        private void AddCurrentSuggestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!MoveAlreadyInList(Moves.Suggestions[CurrentDisplyIndex].Location, Comparison.Select(x => x.Location).ToList()))
                Comparison.Add(Moves.Suggestions[CurrentDisplyIndex]);
            
            RefreshComparisonsList();
        }

        private bool MoveAlreadyInList(Point location, List<Point> comparison)
        {
            foreach (Point point in comparison)
            {
                if (point.X == location.X && point.Y == location.Y)
                    return true;
            }

            return false;
        }

        private void PreviousComparisonButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComparisonList.SelectedItem == null)
                return;

            int index = ComparisonList.SelectedIndex;

            index--;
            if (index < 0)
                index = ComparisonList.Items.Count - 1;

            ComparisonList.SelectedIndex = index;
            ComparisonList.Focus();
        }

        private void NextComparisonButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComparisonList.SelectedItem == null)
                return;

            int index = ComparisonList.SelectedIndex;

            index++;
            if (index > ComparisonList.Items.Count - 1)
                index = 0;

            ComparisonList.SelectedIndex = index;
            ComparisonList.Focus();
        }

        private void RemoveSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComparisonList.SelectedItem == null)
                return;

            Comparison.Remove((SuggestionEvaluationModel)ComparisonList.SelectedItem);
            RefreshComparisonsList();
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshComparisonsList(true);
        }

        private void ComparisonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComparisonList.SelectedItem == null)
                return;

            int suggestionIndex = GetIndexOfSuggestion(Moves.Suggestions, ((SuggestionEvaluationModel)ComparisonList.SelectedItem).Location);

            if (suggestionIndex == -1)
                return;

            CurrentDisplyIndex = suggestionIndex;

            DrawDisplay(Moves.StartingBoard, Moves.NextPiece, ((SuggestionEvaluationModel)ComparisonList.SelectedItem).Location, Moves.Suggestions.Count());
        }

        //todo: Comparison mode
        //todo: Add heat maps (for scores, but also for individual items)
        //todo: Add some kind of sliding scale / calculator to work out when the suggestion would change

        //done: Add buttons to increase and decrease the move number
        //done: find the suggestion where top left corner is clicked
        //done: Display the score data
        //done: Add left and right arrows to buttons
    }
}
