using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1ZMSI
{
    class Calculations
    {
        public static DMU.Math.Matrix matrixI = new DMU.Math.Matrix(new double[] { 0.1, 0.1, 0.1 }, true);

        public static double calculateEnergySynchronous (DMU.Math.Matrix weightsMatrix, DMU.Math.Matrix matrix, DMU.Math.Matrix previousStepMatrix, DMU.Math.Matrix matrixI = null)
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

        public static double calculateEnergyAsynchronous (DMU.Math.Matrix weightsMatrix, DMU.Math.Matrix matrix)
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

        public static bool isMatrixSymmetric (DMU.Math.Matrix matrix)
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

        public static void cleanResultDataStructures (ref TestDataAndFields testDataAndFields)
        {
            testDataAndFields.iterationHistory = new List<SingleIterationResult>();
            testDataAndFields.foundPattern = new List<SingleIterationResult>();
            testDataAndFields.currentValue = testDataAndFields.startingValue;
        }

        public static void fillPointAnalysisData (ref List<PointAnalysisResult> pointAnalysisResults, ResultType resultType, int pointIndex)
        {
            switch (resultType) {
                case ResultType.Static: {
                        pointAnalysisResults[pointIndex].amountOfStaticResult++;
                    }
                    break;

                case ResultType.GoesToPoint: {
                        pointAnalysisResults[pointIndex].amountOfGoesToStaticResult++;
                    }
                    break;

                case ResultType.CreatesCycle: {
                        pointAnalysisResults[pointIndex].amountOfCreatesCycleResult++;
                    }
                    break;

                case ResultType.GoesToCycle: {
                        pointAnalysisResults[pointIndex].amountOGoesToCycleResult++;
                    }
                    break;

                default: { } break;
            }
        }

        public static void runSynchronousAnalysisForTestDataListWithWeights (ref List<TestDataAndFields> listOfTestDataAndFields, ref DMU.Math.Matrix weightsMatrix, bool isWeightsMatrixSymetric = true, List<PointAnalysisResult> pointAnalysisResults = null)
        {
            for (int j = 0; j < listOfTestDataAndFields.Count; j++) {
                TestDataAndFields currentTestDataAndFields = listOfTestDataAndFields[j];
                cleanResultDataStructures(ref currentTestDataAndFields);

                for (int i = 0; i < 8; i++) {
                    DMU.Math.Matrix previousStepMatrix = currentTestDataAndFields.currentValue.Clone();
                    SingleIterationResult iterationResult = new SingleIterationResult();

                    currentTestDataAndFields.currentValue = DMU.Math.Matrix.Multiply(weightsMatrix, currentTestDataAndFields.currentValue);
                    iterationResult.matrixResult = DMU.Math.Matrix.Add(currentTestDataAndFields.currentValue, matrixI);
                    currentTestDataAndFields.currentValue = currentTestDataAndFields.currentValue.ToBiPolar();
                    iterationResult.matrixResultBiPolar = currentTestDataAndFields.currentValue;
                    iterationResult.energy = calculateEnergySynchronous(weightsMatrix, currentTestDataAndFields.currentValue, previousStepMatrix, matrixI);

                    currentTestDataAndFields.iterationHistory.Add(iterationResult);

                    if (previousStepMatrix.Equals(iterationResult.matrixResultBiPolar)) {
                        if (i == 0) {
                            currentTestDataAndFields.resultType = ResultType.Static;
                        } else {
                            currentTestDataAndFields.resultType = ResultType.GoesToPoint;
                        }

                        LayoutGenerationHelpers.setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);

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

                            LayoutGenerationHelpers.setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);

                            if (currentTestDataAndFields.iterationHistory.Count != 1) {
                                currentTestDataAndFields.foundPattern.Add(currentTestDataAndFields.iterationHistory[currentTestDataAndFields.iterationHistory.Count - 2]);
                            } else {
                                SingleIterationResult initialValue = new SingleIterationResult();
                                initialValue.matrixResultBiPolar = currentTestDataAndFields.startingValue;
                                currentTestDataAndFields.foundPattern.Add(initialValue);
                            }

                            currentTestDataAndFields.foundPattern.Add(currentTestDataAndFields.iterationHistory[currentTestDataAndFields.iterationHistory.Count - 1]);

                            break;
                        }
                    }

                    if (i == 7) {
                        LayoutGenerationHelpers.setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                    }

                    currentTestDataAndFields.currentValue = iterationResult.matrixResultBiPolar;
                }

                if (pointAnalysisResults != null) {
                    fillPointAnalysisData(ref pointAnalysisResults, currentTestDataAndFields.resultType, j);
                }

                currentTestDataAndFields.resultLabel.Content = DataConverters.resultTypeToString(currentTestDataAndFields.resultType);
            }
        }

        public static void runAsynchronousAnalysisForTestDataListWithWeights (ref List<TestDataAndFields> listOfTestDataAndFields, ref DMU.Math.Matrix weightsMatrix, int[] asynchronousOrder, List<PointAnalysisResult> pointAnalysisResults = null)
        {
            for (int j = 0; j < listOfTestDataAndFields.Count; j++) {
                TestDataAndFields currentTestDataAndFields = listOfTestDataAndFields[j];
                cleanResultDataStructures(ref currentTestDataAndFields);

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
                        iterationResult.energy = calculateEnergyAsynchronous(weightsMatrix, currentTestDataAndFields.currentValue);

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

                                LayoutGenerationHelpers.setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                                finished = true;

                                break;
                            }

                            int upperTemplateBound = currentTestDataAndFields.iterationHistory.Count - 1;
                            int lowerTemplateBound = upperTemplateBound - 1;
                            int minimalTemplateBound = (int)Math.Ceiling((double)currentTestDataAndFields.iterationHistory.Count / 2.0);
                            bool patternFound = false;
                            List<SingleIterationResult> pattern = new List<SingleIterationResult>();

                            while (lowerTemplateBound >= minimalTemplateBound && !patternFound) {
                                bool patternExists = true;
                                int possiblePatternLength = upperTemplateBound - lowerTemplateBound + 1;

                                for (int currentElementToCheckIndex = upperTemplateBound; currentElementToCheckIndex >= lowerTemplateBound; currentElementToCheckIndex--) {
                                    SingleIterationResult patternResult = currentTestDataAndFields.iterationHistory[currentElementToCheckIndex];
                                    SingleIterationResult checkedResult = currentTestDataAndFields.iterationHistory[(currentElementToCheckIndex - possiblePatternLength)];

                                    if (!patternResult.matrixResultBiPolar.Equals(checkedResult.matrixResultBiPolar) ||
                                        patternResult.energy != checkedResult.energy
                                    ) {
                                        patternExists = false;
                                        break;
                                    }
                                }

                                patternFound = patternExists;

                                lowerTemplateBound--;
                            }

                            if (patternFound) {
                                for (int patternIndex = upperTemplateBound; patternIndex > lowerTemplateBound; patternIndex--) {
                                    pattern.Add(currentTestDataAndFields.iterationHistory[patternIndex]);
                                }

                                int lastUncheckedIndex = lowerTemplateBound - pattern.Count;
                                bool startingValueIsPartOfThePattern = true;

                                if (lastUncheckedIndex >= pattern.Count) {
                                    startingValueIsPartOfThePattern = false;
                                } else {
                                    int patternIndex = pattern.Count - 1;
                                    for (int uncheckedValueIndex = lastUncheckedIndex; uncheckedValueIndex >= 0; uncheckedValueIndex--) {
                                        SingleIterationResult patternResult = pattern[patternIndex];
                                        SingleIterationResult checkedResult = currentTestDataAndFields.iterationHistory[uncheckedValueIndex];

                                        if (!patternResult.matrixResultBiPolar.Equals(checkedResult.matrixResultBiPolar) ||
                                            patternResult.energy != checkedResult.energy
                                        ) {
                                            startingValueIsPartOfThePattern = false;
                                            break;
                                        }

                                        patternIndex--;
                                    }

                                    if (startingValueIsPartOfThePattern) {
                                        if (!pattern[patternIndex].matrixResultBiPolar.Equals(currentTestDataAndFields.startingValue)) {
                                            startingValueIsPartOfThePattern = false;
                                        }
                                    }
                                }

                                if (startingValueIsPartOfThePattern) {
                                    currentTestDataAndFields.resultType = ResultType.CreatesCycle;
                                } else {
                                    currentTestDataAndFields.resultType = ResultType.GoesToCycle;
                                }

                                LayoutGenerationHelpers.setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                                finished = true;
                                pattern.Reverse();
                                currentTestDataAndFields.foundPattern = pattern;

                                break;
                            }
                        }

                        if (i == 7 && k == 2) {
                            LayoutGenerationHelpers.setMatrixValuesToListOfLabels(iterationResult.matrixResultBiPolar, currentTestDataAndFields.resultValueLabels);
                        }
                    }

                    if (pointAnalysisResults != null) {
                        fillPointAnalysisData(ref pointAnalysisResults, currentTestDataAndFields.resultType, j);
                    }

                    if (finished) {
                        currentTestDataAndFields.resultLabel.Content = DataConverters.resultTypeToString(currentTestDataAndFields.resultType);
                        break;
                    }
                }
            }
        }
    }
}
