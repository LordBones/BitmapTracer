using BitmapTracer.Core.basic;
using BitmapTracer.Core.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using BitmapTracer.Core.EdgeDetector;

namespace BitmapTracer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // BitmapSource _originalImage = new BitmapImage();

        CanvasARGB _currentImage = null;

        CanvasARGB _originalImage = null;
        CanvasARGB _lastImage = null;
        private string _currentOpenFilename;


        public MainWindow()
        {
            InitializeComponent();

            string path = LoadFilePath(); 
            if (!string.IsNullOrEmpty(path))
            {
                this._currentOpenFilename = System.IO.Path.GetFileName(path);
                BitmapImage bi = GetBitmapImage(new Uri(path), BitmapCacheOption.OnLoad);
                CanvasARGB canvas = CanvasARGB.CreateCanvasFromBitmap(bi);

                _originalImage = canvas;
                _lastImage = CanvasARGB.Clone(canvas);
                _currentImage = CanvasARGB.Clone(canvas);
            }

            Helper_SetAppTitle(string.Empty);
            ShowImage(false);

            VectorPreview window = new VectorPreview();
            window.Show();
        }

     

        #region actions
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "*.*|*.*|Bitmap Files (*.bmp)|*.bmp|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif" };
            var result = ofd.ShowDialog();
            if (result == false) return;

            this._currentOpenFilename = ofd.FileName;

            BitmapImage bi = GetBitmapImage(new Uri(ofd.FileName), BitmapCacheOption.OnLoad);
            CanvasARGB canvas = CanvasARGB.CreateCanvasFromBitmap(bi);
            _originalImage = canvas;
            SaveFilePath(ofd.FileName);
            _lastImage = CanvasARGB.Clone(canvas);

            ShowImage(false);


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();
            //for (int i = 0; i < 28000; i += 4)
            //{
            //    canvas.Data[i] = 0;
            //    canvas.Data[i + 1] = 255;
            //    canvas.Data[i + 2] = 0;
            //    canvas.Data[i + 3] = 255;
            //}


            CanvasPixel canvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);
            canvasPixel.TransformToInterleaveRGB();
            BasicOperation.PixelFilter2(canvasPixel, inputNumber);
            //canvasPixel.
            //BasicOperation.BlackWhite(canvasPixel);

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(canvasPixel);



            //BitmapSource bs = CanvasARGB.CreateBitmpaFromCanvas(canvas);

            //_originalImage = bs;
            //byte [] array = BitmapToArray(_originalImage);

            performanceCounter.Stop();

            Helper_SetAppTitle(string.Format("{0,000} s", performanceCounter.Elapsed.TotalSeconds));

            ShowImage(false);
            //RenderTargetBitmap.Create( rtb
            //textBox1.Text = ofd.FileName;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();

            RegionDetektor rd = new RegionDetektor();
            CanvasPixel cp = rd.Detect(canvas, inputNumber);
            // cp.TransformFromInterleaveRGB();

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(cp);

            performanceCounter.Stop();

            Helper_SetAppTitle(string.Format("{0,000} s    - regions : {1}", performanceCounter.Elapsed.TotalSeconds, rd.TotalRegions));

            ShowImage(false);

        }
               

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ShowImage(true);

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;
            
            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();

            RegionDetektor rd = new RegionDetektor();
            CanvasPixel cp = rd.DetectOld(canvas, inputNumber);
            //cp.TransformFromInterleaveRGB();

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(cp);

            performanceCounter.Stop();

            Helper_SetAppTitle(string.Format("{0,000} s    - regions : {1}", performanceCounter.Elapsed.TotalSeconds, rd.TotalRegions));

            ShowImage(false);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;
         
            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();

            CanvasPixel tmpCanvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            ColorReductor cr = new ColorReductor();
            cr.Reduce(tmpCanvasPixel, inputNumber);


            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(tmpCanvasPixel);

            performanceCounter.Stop();

            Helper_SetAppTitle( string.Format("{0,000} s    ", performanceCounter.Elapsed.TotalSeconds));

            ShowImage(false);
        }

        private void Button_Click_41(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;
           
            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();

            CanvasPixel tmpCanvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            ColorReductor cr = new ColorReductor();
            cr.ReduceByMask(tmpCanvasPixel, inputNumber);


            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(tmpCanvasPixel);

            performanceCounter.Stop();

            Helper_SetAppTitle(string.Format("{0,000} s    ", performanceCounter.Elapsed.TotalSeconds));

            ShowImage(false);
        }

        private void Button_Click_42(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput(); 

            
            CanvasPixel tmpCanvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            ColorReductor cr = new ColorReductor();
            cr.Reduce2(tmpCanvasPixel, 4);


            canvas = CanvasPixel.CreateBitmpaFromCanvas(tmpCanvasPixel);

            RegionDetektor rd = new RegionDetektor();
            CanvasPixel cp = rd.DetectOld(canvas, inputNumber);
            // cp.TransformFromInterleaveRGB();

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(cp);

            performanceCounter.Stop();

            Helper_SetAppTitle( string.Format("{0,000} s    - regions : {1}", performanceCounter.Elapsed.TotalSeconds, rd.TotalRegions));

            ShowImage(false);

        }

        private void Button_Click_NewBlend(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();


            CanvasPixel tmpCanvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            ColorReductor cr = new ColorReductor();
            CanvasPixel reducted  = cr.Reduce3(tmpCanvasPixel, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);

            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);
            //reducted = cr.Reduce31(reducted, inputNumber);


            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(reducted);

            performanceCounter.Stop();

            Helper_SetAppTitle( string.Format("{0,000} s    ", performanceCounter.Elapsed.TotalSeconds));

            ShowImage(false);

        }
       



        #endregion

        private byte[] BitmapToArray(BitmapImage bi)
        {
            int len = bi.PixelWidth * bi.PixelHeight * 4;
            byte[] tmp = new byte[len];
            bi.CopyPixels(tmp, bi.PixelWidth * 4, 0);
            return tmp;

            byte[] data;
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            byte[] result = new byte[data.Length - 64];
            Buffer.BlockCopy(data, 64, result, 0, result.Length);
            return result;
        }

        private CanvasARGB Get_ProgressOrOrigInput()
        {
            if(CheckProgresMode.IsChecked.Value)
            {
                return this._lastImage;
            }
            else
            {
                return this._originalImage;
            }
        }

        private void SetResultViewTo_Original()
        {
            this._currentImage = this._originalImage;
        }

        private void SetResultViewTo_LastResult()
        {
            this._currentImage = this._lastImage;
        }


        private void ShowImage(bool showOriginal)
        {
            if (showOriginal)
            {
                SetResultViewTo_Original();
            }
            else
            {
                SetResultViewTo_LastResult();
            }

            ImageCanvas.Source = CanvasARGB.CreateBitmpaFromCanvas(_currentImage);
        }

        public static BitmapImage GetBitmapImage(Uri imageAbsolutePath, BitmapCacheOption bitmapCacheOption = BitmapCacheOption.Default)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = bitmapCacheOption;
            image.UriSource = imageAbsolutePath;
            image.EndInit();

            return image;
        }

        public static void SaveFilePath(string path)
        {
            using (StreamWriter sw = new StreamWriter("lastImage.data", false))
            {
                sw.WriteLine(path);
            }
        }

        public static string LoadFilePath()
        {
            if (File.Exists("lastImage.data"))
            {
                string path;
                using (StreamReader sw = new StreamReader("lastImage.data"))
                {
                    path = sw.ReadLine();
                }

                if (File.Exists(path))
                {
                    return path;
                }
            }

            return string.Empty;
        }

        private void TraceNew_Click_1(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();


            CanvasPixel tmpCanvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            canvas = CanvasPixel.CreateBitmpaFromCanvas(tmpCanvasPixel);

            RegionDetector2_0 rd = new RegionDetector2_0(canvas);
            rd.Detect_ByColorToleranceNew(inputNumber);
            CanvasPixel cp = rd.Get_CanvasFromRegions();

            if (CheckSaveToSVG.IsChecked.Value)
                rd.ConvertRegionsToSVG(Helper_GetFileName_ForSVG(this._currentOpenFilename));

            // cp.TransformFromInterleaveRGB();

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(cp);

            performanceCounter.Stop();

            Helper_SetAppTitle( string.Format("{0,000} s    - regions : {1}", performanceCounter.Elapsed.TotalSeconds, rd.TotalRegions));

            ShowImage(false);
        }

        private void TraceNew_Click_2(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            int inputNumber = int.Parse(NumberInput.Text);

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();

            CanvasPixel tmpCanvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);
            canvas = CanvasPixel.CreateBitmpaFromCanvas(tmpCanvasPixel);

            RegionDetector2_0 rd = new RegionDetector2_0(canvas);
            rd.Detect_ByColorTolerance(inputNumber);

            if (CheckSaveToSVG.IsChecked.Value)
                rd.ConvertRegionsToSVG(Helper_GetFileName_ForSVG(this._currentOpenFilename));

            CanvasPixel cp = rd.Get_CanvasFromRegions();
            // cp.TransformFromInterleaveRGB();

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(cp);

            performanceCounter.Stop();

            Helper_SetAppTitle(string.Format("{0,000} s    - regions : {1}", performanceCounter.Elapsed.TotalSeconds, rd.TotalRegions));

            ShowImage(false);
        }

        private string Helper_GetPathFolder_ForSVG()
        {
            string directory = "SVGOut";
            if (!new DirectoryInfo(directory).Exists) new DirectoryInfo(directory).Create();
            return directory;
        }

        private string Helper_GetFileName_ForSVG(string originalFileName)
        {
            string baseName = "base";

            if (!string.IsNullOrEmpty(originalFileName))
            {
                baseName = System.IO.Path.GetFileNameWithoutExtension(originalFileName);
            }

            string directory = Helper_GetPathFolder_ForSVG();

            DateTime date = DateTime.Now;
            string fileName = $"{baseName}{date.Year}_{date.Month}_{date.Day}#{ date.Hour}_{ date.Minute}_{ date.Second}.svg";

            return System.IO.Path.Combine(directory, fileName);
        }

        private void ColorToCSV_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;


            
            CanvasARGB canvas = Get_ProgressOrOrigInput();

            using (TextWriter tw = new StreamWriter("testFile.csv", false, UTF8Encoding.UTF8))
            using (CsvHelper.CsvWriter cw = new CsvHelper.CsvWriter(tw))
            {
                cw.Configuration.Delimiter = ";" ;
                
                cw.Configuration.UseExcelLeadingZerosFormatForNumerics = true;

                cw.WriteField("r");
                cw.WriteField("g");
                cw.WriteField("b");
                cw.WriteField("left r");
                cw.WriteField("left g");
                cw.WriteField("left b");

                cw.NextRecord();


                // zapsani data na vystup
                for (int y = 0;y< canvas.HeightPixel;y++)
                for (int x = 0; x < canvas.WidthPixel; x++)
                    {
                        int index = y * canvas.Width + x * 4;

                        cw.WriteField(canvas.Data[index]);
                        cw.WriteField(canvas.Data[index+1]);
                        cw.WriteField(canvas.Data[index + 2]);

                        if(x > 0)
                        {
                            cw.WriteField(canvas.Data[index-4]);
                            cw.WriteField(canvas.Data[index-4 + 1]);
                            cw.WriteField(canvas.Data[index-4 + 2]);

                        }
                        else
                        {
                            cw.WriteField(0);
                            cw.WriteField(0);
                            cw.WriteField(0);

                        }


                        cw.NextRecord();
                    }


            }


        }

        private void DirectionToFile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;

            Stopwatch performanceCounter = new Stopwatch();
            performanceCounter.Start();

            CanvasARGB canvas = Get_ProgressOrOrigInput();
            CanvasPixel canvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            CanvasEdgeVO canvasEdges = CanvasEdgeVO.CreateEdgesFromCanvas(canvasPixel);

            EdgeDetector.EdgeReducer(canvasEdges, 255);

            EdgesWriteToFile(canvasEdges, "directions3.txt");

            CanvasPixel evisualise = VisualiseDetectedEdges(canvasEdges);

            _lastImage = CanvasPixel.CreateBitmpaFromCanvas(evisualise);

            

            performanceCounter.Stop();

            Helper_SetAppTitle(string.Format("{0,000} s ", performanceCounter.Elapsed.TotalSeconds));

            ShowImage(false);
        }

        private void EdgesWriteToFile(CanvasEdgeVO dataCE, string fileName)
        {
            using (TextWriter tw = new StreamWriter(fileName, false, UTF8Encoding.UTF8))
            {
                // zapsani data na vystup
                for (int y = 0; y < dataCE.Height; y++)
                {
                    for (int x = 0; x < dataCE.Width; x++)
                    {
                        int index = y * dataCE.Width + x;

                        Core.EdgeDetector.EdgePoint gResult = dataCE.Data[index];

                        if (gResult.Intensity < 1) tw.Write(".");
                        else
                        {

                            if (gResult.Direction == GradientDirection.askewFall) tw.Write("\\");
                            else if (gResult.Direction == GradientDirection.askewRaise) tw.Write("/");
                            else if (gResult.Direction == GradientDirection.horizontal) tw.Write("-");
                            else if (gResult.Direction == GradientDirection.vertical) tw.Write("|");
                            else if (gResult.Direction == GradientDirection.none) tw.Write(".");
                        }
                    }

                    tw.WriteLine();
                }
            }
        }

        private CanvasPixel VisualiseDetectedEdges(CanvasEdgeVO data)
        {
            CanvasPixel result = new CanvasPixel(data.Width, data.Height);

            for(int i = 0;i < data.Data.Length;i++)
            {
                int intesityEdge = data.Data[i].Intensity;
                if (intesityEdge > 255) intesityEdge = 255;

                byte oneChannel = (byte)intesityEdge;

                result.Data[i] = Pixel.Create(255, oneChannel, oneChannel, oneChannel);
            }

            return result;
        }

        private void Helper_SetAppTitle(string text)
        {
            string version = Helper_GetAppVersion();

            this.Title = $"{version} |=| {text}";
        }

        private string Helper_GetAppVersion()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            
            string displayableVersion = $"{version}";

            return displayableVersion;
        }

    }
}
