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
using System.Diagnostics;
using Microsoft.Kinect;
using Paelife.KinectFramework;
using Paelife.KinectFramework.MotionDetection;
using Paelife.KinectFramework.FaceRecognition;
using Paelife.KinectFramework.FaceTracking;
using Paelife.KinectFramework.Gestures;
using Paelife.KinectFramework.Postures;//added
using Microsoft.Speech.Recognition;
using System.Threading;
using SpotifyAPI.Local;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web;

namespace Paelife.KinectFramework.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectManager kinectManager;
        private WriteableBitmap colorBitmap;
        private byte[] colorPixels;

        private DateTime backgroundHighlightTime = DateTime.MinValue;
        private DateTime faceRecoDisplayTime = DateTime.MinValue;
        private DateTime centerMessageDisplayTime = DateTime.MinValue;
        private System.Windows.Media.Brush backgroudBrush;

       
        private SpeechRecognitionEngine sr;
        private Thread audioThread;
        private SpotifyLocalAPI _spotify;
        private SpotifyWebAPI _spotifyWeb;
        String id = "";
        Tts t = new KinectFramework.Tts();

        /*
            0 -> reject
            1 -> confirmation
            2 -> ok
            3 -> continue 
        */
        private int state;

        private string command;


        bool kinectStartedSucessfully = false;

        public MainWindow()
        {

            InitializeSpeech();
            InitializeComponent();
            
        }

        #region Speech
        private void InitializeSpeech()
        {

            String GName = "spotifygrammar.grxml";
            _spotify = new SpotifyLocalAPI();
            state = 3;
            command = "";

            //  look at existing recognizers
            //RecognizerInfo info = null;
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine(ri.Culture.EnglishName);
            }

            //creates the speech recognizer engine
            sr = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
            sr.SetInputToDefaultAudioDevice();


            Grammar gr = null;

            //verifies if file exist, and loads the Grammar file, else load defualt grammar
            if (System.IO.File.Exists(GName))
            {

                gr = new Grammar(GName);
                gr.Enabled = true;

                Console.WriteLine("Grammar Loaded");
            }
            else
            {
                Console.WriteLine("Can't read grammar file");

            }

            //load Grammar to speech engine
            sr.LoadGrammar(gr);
            //sr.LoadGrammar(g2);

            //assigns a method, to execute when speech is recognized
            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized2);

            audioThread = new Thread(startAudioListening);

            audioThread.Start();
        }

        private void startAudioListening()
        {
            // Assign input to the recognizer.
            sr.SetInputToDefaultAudioDevice();

            // Begin asynchronous recognition.
            sr.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Connect()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                Console.WriteLine(@"Spotify isn't running!");
                return;
            }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                Console.WriteLine(@"SpotifyWebHelper isn't running!");
                return;
            }

            bool successful = _spotify.Connect();
            if (successful)
            {
                Console.WriteLine(@"Connection to Spotify successful");
                _spotify.ListenForEvents = true;
            }
            else
            {
                Console.WriteLine(@"Couldn't connect to the spotify client");
                Connect();
            }
        }

        private void SpeechRecognized2(object sender, SpeechRecognizedEventArgs e)
        {
            //gets recognized text
            string text = e.Result.Text;
            string tagCmd = e.Result.Semantics["action"].Value != null ? e.Result.Semantics["action"].Value.ToString() : "";
            string tagObj = e.Result.Semantics["object"].Value != null ? e.Result.Semantics["object"].Value.ToString() : "";

            double confid = e.Result.Confidence;

            //actualizar comando a executar

            if (state != 1)
            {
                if (isCommandValid(tagCmd,tagObj))
                    command = tagCmd;
                else
                    command = "";
            }

            if (confid < 0.33)
            {
                if (state != 1)
                    state = 0;
                t.Speak("Não percebi, diz outra vez.");
                Console.WriteLine("Não percebi, diz outra vez.");
            }
            else if (confid >= 0.34 && confid < 0.66)
            {
                t.Speak("Tem a certeza que quer executar o comando " + text + "?");
                Console.WriteLine("Tem a certeza que quer executar o comando " + text + "?");
                state = 1;
            }
            else
            {
                if (state != 1)
                    state = 2;
            }

            if (state == 2)
                if (PostureVerify.getPosture() && (command == "volumeUp" || command == "volumeDown"))
                {
                    execMethod(command, tagObj);
                    PostureVerify.setPosture(false);
                }

                else
                {
                    if (command != "volumeUp" && command != "volumeDown")
                    {
                        execMethod(command, tagObj);
                    }
                    PostureVerify.setPosture(false);
                }

            else if (state == 1 && confid > 0.66)
            {
                if (text == "sim")
                {
                    state = 3;
                    if (PostureVerify.getPosture() && (command == "volumeUp" || command == "volumeDown"))
                    {
                        execMethod(command, tagObj);
                        PostureVerify.setPosture(false);
                    }

                    else
                    {
                        if (command != "volumeUp" && command != "volumeDown")
                        {
                            execMethod(command, tagObj);
                        }
                        PostureVerify.setPosture(false);
                    }
                }
                else if (text == "não")
                {
                    t.Speak("Então volta a repetir o comando por favor.");
                    state = 3;
                }
            }

            Console.WriteLine("Comando: " + text);
        }

        private bool isCommandValid(string action, string obj)
        {
            switch (action)
            {
                case "playMusic":
                    return (obj == "lambreta" || obj == "contentores" || obj == "não há estrelas no céu");
                case "playAlbum":
                    return (obj == "quinto" || obj == "concentrado" || obj == "a espuma das canções");
                case "playArtist":
                    return (obj == "António Zambujo" || obj == "Rui Veloso" || obj == "Pedro Abrunhosa");
                case "createList":
                case "addList":
                    return (obj == "Album de Verão" || obj == "jantar com os amigos" || obj == "ginásio");
                default:
                    return true;
            }
        }

        private async void execMethod(string cmd, string obj)
        {
            if (cmd == "open" && !SpotifyLocalAPI.IsSpotifyRunning()) // Start Spotify
            {
                Console.WriteLine("Abrir Spotify");
                SpotifyLocalAPI.RunSpotify();
                SpotifyLocalAPI.RunSpotifyWebHelper();
                Connect();
                AuthConnect();
                t.Speak("O que queres ouvir?");
            }

            if (SpotifyLocalAPI.IsSpotifyRunning())
            {

                if (cmd == "pause") // Pausar
                {
                    Console.WriteLine("Pausa");
                    await _spotify.Pause();
                    t.Speak("Estarei aqui à tua espera.");
                }

                if (cmd == "play") // Retomar
                {
                    Console.WriteLine("Retomar");
                    await _spotify.Play();
                }

                if (cmd == "mute") // Silenciar
                {
                    Console.WriteLine("Silenciar");
                    _spotify.Mute();
                    t.Speak("Já podes atender.");
                }

                if (cmd == "unmute") // Volume
                {
                    Console.WriteLine("Volume");
                    _spotify.UnMute();
                }


                if (cmd == "volumeUp") // Aumentar volume
                {
                    Console.WriteLine("Aumentar volume");
                    _spotify.SetSpotifyVolume(PostureVerify.getVol() + 25);
                }

                if (cmd == "volumeDown") // Diminuir volume
                {
                    Console.WriteLine("Dimiuir volume");
                    _spotify.SetSpotifyVolume(PostureVerify.getVol() - 25);
                }

                if (cmd == "close") // Fechar Spotify
                {
                    Console.WriteLine("Fechar");
                    SpotifyLocalAPI.StopSpotify();
                    t.Speak("Obrigado pela tua preferência. Até à próxima!");
                }

                if (cmd == "createList") // Criar lista de reprodução
                {
                    String name = obj;
                    Console.WriteLine("Criar lista de reprodução");
                    FullPlaylist playlist = _spotifyWeb.CreatePlaylist(id, name);
                    if (!playlist.HasError())
                        Console.WriteLine("Playlist-URI:" + playlist.Uri);

                    t.Speak("Lista de reprodução " + name + " criada com sucesso");

                }

                if (cmd == "addList") // Adicionar à lista de reprodução
                {
                    String name = obj;
                    Console.WriteLine("Adicionar à lista de reprodução");
                    String uri = _spotify.GetStatus().Track.TrackResource.Uri;
                    List<SimplePlaylist> playlist = _spotifyWeb.GetUserPlaylists(id).Items;
                    int index = playlist.FindIndex(x => x.Name == name);
                    _spotifyWeb.AddPlaylistTrack(id, _spotifyWeb.GetUserPlaylists(id).Items[index].Id, uri);
                    t.Speak("Música adicionada com sucesso!");
                }

                if (cmd == "removeList") // Remover música da lista de reprodução
                {
                    Console.WriteLine("Remover música da lista de reprodução");
                    String uri = _spotify.GetStatus().Track.TrackResource.Uri;
                    String playlist = _spotifyWeb.GetUserPlaylists(id).Items[0].Id;
                    _spotifyWeb.RemovePlaylistTrack(id, playlist, new DeleteTrackUri(uri));
                    t.Speak("Música removida!");
                }

                if (cmd == "identify") // Reconhecer música actual
                {
                    Console.WriteLine("Não me lembro do nome desta música");
                    String name = _spotify.GetStatus().Track.TrackResource.Name;
                    t.Speak("A música que estás a ouvir é" + name);
                }


                if (cmd == "playMusic") // Ouvir música
                {
                    SearchType search_type = SearchType.Track;
                    SearchItem item;
                    String musica;

                    musica = obj;
                    Console.WriteLine("Ouvir " + musica + "!");
                    item = _spotifyWeb.SearchItems(musica, search_type, 1, 0, "pt");
                    await _spotify.PlayURL(item.Tracks.Items[0].Uri, "");
                           
                    

                }

                if (cmd == "playAlbum") // Ouvir álbum
                {
                    SearchType search_type = SearchType.Album;
                    SearchItem item;
                    String album;

                    album = obj;
                    Console.WriteLine("Ouvir " + album + "!");
                    item = _spotifyWeb.SearchItems(album, search_type, 1, 0, "pt");
                    await _spotify.PlayURL(item.Albums.Items[0].Uri, "");

                }

                if (cmd == "playArtist") // Ouvir artista
                {
                    SearchType search_type = SearchType.Track;
                    SearchItem item;
                    String artist;

                    artist = obj;
                    Console.WriteLine("Ouvir " + artist + "!");
                    item = _spotifyWeb.SearchItems(artist, search_type, 1, 0, "pt");
                    await _spotify.PlayURL(item.Artists.Items[0].Uri, "");

                }

            }
            else
            {
                t.Speak("Spotify não está aberto.");
            }
        }

        private async void AuthConnect()
        {
            WebAPIFactory webApiFactory = new WebAPIFactory(
                "http://localhost",
                8000,
                "09a444084b5d41b28ba2c1e79d34be31",
                Scope.UserReadPrivate,
                TimeSpan.FromSeconds(60)
            );

            bool check = false;
            int count = 0;
            while ((_spotifyWeb == null || check) && count < 15)
            {
                count++;
                check = false;
                try
                {
                    _spotifyWeb = await webApiFactory.GetWebApi();
                    Thread.Sleep(1500);
                }

                catch (Exception ex)
                {
                    check = true;
                    Console.WriteLine(ex.Message);
                }

            }

            id = _spotifyWeb.GetPrivateProfile().Id;

        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            backgroudBrush = this.Background;
            
            kinectManager = new KinectManager();
            InitializeKinectManager();

        }

        private void InitializeKinectManager()
        {
            if (kinectManager.KinectSensor != null && !kinectStartedSucessfully)
            {
                kinectManager.KinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(KinectSensor_ColorFrameReady);

                this.colorPixels = new byte[kinectManager.KinectSensor.ColorStream.FramePixelDataLength];
                this.colorBitmap = new WriteableBitmap(kinectManager.KinectSensor.ColorStream.FrameWidth, kinectManager.KinectSensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                this.image1.Source = this.colorBitmap;

                kinectManager.PropertyChanged +=
                    new System.ComponentModel.PropertyChangedEventHandler(kinectManager_PropertyChanged);

                //kinectManager.KinectSensor.ElevationAngle = 15;

                textBoxMessagesCenter.Visibility = System.Windows.Visibility.Collapsed;
                kinectStartedSucessfully = true;

                kinectManager.DetectMotion = true;
                kinectManager.MotionDetector.MotionDetected +=
                    new EventHandler(motionDetector_MotionDetected);

                kinectManager.FaceTracking = true;
                kinectManager.FaceTracker.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FaceTracker_PropertyChanged);

                GestureDetector LeftHandSwipeGestureDetector =
                new SwipeGestureDetector(Microsoft.Kinect.JointType.HandLeft);
                LeftHandSwipeGestureDetector.OnGestureDetected +=
                    new Action<string>(LeftHandSwipeGestureDetector_OnGestureDetected);
                kinectManager.GestureDetectors.Add(LeftHandSwipeGestureDetector);
        

                PostureDetector PostureDetector =
                    new AlgorithmicPostureDetector();
                PostureDetector.PostureDetected +=
                    new Action<string>(PostureDetector_PostureDetected);
                kinectManager.PostureDetectors.Add(PostureDetector);
            }

            if (!kinectStartedSucessfully)
            {
                textBoxMessagesCenter.Text = "DISCONNECTED";
                textBoxMessagesCenter.Visibility = System.Windows.Visibility.Visible;
            }


        }

        void LeftHandSwipeGestureDetector_OnGestureDetected(string gesture)
        {
            textBoxMessagesCenter.Text = "Left Hand: " + gesture;
            textBoxMessagesCenter.Visibility = System.Windows.Visibility.Visible;

            centerMessageDisplayTime = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new UpdateUIDelegate(UpdateUI));
        }

        void RightHandSwipeGestureDetector_OnGestureDetected(string gesture)
        {
            textBoxMessagesCenter.Text = "Right Hand: " + gesture;
            textBoxMessagesCenter.Visibility = System.Windows.Visibility.Visible;

            centerMessageDisplayTime = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new UpdateUIDelegate(UpdateUI));
        }

        void PostureDetector_PostureDetected(string posture)
        {
            textBoxMessagesCenter.Text = posture;
            textBoxMessagesCenter.Visibility = System.Windows.Visibility.Visible;

            centerMessageDisplayTime = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new UpdateUIDelegate(UpdateUI));
        }

        void kinectManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDisconnected")
            {
                if (((KinectManager)sender).IsDisconnected)
                {
                    //aqui devia ser chamada uma funcao de uma funcao de des-inicializacao
                    textBoxMessagesCenter.Text = "DISCONNECTED";
                    textBoxMessagesCenter.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    InitializeKinectManager();
            }

            if (e.PropertyName == "NumberOfDetectUsers")
                textBlockStatus.Text = "Number of Detected Users = " + ((KinectManager)sender).NumberOfDetectUsers;
        }


        void FaceTracker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            textBlockFace.Text = "Face Tracking Status = ";

            switch (kinectManager.FaceTracker.FaceTrackingCurrentState)
            {
                case FaceTracker.FaceTrackingState.BothUsersAreLooking:
                    textBlockFace.Text += "BothUsersAreLooking";
                    break;
                case FaceTracker.FaceTrackingState.FarthestUserIsLooking:
                    textBlockFace.Text += "FarthestUserIsLooking";
                    break;
                case FaceTracker.FaceTrackingState.NearestUserIsLooking:
                    textBlockFace.Text += "NearestUserIsLooking";
                    break;
                case FaceTracker.FaceTrackingState.NeitherUserIsLooking:
                    textBlockFace.Text += "NeitherUserIsLooking";
                    break;
                case FaceTracker.FaceTrackingState.UserIsLooking:
                    textBlockFace.Text += "UserIsLooking";
                    break;
                case FaceTracker.FaceTrackingState.UserNotLooking:
                    textBlockFace.Text += "UserNotLooking";
                    break;
                case FaceTracker.FaceTrackingState.Disabled:
                    textBlockFace.Text += "Disabled";
                    break;
                case FaceTracker.FaceTrackingState.UnableToDetectFaces:
                    textBlockFace.Text += "UnableToDetectFaces";
                    break;
            }
        }

        void KinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame colorImageFrame = null;
            colorImageFrame = e.OpenColorImageFrame();
            if (colorImageFrame == null)
                return;

            colorImageFrame.CopyPixelDataTo(this.colorPixels);
            this.colorBitmap.WritePixels(
                new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                this.colorPixels,
                this.colorBitmap.PixelWidth * sizeof(int),
                0);

            if (colorImageFrame != null)
                colorImageFrame.Dispose();
        }


        void motionDetector_MotionDetected(object sender, EventArgs e)
        {
            this.backgroundHighlightTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new UpdateUIDelegate(UpdateUI));
        }



        private delegate void UpdateUIDelegate();

        private void UpdateUI()
        {
            if (DateTime.UtcNow < this.backgroundHighlightTime)
            {
                this.Background = System.Windows.Media.Brushes.Red;
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new UpdateUIDelegate(UpdateUI));
            }
            else
                this.Background = backgroudBrush;

            if (DateTime.UtcNow < this.faceRecoDisplayTime)
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new UpdateUIDelegate(UpdateUI));
            else textBlockFaceRecognition.Text = "";

            if (DateTime.UtcNow < this.centerMessageDisplayTime)
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new UpdateUIDelegate(UpdateUI));
            else textBoxMessagesCenter.Text = "";
        }

        private void buttonSaveFace_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            try
            {
                kinectManager.FaceRecognizer.DetectAndSaveFace(textBoxUsername.Text);
                msg = "Face of " + textBoxUsername.Text + " was saved successfuly.";
            }
            catch (FaceRecognitionException ex)
            {
                msg = ex.Message;                
            }
            finally
            {
                textBlockFaceRecognition.Text = msg;

                faceRecoDisplayTime = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new UpdateUIDelegate(UpdateUI));
            }
        }

        private void buttonRecognizeFace_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            try
            {
                string user = kinectManager.FaceRecognizer.RecognizeFace();
                if (user != string.Empty)
                {
                    msg = "Face recognized = " + user;

                    if (kinectManager.FaceRecognizer.NumberOfImages(user) < 5)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            "User \"" + user + "\" was recognized. Is this correct?",
                            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                            kinectManager.FaceRecognizer.LastRecognitionWasSuccessful();
                        else msg = "Last face recognition was unsuccessful. Please save more images of users.";
                    }
                }
                else msg = "Couldn't recognize the user. Please save more images of users.";
            }
            catch (FaceRecognitionException ex)
            {
                msg = ex.Message;
            }
            finally
            {
                textBlockFaceRecognition.Text = msg;

                faceRecoDisplayTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new UpdateUIDelegate(UpdateUI));
            }
        }

       
    }
}
