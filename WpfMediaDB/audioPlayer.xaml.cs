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
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;

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
            if ((aPlayer.Source != null) && (aPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = aPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = aPlayer.Position.TotalSeconds;
            }
        }

        // Now setting commands that consist of methods that can be executed and the arguments that follow
        // and the behaviour of the audioplayer once an executed method has taken place
        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (aPlayer != null) && (aPlayer.Source != null);
            // where neither the aPlayer MediaElement instance equals not null as well as the source property
            // not equals null.
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // once select song has been pressed, this can be used to play an entry in the database
            // PROVIDED the file url exists
            try
            {
                aPlayer.Play();
                audioPlayerIsPlaying = true;
            }
            // The executed method ensures that the boolean class confirms as true
            // if button is executed on an invalid file url, the following message comes up
            catch (Exception)
            {
                MessageBox.Show("Well, the program broke, try entering a valid address for the file you want to play.", "Darn");
            }
        }

        // self-explanatory, the remaining Execute Routed events are just part of the buttons
        // commanded to behave like this
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
            DispatcherTimer timer = new DispatcherTimer();
            aPlayer.Stop();
            audioPlayerIsPlaying = false;
            // Simply sets the the boolean class as false once executed, returning the time of the song to zero.
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
            //  song has finished, slider will stop moving
            userIsDraggingSlider = false;
            // a new value will have been set.
            aPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // the time label will respond to any changes in the progress slider accordingly
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // this single line means that the volume of the music will be determined by the set values of the slider
            aPlayer.Volume = (double)volumeSlider.Value;
        }
    }
}
