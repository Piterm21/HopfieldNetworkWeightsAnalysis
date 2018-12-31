using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1ZMSI
{
    class DataConverters
    {
        public static string resultTypeToString (ResultType resultType)
        {
            string result = "";

            switch (resultType) {
                case ResultType.Static: {
                        result = "Punkt stały";
                    }
                    break;

                case ResultType.GoesToPoint: {
                        result = "Wpada do punktu stałego";
                    }
                    break;

                case ResultType.CreatesCycle: {
                        result = "Tworzy cykl";
                    }
                    break;

                case ResultType.GoesToCycle: {
                        result = "Wpada w cykl";
                    }
                    break;

                default: { } break;
            }

            return result;
        }

        public static string convertResultsToString (List<TestDataAndFields> resultsToConvert, DMU.Math.Matrix weightsMatrix, string title)
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
                        }
                        break;

                    case ResultType.CreatesCycle:
                    case ResultType.GoesToCycle: {
                            stringBuilder.Append(": ");
                            int index = 0;

                            foreach (SingleIterationResult patternElement in testDataAndFields.foundPattern) {
                                stringBuilder.Append("[" + DMU.Math.Matrix.Transpose(patternElement.matrixResultBiPolar).ToString(matrixNumberFormatting, " ", "") + "]");

                                if (index < (testDataAndFields.foundPattern.Count - 1)) {
                                    if (testDataAndFields.synchronousMode) {
                                        stringBuilder.Append(" <-> ");
                                    } else {
                                        stringBuilder.Append(" -> ");
                                    }
                                }

                                index++;
                            }
                        }
                        break;
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public static string convertPointAnalysisDataToString (List<PointAnalysisResult> pointAnalysisResults)
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
    }
}
