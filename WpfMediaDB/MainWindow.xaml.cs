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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // set boolean value classes to confirm the following
        // that by default of opening the player,
        // audio is not playing and progress is at 0
        private bool audioPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private const String access7ConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\entries.accdb";
        // Data components
        private OleDbConnection myConnection;
        private DataTable myDataTable;
        private OleDbDataAdapter myAdapter;
        private OleDbCommandBuilder myCommandBuilder;

        // Index of the current record
        private int currentRecord = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            String command = "SELECT * FROM Music";
            try 
            {
                myConnection = new OleDbConnection(access7ConnectionString);
                myAdapter = new OleDbDataAdapter(access7ConnectionString, myConnection);
                myCommandBuilder = new OleDbCommandBuilder(myAdapter);
                myDataTable = new DataTable();
                FillDataTable(command);

                DisplayRow(currentRecord);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // Set parameters for how the player will be timed by declaring the
            // timer instance and setting its interval of incrementing every second
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void FillDataTable(string selectCommand)
        {
            try
            {
                myConnection.Open();
                myAdapter.SelectCommand.CommandText = selectCommand;
                // Get schema for entries table to fully configure datatable
                myAdapter.FillSchema(myDataTable, SchemaType.Source);
                // Fill the datatable with the rows returned by the select command
                myAdapter.Fill(myDataTable);
                myConnection.Close();
            }

            catch(Exception ex)
            {
                MessageBox.Show("Error in FillDataTable: \r\n" + ex.Message);
                // close the myConnection class
                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Close();
                }
            }

        }

        private void DisplayRow(int rowIndex)
        { 
            // Check if given row can be retrieved
            if (myDataTable.Rows.Count == 0)
            return; // nowt to display
            if (rowIndex >= myDataTable.Rows.Count)
                return; // the index is out of range

            try
            {
                DataRow row = myDataTable.Rows[rowIndex];
                trackNumberTextBox.Text = row["TrackNo"].ToString();
                trackNameTextbox.Text = row["TrackName"].ToString();
                artistTextbox.Text = row["Artist"].ToString();
                albumTextbox.Text = row["Album"].ToString();
                genreTextbox.Text = row["Genre"].ToString();
                yearReleasedTextbox.Text = row["YearReleased"].ToString();
                filePathTextbox.Text = row["FilePath"].ToString();
            }
            catch (Exception ex)
            { 
                MessageBox.Show("Error in DisplayRow : \r\n" + ex.Message);
            }

            statusLbl.Content = (currentRecord + 1).ToString() + " of " + myDataTable.Rows.Count.ToString();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // Increments on the bases that currentRecord is not equal to
            // the amount of data entries
            if (currentRecord != myDataTable.Rows.Count - 1)
            {
                currentRecord++;
                DisplayRow(currentRecord);
            }
            // when last entry has been reached, following code has been implemented to produce a message box.
            else
            {
                const string caption = "Can't go further";
                const string message = "Reached final entry";
                var result = MessageBox.Show(message, caption);
            }
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            // provided current record is greater than zero, it'll
            // be possible to decrement each time previous button is clicked
            // to go back on entries
            if (currentRecord > 0)
            {
                currentRecord--;
                DisplayRow(currentRecord);
            }
            // otherwise the message informs the user that they are at the first entry
            else
            {
                const string caption = "Can't go further back";
                const string message = "At first entry";
                var result = MessageBox.Show(message, caption);
            }
        }

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {
            currentRecord = 0;
            DisplayRow(currentRecord);
        }

        private void lastButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentRecord != myDataTable.Rows.Count - 1)
            {
                currentRecord = myDataTable.Rows.Count - 1;
                DisplayRow(currentRecord);
            }
        }

        private void modify_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editDialog = new EditWindow(myDataTable.Rows[currentRecord]);
            // edit window as Dialog type prevents users from using main window whilst editing
            if (editDialog.ShowDialog() == true)
            {
                // Display the edited row
                DisplayRow(currentRecord);
                // Commit changes to database
                UpdateDB();
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Get a new row from the data table
            DataRow newRow = myDataTable.NewRow();
            EditWindow editDialog = new EditWindow(newRow);
            // this procedure is similiar to that of the modify button click event
            if (editDialog.ShowDialog() == true)
            {
                myDataTable.Rows.Add(newRow);
                // Locate the newly added row
                currentRecord = myDataTable.Rows.IndexOf(newRow);
                DisplayRow(currentRecord);
                // Commit changes to changes database
                UpdateDB();
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this song entry?", "Delete song", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // mark row for deletion
                    myDataTable.Rows[currentRecord].Delete();
                    // commit the deletion to database
                    UpdateDB();
                    currentRecord = 0;
                    DisplayRow(currentRecord);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in deleteButton_Click: \r\n" + ex.Message);
                }
            }
        }

        private void UpdateDB()
        {
            try
            {
                myConnection.Open();
                myAdapter.Update(myDataTable);
                myConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in methods of updating : \r\n" + ex.Message);
            }
        }
        

        private void exitMenuButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        // Now with the command methods, where each command can be executed and will have corresponding
        // instructions as to what will occur in the executed method. 
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "*.mp3";
            ofd.Filter = "Audio files (*.mp3; *.mpg; *.mpeg; *.wav; *.aac)| *.mp3; *.mpg; *.mpeg; *.wav; *.aac";
            if (ofd.ShowDialog() == true)
            {
                // the media element is  
                aPlayer.Source = new Uri(ofd.FileName);

                // these three lines link the
                ID3v1TagReader tr = new ID3v1TagReader();
                ID3v1Tag ti = new ID3v1Tag();
                ti = tr.ReadID3v1Tag(ofd.FileName);

                filePathTextbox.Text = ofd.FileName;
                trackNameTextbox.Text = ti.TrackName;
                artistTextbox.Text = ti.ArtistsName;
                albumTextbox.Text = ti.AlbumName;
                genreTextbox.Text = ti.Genres;
                yearReleasedTextbox.Text = ti.Year;

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
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = filePathTextbox.Text.ToString();
                aPlayer.Source = new Uri(ofd.FileName);
                aPlayer.Play();
                audioPlayerIsPlaying = true;
            }
            // The executed method ensures that the boolean class confirms as true
            catch (Exception)
            {
                MessageBox.Show("Well, the program broke, try entering a valid address for the file you want to play.", "Darn");
            }
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
            aPlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // the search button function now changes the remaining textboxes whenever a line is entered
            // if there exists entries with similiar attributes like same artist and album, will navigate through available results
            myDataTable.Clear();
            string command = "SELECT * FROM Music WHERE TrackName LIKE '%" + searchTextbox.Text+ "%' OR Artist LIKE '%" + searchTextbox.Text + "%' OR Album LIKE '%" + searchTextbox.Text + "%' OR Genre LIKE '%" + searchTextbox.Text + "&' ";
            FillDataTable(command);
            DisplayRow(currentRecord);
            statusLbl.Content = (currentRecord + 1).ToString() + " of " + myDataTable.Rows.Count.ToString();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            // possible to play any song based on its URL
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = filePathTextbox.Text.ToString();
                aPlayer.Source = new Uri(ofd.FileName);
            }

            catch (Exception)
            {
                MessageBox.Show("Well, the program broke, try entering a valid address for the file you want to play.", "Darn");
            }
        }

    }
}
