using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using AI_LUL;
using System.Windows.Threading;
using System.IO;
using Brushes = System.Windows.Media.Brushes;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SqlClient;

namespace test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoCapture capture;
        CascadeClassifier head;
        public volatile DispatcherTimer timer;
        public volatile DispatcherTimer csvUpdate;
        string humanheightcm = "10";
        System.Drawing.Rectangle face;
        System.Drawing.Rectangle body;
        Image<Bgr, Byte> imgInput;
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        List<string> result;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            //CaptureFrame.IsEnabled = false;
        }

        private void csvUpdate_tick(object sender, EventArgs e)
        {
            result = loadCsvFile();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //StartPython();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Image<Bgr, Byte> currentFrame = capture.QueryFrame().ToImage<Bgr, Byte>();
            Image<Bgr, Byte> anotherFrame = capture.QueryFrame().ToImage<Bgr, Byte>();
            Image<Gray, Byte> GrayImage = new Image<Gray, Byte>(currentFrame.Bitmap);
            CvInvoke.CvtColor(currentFrame, GrayImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            imgInput = anotherFrame;

            if (currentFrame != null)
            {
                System.Drawing.Rectangle[] detectedfaces = head.DetectMultiScale(GrayImage, 1.1, 5, new System.Drawing.Size(50, 50), new System.Drawing.Size(200, 200));
                foreach (var faces in detectedfaces)
                {
                    face = faces;
                    body = faces;
                    body.Inflate(face.Width * 3, face.Height * 4);
                    trackface.measureface(faces, currentFrame, humanheightcm);
                }
                BitmapSource bs = ToBitmapSource(currentFrame);
                TrackingVideo.Source = bs;
            }
        }

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap
                
                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                return bs;
            }
        }

        private void StartCapture_Click(object sender, RoutedEventArgs e)
        {
            //StartPython();
           
            CaptureFrame.IsEnabled = true;
            capture = new VideoCapture();
            head = new CascadeClassifier(@"haarcascade_frontalface_default.xml");
            //timer 1 stuff + Stopwatch
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(1);       //set the refresh rate to 1 mili seconds
            timer.Start();
            csvUpdate = new DispatcherTimer();
            csvUpdate.Tick += new EventHandler(csvUpdate_tick);
            csvUpdate.Interval = new TimeSpan(5);
            csvUpdate.Start();

            //start capture images with webcam
            capture.Start();
            //backgroundWorker1.RunWorkerAsync();
            List<string> result = loadCsvFile();
        }

        int counter = 0;
        private void CaptureFrame_Click(object sender, RoutedEventArgs e)
        {
            if (TrackingVideo.Source != null)
            {
                if (face != null)
                {
                    imgInput.ROI = face;
                    Image<Bgr, Byte> temp = imgInput.CopyBlank();
                    imgInput.CopyTo(temp);
                    imgInput.ROI = System.Drawing.Rectangle.Empty;

                    BitmapSource bs = ToBitmapSource(temp);
                    FacePic.Source = bs;

                    string filePath = "OUTPUT_PICS_face\\face" + counter + ".jpg";
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bs));
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        encoder.Save(stream);
                }
                if (body != null)
                {
                    imgInput.ROI = body;
                    Image<Bgr, Byte> temp = imgInput.CopyBlank();
                    imgInput.CopyTo(temp);
                    imgInput.ROI = System.Drawing.Rectangle.Empty;

                    BitmapSource bs = ToBitmapSource(temp);
                    BodyPic.Source = bs;

                    string filePath = "OUTPUT_PICS_body\\body" + counter + ".jpg";
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bs));
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        encoder.Save(stream);
                }
                counter++;
            }
            
        }

        int imagecounter = 1;
        private void Previous_Click_1(object sender, RoutedEventArgs e)
        {
            if (imagecounter > 1)
            {
                imagecounter--;
                Console.WriteLine(imagecounter);
                Advertisement.Source = new BitmapImage(new Uri(@"C:\Users\User\Desktop\AIHackathon\test\test\adimages\image" + imagecounter + ".jpeg"));
            }
            else
            {
                imagecounter = 4;
                Advertisement.Source = new BitmapImage(new Uri(@"C:\Users\User\Desktop\AIHackathon\test\test\adimages\image" + imagecounter + ".jpeg"));
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (imagecounter < 4)
            {
                imagecounter++;
                Console.WriteLine(imagecounter);
                Advertisement.Source = new BitmapImage(new Uri(@"C:\Users\User\Desktop\AIHackathon\test\test\adimages\image" + imagecounter + ".jpeg"));
            }
            else
            {
                imagecounter = 1;
                Advertisement.Source = new BitmapImage(new Uri(@"C:\Users\User\Desktop\AIHackathon\test\test\adimages\image" + imagecounter + ".jpeg"));
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button thisButt = sender as Button;
            thisButt.Background = Brushes.Red;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button thisButt = sender as Button;
            thisButt.Background = Brushes.Black;
        }

        private void StartPython()
        {
            string python = @"py.exe";

            // python app to call 
            string myPythonApp = @"C:\Users\ivanl\OneDrive\Documents\GitHub\AI-LUL-WPF-Version\test\bin\Debug\WEB_SERVER.py";
            //string myPythonApp = "WEB_SERVER.py";
            // Create new process start info 
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);

            // make sure we can read the output from stdout 
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;

            // start python app with 3 arguments  
            // 1st arguments is pointer to itself,  
            // 2nd and 3rd are actual arguments we want to send 
            myProcessStartInfo.Arguments = myPythonApp;

            Process myProcess = new Process();
            // assign start information to the process 
            myProcess.StartInfo = myProcessStartInfo;

            // start the process 
            myProcess.Start();

            // Read the standard output of the app we called.  
            // in order to avoid deadlock we will read output first 
            // and then wait for process terminate: 
            StreamReader myStreamReader = myProcess.StandardOutput;
            string one = myStreamReader.ReadLine();
            string two = myStreamReader.ReadLine();
            string three = myStreamReader.ReadLine();
            Debug.Print(one+two+three);
            myProcess.WaitForExit();
            myProcess.Close();
        }

        private List<string> loadCsvFile()
        {
            List<string> searchList = new List<string>();
            try {
                var reader = new StreamReader(File.OpenRead(@"data.csv"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Console.WriteLine(line);
                    searchList.Add(line);
                    string[] words = line.Split(',');

                    AgeLabel.Content = words[1];
                    GenderLabel.Content = words[2];
                    ClassLabel.Content = words[10];

                    List<string> emotionlist = new List<string> { "Angry", "Disgust", "Fear", "Happy", "Sad", "Surprised", "Neutral" };

                    double angry;
                    double disgust;
                    double fear;
                    double happy;
                    double sad;
                    double surprised;
                    double neutral;

                    double.TryParse(words[3], out angry);
                    double.TryParse(words[4], out disgust);
                    double.TryParse(words[5], out fear);
                    double.TryParse(words[6], out happy);
                    double.TryParse(words[7], out sad);
                    double.TryParse(words[8], out surprised);
                    double.TryParse(words[9], out neutral);

                    List<double> emotion = new List<double> { angry, disgust, fear, happy, sad, surprised, neutral };
                    int finalemotion = emotion.IndexOf(emotion.Max());

                    if(emotionlist[finalemotion] == "Happy")
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F604);
                    }
                    else if (emotionlist[finalemotion] == "Neutral")
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F610);
                    }
                    else if (emotionlist[finalemotion] == "Sad")
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F622);
                    }
                    else if (emotionlist[finalemotion] == "Angry")
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F626);
                    }
                    else if (emotionlist[finalemotion] == "Surprised")
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F62E);
                    }
                    else if (emotionlist[finalemotion] == "Disgust")
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F615);
                    }
                    else
                    {
                        EmotionLabel.Content = char.ConvertFromUtf32(0x1F60D);
                    }
                    EmotionLabel2.Content = emotionlist[finalemotion];
                    foreach (string word in words)
                    {
                        Console.WriteLine(word);
                    }

                }
            }
            catch
            {

            }

            
            return searchList;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
