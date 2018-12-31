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

namespace Projekt1ZMSI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MoreWindow : Window
    {
        public MoreWindow ()
        {
            InitializeComponent();
        }

        public void displayInformation(TestDataAndFields testDataAndFields)
        {
            DMU.Math.Matrix currentValue = testDataAndFields.startingValue;
            string text = "Badanie " + (testDataAndFields.index + 1) + " Wektor: " + currentValue.ToString("F0");
            this.Title = text;
            SourceInfo.Header = text;

            Label label = new Label();
            {
                label.VerticalAlignment = VerticalAlignment.Center;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Content = MainWindow.resultTypeToString(testDataAndFields.resultType);
            }
            results.Children.Add(label);

            int i = 1;
            foreach (SingleIterationResult resultOfStep in testDataAndFields.iterationHistory) {
                GroupBox groupBox = new GroupBox();
                {
                    groupBox.Header = "Krok " + i;
                    Grid grid = new Grid();
                    {
                        MainWindow.addNMStarWideColumnsToGrid(4, 1, ref grid);
                        MainWindow.addNMStarHighRowsToGrid(3, 1, ref grid);
                        MainWindow.addNMStarHighRowsToGrid(1, 1, ref grid);

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
                            MainWindow.addNMStarHighRowsToGrid(3, 1, ref gridOutputs);
                            Grid.SetColumn(gridOutputs, 1);
                            MainWindow.addNLabelsToGridWithArrayAsContent(3, ref gridOutputs, resultOfStep.matrixResult.ToArray());
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
                            MainWindow.addNMStarHighRowsToGrid(3, 1, ref gridOutputsBiPolar);
                            Grid.SetColumn(gridOutputsBiPolar, 3);
                            MainWindow.addNLabelsToGridWithArrayAsContent(3, ref gridOutputsBiPolar, resultOfStep.matrixResultBiPolar.ToArray());
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
