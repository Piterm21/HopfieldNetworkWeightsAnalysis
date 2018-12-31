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
        DMU.Math.Matrix matrixI = new DMU.Math.Matrix(new double[] { 0.1, 0.1, 0.1 }, true);
        DMU.Math.Matrix passedWeightsMatrix;

        List<DMU.Math.Matrix> randomWeigthMatrixes;
        List<List<TestDataAndFields>> randomWeightsMatrixesAnalysisResults;
        List<PointAnalysisResult> pointAnalysisResults;

        public static void addNMStarWideColumnsToGrid (int n, int m, ref Grid grid)
        {
            for (int i = 0; i < n; i++) {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(m, GridUnitType.Star);
                grid.ColumnDefinitions.Add(columnDefinition);
            }
        }

        public static void addNMStarHighRowsToGrid (int n, int m, ref Grid grid)
        {
            for (int i = 0; i < n; i++) {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(m, GridUnitType.Star);
                grid.RowDefinitions.Add(rowDefinition);
            }
        }

        public static void addNLabelsToGridWithBitsOfIAsContent (int n, ref Grid grid, int value)
        {
            for (int i = 0; i < n; i++) {
                Label label = new Label();
                Grid.SetRow(label, i);
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.BorderBrush = Brushes.Black;
                label.BorderThickness = new Thickness(1);
                label.Content = (value >> 2 - i & 0x1) == 0 ? -1 : 1;
                grid.Children.Add(label);
            }
        }

        public static void addNLabelsToGridWithArrayAsContent<T> (int n, ref Grid grid, T[] values)
        {
            for (int i = 0; i < n; i++) {
                Label label = new Label();
                Grid.SetRow(label, i);
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.BorderBrush = Brushes.Black;
                label.BorderThickness = new Thickness(1);
                label.Content = values[i];
                grid.Children.Add(label);
            }
        }

        public static void addNLabelsToGridWithArrayAsContentSetColumnAddToResultList<T> (int n, int column, ref Grid grid, T[] values, List<PointAnalysisResult> resultsList)
        {
            for (int i = 0; i < n; i++) {
                Label label = new Label();
                Grid.SetColumn(label, column);
                Grid.SetRow(label, i);
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.BorderBrush = Brushes.Black;
                label.BorderThickness = new Thickness(1);
                label.Content = values[i];
                grid.Children.Add(label);

                if (i != 0) {
                    resultsList[i - 1].labelResults.Add(label);
                }
            }
        }

        public static void addNLabelsToUniformGridWithArrayAsContent<T> (int n, ref UniformGrid grid, T[] values)
        {
            for (int i = 0; i < n; i++) {
                Label label = new Label();
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.BorderBrush = Brushes.Black;
                label.BorderThickness = new Thickness(1);
                label.Content = values[i];
                grid.Children.Add(label);
            }
        }

        public static void addNLabelsToGridWithStringValueAndAddToList (int n, ref Grid grid, string value, ref List<Label> list)
        {
            for (int i = 0; i < n; i++) {
                Label label = new Label();
                Grid.SetRow(label, i);
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.BorderBrush = Brushes.Black;
                label.BorderThickness = new Thickness(1);
                label.Content = value;
                grid.Children.Add(label);
                list.Add(label);
            }
        }

        private UniformGrid createTestDataAndFieldsReturnUniformGridContainingResultElements(bool isSynchronousMode, ref List<TestDataAndFields> listOfTestDataAndFields)
        {
            UniformGrid uniformGrid = new UniformGrid();
            uniformGrid.Rows = 2;

            for (int i = 0; i < 8; i++) {
                TestDataAndFields currentTestDataAndFields = new TestDataAndFields();
                currentTestDataAndFields.iterationHistory = new List<SingleIterationResult>();
                currentTestDataAndFields.resultValueLabels = new List<Label>();
                currentTestDataAndFields.index = i;
                currentTestDataAndFields.synchronousMode = isSynchronousMode;

                GroupBox groupBox = new GroupBox();
                {
                    groupBox.Header = "Badanie " + (i + 1);
                    Grid grid = new Grid();
                    {
                        addNMStarWideColumnsToGrid(3, 1, ref grid);
                        addNMStarHighRowsToGrid(1, 3, ref grid);
                        addNMStarHighRowsToGrid(2, 1, ref grid);

                        Grid gridInputs = new Grid();
                        {
                            addNMStarHighRowsToGrid(3, 1, ref gridInputs);
                            Grid.SetColumn(gridInputs, 0);
                            addNLabelsToGridWithBitsOfIAsContent(3, ref gridInputs, i);

                            currentTestDataAndFields.startingValue = new DMU.Math.Matrix(
                                new double[] {
                                        ((i >> (2 - 0)) & 0x1) == 0 ? -1 : 1,
                                        ((i >> (2 - 1)) & 0x1) == 0 ? -1 : 1,
                                        ((i >> (2 - 2)) & 0x1) == 0 ? -1 : 1
                                },
                                true
                            );

                            currentTestDataAndFields.currentValue = currentTestDataAndFields.startingValue.Clone();
                        }

                        grid.Children.Add(gridInputs);

                        Label label = new Label();
                        {
                            Grid.SetColumn(label, 1);
                            Grid.SetRow(label, 0);
                            label.VerticalAlignment = VerticalAlignment.Center;
                            label.HorizontalAlignment = HorizontalAlignment.Center;
                            label.Content = "=>";
                        }
                        grid.Children.Add(label);

                        Grid gridOutputs = new Grid();
                        {
                            addNMStarHighRowsToGrid(3, 1, ref gridOutputs);
                            Grid.SetColumn(gridOutputs, 2);
                            addNLabelsToGridWithStringValueAndAddToList(3, ref gridOutputs, "-", ref currentTestDataAndFields.resultValueLabels);
                        }
                        grid.Children.Add(gridOutputs);

                        label = new Label();
                        {
                            Grid.SetColumn(label, 0);
                            Grid.SetRow(label, 1);
                            Grid.SetColumnSpan(label, 3);
                            label.HorizontalAlignment = HorizontalAlignment.Center;
                            currentTestDataAndFields.resultLabel = label;
                        }
                        grid.Children.Add(label);

                        Button button = new Button();
                        {
                            Grid.SetColumn(button, 0);
                            Grid.SetRow(button, 2);
                            Grid.SetColumnSpan(button, 3);
                            button.Content = "Więcej";
                            button.Tag = currentTestDataAndFields;
                            button.Click += displayMoreInformation;
                            currentTestDataAndFields.moreButton = button;
                        }
                        grid.Children.Add(button);
                    }
                    groupBox.Content = grid;
                }
                uniformGrid.Children.Add(groupBox);
                listOfTestDataAndFields.Add(currentTestDataAndFields);
            }

            return uniformGrid;
        }

        public MainWindow ()
        {
            InitializeComponent();
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
                    modeGroupBox.Content = createTestDataAndFieldsReturnUniformGridContainingResultElements(synchronousMode, ref targetTestDataAndFields);
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

        private double calculateEnergySynchonized (DMU.Math.Matrix weightsMatrix, DMU.Math.Matrix matrix, DMU.Math.Matrix previousStepMatrix, DMU.Math.Matrix matrixI = null)
        {
            double result = 0;

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result += weightsMatrix.GetElement(i, j) * matrix.GetElement(i, 0) * previousStepMatrix.GetElement(j, 0);
                }
            }

            result = -result;

            if (matrixI != null) {
                for (int i = 0; i < 3; i++) {
                    result += matrixI.GetElement(i, 0) * (matrix.GetElement(i, 0) + previousStepMatrix.GetElement(i, 0));
                }
            }

            return Math.Round(result, 3);
        }

        private double calculateEnergyAsynchonized (DMU.Math.Matrix weightsMatrix, DMU.Math.Matrix matrix)
        {
            double result = 0;

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result += weightsMatrix.GetElement(i, j) * matrix.GetElement(i, 0) * matrix.GetElement(j, 0);
                }
            }

            result = -(result / 2);

            return Math.Round(result, 3);
        }

        private void setMatrixValuesToListOfLabels (DMU.Math.Matrix matrix, List<Label> labels)
        {
            for (int i = 0; i < labels.Count; i++) {
                labels[i].Content = matrix.GetElement(i, 0);
            }
        }

        private bool isMatrixSymmetric (DMU.Math.Matrix matrix)
        {
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (!matrix.GetElement(i, j).Equals(matrix.GetElement(j, i))) {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string resultTypeToString (ResultType resultType)
        {
            string result = "";

            switch (resultType) {
                case ResultType.Static: {
                    result = "Punkt stały";
                } break;

                case ResultType.GoesToPoint: {
                    result = "Wpada do punktu stałego";
                } break;

                case ResultType.CreatesCycle: {
                    result = "Tworzy cykl";
                } break;

                case ResultType.GoesToCycle: {
                    result = "Wpada w cykl";
                } break;

                default: { } break;
            }

            return result;
        }

        private void fillPointAnalysisData(ref List<PointAnalysisResult> pointAnalysisResults, ResultType resultType, int pointIndex)
        {
            switch (resultType) {
                case ResultType.Static: {
                    pointAnalysisResults[pointIndex].amountOfStaticResult++;
                } break;

                case ResultType.GoesToPoint: {
                    pointAnalysisResults[pointIndex].amountOfGoesToStaticResult++;
                } break;

                case ResultType.CreatesCycle: {
                    pointAnalysisResults[pointIndex].amountOfCreatesCycleResult++;
                } break;

                case ResultType.GoesToCycle: {
                    pointAnalysisResults[pointIndex].amountOGoesToCycleResult++;
                } break;

                default: { } break;
            }
        }

        private void runSynchronousAnalysisForTestDataListWithWeights (ref List<TestDataAndFields> listOfTestDataAndFields, ref DMU.Math.Matrix weightsMatrix, bool isWeightsMatrixSymetric = true, List<PointAnalysisResult> pointAnalysisResults = null)
        {
            for (int j = 0; j < listOfTestDataAndFields.Count; j++) {
                TestDataAndFields currentTestDataAndFields = listOfTestDataAndFields[j];
                currentTestDataAndFields.iterationHistory = new List<SingleIterationResult>();
                currentTestDataAndFields.currentValue = currentTestDataAndFields.startingValue;

                for (int i = 0; i < 8; i++) {
                    DMU.Math.Matrix previousStepMatrix = currentTestDataAndFields.currentValue.Clone();
                    SingleIterationResult iterationResult = new SingleIterationResult();

                    currentTestDataAndFields.currentValue = DMU.Math.Matrix.Multiply(weightsMatrix, currentTestDataAndFields.currentValue);
                    iterationResult.matrixResult = DMU.Math.Matrix.Add(currentTestDataAndFields.currentValue, matrixI);
                    currentTestDataAndFields.currentValue = currentTestDataAndFields.currentValue.ToBiPolar();
                    iterationResult.matrixResultBiPolar = currentTestDataAndFields.currentValue;
                    iterationResult.energy = calculateEnergySynchonized(weightsMatrix, currentTestDataAndFields.currentValue, previousStepMatrix, matrixI);

                    currentTestDataAndFields.iterationHistory.Add(iterationResult);

                    if (previousStepMatrix.Equals(iterationResult.matrixResultBiPolar)) {
                        if (i == 0) {
                            currentTestDataAndFields.resultType = ResultType.Static;
                        } else {
                            currentTestDataAndFields.resultType = ResultType.GoesToPoint;
                        }

                        setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);

                        break;
                    }

                    if (isWeightsMatrixSymetric && i != 0) {
                        double previousStepEnergy = currentTestDataAndFields.iterationHistory[i - 1].energy;
                        if (previousStepEnergy == iterationResult.energy) {
                            if (currentTestDataAndFields.startingValue.Equals(currentTestDataAndFields.currentValue)) {
                                currentTestDataAndFields.resultType = ResultType.CreatesCycle;
                            } else {
                                currentTestDataAndFields.resultType = ResultType.GoesToCycle;
                            }

                            setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);

                            break;
                        }
                    }

                    if (i == 7) {
                        setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                    }

                    currentTestDataAndFields.currentValue = iterationResult.matrixResultBiPolar;
                }

                if (pointAnalysisResults != null) {
                    fillPointAnalysisData(ref pointAnalysisResults, currentTestDataAndFields.resultType, j);
                }

                currentTestDataAndFields.resultLabel.Content = resultTypeToString(currentTestDataAndFields.resultType);
            }
        }

        private void runAsynchronousAnalysisForTestDataListWithWeights (ref List<TestDataAndFields> listOfTestDataAndFields, ref DMU.Math.Matrix weightsMatrix, List<PointAnalysisResult> pointAnalysisResults = null)
        {
            for (int j = 0; j < listOfTestDataAndFields.Count; j++) {
                TestDataAndFields currentTestDataAndFields = listOfTestDataAndFields[j];
                for (int i = 0; i < 8; i++) {
                    bool finished = false;

                    for (int k = 0; k < 3; k++) {
                        DMU.Math.Matrix previousStepMatrix = currentTestDataAndFields.currentValue.Clone();
                        SingleIterationResult iterationResult = new SingleIterationResult();

                        currentTestDataAndFields.currentValue = DMU.Math.Matrix.Multiply(weightsMatrix, currentTestDataAndFields.currentValue);

                        for (int l = 1; l <= 3; l++) {
                            if (l != asynchronousOrder[k]) {
                                currentTestDataAndFields.currentValue.SetElement(l - 1, 0, previousStepMatrix.GetElement(l - 1, 0));
                            }
                        }

                        iterationResult.matrixResult = currentTestDataAndFields.currentValue;
                        currentTestDataAndFields.currentValue = currentTestDataAndFields.currentValue.ToBiPolar();
                        iterationResult.matrixResultBiPolar = currentTestDataAndFields.currentValue;
                        iterationResult.energy = calculateEnergyAsynchonized(weightsMatrix, currentTestDataAndFields.currentValue);

                        currentTestDataAndFields.iterationHistory.Add(iterationResult);

                        if (i != 0 || (i == 0 && k == 2)) {
                            DMU.Math.Matrix thirdToLastStepResult = i != 0 ? currentTestDataAndFields.iterationHistory[i * 3 + k - 3].matrixResultBiPolar : currentTestDataAndFields.startingValue;
                            DMU.Math.Matrix secondToLastStepResult = currentTestDataAndFields.iterationHistory[i * 3 + k - 2].matrixResultBiPolar;
                            DMU.Math.Matrix previousStepResult = currentTestDataAndFields.iterationHistory[i * 3 + k - 1].matrixResultBiPolar;

                            if (currentTestDataAndFields.currentValue.Equals(previousStepResult) &&
                                currentTestDataAndFields.currentValue.Equals(secondToLastStepResult) &&
                                currentTestDataAndFields.currentValue.Equals(thirdToLastStepResult)
                                ) {
                                if (currentTestDataAndFields.currentValue.Equals(currentTestDataAndFields.startingValue)) {
                                    currentTestDataAndFields.resultType = ResultType.Static;
                                } else {
                                    currentTestDataAndFields.resultType = ResultType.GoesToPoint;
                                }

                                setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                                finished = true;

                                break;
                            }
                        }

                        if (i == 7 && k == 2) {
                            setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                        }
                    }

                    if (pointAnalysisResults != null) {
                        fillPointAnalysisData(ref pointAnalysisResults, currentTestDataAndFields.resultType, j);
                    }

                    if (finished) {
                        currentTestDataAndFields.resultLabel.Content = resultTypeToString(currentTestDataAndFields.resultType);
                        break;
                    }
                }
            }
        }

        private bool checkAndSaveAsychrnonousActivationOrder()
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
            bool isWeightsMatrixSymetric = isMatrixSymmetric(passedWeightsMatrix);

            if (weightsOK && checkAndSaveAsychrnonousActivationOrder()) {
                runSynchronousAnalysisForTestDataListWithWeights(ref synchronousTestDataAndFields, ref passedWeightsMatrix, isWeightsMatrixSymetric);
                runAsynchronousAnalysisForTestDataListWithWeights(ref asynchronousTestDataAndFields, ref passedWeightsMatrix);

                passedWeightsModeHasResults = true;
            }
        }

        private void displayMoreInformation (object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            TestDataAndFields testDataAndFields = (TestDataAndFields)button.Tag;

            MoreWindow moreWindow = new MoreWindow();
            moreWindow.displayInformation(testDataAndFields);
            moreWindow.Show();
        }

        private string convertResultsToString(List<TestDataAndFields> resultsToConvert, DMU.Math.Matrix weightsMatrix, string title)
        {
            string matrixNumberFormatting = "F1";
            string matrixColumnDelimiteFormatting = "\t";
            string matrixRowDelimiterFormatting = "\n";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(title);
            stringBuilder.AppendLine("Macierz wag (W):");
            stringBuilder.Append(weightsMatrix.ToString(matrixNumberFormatting, matrixColumnDelimiteFormatting, matrixRowDelimiterFormatting));

            foreach (TestDataAndFields testDataAndFields in resultsToConvert) {
                stringBuilder.AppendLine("----------------------------------------------------");
                stringBuilder.AppendLine("--- Badanie nr. " + (testDataAndFields.index + 1) + " ---");
                stringBuilder.AppendLine("----------------------------------------------------");
                stringBuilder.AppendLine("Badany wektor:");
                stringBuilder.AppendLine(DMU.Math.Matrix.Transpose(testDataAndFields.startingValue).ToString(matrixNumberFormatting, matrixColumnDelimiteFormatting, matrixRowDelimiterFormatting));

                int i = 1;
                foreach (SingleIterationResult singleIterationResult in testDataAndFields.iterationHistory) {
                    stringBuilder.AppendLine("Krok: " + i + "-------------------");
                    stringBuilder.AppendLine("Potencjał wejściowy (U):");
                    stringBuilder.AppendLine(DMU.Math.Matrix.Transpose(singleIterationResult.matrixResult).ToString(matrixNumberFormatting, matrixColumnDelimiteFormatting, ""));
                    stringBuilder.AppendLine("Potencjał wyjściowy (V):");
                    stringBuilder.AppendLine(DMU.Math.Matrix.Transpose(singleIterationResult.matrixResultBiPolar).ToString(matrixNumberFormatting, matrixColumnDelimiteFormatting, matrixRowDelimiterFormatting));
                    stringBuilder.AppendLine("Energia(" + i + ")= " + singleIterationResult.energy);
                    stringBuilder.AppendLine();

                    i++;
                }

                stringBuilder.Append("Wniosek: Punkt [" + DMU.Math.Matrix.Transpose(testDataAndFields.startingValue).ToString(matrixNumberFormatting, " ", "") + "] " + resultTypeToString(testDataAndFields.resultType));
                switch (testDataAndFields.resultType) {
                    case ResultType.GoesToPoint: {
                            stringBuilder.Append(": [" + DMU.Math.Matrix.Transpose(testDataAndFields.currentValue).ToString(matrixNumberFormatting, " ", "") + "]");
                    } break;

                    case ResultType.CreatesCycle:
                    case ResultType.GoesToCycle: {
                            SingleIterationResult secondToLastIterationResult = testDataAndFields.iterationHistory[testDataAndFields.iterationHistory.Count - 2];
                            stringBuilder.Append(": [" + DMU.Math.Matrix.Transpose(testDataAndFields.currentValue).ToString(matrixNumberFormatting, " ", "") + "] <->" + " [" + DMU.Math.Matrix.Transpose(secondToLastIterationResult.matrixResultBiPolar).ToString(matrixNumberFormatting, " ", "") + "]");
                    } break;
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private string convertPointAnalysisDataToString(List<PointAnalysisResult> pointAnalysisResults)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                "Wektor".PadRight(10) + " " + "Stały".PadRight(5) + " " +
                "Zbieżny".PadRight(7) + " " +
                "Tworzy cykl".PadRight(11) + " " +
                "Wpada w cykl".PadRight(12)
            );

            foreach (PointAnalysisResult pointAnalysisResult in pointAnalysisResults) {
                stringBuilder.AppendLine(
                    ("[" + DMU.Math.Matrix.Transpose(pointAnalysisResult.point).ToString("F0", " ", "") + "]").PadRight(10) + " " +
                    pointAnalysisResult.amountOfStaticResult.ToString().PadRight(5) + " " +
                    pointAnalysisResult.amountOfGoesToStaticResult.ToString().PadRight(7) + " " +
                    pointAnalysisResult.amountOfCreatesCycleResult.ToString().PadRight(11) + " " +
                    pointAnalysisResult.amountOGoesToCycleResult.ToString().PadRight(12)
                );
            }

            return stringBuilder.ToString();
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
                        string text = convertResultsToString(synchronousTestDataAndFields, passedWeightsMatrix, "Badanie w trybie synchronicznym. Wektor I = [" + DMU.Math.Matrix.Transpose(matrixI).ToString("F1", " ", "") + "]");
                        text += "\n############################\n";
                        text += convertResultsToString(asynchronousTestDataAndFields, passedWeightsMatrix, "Badanie w trybie asynchronicznym. Kolejność aktywacji neuronów: " + asynchronousOrder[0] + ", " + asynchronousOrder[1] + ", " + asynchronousOrder[2]);

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
                                title = "Badanie w trybie synchronicznym. Wektor I = [" + DMU.Math.Matrix.Transpose(matrixI).ToString("F1", " ", "") + "]";
                            } else {
                                title = "Badanie w trybie asynchronicznym. Kolejność aktywacji neuronów: " + asynchronousOrder[0] + ", " + asynchronousOrder[1] + ", " + asynchronousOrder[2];
                            }

                            text += convertResultsToString(listOfTestDataAndFields, weightsMatrix, title);

                            if (i + 1 < randomWeightsMatrixesAnalysisResults.Count) {
                                text += "\n############################\n";
                            }
                        }

                        text += "\n######### PODSUMOWANIE #########\n";
                        text += "Liczba losowań: " + randomWeigthMatrixes.Count + "\n\n";
                        text += convertPointAnalysisDataToString(pointAnalysisResults);

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
                                        addNMStarHighRowsToGrid(9, 1, ref resultsSummaryGrid);
                                        addNMStarWideColumnsToGrid(5, 1, ref resultsSummaryGrid);
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

                                        addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 0, ref resultsSummaryGrid, labelValues0, pointAnalysisResults);
                                        addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 1, ref resultsSummaryGrid, labelValues1, pointAnalysisResults);
                                        addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 2, ref resultsSummaryGrid, labelValues2, pointAnalysisResults);
                                        addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 3, ref resultsSummaryGrid, labelValues3, pointAnalysisResults);
                                        addNLabelsToGridWithArrayAsContentSetColumnAddToResultList(9, 4, ref resultsSummaryGrid, labelValues4, pointAnalysisResults);
                                    }
                                    resultsSummaryGroupBox.Content = resultsSummaryGrid;
                                }
                                stackPanel.Children.Add(resultsSummaryGroupBox);

                                for (int i = 0; i < amountOfRandomWeightsMatrixes; i++) {
                                    Grid grid = new Grid();
                                    {
                                        addNMStarHighRowsToGrid(1, 1, ref grid);
                                        addNMStarHighRowsToGrid(1, 3, ref grid);
                                        addNMStarWideColumnsToGrid(3, 1, ref grid);

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
                                                addNLabelsToUniformGridWithArrayAsContent(9, ref weightsMatrixGrid, weightsMatrix.ToArray());
                                            }
                                            groupBox.Content = weightsMatrixGrid;
                                        }
                                        grid.Children.Add(groupBox);

                                        List<TestDataAndFields> currentTestDataAndFieldsList = new List<TestDataAndFields>();

                                        UniformGrid uniformGrid = createTestDataAndFieldsReturnUniformGridContainingResultElements(randomWeightsSynchronousMode, ref currentTestDataAndFieldsList);
                                        Grid.SetRow(uniformGrid, 1);
                                        Grid.SetColumnSpan(uniformGrid, 3);
                                        grid.Children.Add(uniformGrid);

                                        randomWeightsMatrixesAnalysisResults.Add(currentTestDataAndFieldsList);

                                        if (randomWeightsSynchronousMode) {
                                            runSynchronousAnalysisForTestDataListWithWeights(ref currentTestDataAndFieldsList, ref weightsMatrix, true, pointAnalysisResults);
                                        } else {
                                            runAsynchronousAnalysisForTestDataListWithWeights(ref currentTestDataAndFieldsList, ref weightsMatrix, pointAnalysisResults);
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
