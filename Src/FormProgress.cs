using System;
using System.Windows.Forms;

namespace CPM
{
    public partial class FormProgress : Form
    {
        #region Members

        // Start and total value
        int start, total;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start"></param>
        /// <param name="total"></param>
        /// <param name="text"></param>
        public FormProgress(int start, int total, string text = "Progress")
        {
            InitializeComponent();

            this.start = start;
            this.total = total;
            this.Text = text;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProgress_Load(object sender, EventArgs e)
        {
            SetValue(start, total);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set value for progressbar
        /// </summary>
        /// <param name="value"></param>
        /// <param name="total"></param>
        public void SetValue(int value, int total)
        {
            lblFrom.Text = value.ToString() + " From " + total.ToString();
            if (total > 0)
            {
                progressBar.Value = 100 * value / total;
            }   else
            {
                progressBar.Value = 0;
            }

            Application.DoEvents();
        }

        #endregion
    }
}
