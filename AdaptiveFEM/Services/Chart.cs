using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AdaptiveFEM.Services
{
    public class Chart
    {
        private string _pythonExePath =
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\Services\PythonModules\venv\Scripts\python.exe"));

        public string _potentialDataFileAddress =
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\Services\PythonModules\potential.txt"));

        public string _contourPlotModuleAddress =
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\Services\PythonModules\contour_plot.py"));

        public string _contourPlotPNGAddress =
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                @"..\..\..\Services\PythonModules\potential.txt.png"));

        public BitmapImage ContourPlot(Dictionary<Point, double> potential, int xSize, int ySize)
        {
            // Write potential to file
            WritePotentialData(potential);

            // Draw plot
            DrawContourPlot(xSize, ySize);

            // Get png plot
            return new BitmapImage(new Uri(_contourPlotPNGAddress));
        }

        public BitmapImage ElectricFieldPlot(Dictionary<Point, double> potential, int xSize, int ySize)
        {
            WritePotentialData(potential);

            DrawElectricField(xSize, ySize);

            return new BitmapImage();
        }

        private void DrawElectricField(int xSize, int ySize)
        {
            throw new NotImplementedException();
        }

        private void WritePotentialData(Dictionary<Point, double> potential)
        {
            bool headerWritten = false;

            using (StreamWriter file = new StreamWriter(_potentialDataFileAddress))
                try
                {
                    foreach (var kvp in potential)
                    {
                        if (!headerWritten)
                        {
                            file.WriteLine("X,Y,V");
                            headerWritten = true;
                        }
                        file.WriteLine($"{kvp.Key.X},{kvp.Key.Y},{kvp.Value}");
                    }
                }
                finally
                {
                    if (file != null)
                    {
                        ((IDisposable)file).Dispose();
                    }
                }
        }

        private void DrawContourPlot(int xSize, int ySize)
        {
            string args = $"{_contourPlotModuleAddress} {_potentialDataFileAddress} {xSize} {ySize}";

            RunPythonFile(_pythonExePath, args);
        }

        private void RunPythonFile(string exeFilePath, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = _pythonExePath;
            startInfo.Arguments = args;

            Process.Start(startInfo);
        }
    }
}
