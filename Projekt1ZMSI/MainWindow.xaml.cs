using System;
using System.Collections.Generic;
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
using System.Windows.Controls.Primitives;
using System.IO;
using System.Globalization;

namespace Projekt1ZMSI
{
    public enum ResultType { None, Static, CreatesCycle, GoesToPoint, GoesToCycle };

    public class SingleIterationResult
    {
        public DMU.Math.Matrix matrixResult;
        public DMU.Math.Matrix matrixResultBiPolar;
        public double energy;
    }

    public class TestDataAndFields
    {
        public DMU.Math.Matrix startingValue;
        public DMU.Math.Matrix currentValue;
        public List<SingleIterationResult> iterationHistory;
        public List<SingleIterationResult> foundPattern;
        public ResultType resultType;
        public int index;
        public bool synchronousMode;

        public List<Label> resultValueLabels;
        public Label resultLabel;
        public Button moreButton;
    }

    public class PointAnalysisResult
    {
        public DMU.Math.Matrix point;
        public int amountOfStaticResult;
        public int amountOfGoesToStaticResult;
        public int amountOfCreatesCycleResult;
        public int amountOGoesToCycleResult;
        public List<Label> labelResults;
    }

    public partial class MainWindow : Window
    {
        int[] asynchronousOrder;
        bool passedWeightsMode = true;
        bool passedWeightsModeHasResults = false;
        bool randomWeightsModeHasResults = false;
        bool randomWeightsSynchronousMode = true;

        List<TestDataAndFields> synchronousTestDataAndFields;
        List<TestDataAndFields> asynchronousTestDataAndFields;
        DMU.Math.Matrix passedWeightsMatrix;

        List<DMU.Math.Matrix> randomWeigthMatrixes;
        List<List<TestDataAndFields>> randomWeightsMatrixesAnalysisResults;
        List<PointAnalysisResult> pointAnalysisResults;

        public MainWindow ()
        {
            InitializeComponent();

            CultureInfo.CurrentCulture = new CultureInfo("pl-PL");
            CultureInfo.CurrentUICulture = new CultureInfo("pl-PL");

            string[] modes = new string[2] { "Tryb synchroniczny", "Tryb asynchroniczny" };
            bool synchronousMode = true;
            synchronousTestDataAndFields = new List<TestDataAndFields>();
            asynchronousTestDataAndFields = new List<TestDataAndFields>();
            pointAnalysisResults = new List<PointAnalysisResult>();
            randomWeigthMatrixes = new List<DMU.Math.Matrix>();
            randomWeightsMatrixesAnalysisResults = new List<List<TestDataAndFields>>();

            List<TestDataAndFields> targetTestDataAndFields = synchronousTestDataAndFields;
            for (int j = 0; j < modes.Length; j++) {
                GroupBox modeGroupBox = new GroupBox();
                {
                    modeGroupBox.Header = modes[j];
                    modeGroupBox.Content = LayoutGenerationHelpers.createTestDataAndFieldsReturnUniformGridContainingResultElements(synchronousMode, ref targetTestDataAndFields);
                }
                resultsPassedWeights.Children.Add(modeGroupBox);
                targetTestDataAndFields = asynchronousTestDataAndFields;
                synchronousMode = false;
            }

            foreach (TestDataAndFields testDataAndFields in synchronousTestDataAndFields) {
                PointAnalysisResult pointAnalysisResult = new PointAnalysisResult();
                pointAnalysisResult.point = testDataAndFields.startingValue.Clone();
                pointAnalysisResult.labelResults = new List<Label>();
                pointAnalysisResults.Add(pointAnalysisResult);
            }
        }

        private bool checkAndSaveAsychrnonousActivationOrder ()
        {
            bool result = true;

            asynchronousOrder = new int[] { 0, 0, 0 };

            IEnumerable<TextBox> neuronActivationOrderTextBoxes = neuronActivationOrder.Children.OfType<TextBox>();
            for (int i = 0; i < 3; i++) {
                TextBox textBox = neuronActivationOrderTextBoxes.ElementAt(i);
                textBox.Background = Brushes.White;
                int value = 0;

                if (int.TryParse(textBox.Text, out value)) {
                    if (value == 1 || value == 2 || value == 3) {
                        bool foundCollision = false;

                        for (int j = i; j >= 0; j--) {
                            if (value == asynchronousOrder[j]) {
                                foundCollision = true;
                            }
                        }

                        if (foundCollision) {
                            result = false;
                            textBox.Background = Brushes.Red;
                        } else {
                            asynchronousOrder[i] = value;
                        }
                    } else {
                        result = false;
                        textBox.Background = Brushes.Red;
                    }
                } else {
                    result = false;
                    textBox.Background = Brushes.Red;
                }
            }

            return result;
        }

