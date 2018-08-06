using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Thermometer.Properties;
using FTD2XX;

namespace Thermometer
{
    public partial class MainForm : Form
    {
        // Public variables
        public static bool LinesChecked;
        public static bool ShutOffChecked;
        public static decimal widthGraph;
        public static decimal widthShutOff;
        public static Color BackgroundColor;
        public static Color LinesColor;
        public static Color GraphColor;
        public static Color ShutOffColor;

        // Constants
        const int SCREEN_WIDTH           = 700;   // px
        const int SCREEN_HEIGHT          = 500;   // px
        const int HORIZONTAL_SPAN        = 50;    // px
        const int VERTICAL_SPAN          = 25;    // px
        const int DASH_LENGTH            = 5;     // px
        const int LENGTH_TO_Y0           = 21;    // px
        const int LENGTH_TO_X0           = 65;    // px
        const int LABEL_POS_Y            = 5;     // px
        const int LABEL_POS_X            = 14;    // px
        const int NUM_VERTICAL_LINES     = (SCREEN_WIDTH / HORIZONTAL_SPAN) + 1; // px
        const int NUM_HORIZONTAL_LINES   = (SCREEN_HEIGHT / VERTICAL_SPAN) + 1;  // px
        const int TIME_INTERVAL          = 100;   // Interval of timer 100 mc
        const int TIME_INTERVAL_MAX      = 150;   // 150 mc
        const int TIME_INTERVAL_MIN      = 50;    // 50 mc
        const double TIME_COEFFICIENT    = 0.05;
        const int FAILURE_DEVICE         = 1;
        const int FAILURE_SENSOR         = 2;
        const int CYCLE_СOUNTER_MAX      = 60000; // 100 minutes
        const int MAX_TEMP               = 280;   // Maximum measurement temperature
        // Default settings
        const decimal DEFAULT_MAX_TEMP   = 40;
        const decimal DEFAULT_MIN_TEMP   = 20;
        const decimal DEFAULT_RATE       = (decimal)0.1;
        const bool DEFAULT_AUTO_SCALING  = true;

        // Global variables
        static byte AppStatus            = 0;
        static bool flag                 = true;
        static int cycleСounter          = 0;
        static double[] graphBuffer      = new double[CYCLE_СOUNTER_MAX];
        static string dataTable;
        DateTime nextTick;
        TimeSpan interval                = new TimeSpan(0, 0, 0, 0, TIME_INTERVAL);
        Label[] labelY                   = new Label[NUM_HORIZONTAL_LINES];
        Label[] labelX                   = new Label[NUM_VERTICAL_LINES];
        MLX90614 sensor                  = new MLX90614();
        FTDI.FT_STATUS ftStatus          = FTDI.FT_STATUS.FT_OTHER_ERROR;



        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Init
            CreateDynLabels(); // Create labels for X and Y axes
            UpdateScaleY();
            UpdateScaleX();

            buttonStart.Enabled = true;
            buttonStart.BackColor = Color.CornflowerBlue;
            buttonStart.ForeColor = Color.White;

            labelKelvin.Text = "             K";
            labelCelsius.Text = "           °C";
            labelProcess.Text = "Stopped";

            timerSensorReading.Interval = TIME_INTERVAL;

            // Read settings
            LinesChecked = Settings.Default.LinesChecked;
            ShutOffChecked = Settings.Default.ShutOffChecked;
            checkBoxAutoScaling.Checked = Settings.Default.AutoScaling;
            numericUpDownMax.Value = Settings.Default.MaxTemp;
            numericUpDownMin.Value = Settings.Default.MinTemp;
            numericUpDownShutOff.Value = Settings.Default.ShutOff;
            numericUpDownRate.Value = Settings.Default.Rate;
            widthGraph = Settings.Default.WidthGraph;
            widthShutOff = Settings.Default.WidthShutOff;
            BackgroundColor = Settings.Default.BackgroundColor;
            LinesColor = Settings.Default.LinesColor;
            GraphColor = Settings.Default.GraphColor;
            ShutOffColor = Settings.Default.ShutOffColor;

            pictureBoxScreen.Invalidate();
        }

