//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using.System.Windows.Threading;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WpfMediaDB
{
    /// <summary>
    /// Interaction logic for audioPlayer.xaml
    /// </summary>
    public partial class audioPlayer : Window
    {
        // set boolean value classes to confirm the following
        // that by default of opening the player,
        // audio is not playing and progress is at 0
        private bool audioPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;

        public audioPlayer()
        {
            InitializeComponent();
            // Set parameters for how the player will be timed by declaring the
            // timer instance and setting its interval of incrementing every second
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }



        private void timer_Tick(object sender, EventArgs e)
        {
            // Ternary operators for the timer tick method
            // with the conditions of audio player progressing by at least 1 second
            // means that the timer will be increased and the slider will increment accordingly
            if((aPlayer.Source != null) && (aPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = aPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = aPlayer.Position.TotalSeconds;
            }
        }

        // Now with the command methods, where each command can be executed and will have corresponding
        // instructions as to what will occur in the executed method. 
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Audio files (*.mp3; *.mpg; *mpeg; *.wav; *.aac)| *.mp3; *.mpg; *mpeg; *.wav; *.aac";
            if(ofd.ShowDialog() == true)
                aPlayer.Source = new Uri(ofd.FileName);
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (aPlayer != null) && (aPlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            aPlayer.Play();
            audioPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = audioPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            aPlayer.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = audioPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            aPlayer.Stop();
            audioPlayerIsPlaying = false;
        }

        // userIsDraggingSlider boolean class will be referenced in these methods
        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            // command has started, so the parameters are now set to true
            // this means the user will be free to drag slider
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            // user has stopped dragging slider, slider will stop moving
            userIsDraggingSlider = false;
            // a new value will have been set.
            aPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // the time label will respond to any changes in the progress slider accordingly
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 
            aPlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }
    }
}
