using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Encog.Engine.Opencl;
using Encog.Engine.Util;
using Encog;
using Encog.Neural.NeuralData;
using Encog.Util.Banchmark;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Simple;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Encog.Engine.Network.Train.Prop;
using Encog.Neural.Networks.Training.Propagation;
using System.Runtime.InteropServices;

namespace EncogOpenCLBenchmark
{
    /// <summary>
    /// Interaction logic for BenchmarkWindow.xaml
    /// </summary>
    public partial class BenchmarkWindow : Window
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        private int paramGlobalRatio;
        private double paramLocalRatio;
        private double paramSegRatio;
        private int paramInputNeurons;
        private int paramHidden1Neurons;
        private int paramHidden2Neurons;
        private int paramOutputNeurons;
        private int paramIterationsPerCall;
        private int paramTrainingIterations;
        private int paramTrainingSetSize;
        private EncogCLDevice device;


        public BenchmarkWindow()
        {
            InitializeComponent();

            bool runningWin32NT = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? true : false;
            bool consoleAllocated = false;

            try
            {
                if (runningWin32NT) consoleAllocated = AllocConsole();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Clootils Error");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Encog.Encog.Instance.InitCL();
                this.TextStats.IsReadOnly = true;
                this.ButtonShare.IsEnabled = false;

                foreach (EncogCLDevice device in Encog.Encog.Instance.CL.Devices)
                {
                    this.ListDevices.Items.Add(new FoundDevice(device));
                }

                SetToDefaults();
            }
            catch (EncogError ex)
            {
                MessageBox.Show(ex.ToString(), "Can't Access OpenCL");
            }



        }

        private void SetToDefaults()
        {
            this.TextGlobalRatio.Text = "1";
            this.TextLocalRatio.Text = "1.0";
            this.TextSegRatio.Text = "1.0";
            this.TextInputNeurons.Text = "10";
            this.TextHidden1Neurons.Text = "20";
            this.TextHidden2Neurons.Text = "5";
            this.TextOutputNeurons.Text = "1";
            this.TextIterationsPerCall.Text = "1";
            this.TextTrainingIterations.Text = "100";
            this.TextTrainingSetSize.Text = "10000";
        }

        private void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FoundDevice found = (FoundDevice)this.ListDevices.SelectedItem;
            this.device = found.Device;