        // Menu -------------------------------------------------------------------------------------
        private void saveAsImageToolStripMenuItem_Click(object sender, EventArgs e)  // Save as Image
        {
            DateTime localDate = DateTime.Now;

            var saveFileDialog = new SaveFileDialog
            {
                //InitialDirectory = "c:\\",
                Title = "Save as Image",
                FileName = "maghyp-" + localDate.ToString("yyyy-MM-dd") + ".png",
                Filter = "Image files (*.png)|*.png|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)  // Write to file
            {
                Bitmap bitmap = new Bitmap(groupBoxScreen.Width, groupBoxScreen.Height);
                groupBoxScreen.DrawToBitmap(bitmap, groupBoxScreen.ClientRectangle);
                bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.Dispose();
            }
        }

        private void saveAsTextToolStripMenuItem_Click(object sender, EventArgs e)  // Save as Text
        {
            DateTime localDate = DateTime.Now;

            var saveFileDialog = new SaveFileDialog
            {
                //InitialDirectory = "c:\\",
                Title = "Save as Table",
                FileName = "maghyp-" + localDate.ToString("yyyy-MM-dd") + ".txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)  // Write to file
            {
                File.WriteAllText(saveFileDialog.FileName, dataTable);
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)     // Color
        {
            ColorForm colorDialog = new ColorForm();

            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                pictureBoxScreen.Invalidate();
            }

            colorDialog.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)         // Exit
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)       // About
        {
            AboutForm aboutDialog = new AboutForm();

            if (aboutDialog.ShowDialog(this) == DialogResult.OK)
            {
            }

            aboutDialog.Dispose();
        }

        // Screen -----------------------------------------------------------------------------------
        private void pictureBoxScreen_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            pictureBoxScreen.BackColor = BackgroundColor;

            int X1;
            int Y1;
            int X2;
            int Y2;

            // Draw horizontal and vertical scale lines
            if (LinesChecked)
            {
                Pen penLines = new Pen(LinesColor, 1);
                //penLines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Draw horizontal lines
                X1 = 0;
                Y2 = Y1 = SCREEN_HEIGHT;
                X2 = SCREEN_WIDTH;

                for (int i = 0; i < NUM_HORIZONTAL_LINES; i++)
                {
                    e.Graphics.DrawLine(penLines, X1, Y1, X2, Y2);
                    Y2 = Y1 -= VERTICAL_SPAN;
                }

                // Draw vertical lines
                X2 = X1 = SCREEN_WIDTH;
                Y1 = 0;
                Y2 = SCREEN_HEIGHT;

                for (int i = 0; i < NUM_VERTICAL_LINES; i++)
                {
                    e.Graphics.DrawLine(penLines, X1, Y1, X2, Y2);
                    X2 = X1 -= HORIZONTAL_SPAN;
                }

                penLines.Dispose();
            }

            // Draw horizontal line for shut-off signal
            if (ShutOffChecked)
            {
                Pen penShutOffLine = new Pen(ShutOffColor, (float)widthShutOff);

                X1 = 0;
                X2 = SCREEN_WIDTH;
                Y2 = Y1 = (int)((numericUpDownMax.Value - numericUpDownShutOff.Value) *
                    (SCREEN_HEIGHT / (numericUpDownMax.Value - numericUpDownMin.Value)));

                if ((Y1 > 0) && (Y1 < SCREEN_HEIGHT)) e.Graphics.DrawLine(penShutOffLine, X1, Y1, X2, Y2);

                penShutOffLine.Dispose();
            }

            // Draw graph
            int rate = (int)(numericUpDownRate.Value * 10);

            if (cycleСounter > rate)
            {
                Pen penGraph = new Pen(GraphColor, (float)widthGraph)
                {
                    LineJoin = System.Drawing.Drawing2D.LineJoin.Round
                };

                int index = ((cycleСounter - 1) / rate) + 1;
                Point[] curvePoints = new Point[index];

                X1 = 0;
                Y1 = 0;

                for (int i = 0; i < index; i++)
                {
                    Y1 = (int)((numericUpDownMax.Value - (decimal)graphBuffer[i * rate]) *
                        (SCREEN_HEIGHT / (numericUpDownMax.Value - numericUpDownMin.Value)));
                    X1 = i;

                    curvePoints[i] = new Point(X1, Y1);
                }

                e.Graphics.DrawLines(penGraph, curvePoints);

                if ((X1 > SCREEN_WIDTH - 10) && (checkBoxAutoScaling.Checked == true) && (numericUpDownRate.Value < 10))
                {
                    numericUpDownRate.Value += (decimal)0.1;
                }

                if ((Y1 < 10) && (checkBoxAutoScaling.Checked == true) && (numericUpDownMax.Value < MAX_TEMP))
                {
                    numericUpDownMax.Value += 2;
                }

                penGraph.Dispose();
            }

            // Draw border of screen
            Pen penRectangle = new Pen(Color.Black, 1);
            e.Graphics.DrawRectangle(penRectangle, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

            penRectangle.Dispose();
        }

        private void groupBoxScreen_Paint(object sender, PaintEventArgs e)
        {
            int X1;
            int Y1;
            int X2;
            int Y2;

            Pen penDash = new Pen(Color.Black, 1);

            // Draw horizontal dash
            X1 = LENGTH_TO_X0;
            Y2 = Y1 = SCREEN_HEIGHT + LENGTH_TO_Y0;
            X2 = LENGTH_TO_X0 - DASH_LENGTH;

            for (int i = 0; i < NUM_HORIZONTAL_LINES; i++)
            {
                e.Graphics.DrawLine(penDash, X1, Y1, X2, Y2);
                Y2 = Y1 -= VERTICAL_SPAN;
            }

            // Draw vertical dash
            X2 = X1 = SCREEN_WIDTH + LENGTH_TO_X0;
            Y1 = LENGTH_TO_Y0 + SCREEN_HEIGHT + DASH_LENGTH;
            Y2 = SCREEN_HEIGHT + LENGTH_TO_Y0;

            for (int i = 0; i < NUM_VERTICAL_LINES; i++)
            {
                e.Graphics.DrawLine(penDash, X1, Y1, X2, Y2);
                X2 = X1 -= HORIZONTAL_SPAN;
            }

            penDash.Dispose();
        }

        private void labelTemperature_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, 100);
            e.Graphics.RotateTransform(-90);
            e.Graphics.DrawString("Temperature (°C)", new Font("Tahoma", 9), Brushes.Black, 0, 0);
        }

        // Save settings ----------------------------------------------------------------------------
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.LinesChecked = LinesChecked;
            Settings.Default.ShutOffChecked = ShutOffChecked;
            Settings.Default.AutoScaling = checkBoxAutoScaling.Checked;
            Settings.Default.MaxTemp = numericUpDownMax.Value;
            Settings.Default.MinTemp = numericUpDownMin.Value;
            Settings.Default.ShutOff = numericUpDownShutOff.Value;
            Settings.Default.Rate = numericUpDownRate.Value;
            Settings.Default.WidthGraph = widthGraph;
            Settings.Default.WidthShutOff = widthShutOff;
            Settings.Default.BackgroundColor = BackgroundColor;
            Settings.Default.LinesColor = LinesColor;
            Settings.Default.GraphColor = GraphColor;
            Settings.Default.ShutOffColor = ShutOffColor;
            Settings.Default.Save();

            timerSensorReading.Stop();

            try
            {
                sensor.Close();
            }
            catch { }
        }

        // NumericUpDown handlers -------------------------------------------------------------------
        private void numericUpDownMax_ValueChanged(object sender, EventArgs e)
        {
            UpdateScaleY();
            pictureBoxScreen.Invalidate();
        }

        private void numericUpDownMin_ValueChanged(object sender, EventArgs e)
        {
            UpdateScaleY();
            pictureBoxScreen.Invalidate();
        }

        private void numericUpDownShutOff_ValueChanged(object sender, EventArgs e)
        {
            pictureBoxScreen.Invalidate();
        }
        private void numericUpDownRate_ValueChanged(object sender, EventArgs e)
        {
            UpdateScaleX();
            pictureBoxScreen.Invalidate();
        }

        // Button handlers --------------------------------------------------------------------------
        private void buttonStart_Click(object sender, EventArgs e)
        {
            uint devCount = 0;
            bool driverInstalled = true;

            buttonStart.BackColor = SystemColors.Control;
            buttonStart.ForeColor = SystemColors.ControlDark;
            buttonStart.Enabled = false;

            labelDevice.Text = "";
            labelSensor.Text = "";

            dataTable = "Tem/°C  Time/s\r\n";
            cycleСounter = 0;

            Application.DoEvents();

            // Check if the driver is installed by accessing a function in a try-catch
            try
            {
                ftStatus = sensor.GetNumberOfDevices(ref devCount);
            }
            catch
            {
                driverInstalled = false;
            }

            if (driverInstalled == false)
            {
                Error("FTDI driver is not installed", MessageBoxIcon.Warning, 0);
            }
            else
            {
                // Check how many FTDI devices are connected, if no then exit
                if ((devCount == 0) || (ftStatus != FTDI.FT_STATUS.FT_OK))
                {
                    Error("FTDI device not found", MessageBoxIcon.Warning, FAILURE_DEVICE);
                }
                else
                {
                    // Open the device
                    ftStatus = sensor.OpenByIndex(0);

                    if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    {
                        Error("FTDI device is not responding", MessageBoxIcon.Error, FAILURE_DEVICE);
                    }
                    else
                    {
                        // Set up the MPSSE
                        AppStatus = sensor.I2C_ConfigureMpsse();

                        if (AppStatus != 0)
                        {
                            sensor.Close();

                            Error("FTDI device configuration error", MessageBoxIcon.Error, FAILURE_DEVICE);
                        }
                        else
                        {
                            labelDevice.Text = "OK";
                            labelDevice.ForeColor = Color.LightSeaGreen;

                            labelSensor.Text = "OK";
                            labelSensor.ForeColor = Color.LightSeaGreen;

                            buttonStop.BackColor = Color.HotPink;
                            buttonStop.ForeColor = Color.White;
                            buttonStop.Enabled = true;

                            labelProcess.Text = "Running";
                            nextTick = DateTime.Now;
                            timerSensorReading.Start();
                        }
                    }
                }
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            timerSensorReading.Stop();

            buttonStop.BackColor = SystemColors.Control;
            buttonStop.ForeColor = SystemColors.ControlDark;
            buttonStop.Enabled = false;

            //I2C_SetGPIOValuesHigh(1, 0); // Set 0 on AC0 pin
            //flag = true;

            sensor.Close();

            labelProcess.Text = "Stopped";
            buttonStart.BackColor = Color.CornflowerBlue;
            buttonStart.ForeColor = Color.White;
            buttonStart.Enabled = true;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            cycleСounter = 0;
            dataTable = "Tem/°C  Time/s\r\n";
            labelKelvin.Text = "             K";
            labelCelsius.Text = "           °C";
            pictureBoxScreen.Invalidate();
        }

        private void buttonSetDefault_Click(object sender, EventArgs e)
        {
            numericUpDownMax.Value = DEFAULT_MAX_TEMP;
            numericUpDownMin.Value = DEFAULT_MIN_TEMP;
            numericUpDownRate.Value = DEFAULT_RATE;
            checkBoxAutoScaling.Checked = DEFAULT_AUTO_SCALING;
        }

        // Tick of timer ----------------------------------------------------------------------------
        private void timerSensorReading_Tick(object sender, EventArgs e)
        {
            // Synchronization of timer
            TimeSpan timeSpanError = interval - (DateTime.Now - nextTick);
            int timeInterval = TIME_INTERVAL + (int)(timeSpanError.Milliseconds * TIME_COEFFICIENT);
            if (timeInterval > TIME_INTERVAL_MAX) timeInterval = TIME_INTERVAL_MAX;
            if (timeInterval < TIME_INTERVAL_MIN) timeInterval = TIME_INTERVAL_MIN;
            timerSensorReading.Interval = timeInterval;
            nextTick += interval;

            // Sensor reading
            AppStatus = sensor.Reading();

            for (int i = 0; (i < 2) && (AppStatus != 0); i++) // If there is an error, then repeat reading
            {
                Thread.Sleep(10);
                AppStatus = sensor.Reading();
            }

            if (AppStatus != 0)
            {
                timerSensorReading.Stop();
                sensor.Close();

                labelProcess.Text = "Stopped";
                buttonStop.BackColor = SystemColors.Control;
                buttonStop.ForeColor = SystemColors.ControlDark;
                buttonStop.Enabled = false;

                Error("MLX90614 sensor is not responding", MessageBoxIcon.Error, FAILURE_SENSOR);
            }
            else
            {
                double KelvinValue = sensor.getValue() * 0.02;
                double CelsiusValue = KelvinValue - 273.15;

                labelKelvin.Text = KelvinValue.ToString("0.00") + " K";

                labelCelsius.Text = CelsiusValue.ToString("0.00") + " °C";


                dataTable += (CelsiusValue.ToString("000.00") + "  " +
                    (cycleСounter * 0.1).ToString("0000.0") + "\r\n");

                if ((decimal)CelsiusValue > numericUpDownShutOff.Value)
                {
                    if (flag == false)
                    {
                        sensor.I2C_SetGPIOValuesHigh(1, 0); // Set 0 on AC0 pin
                        flag = true;
                    }
                }
                else
                {
                    if (flag == true)
                    {
                        sensor.I2C_SetGPIOValuesHigh(1, 1); // Set 1 on AC0 pin
                        flag = false;
                    }
                }

                graphBuffer[cycleСounter] = CelsiusValue;
                cycleСounter++;
                pictureBoxScreen.Invalidate();

                if (cycleСounter >= CYCLE_СOUNTER_MAX)
                {
                    buttonStop.PerformClick();

                    MessageBox.Show("Time limit is exceeded",
                    "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Functions --------------------------------------------------------------------------------
        private void CreateDynLabels()
        {
            int X;
            int Y;

            // Creat dynamic labels Y
            X = LENGTH_TO_X0 - 39;
            Y = LENGTH_TO_Y0 + SCREEN_HEIGHT - LABEL_POS_Y;

            for (int i = 0; i < NUM_HORIZONTAL_LINES; i++)
            {
                labelY[i] = new Label
                {
                    Left = X,
                    Top = Y,
                    Size = new Size(34, 13),
                    TextAlign = ContentAlignment.MiddleRight
                };

                groupBoxScreen.Controls.Add(labelY[i]);
                Y -= VERTICAL_SPAN;
            }

            // Creat dynamic labels X
            X = LENGTH_TO_X0 - LABEL_POS_X;
            Y = SCREEN_HEIGHT + LENGTH_TO_Y0 + 10;

            for (int i = 0; i < NUM_VERTICAL_LINES; i++)
            {
                labelX[i] = new Label
                {
                    Left = X,
                    Top = Y,
                    Size = new Size(32, 13),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                groupBoxScreen.Controls.Add(labelX[i]);
                X += HORIZONTAL_SPAN;
            }
        }

        private void UpdateScaleY()
        {
            decimal valueYSpan = (numericUpDownMax.Value - numericUpDownMin.Value) / (NUM_HORIZONTAL_LINES - 1);
            decimal valueY = numericUpDownMin.Value;

            // Set text to labels Y
            for (int i = 0; i < NUM_HORIZONTAL_LINES; i++)
            {
                labelY[i].Text = Convert.ToString(valueY);
                valueY += valueYSpan;
            }
        }

        private void UpdateScaleX()
        {
            int valueXSpan = (int)(numericUpDownRate.Value * HORIZONTAL_SPAN);
            int valueX = 0;

            // Set text to labels X
            for (int i = 0; i < NUM_VERTICAL_LINES; i++)
            {
                labelX[i].Text = Convert.ToString(valueX);
                valueX += valueXSpan;
            }
        }

        private void Error(String message, MessageBoxIcon messageBoxIcon, int failure)
        {
            if (failure == FAILURE_DEVICE) {
                labelDevice.Text = "Failure";
                labelDevice.ForeColor = Color.DeepPink;
            } else if (failure == FAILURE_SENSOR) {
                labelSensor.Text = "Failure";
                labelSensor.ForeColor = Color.DeepPink;
            }

            buttonStart.BackColor = Color.CornflowerBlue;
            buttonStart.ForeColor = Color.White;
            buttonStart.Enabled = true;

            MessageBox.Show(message, "", MessageBoxButtons.OK, messageBoxIcon);
        }
    }
}
