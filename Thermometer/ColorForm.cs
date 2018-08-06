using System;
using System.Drawing;
using System.Windows.Forms;

namespace Thermometer
{
    public partial class ColorForm : Form
    {
        // Default settings
        const bool DEFAULT_LINES_CHECKED      = true;
        const bool DEFAULT_SHUT_OFF_CHECKED   = false;
        const decimal DEFAULT_WIDTH_GRAPH     = 3;
        const decimal DEFAULT_WIDTH_SHUT_OFF  = 1;
        Color defaultColorBackground          = Color.WhiteSmoke;
        Color defaultColorLines               = Color.LightGray;
        Color defaultColorGraph               = Color.CornflowerBlue;
        Color defaultColorShutOff             = Color.HotPink;


        public ColorForm()
        {
            InitializeComponent();
        }

        private void ColorForm_Load(object sender, EventArgs e)
        {
            // Init
            checkBoxLines.Checked = MainForm.LinesChecked;
            checkBoxShutOff.Checked = MainForm.ShutOffChecked;
            numericUpDownWidthGraph.Value = MainForm.widthGraph;
            numericUpDownWidthShutOff.Value = MainForm.widthShutOff;
            pictureBoxBack.BackColor = MainForm.BackgroundColor;
            pictureBoxLines.BackColor = MainForm.LinesColor;
            pictureBoxGraph.BackColor = MainForm.GraphColor;
            pictureBoxShutOff.BackColor = MainForm.ShutOffColor;
        }

        // Button handlers --------------------------------------------------------------------------
        private void buttonOK_Click(object sender, EventArgs e)
        {
            MainForm.LinesChecked = checkBoxLines.Checked;
            MainForm.ShutOffChecked = checkBoxShutOff.Checked;
            MainForm.widthGraph = numericUpDownWidthGraph.Value;
            MainForm.widthShutOff = numericUpDownWidthShutOff.Value;
            MainForm.BackgroundColor = pictureBoxBack.BackColor;
            MainForm.LinesColor = pictureBoxLines.BackColor;
            MainForm.GraphColor = pictureBoxGraph.BackColor;
            MainForm.ShutOffColor = pictureBoxShutOff.BackColor;

            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            checkBoxLines.Checked = DEFAULT_LINES_CHECKED;
            checkBoxShutOff.Checked = DEFAULT_SHUT_OFF_CHECKED;
            numericUpDownWidthGraph.Value = DEFAULT_WIDTH_GRAPH;
            numericUpDownWidthShutOff.Value = DEFAULT_WIDTH_SHUT_OFF;
            pictureBoxBack.BackColor = defaultColorBackground;
            pictureBoxLines.BackColor = defaultColorLines;
            pictureBoxGraph.BackColor = defaultColorGraph;
            pictureBoxShutOff.BackColor = defaultColorShutOff;
        }

        // Set background color ---------------------------------------------------------------------
        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            ColorDialog ColorDialog = new ColorDialog
            {
                AllowFullOpen = true,
                //ShowHelp = true;
                Color = pictureBoxBack.BackColor
            };

            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxBack.BackColor = ColorDialog.Color;
            }

            ColorDialog.Dispose();
        }

        // Set line color ---------------------------------------------------------------------------
        private void pictureBoxLines_Click(object sender, EventArgs e)
        {
            ColorDialog ColorDialog = new ColorDialog
            {
                AllowFullOpen = true,
                Color = pictureBoxLines.BackColor
            };

            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxLines.BackColor = ColorDialog.Color;
            }

            ColorDialog.Dispose();
        }

        // Set graph color --------------------------------------------------------------------------
        private void pictureBoxGraph_Click(object sender, EventArgs e)
        {
            ColorDialog ColorDialog = new ColorDialog
            {
                AllowFullOpen = true,
                Color = pictureBoxGraph.BackColor
            };

            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxGraph.BackColor = ColorDialog.Color;
            }

            ColorDialog.Dispose();
        }

        // Set shut-off color -----------------------------------------------------------------------
        private void pictureBoxShutOff_Click(object sender, EventArgs e)
        {
            ColorDialog ColorDialog = new ColorDialog
            {
                AllowFullOpen = true,
                Color = pictureBoxShutOff.BackColor
            };

            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxShutOff.BackColor = ColorDialog.Color;
            }

            ColorDialog.Dispose();
        }
    }
}
