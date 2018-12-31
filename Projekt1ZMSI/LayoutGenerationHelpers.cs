using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Projekt1ZMSI
{
    class LayoutGenerationHelpers
    {
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

        public static UniformGrid createTestDataAndFieldsReturnUniformGridContainingResultElements (bool isSynchronousMode, ref List<TestDataAndFields> listOfTestDataAndFields)
        {
            UniformGrid uniformGrid = new UniformGrid();
            uniformGrid.Rows = 2;

            for (int i = 0; i < 8; i++) {
                TestDataAndFields currentTestDataAndFields = new TestDataAndFields();
                currentTestDataAndFields.iterationHistory = new List<SingleIterationResult>();
                currentTestDataAndFields.foundPattern = new List<SingleIterationResult>();
                currentTestDataAndFields.resultValueLabels = new List<Label>();
                currentTestDataAndFields.index = i;
                currentTestDataAndFields.synchronousMode = isSynchronousMode;

                GroupBox groupBox = new GroupBox();
                {
                    groupBox.Header = "Badanie " + (i + 1);
                    Grid grid = new Grid();
                    {
                        LayoutGenerationHelpers.addNMStarWideColumnsToGrid(3, 1, ref grid);
                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(1, 3, ref grid);
                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(2, 1, ref grid);

                        Grid gridInputs = new Grid();
                        {
                            LayoutGenerationHelpers.addNMStarHighRowsToGrid(3, 1, ref gridInputs);
                            Grid.SetColumn(gridInputs, 0);
                            LayoutGenerationHelpers.addNLabelsToGridWithBitsOfIAsContent(3, ref gridInputs, i);

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
                            LayoutGenerationHelpers.addNMStarHighRowsToGrid(3, 1, ref gridOutputs);
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
                            button.Click += MainWindow.displayMoreInformation;
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

        public static void setMatrixValuesToListOfLabels (DMU.Math.Matrix matrix, List<Label> labels)
        {
            for (int i = 0; i < labels.Count; i++) {
                labels[i].Content = matrix.GetElement(i, 0);
            }
        }
    }
}