            StringBuilder str = new StringBuilder();
            str.Append("Device Name:\t" + device.Device.Name + "\n");
            str.Append("Device Vendor:\t" + device.Device.Vendor + "\n");
            str.Append("Device Driver:\t" + device.Device.DriverVersion + "\n");
            str.Append("***Platform Stats\n");
            str.Append("Platform Name:\t" + device.Platform.Platform.Name + "\n");
            str.Append("Platform Vendor:\t" + device.Platform.Platform.Vendor + "\n");
            str.Append("Platform Version:\t" + device.Platform.Platform.Version + "\n");
            str.Append("Platform Extensions:\t");
            foreach (String ex in device.Platform.Platform.Extensions)
            {
                str.Append("[");
                str.Append(ex);
                str.Append("] ");
            }
            str.Append("\n");
            str.Append("***Platform Stats\n");
            try { str.Append("AddressBits:\t" + device.Device.AddressBits + "\n"); }
            catch (Exception) { }
            try { str.Append("DriverVersion:\t" + device.Device.DriverVersion + "\n"); }
            catch (Exception) { }
            try { str.Append("LittleEndian:\t" + device.Device.EndianLittle + "\n"); }
            catch (Exception) { }
            try { str.Append("ErrorCorrection Support:\t" + device.Device.ErrorCorrectionSupport + "\n"); }
            catch (Exception) { }
            try { str.Append("ExecutionCapabilities:\t" + device.Device.ExecutionCapabilities + "\n"); }
            catch (Exception) { }
            try { str.Append("GlobalMemoryCacheLineSize:\t" + device.Device.GlobalMemoryCacheLineSize + "\n"); }
            catch (Exception) { }
            try { str.Append("GlobalMemoryCacheSize:\t" + device.Device.GlobalMemoryCacheSize + "\n"); }
            catch (Exception) { }
            try { str.Append("GlobalMemoryCacheType:\t" + device.Device.GlobalMemoryCacheType + "\n"); }
            catch (Exception) { }
            try { str.Append("GlobalMemorySize:\t" + device.Device.GlobalMemorySize + " (" + Format.FormatMemory(device.Device.GlobalMemorySize) + ")\n"); }
            catch (Exception) { }
            try { str.Append("HostUnifiedMemory:\t" + device.Device.HostUnifiedMemory + "\n"); }
            catch (Exception) { }
            try { str.Append("Image2DMaxHeight:\t" + device.Device.Image2DMaxHeight + "\n"); }
            catch (Exception) { }
            try { str.Append("Image2DMaxWidth:\t" + device.Device.Image2DMaxWidth + "\n"); }
            catch (Exception) { }
            try { str.Append("Image3DMaxHeight:\t" + device.Device.Image3DMaxHeight + "\n"); }
            catch (Exception) { }
            try { str.Append("Image3DMaxWidth:\t" + device.Device.Image3DMaxWidth + "\n"); }
            catch (Exception) { }
            try { str.Append("ImageSupport:\t" + device.Device.ImageSupport + "\n"); }
            catch (Exception) { }
            try { str.Append("LocalMemorySize:\t" + device.Device.LocalMemorySize + " (" + Format.FormatMemory(device.Device.LocalMemorySize) + ")\n"); }
            catch (Exception) { }
            try { str.Append("LocalMemoryType:\t" + device.Device.LocalMemoryType + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxClockFrequency:\t" + device.Device.MaxClockFrequency + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxComputeUnits:\t" + device.Device.MaxComputeUnits + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxConstantArguments:\t" + device.Device.MaxConstantArguments + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxConstantBufferSize:\t" + device.Device.MaxConstantBufferSize + " (" + Format.FormatMemory(device.Device.MaxConstantBufferSize) + ")\n"); }
            catch (Exception) { }
            try { str.Append("MaxMemoryAllocationSize:\t" + device.Device.MaxMemoryAllocationSize + " (" + Format.FormatMemory(device.Device.MaxMemoryAllocationSize) + ")\n"); }
            catch (Exception) { }
            try { str.Append("MaxParameterSize:\t" + device.Device.MaxParameterSize + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxReadImageArguments:\t" + device.Device.MaxReadImageArguments + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxSamplers:\t" + device.Device.MaxSamplers + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxWorkGroupSize:\t" + device.Device.MaxWorkGroupSize + "\n"); }
            catch (Exception) { }
            try { str.Append("MaxWorkItemDimensions:\t" + device.Device.MaxWorkItemDimensions + "\n"); }
            catch (Exception) { }
            try
            {
                str.Append("MaxWorkItemSizes:\t");
                foreach (long ex in device.Device.MaxWorkItemSizes)
                {
                    str.Append("[");
                    str.Append(ex);
                    str.Append("] ");
                }
                str.Append("\n");
            }
            catch (Exception) { }
            try { str.Append("MaxWriteImageArguments:\t" + device.Device.MaxWriteImageArguments + "\n"); }
            catch (Exception) { }
            try { str.Append("MemoryBaseAddressAlignment:\t" + device.Device.MemoryBaseAddressAlignment + "\n"); }
            catch (Exception) { }
            try { str.Append("MinDataTypeAlignmentSize:\t" + device.Device.MinDataTypeAlignmentSize + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthChar:\t" + device.Device.NativeVectorWidthChar + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthDouble:\t" + device.Device.NativeVectorWidthDouble + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthFloat:\t" + device.Device.NativeVectorWidthFloat + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthHalf:\t" + device.Device.NativeVectorWidthHalf + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthInt:\t" + device.Device.NativeVectorWidthInt + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthLong:\t" + device.Device.NativeVectorWidthLong + "\n"); }
            catch (Exception) { }
            try { str.Append("NativeVectorWidthShort:\t" + device.Device.NativeVectorWidthShort + "\n"); }
            catch (Exception) { }
            try { str.Append("OpenCLCVersion:\t" + device.Device.OpenCLCVersion + "\n"); }
            catch (Exception) { }
            try { str.Append("OpenCLCVersionString:\t" + device.Device.OpenCLCVersionString + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthChar:\t" + device.Device.PreferredVectorWidthChar + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthDouble:\t" + device.Device.PreferredVectorWidthDouble + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthFloat:\t" + device.Device.PreferredVectorWidthFloat + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthHalf:\t" + device.Device.PreferredVectorWidthHalf + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthInt:\t" + device.Device.PreferredVectorWidthInt + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthLong:\t" + device.Device.PreferredVectorWidthLong + "\n"); }
            catch (Exception) { }
            try { str.Append("PreferredVectorWidthShort:\t" + device.Device.PreferredVectorWidthShort + "\n"); }
            catch (Exception) { }
            try { str.Append("ProfilingTimerResolution:\t" + device.Device.ProfilingTimerResolution + "\n"); }
            catch (Exception) { }
            try { str.Append("SingleCapabilites:\t" + device.Device.SingleCapabilites + "\n"); }
            catch (Exception) { }
            try { str.Append("Type:\t" + device.Device.Type + "\n"); }
            catch (Exception) { }
            try { str.Append("VendorId:\t" + device.Device.VendorId + "\n"); }
            catch (Exception) { }
            try { str.Append("Vendor:\t" + device.Device.Vendor + "\n"); }
            catch (Exception) { }
            try { str.Append("Version:\t" + device.Device.Version + "\n"); }
            catch (Exception) { }
            try { str.Append("VersionString:\t" + device.Device.VersionString + "\n"); }
            catch (Exception) { }
            try { str.Append("Extensions:"); }
            catch (Exception) { }
            foreach (String ex in device.Device.Extensions)
            {
                str.Append("[");
                str.Append(ex);
                str.Append("] ");
            }
            str.Append("\n");

            this.TextStats.Text = str.ToString();
        }

        public void BenchmarkProc()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    this.StatusText.Content = "Starting benchmark.";
                }));

                INeuralDataSet training = RandomTrainingFactory.Generate(
                    this.paramTrainingSetSize,
                    this.paramInputNeurons,
                    this.paramOutputNeurons, -1, 1);
                BasicNetwork network = EncogUtility.SimpleFeedForward(
                    training.InputSize, this.paramHidden1Neurons, this.paramHidden2Neurons, training.IdealSize, true);

                EncogCLDevice device = Encog.Encog.Instance.CL.ChooseDevice();
                OpenCLTrainingProfile profile = new OpenCLTrainingProfile(device,
                    this.paramLocalRatio,this.paramGlobalRatio,this.paramSegRatio);


                Propagation train = new ResilientPropagation(network, training, profile);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while( train.CurrentIteration<this.paramTrainingIterations)
                {
                    train.Iteration(this.paramIterationsPerCall);
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        this.StatusText.Content = "Iteration:" + train.CurrentIteration;
                    }));
                }
                stopwatch.Stop();
                long clTime = stopwatch.ElapsedMilliseconds;

                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    this.LabelGlobalWorkgroupSize.Content = Format.FormatInteger(profile.KernelGlobalWorkgroup);
                    this.LabelLocalWorkgroupSize.Content = Format.FormatInteger(profile.KernelLocalWorkgroup);
                    this.LabelNumberCalls.Content = Format.FormatInteger(profile.KernelNumberOfCalls);
                    this.LabelRemainder.Content = Format.FormatInteger(profile.KernelRemainder);
                    this.LabelRemainderGlobal.Content = Format.FormatInteger(profile.KernelRemainderGlobal);
                    this.LabelRemainderPer.Content = Format.FormatInteger(profile.KernelRemainderPer);
                    this.LabelWorkPerCall.Content = Format.FormatInteger(profile.KernelWorkPerCall);
                    this.LabelBenchmarkTime.Content = Format.FormatDouble(clTime/1000.0,4) + "sec";
                    this.LabelActualIterations.Content = Format.FormatInteger( train.CurrentIteration );
                    this.ButtonShare.IsEnabled = true;
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
            finally
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    this.ButtonDefaults.IsEnabled = true;
                    this.ButtonEncog.IsEnabled = true;
                    this.StatusText.Content = "Ready.";
                }));
            }
        }

        private void ButtonDefaults_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to lose your custom benchmark settings, \n and revert to the standard Encog OpenCL benchmark?", "Reset to Defaults", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SetToDefaults();
            }
        }

        private void ButtonEncog_Click(object sender, RoutedEventArgs e)
        {
            if (this.device == null)
            {
                MessageBox.Show("You must select an OpenCL device first.");
                return;
            }
            this.ButtonDefaults.IsEnabled = false;
            this.ButtonEncog.IsEnabled = false;
            this.ButtonShare.IsEnabled = false;

            try
            {
                this.paramGlobalRatio = int.Parse(TextGlobalRatio.Text);
                this.paramLocalRatio = double.Parse(TextLocalRatio.Text);
                this.paramSegRatio = double.Parse(TextSegRatio.Text);
                this.paramInputNeurons = int.Parse(TextInputNeurons.Text);
                this.paramHidden1Neurons = int.Parse(TextHidden1Neurons.Text);
                this.paramHidden2Neurons = int.Parse(TextHidden2Neurons.Text);
                this.paramOutputNeurons = int.Parse(TextOutputNeurons.Text);
                this.paramIterationsPerCall = int.Parse(TextIterationsPerCall.Text);
                this.paramTrainingIterations = int.Parse(TextTrainingIterations.Text);
                this.paramTrainingSetSize = int.Parse(TextTrainingSetSize.Text);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Number, Check Your Input");
                return;
            }

            Thread thread = new Thread(this.BenchmarkProc);
            thread.Start();
        }

        private void ButtonShare_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
