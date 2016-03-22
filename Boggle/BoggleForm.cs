using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boggle
{
    /**
     * A Form to display Boggle Puzzles, allow users to "play" the game by
     * guessing words, and solve the board/highlight solutions. Also allows
     * users to set the dimensions of the board, shake the board, and alter
     * the playing time.
     * 
     * @author Jason Allen
     */
    public partial class BoggleForm : Form
    {
        private Label[,] letterLabels;
        private SortedDictionary<String, Boolean[,]> solutionsAndPaths;
        private Panel lettersPanel;
        private FlowLayoutPanel solutionsPanel;
        private Panel playPanel;
        private int currentDimension = 6;
        private NumericUpDown setTimeUpDown;
        private NumericUpDown setGridUpDown;
        private int time = 60;
        private int currentTime;
        private Label currentScoreLabel;
        private Label timeLabel;
        private Label highScoreLabel;
        private Button solveButton;
        private Button playButton;
        private Timer timer;
        private int currentScore;
        private int highScore;
        private TextBox guessesDisplayArea;
        private TextBox guessBox;
        private List<String> guessed;
        private Button guessButton;
        private Button setSettingsButton;
        private Label guessLabel;

        /**
         * Constructs a BoggleForm object to display the game on
         */
        public BoggleForm()
        {
            InitializeComponent();
            this.MaximumSize = new Size(1200, 750);
            CreateNewBoggleBoard(currentDimension);
        }

         /**
         * Solves the puzzle and places the panels to view the letters, solutions, and play
         * the game
         * 
         * @param dimension: the dimensions of the board
         */
        private void CreateNewBoggleBoard(int dimension)
        {
            currentTime = time;
            currentScore = 0;
            guessed = new List<String>();

            // timers can stubbornly persist due to threading, so it best to stop them manually
            // instead of simply instantiate a new one if one already exists from a previous
            // game
            if (timer != null)
            {
                timer.Stop();
            }
            else
            {
                timer = new Timer();
                timer.Tick += new EventHandler(TimerTicker);
                timer.Interval = 1000;
            }

            // get a puzzle and solve it
            Board board = new Board(dimension);
            String[,] letters = board.getLetters();
            BoggleDictionary dictionary = new BoggleDictionary();
            Solver solver = new Solver(letters);
            solutionsAndPaths = solver.Solve(dictionary);

            // set up the labels for each letter on the board
            letterLabels = new Label[dimension, dimension];
            TableLayoutPanel lettersTable = new TableLayoutPanel();
            lettersTable.ColumnCount = dimension;
            lettersTable.RowCount = dimension;
            lettersTable.BackColor = Color.White;
            lettersTable.AutoScroll = false;
            lettersTable.AutoSize = true;
            lettersTable.Anchor = AnchorStyles.None;

            TableLayoutPanel lettersAndShakePanel = new TableLayoutPanel();
            lettersAndShakePanel.Dock = DockStyle.Fill;
            lettersAndShakePanel.ColumnCount = 1;
            lettersAndShakePanel.RowCount = 2;

            lettersPanel = new Panel();
            lettersPanel.AutoScroll = true;
            lettersPanel.AutoSize = false;
            lettersPanel.Anchor = AnchorStyles.None;
            lettersPanel.Width = 550;
            lettersPanel.Height = lettersPanel.Width;
            lettersPanel.Controls.Add(lettersTable);

            // Set up the shake button with handler to create a new board
            Button shakeButton = new Button();
            shakeButton.Text = "Shake";
            shakeButton.Click += new EventHandler(Shake);
            shakeButton.Anchor = AnchorStyles.None;

            lettersAndShakePanel.Controls.Add(shakeButton, 0, 0);
            lettersAndShakePanel.Controls.Add(lettersPanel, 0, 1);

            this.Controls.Add(lettersAndShakePanel);

            // make a new panel for each letter grid space, then put a panel in each and set the background to white, and fill with appropriate letter
            for (int i = 0; i < letterLabels.GetLength(0); i++)
            {
                for (int j = 0; j < letterLabels.GetLength(0); j++)
                {
                    Label l = new Label();
                    l.Text = letters[i, j];
                    l.BackColor = Color.White;
                    l.AutoSize = false;
                    l.TextAlign = ContentAlignment.MiddleCenter;
                    l.Dock = DockStyle.Fill;
                    l.Width = lettersPanel.Width / letterLabels.GetLength(0);
                    l.Height = lettersPanel.Width / letterLabels.GetLength(0);
                    letterLabels[i, j] = l;
                    l.Margin = new Padding(0, 0, 0, 0);
                    l.BorderStyle = BorderStyle.FixedSingle;
                    lettersTable.Controls.Add(l, i, j);
                }

            }

            // set up solutions panel
            solutionsPanel = new FlowLayoutPanel();
            solutionsPanel.AutoScroll = true;
            solutionsPanel.Dock = DockStyle.Right;

            solveButton = new Button();
            solveButton.Text = "Solve";
            solveButton.Margin = new Padding(55, 30, 0, 0);
            solutionsPanel.Controls.Add(solveButton);
            solveButton.Click += new EventHandler(SolveButtonHandler);
            this.Controls.Add(solutionsPanel);

            // set up play panel
            playPanel = new FlowLayoutPanel();
            playPanel.Dock = DockStyle.Left;

            playButton = new Button();
            playButton.Text = "Play";
            playButton.Margin = new Padding(65, 30, 0, 0);
            playButton.Margin = new Padding(65, 30, 0, 0);
            playButton.Click += new EventHandler(PlayHandler);
            playPanel.Controls.Add(playButton);
            this.Controls.Add(playPanel);

            guessesDisplayArea = new TextBox();
            guessesDisplayArea.Height = 280;
            guessesDisplayArea.Width = 200;
            guessesDisplayArea.Multiline = true;
            guessesDisplayArea.Margin = new Padding(15, 15, 0, 0);
            guessesDisplayArea.Font = new Font(FontFamily.GenericMonospace, 9);
            guessesDisplayArea.ReadOnly = true;
            guessesDisplayArea.BackColor = Color.LightGray;
            guessesDisplayArea.ScrollBars = ScrollBars.Vertical;

            timeLabel = new Label();
            timeLabel.Width = 200;
            timeLabel.AutoSize = false;
            timeLabel.TextAlign = ContentAlignment.MiddleCenter;
            timeLabel.Margin = new Padding(0, 10, 0, 0);
            timeLabel.Text = "Time: " + time;
            timeLabel.Font = new Font("Verdana", 12, FontStyle.Bold);
            playPanel.Controls.Add(timeLabel);

            highScoreLabel = new Label();
            highScoreLabel.Width = 200;
            highScoreLabel.AutoSize = false;
            highScoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            highScoreLabel.Margin = new Padding(0, 4, 0, 0);
            highScoreLabel.Text = "High Score: " + highScore;
            highScoreLabel.Font = new Font("Verdana", 10);

            currentScoreLabel = new Label();
            currentScoreLabel.Width = 200;
            currentScoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            currentScoreLabel.Margin = new Padding(0, 4, 0, 0);
            currentScoreLabel.Text = "Current Score: 0";
            currentScoreLabel.Font = new Font("Verdana", 8);

            TableLayoutPanel guessLabelAndBoxPanel = new TableLayoutPanel();
            guessLabelAndBoxPanel.ColumnCount = 3;
            guessLabelAndBoxPanel.RowCount = 2;
            guessLabelAndBoxPanel.Margin = new Padding(40, 0, 0, 0);

            guessLabel = new Label();
            guessLabel.Text = "Guess word:";
            guessLabel.Width = 80;
            guessLabel.Font = new Font("Verdana", 8);
            guessLabel.Anchor = AnchorStyles.Bottom;
            guessLabelAndBoxPanel.Controls.Add(guessLabel, 0, 0);

            guessBox = new TextBox();
            guessBox.CharacterCasing = CharacterCasing.Upper;
            guessBox.Width = 62;
            guessBox.Anchor = AnchorStyles.Top;
            guessBox.Enabled = false;
            guessLabelAndBoxPanel.Controls.Add(guessBox, 1, 0);

            guessButton = new Button();
            guessButton.Text = "Go";
            guessButton.Width = 30;
            this.AcceptButton = guessButton;
            guessButton.Enabled = false;
            guessBox.KeyDown += new KeyEventHandler(GuessEnterHandler);
            guessButton.Click += new EventHandler(GuessHandler);
            guessLabelAndBoxPanel.Controls.Add(guessButton, 2, 0);

            // set up settings panel at bottom of play panel
            Label settingsLabel = new Label();
            settingsLabel.Width = 100;
            settingsLabel.AutoSize = false;
            settingsLabel.TextAlign = ContentAlignment.MiddleCenter;
            settingsLabel.Margin = new Padding(0, 0, 0, 10);
            settingsLabel.Text = "Settings";
            settingsLabel.Font = new Font("Verdana", 11, FontStyle.Bold);

            TableLayoutPanel settingsPanel = new TableLayoutPanel();
            settingsPanel.ColumnCount = 2;
            settingsPanel.RowCount = 3;
            
            Label setTimeLabel = new Label();
            setTimeLabel.Text = "Set time (30-999s):";
            setTimeLabel.Width = 120;
            setTimeLabel.Font = new Font("Verdana", 7);
            setTimeLabel.Anchor = AnchorStyles.Bottom;
            settingsPanel.Controls.Add(setTimeLabel, 0, 0);
            setTimeUpDown = new NumericUpDown();
            setTimeUpDown.Maximum = 999;
            setTimeUpDown.Minimum = 30;
            setTimeUpDown.Width = 40;
            setTimeUpDown.Value = time;
            settingsPanel.Controls.Add(setTimeUpDown, 1, 0);

            Label setGridSizeLabel = new Label();
            setGridSizeLabel.Text = "Set grid size (3-20):";
            setGridSizeLabel.Width = 120;
            setGridSizeLabel.Font = new Font("Verdana", 7);
            setGridSizeLabel.Anchor = AnchorStyles.Bottom;
            settingsPanel.Controls.Add(setGridSizeLabel, 0, 1);
            setGridUpDown = new NumericUpDown();
            setGridUpDown.Maximum = 20;
            setGridUpDown.Minimum = 3;
            setGridUpDown.Width = 40;
            setGridUpDown.Value = currentDimension;
            settingsPanel.Controls.Add(setGridUpDown, 1, 1);

            setSettingsButton = new Button();
            setSettingsButton.Text = "Set";
            setSettingsButton.Width = 40;
            setSettingsButton.Anchor = AnchorStyles.Top;
            settingsPanel.Controls.Add(setSettingsButton, 2, 3);
            setSettingsButton.Click += new EventHandler(SetSettingsHandler);

            guessLabelAndBoxPanel.Margin = new Padding(12, 15, 0, 0);
            settingsPanel.Margin = new Padding(12, 0, 0, 0);

            playPanel.Controls.Add(highScoreLabel);
            playPanel.Controls.Add(currentScoreLabel);
            playPanel.Controls.Add(guessesDisplayArea);
            playPanel.Controls.Add(guessLabelAndBoxPanel);

            playPanel.Controls.Add(settingsLabel);
            playPanel.Controls.Add(settingsPanel);

            this.Width = lettersPanel.Width + 450;
            this.Height = this.Width - 285;
        }

        /**
        * Starts playing the game, so players can guess words
        * and score points
        * 
        * @param sender: the play button
        * @param e: unused event arguments
        */
        private void PlayHandler(Object sender, EventArgs e)
        {
            solveButton.Enabled = false;
            playButton.Enabled = false;
            setSettingsButton.Enabled = false;
            guessesDisplayArea.BackColor = Color.White;
            guessBox.Enabled = true;
            guessButton.Enabled = true;
            guessBox.Focus();
            timer.Start();
        }

        /**
        * Starts playing the game, so players can guess words
        * and score points
        * 
        * @param sender: the play button
        * @param e: unused event arguments
        */
        private void GuessHandler(object sender, EventArgs e)
        {
            String guess = guessBox.Text.ToUpper();
            guessBox.Clear();
            guessBox.Focus();
            if (solutionsAndPaths.ContainsKey(guess) && !guessed.Contains(guess))
            {
                guessLabel.Text = "Guess word:";
                guessed.Add(guess);
                int wordScore = guess.Length - 2;
                currentScore += wordScore;
                currentScoreLabel.Text = "Current Score: " + currentScore;
                guess = guess + ": ";
                guess = guess.PadRight(20);
                guess += wordScore + " pts" + "\r\n";
                guessesDisplayArea.AppendText(guess.PadRight(12));
            }
            else
            {
                guessLabel.Text = "Try again!";
            }
        }

        /**
        * Guess handler when user presses enter rather than "Go" button
        * 
        * @param sender: the guessBox
        * @param e: unused event arguments
        */
        private void GuessEnterHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GuessHandler(sender, e);
            }
        }

        /**
        * Handles the ticking on the timer, which determines how long is left
        * and when the game ends
        * 
        * @param sender: the timer
        * @param e: unused event arguments
        */
        private void TimerTicker(Object sender, EventArgs e)
        {
            currentTime -= 1;
            timeLabel.Text = "Time: " + currentTime;
            if (currentTime == 0)
            {
                timer.Stop();
                String highScoreMessage = "";
                guessBox.Enabled = false;
                guessButton.Enabled = false;
                solveButton.Enabled = true;
                setSettingsButton.Enabled = true;

                if (currentScore > highScore)
                {
                    highScoreMessage = ". New high score!";
                    highScore = currentScore;
                    highScoreLabel.Text = "High Score: " + highScore;
                }
                MessageBox.Show("Your score was: " + currentScore + highScoreMessage);
                currentTime = time;
            }
        }

        /**
        * Solves the puzzle and adds radiobuttons to the solutionspanel that highlight
        * each solution on the letters grid
        * 
        * @param sender: the solveButton
        * @param e: unused event arguments
        */
        private void SolveButtonHandler(Object sender, EventArgs e)
        {
            solveButton.Enabled = false;
            playButton.Enabled = false;
            foreach (KeyValuePair<String, Boolean[,]> solution in solutionsAndPaths)
            {
                RadioButton radioButton = new RadioButton();
                radioButton.Text = solution.Key;
                radioButton.CheckedChanged += new EventHandler(RadioButtonHandler);
                solutionsPanel.Controls.Add(radioButton);
            }
        }

        /**
        * Creates a new board (possibly with new dimensions based on what is
        * set in currentDimension)
        * 
        * @param sender: the shakeButton
        * @param e: unused event arguments
        */
        public void Shake(Object sender, EventArgs e)
        {
            this.Controls.Clear();
            CreateNewBoggleBoard(currentDimension);
        }

        /**
        * Can adjust the time or grid size based on settings panel
        * 
        * @param sender: the setButton
        * @param e: unused event arguments
        */
        public void SetSettingsHandler(Object sender, EventArgs e)
        {
            int newTime = (int)setTimeUpDown.Value;
            if (newTime != time)
            {
                time = newTime;
                timeLabel.Text = "Time: " + time;
            }

            int newDimension = (int)setGridUpDown.Value;
            if (newDimension != currentDimension)
            {
                currentDimension = newDimension;
                Shake(sender, e);
            }
        }

        /**
        * Highlights appropriate solutions on the grid as each radiobutton
        * is clicked
        * 
        * @param sender: any of the radiobuttons, if checked the highlighting changes
        * @param e: unused event arguments
        */
        private void RadioButtonHandler(Object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                RadioButton rb = (RadioButton) sender;
                Boolean[,] path = solutionsAndPaths[rb.Text];

                for (int i = 0; i < path.GetLength(0); i++)
                {
                    for (int j = 0; j < path.GetLength(0); j++)
                    {
                        if (path[i, j])
                        {
                            letterLabels[i, j].BackColor = Color.Yellow;
                        }
                        else
                        {
                            letterLabels[i, j].BackColor = Color.White;
                        }
                    }
                }
            }
        }
    }

}
