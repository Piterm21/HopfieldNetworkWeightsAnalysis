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
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Projekt1ZMSI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MoreWindow : Window
    {
        Brush[] brushes = new Brush[] {
            Brushes.Blue,
            Brushes.Gray,
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Red,
            Brushes.Purple,
            Brushes.Black,
            Brushes.Orange
        };

        public MoreWindow ()
        {
            InitializeComponent();
        }

        private int biPolarToNumber (double[] biPolar)
        {
            int result = 0;

            for (int i = biPolar.Length - 1; i >= 0 ; i--) {
                if (biPolar[i] == 1) {
                    result |= (1 << i);
                }
            }

            return result;
        }

        public void displayInformation(TestDataAndFields testDataAndFields)
        {
            DMU.Math.Matrix currentValue = testDataAndFields.startingValue;
            string text = "Badanie " + (testDataAndFields.index + 1) + " Wektor: " + currentValue.ToString("F0");
            this.Title = text;
            SourceInfo.Header = text;
            List<Rectangle> rectangles = new List<Rectangle>();

            Label label = new Label();
            {
                label.VerticalAlignment = VerticalAlignment.Center;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Content = DataConverters.resultTypeToString(testDataAndFields.resultType);
            }
            results.Children.Add(label);

            UniformGrid uniformGrid = new UniformGrid();
            {
                uniformGrid.Rows = 1;
                for (int rectIndex = 0; rectIndex < testDataAndFields.iterationHistory.Count + 1; rectIndex++) {
                    Rectangle rect = new Rectangle();
                    {
                        rect.Width = 20;
                        rect.Height = 20;
                        rectangles.Add(rect);
                    }
                    uniformGrid.Children.Add(rect);
                }
            }
            results.Children.Add(uniformGrid);
            rectangles[0].Fill = brushes[biPolarToNumber(testDataAndFields.startingValue.ToArray())];

            int i = 1;
            foreach (SingleIterationResult resultOfStep in testDataAndFields.iterationHistory) {
                GroupBox groupBox = new GroupBox();
                {
                    groupBox.Header = "Krok " + i;
                    rectangles[i].Fill = brushes[biPolarToNumber(resultOfStep.matrixResultBiPolar.ToArray())];
                    Grid grid = new Grid();
                    {
                        LayoutGenerationHelpers.addNMStarWideColumnsToGrid(4, 1, ref grid);
                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(3, 1, ref grid);
                        LayoutGenerationHelpers.addNMStarHighRowsToGrid(1, 1, ref grid);

                        label = new Label();
                        {
                            Grid.SetColumn(label, 0);
                            Grid.SetRow(label, 0);
                            label.VerticalAlignment = VerticalAlignment.Center;
                            label.HorizontalAlignment = HorizontalAlignment.Center;
                            label.Content = "Potencjał wejściowy (U):";
                        }
                        grid.Children.Add(label);

                        Grid gridOutputs = new Grid();
                        {
                            LayoutGenerationHelpers.addNMStarHighRowsToGrid(3, 1, ref gridOutputs);
                            Grid.SetColumn(gridOutputs, 1);
                            LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContent(3, ref gridOutputs, resultOfStep.matrixResult.ToArray());
                        }
                        grid.Children.Add(gridOutputs);

                        label = new Label();
                        {
                            Grid.SetColumn(label, 2);
                            Grid.SetRow(label, 0);
                            label.VerticalAlignment = VerticalAlignment.Center;
                            label.HorizontalAlignment = HorizontalAlignment.Center;
                            label.Content = "Potencjał wyjściowy (V):";
                        }
                        grid.Children.Add(label);

                        Grid gridOutputsBiPolar = new Grid();
                        {
                            LayoutGenerationHelpers.addNMStarHighRowsToGrid(3, 1, ref gridOutputsBiPolar);
                            Grid.SetColumn(gridOutputsBiPolar, 3);
                            LayoutGenerationHelpers.addNLabelsToGridWithArrayAsContent(3, ref gridOutputsBiPolar, resultOfStep.matrixResultBiPolar.ToArray());
                        }
                        grid.Children.Add(gridOutputsBiPolar);

                        label = new Label();
                        {
                            Grid.SetColumn(label, 0);
                            Grid.SetRow(label, 1);
                            Grid.SetColumnSpan(label, 4);
                            label.VerticalAlignment = VerticalAlignment.Center;
                            label.HorizontalAlignment = HorizontalAlignment.Center;
                            label.Content = "Energia: " + resultOfStep.energy;
                        }
                        grid.Children.Add(label);
                    }
                    groupBox.Content = grid;
                }
                results.Children.Add(groupBox);

                i++;
            }
        }
    }
}