        private void startPassedWeightsAnalysis (object sender, RoutedEventArgs e)
        {
            IEnumerable<TextBox> weightTextBoxes = weights.Children.OfType<TextBox>();
            bool weightsOK = true;
            double[,] weightValues = new double[3, 3];

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    TextBox textBox = weightTextBoxes.ElementAt(i * 3 + j);
                    textBox.Background = Brushes.White;
                    double value = 0;

                    if (double.TryParse(textBox.Text, out value)) {
                        weightValues[i, j] = value;
                    } else {
                        weightsOK = false;
                        textBox.Background = Brushes.Red;
                    }
                }
            }

            passedWeightsMatrix = new DMU.Math.Matrix(weightValues);
            bool isWeightsMatrixSymetric = Calculations.isMatrixSymmetric(passedWeightsMatrix);

            if (weightsOK && checkAndSaveAsychrnonousActivationOrder()) {
                Calculations.runSynchronousAnalysisForTestDataListWithWeights(ref synchronousTestDataAndFields, ref passedWeightsMatrix, isWeightsMatrixSymetric);
                Calculations.runAsynchronousAnalysisForTestDataListWithWeights(ref asynchronousTestDataAndFields, ref passedWeightsMatrix, asynchronousOrder);

                passedWeightsModeHasResults = true;
            }
        }

        public static void displayMoreInformation (object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            TestDataAndFields testDataAndFields = (TestDataAndFields)button.Tag;

            MoreWindow moreWindow = new MoreWindow();
            moreWindow.displayInformation(testDataAndFields);
            moreWindow.Show();
        }

        private void saveResultsToTextFile (object sender, RoutedEventArgs e)
        {
            if (passedWeightsMode) {
                if (passedWeightsModeHasResults) {
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                    saveFileDialog.DefaultExt = "txt";
                    saveFileDialog.FileName = "wyniki";
                    saveFileDialog.Filter = "Plik tekstowy (*.txt)|*.txt";

                    if (saveFileDialog.ShowDialog() == true) {
                        string text = DataConverters.convertResultsToString(synchronousTestDataAndFields, passedWeightsMatrix, "Badanie w trybie synchronicznym. Wektor I = [" + DMU.Math.Matrix.Transpose(Calculations.matrixI).ToString("F1", " ", "") + "]");
                        text += "\n############################\n";
                        text += DataConverters.convertResultsToString(asynchronousTestDataAndFields, passedWeightsMatrix, "Badanie w trybie asynchronicznym. Kolejność aktywacji neuronów: " + asynchronousOrder[0] + ", " + asynchronousOrder[1] + ", " + asynchronousOrder[2]);

                        File.WriteAllText(saveFileDialog.FileName, text);
                    }
                }
            } else {
                if (randomWeightsModeHasResults) {
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                    saveFileDialog.DefaultExt = "txt";
                    saveFileDialog.FileName = "wyniki";
                    saveFileDialog.Filter = "Plik tekstowy (*.txt)|*.txt";

                    if (saveFileDialog.ShowDialog() == true) {
                        string text = "";

                        for (int i = 0; i < randomWeightsMatrixesAnalysisResults.Count; i++) {
                            List<TestDataAndFields> listOfTestDataAndFields = randomWeightsMatrixesAnalysisResults[i];
                            DMU.Math.Matrix weightsMatrix = randomWeigthMatrixes[i];
                            string title = "";

                            if (randomWeightsSynchronousMode) {
                                title = "Badanie w trybie synchronicznym. Wektor I = [" + DMU.Math.Matrix.Transpose(Calculations.matrixI).ToString("F1", " ", "") + "]";
                            } else {
                                title = "Badanie w trybie asynchronicznym. Kolejność aktywacji neuronów: " + asynchronousOrder[0] + ", " + asynchronousOrder[1] + ", " + asynchronousOrder[2];
                            }

                            text += DataConverters.convertResultsToString(listOfTestDataAndFields, weightsMatrix, title);

                            if (i + 1 < randomWeightsMatrixesAnalysisResults.Count) {
                                text += "\n############################\n";
                            }
                        }

                        text += "\n######### PODSUMOWANIE #########\n";
                        text += "Liczba losowań: " + randomWeigthMatrixes.Count + "\n\n";
                        text += DataConverters.convertPointAnalysisDataToString(pointAnalysisResults);

                        File.WriteAllText(saveFileDialog.FileName, text);
                    }
                }
            }
        }

        private void setRandomWeightsModeSynchronous (object sender, RoutedEventArgs e)
        {
            randomWeightsSynchronousMode = true;
        }

        private void setRandomWeightsModeAsynchronous (object sender, RoutedEventArgs e)
        {
            randomWeightsSynchronousMode = false;
        }

        private void setPassedWeightsMode (object sender, RoutedEventArgs e)
        {
            passedWeightsMode = true;
            resultsPassedWeights.Visibility = Visibility.Visible;
            passedWeightsParameters.Visibility = Visibility.Visible;
            resultsRandomWeights.Visibility = Visibility.Collapsed;
            randomWeightsParameters.Visibility = Visibility.Collapsed;
        }

        private void setRandomWeightsMode (object sender, RoutedEventArgs e)
        {
            passedWeightsMode = false;
            resultsRandomWeights.Visibility = Visibility.Visible;
            randomWeightsParameters.Visibility = Visibility.Visible;
            resultsPassedWeights.Visibility = Visibility.Collapsed;
            passedWeightsParameters.Visibility = Visibility.Collapsed;
        }

        private void startRandomWeightsAnalysis (object sender, RoutedEventArgs e)
        {
            int amountOfRandomWeightsMatrixes = 0;
            amountOfRandomWeightsMatrixesTextBox.Background = Brushes.White;

            if (randomWeightsSynchronousMode || (!randomWeightsSynchronousMode && checkAndSaveAsychrnonousActivationOrder())) {
                if (int.TryParse(amountOfRandomWeightsMatrixesTextBox.Text, out amountOfRandomWeightsMatrixes)) {
                    if (amountOfRandomWeightsMatrixes > 0) {
                        Random random = new Random();

                        foreach (PointAnalysisResult pointAnalysisResult in pointAnalysisResults) {
                            pointAnalysisResult.labelResults.Clear();
                            pointAnalysisResult.amountOfStaticResult = 0;
                            pointAnalysisResult.amountOfGoesToStaticResult = 0;
                            pointAnalysisResult.amountOfCreatesCycleResult = 0;
                            pointAnalysisResult.amountOGoesToCycleResult = 0;
                        }

                        resultsRandomWeights.Children.Clear();
                        randomWeightsMatrixesAnalysisResults.Clear();
                        randomWeigthMatrixes.Clear();

                        GroupBox modeGroupBox = new GroupBox();
                        {
                            modeGroupBox.Header = randomWeightsSynchronousMode ? "Tryb synchroniczny" : "Tryb asynchroniczny";
                            StackPanel stackPanel = new StackPanel();
                            {
                                GroupBox resultsSummaryGroupBox = new GroupBox();
                                {
                                    resultsSummaryGroupBox.Header = "Podsumowanie";
                                    Grid resultsSummaryGrid = new Grid();
                                    {
                                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(9, 1, ref resultsSummaryGrid);
                                        LayoutGenerationHelpers.addNMStarWideColumnsToGrid(5, 1, ref resultsSummaryGrid);
                                        string[] labelValues0 = new string[9];
                                        string[] labelValues1 = new string[9];
                                        string[] labelValues2 = new string[9];
                                        string[] labelValues3 = new string[9];
                                        string[] labelValues4 = new string[9];
                                        labelValues0[0] = "Wektor";
                                        labelValues1[0] = "Stały";
                                        labelValues2[0] = "Zbieżny";
                                        labelValues3[0] = "Tworzy cykl";
                                        labelValues4[0] = "Wpada w cykl";

                                        int labelIndex = 1;
                                        foreach (PointAnalysisResult pointAnalysisResult in pointAnalysisResults) {
                                            labelValues0[labelIndex] = "[" + DMU.Math.Matrix.Transpose(pointAnalysisResult.point).ToString("F0", " ", "") + "]";
                                            labelValues1[labelIndex] = "" + pointAnalysisResult.amountOfStaticResult;
                                            labelValues2[labelIndex] = "" + pointAnalysisResult.amountOfGoesToStaticResult;
                                            labelValues3[labelIndex] = "" + pointAnalysisResult.amountOfCreatesCycleResult;
                                            labelValues4[labelIndex] = "" + pointAnalysisResult.amountOGoesToCycleResult;
                                            labelIndex++;
                                        }

                                        LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 0, ref resultsSummaryGrid, labelValues0, pointAnalysisResults);
                                        LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 1, ref resultsSummaryGrid, labelValues1, pointAnalysisResults);
                                        LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 2, ref resultsSummaryGrid, labelValues2, pointAnalysisResults);
                                        LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 3, ref resultsSummaryGrid, labelValues3, pointAnalysisResults);
                                        LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 4, ref resultsSummaryGrid, labelValues4, pointAnalysisResults);
                                    }
                                    resultsSummaryGroupBox.Content = resultsSummaryGrid;
                                }
                                stackPanel.Children.Add(resultsSummaryGroupBox);

                                for (int i = 0; i < amountOfRandomWeightsMatrixes; i++) {
                                    Grid grid = new Grid();
                                    {
                                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(1, 1, ref grid);
                                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(1, 3, ref grid);
                                        LayoutGenerationHelpers.addNMStarWideColumnsToGrid(3, 1, ref grid);

                                        DMU.Math.Matrix weightsMatrix = new DMU.Math.Matrix(3, 3);

                                        for (int j = 0; j < 3; j++) {
                                            int k = 0;

                                            while (k <= j) {
                                                weightsMatrix.SetElement(j, k, Math.Round(((random.NextDouble() * 100) - 50), 3));
                                                weightsMatrix.SetElement(k, j, weightsMatrix.GetElement(j, k));
                                                k++;
                                            }
                                        }

                                        GroupBox groupBox = new GroupBox();
                                        {
                                            groupBox.Header = "Wagi";
                                            Grid.SetRow(groupBox, 0);
                                            Grid.SetColumn(groupBox, 1);
                                            UniformGrid weightsMatrixGrid = new UniformGrid();
                                            {
                                                LayoutGenerationHelpers.addNLabelsToUniformGridWithArrayAsContent(9, ref weightsMatrixGrid, weightsMatrix.ToArray());
                                            }
                                            groupBox.Content = weightsMatrixGrid;
                                        }
                                        grid.Children.Add(groupBox);

                                        List<TestDataAndFields> currentTestDataAndFieldsList = new List<TestDataAndFields>();

                                        UniformGrid uniformGrid = LayoutGenerationHelpers.createTestDataAndFieldsReturnUniformGridContainingResultElements(randomWeightsSynchronousMode, ref currentTestDataAndFieldsList);
                                        Grid.SetRow(uniformGrid, 1);
                                        Grid.SetColumnSpan(uniformGrid, 3);
                                        grid.Children.Add(uniformGrid);

                                        randomWeightsMatrixesAnalysisResults.Add(currentTestDataAndFieldsList);

                                        if (randomWeightsSynchronousMode) {
                                            Calculations.runSynchronousAnalysisForTestDataListWithWeights(ref currentTestDataAndFieldsList, ref weightsMatrix, true, pointAnalysisResults);
                                        } else {
                                            Calculations.runAsynchronousAnalysisForTestDataListWithWeights(ref currentTestDataAndFieldsList, ref weightsMatrix, asynchronousOrder, pointAnalysisResults);
                                        }

                                        randomWeigthMatrixes.Add(weightsMatrix);
                                    }
                                    stackPanel.Children.Add(grid);
                                }
                            }
                            modeGroupBox.Content = stackPanel;
                        }
                        resultsRandomWeights.Children.Add(modeGroupBox);

                        foreach (PointAnalysisResult pointAnalysisResult in pointAnalysisResults) {
                            pointAnalysisResult.labelResults[1].Content = pointAnalysisResult.amountOfStaticResult;
                            pointAnalysisResult.labelResults[2].Content = pointAnalysisResult.amountOfGoesToStaticResult;
                            pointAnalysisResult.labelResults[3].Content = pointAnalysisResult.amountOfCreatesCycleResult;
                            pointAnalysisResult.labelResults[4].Content = pointAnalysisResult.amountOGoesToCycleResult;
                        }

                        randomWeightsModeHasResults = true;
                    } else {
                        amountOfRandomWeightsMatrixesTextBox.Background = Brushes.Red;
                    }
                } else {
                    amountOfRandomWeightsMatrixesTextBox.Background = Brushes.Red;
                }
            }
        }
    }
}
