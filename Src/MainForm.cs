using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Z80_RC2014.Properties;

namespace Z80_RC2014
{
    public partial class MainForm : Form
    {
        #region Members

        // Assembler object (main program and compact flash code)
        private AssemblerZ80 assemblerZ80;
        private AssemblerZ80CompactFlash assemblerZ80CompactFlash;

        // Rows of memory panel   
        private Label[] memoryAddressLabels = new Label[0x10];

        // Columns of memory panel
        private Label[] memoryAddressIndexLabels = new Label[0x10];

        // Contents of memory panel table
        private Label[,] memoryTableLabels = new Label[0x10, 0x10];

        // Rows of ports panel   
        private Label[] portAddressLabels = new Label[0x10];

        // Columns of ports panel
        private Label[] portAddressIndexLabels = new Label[0x10];

        // Contents of ports panel table
        private Label[,] portTableLabels = new Label[0x10, 0x10];

        // File selected for loading/saving 
        private string sourceFileProgram = "";
        private string sourceFileCompactFlash = "";

        // Next instruction address
        private UInt16 nextInstrAddress = 0;

        // Line on which a breakpoint has been set
        private int lineBreakPoint = -1;

        // Tooltip for button/menu items
        private ToolTip toolTip;

        // Delay for running program
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        // Terminal interface
        private FormTerminal formTerminal = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            toolStripButtonRun.Enabled = false;
            toolStripButtonFast.Enabled = false;
            toolStripButtonStep.Enabled = false;
            toolStripButtonStop.Enabled = false;

            pbBreakPoint.Image = new Bitmap(pbBreakPoint.Height, pbBreakPoint.Width);
            Graphics g = pbBreakPoint.CreateGraphics();
            g.Clear(Color.LightGray);

            // Scroll memory panel with mousewheel
            this.panelMemory.MouseWheel += PanelMemory_MouseWheel;
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// MainForm loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set location of mainform
            this.Location = new Point(10, 10);

            // Tooltip with line (address) info
            toolTip = new ToolTip();
            toolTip.OwnerDraw = true;
            toolTip.IsBalloon = false;
            toolTip.BackColor = Color.Azure;
            toolTip.Draw += ToolTip_Draw;
            toolTip.Popup += ToolTip_Popup;

            // Create font for header text
            Font font = new Font("Tahoma", 9.75F, FontStyle.Bold);

            // We can view 256 bytes of memory at a time, it will be in form of 16 X 16
            for (int i = 0; i < 0x10; i++)
            {
                Label label = new Label();
                label.Name = "memoryAddressLabel" + i.ToString("X");
                label.Font = font;
                label.Text = (i * 16).ToString("X").PadLeft(4, '0');
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Visible = true;
                label.Size = new System.Drawing.Size(44, 15);
                label.Location = new Point(10, 20 + 20 * i);
                label.BackColor = SystemColors.GradientInactiveCaption;
                panelMemoryInfo.Controls.Add(label);

                memoryAddressLabels[i] = label;
            }

            // MemoryAddressIndexLabels, display the top row required for the memory table
            for (int i = 0; i < 0x10; i++)
            {
                Label label = new Label();
                label.Name = "memoryAddressIndexLabel" + i.ToString("X");
                label.Font = font;
                label.Text = i.ToString("X");
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Visible = true;
                label.Size = new System.Drawing.Size(20, 15);
                label.Location = new Point(60 + 30 * i, 0);
                label.BackColor = SystemColors.GradientInactiveCaption;
                panelMemoryInfo.Controls.Add(label);

                memoryAddressIndexLabels[i] = label;
            }

            // MemoryTableLabels, display the memory contents
            for (int i = 0; i < 0x10; i++)
            {
                for (int j = 0; j < 0x10; j++)
                {
                    Label label = new Label();
                    int address = 16 * i + j;
                    label.Name = "memoryTableLabel" + address.ToString("X").PadLeft(2, '0');
                    label.Text = null;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.Visible = true;
                    label.Size = new System.Drawing.Size(24, 15);
                    label.Location = new Point(60 + 30 * j, 20 + 20 * i);
                    panelMemoryInfo.Controls.Add(label);

                    memoryTableLabels[i, j] = label;
                }
            }

            // PortAddressLabels, display initial labels from 0x00 to 0x10 
            for (int i = 0; i < 0x10; i++)
            {
                Label label = new Label();
                label.Name = "portAddressLabel" + i.ToString("X");
                label.Font = font;
                label.Text = (i * 16).ToString("X").PadLeft(2, '0');
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Visible = true;
                label.Size = new System.Drawing.Size(40, 15);
                label.Location = new Point(10, 20 + 20 * i);
                label.BackColor = SystemColors.GradientInactiveCaption;
                panelPortInfo.Controls.Add(label);

                portAddressLabels[i] = label;
            }

            // portAddressIndexLabels, display the top row required for the port table
            for (int i = 0; i < 0x10; i++)
            {
                Label label = new Label();
                label.Name = "portAddressIndexLabel" + i.ToString("X");
                label.Font = font;
                label.Text = (i * 16).ToString("X");
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Visible = true;
                label.Size = new System.Drawing.Size(20, 15);
                label.Location = new Point(60 + 30 * i, 0);
                label.BackColor = SystemColors.GradientInactiveCaption;
                panelPortInfo.Controls.Add(label);

                portAddressIndexLabels[i] = label;
            }

            // portTableLabels, display the port contents
            for (int i = 0; i < 0x10; i++)
            {
                for (int j = 0; j < 0x10; j++)
                {
                    Label label = new Label();
                    int address = 16 * i + j;
                    label.Name = "portTableLabel" + address.ToString("X");
                    label.Text = null;
                    label.Visible = true;
                    label.Size = new System.Drawing.Size(30, 15);
                    label.Location = new Point(60 + 30 * j, 20 + 20 * i);
                    panelPortInfo.Controls.Add(label);

                    portTableLabels[i, j] = label;
                }
            }

            timer.Interval = Convert.ToInt32(numericUpDownDelay.Value);
            timer.Tick += new EventHandler(TimerEventProcessor);

            // Initialize the buttons (add a tag with info)
            InitButtons();

            // Setup terminal screen 
            if (formTerminal == null)
            {
                int x = this.Location.X + this.Width + 2;
                int y = this.Location.Y;

                if (x > Screen.PrimaryScreen.WorkingArea.Width)
                {
                    x = this.Width / 2;
                    y = this.Height / 2;
                }

                formTerminal = new FormTerminal(x, y);
                formTerminal.Show();
            }
        }

        /// <summary>
        /// Timer event handler
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void TimerEventProcessor(Object obj, EventArgs myEventArgs)
        {
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)obj;

            UInt16 currentInstrAddress = nextInstrAddress;

            string error = assemblerZ80.RunInstruction(currentInstrAddress, ref nextInstrAddress);

            UpdateTerminal();

            UInt16 startViewAddress = Convert.ToUInt16(memoryAddressLabels[0].Text, 16);

            if (!chkLock.Checked)
            {
                if (nextInstrAddress > startViewAddress + 0x100) startViewAddress = (UInt16)(nextInstrAddress & 0xFFF0);
                if (nextInstrAddress < startViewAddress) startViewAddress = (UInt16)(nextInstrAddress & 0xFFF0);
            }

            UpdateMemoryPanel(startViewAddress, nextInstrAddress);
            UpdatePortPanel();
            UpdateRegisters();
            UpdateFlags();
            UpdateInterrupts();

            if ((error == "") && (assemblerZ80.RAMprogramLine[currentInstrAddress] != -1))
            {
                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[currentInstrAddress], false);

                if ((lineBreakPoint >= 0) && (assemblerZ80.RAMprogramLine[nextInstrAddress] == lineBreakPoint))
                {
                    timer.Enabled = false;

                    // Enable event handler for updating row/column 
                    richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);

                    ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);
                    if (chkLock.Checked)
                    {
                        UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
                    } else
                    {
                        UpdateMemoryPanel(currentInstrAddress, nextInstrAddress);
                    }

                    UpdatePortPanel();
                    UpdateRegisters();
                    UpdateFlags();
                    UpdateInterrupts();

                    toolStripButtonRun.Enabled = true;
                    toolStripButtonFast.Enabled = true;
                    toolStripButtonStep.Enabled = true;
                    toolStripButtonStop.Enabled = false;
                }
            } else if ((error == "") && chkBreakOnExternalCode.Checked && (assemblerZ80.RAMprogramLine[nextInstrAddress] == -1))
            {
                timer.Enabled = false;

                // Enable event handler for updating row/column 
                richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);

                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);
                if (chkLock.Checked)
                {
                    UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
                }
                else
                {
                    UpdateMemoryPanel(currentInstrAddress, nextInstrAddress);
                }

                UpdatePortPanel();
                UpdateRegisters();
                UpdateFlags();
                UpdateInterrupts();

                toolStripButtonRun.Enabled = true;
                toolStripButtonFast.Enabled = true;
                toolStripButtonStep.Enabled = true;
                toolStripButtonStop.Enabled = false;
            } else if (error == "System Halted")
            {
                toolStripButtonRun.Enabled = false;
                toolStripButtonFast.Enabled = false;
                toolStripButtonStep.Enabled = false;
                toolStripButtonStop.Enabled = false;

                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[currentInstrAddress], false);
                MessageBox.Show(error, "SYSTEM HALTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Show/Hide Terminal interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTerminal_CheckedChanged(object sender, EventArgs e)
        {
            if (formTerminal == null)
            {
                int x = this.Location.X + this.Width + 2;
                int y = this.Location.Y;

                if (x > Screen.PrimaryScreen.WorkingArea.Width)
                {
                    x = this.Width / 2;
                    y = this.Height / 2;
                }

                formTerminal = new FormTerminal(x, y);
                formTerminal.Show();
            } else
            {
                formTerminal.Close();
                formTerminal = null;
            }

            UpdatePortPanel();
        }

        /// <summary>
        /// Memory startaddress changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMemoryStartAddress_TextChanged(object sender, EventArgs e)
        {
            string hexdigits = "1234567890ABCDEFabcdef";
            bool noHex = false;
            foreach (char c in tbMemoryStartAddress.Text)
            {
                if (hexdigits.IndexOf(c) < 0)
                {
                    MessageBox.Show("Only hexadecimal values", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    noHex = true;
                }
            }

            if (noHex) tbMemoryStartAddress.Text = "0000";
        }

        /// <summary>
        /// View memory from this address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMemoryStartAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
            }
        }

        /// <summary>
        /// Startaddress changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSetProgramCounter_TextChanged(object sender, EventArgs e)
        {
            string hexdigits = "1234567890ABCDEFabcdef";
            bool noHex = false;
            foreach (char c in tbSetProgramCounter.Text)
            {
                if (hexdigits.IndexOf(c) < 0)
                {
                    MessageBox.Show("Only hexadecimal values", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    noHex = true;
                }
            }

            if (noHex) tbSetProgramCounter.Text = "0000";
        }

        /// <summary>
        /// Startaddress changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSetProgramCounter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter) && (assemblerZ80 != null))
            {
                nextInstrAddress = Convert.ToUInt16(tbSetProgramCounter.Text, 16);
                labelPCRegister.Text = tbSetProgramCounter.Text;
                
                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);

                if (!chkLock.Checked) UpdateMemoryPanel(nextInstrAddress, nextInstrAddress);
            }
        }

        /// <summary>
        /// Timer delay while running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownDelay_ValueChanged(object sender, EventArgs e)
        {
            timer.Interval = Convert.ToInt32(numericUpDownDelay.Value);
        }

        /// <summary>
        /// Add/Change breakpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void pbBreakPoint_MouseClick(object sender, MouseEventArgs e)
        {
            // Get character index of mouse Y position in current program
            int index = richTextBoxProgram.GetCharIndexFromPosition(new Point(0, e.Y));

            // Get line number
            lineBreakPoint = richTextBoxProgram.GetLineFromCharIndex(index);

            // Set (update) breakpoint on screen
            UpdateBreakPoint(lineBreakPoint);
        }

        /// <summary>
        /// Main form resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            // Set (update) breakpoint on screen
            UpdateBreakPoint(lineBreakPoint);
        }

        /// <summary>
        /// Draw tooltip with specific font
        /// </summary>
        private void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font font = new Font(FontFamily.GenericMonospace, 12.0f);
            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, font, Brushes.Black, new Point(2, 2));
        }

        /// <summary>
        /// Set font for the tooltip popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            Font font = new Font(FontFamily.GenericMonospace, 12.0f);
            Size size = TextRenderer.MeasureText(toolTip.GetToolTip(e.AssociatedControl), font);
            e.ToolTipSize = new Size(size.Width + 3, size.Height + 3);
        }

        /// <summary>
        /// Change memory view range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelMemory_MouseWheel(object sender, MouseEventArgs e)
        {
            if (assemblerZ80 != null)
            {
                if (e.Delta < 0)
                {
                    if (Convert.ToUInt16(memoryAddressLabels[0].Text, 16) < 0xFFF0)
                    {
                        UInt16 n = (UInt16)(Convert.ToUInt16(memoryAddressLabels[0].Text, 16) + 0x0010);

                        tbMemoryStartAddress.Text = n.ToString("X4");
                        UpdateMemoryPanel(n, nextInstrAddress);
                    }
                }

                if (e.Delta > 0)
                {
                    if (Convert.ToUInt16(memoryAddressLabels[0].Text, 16) >= 0x0010)
                    {
                        UInt16 n = (UInt16)(Convert.ToUInt16(memoryAddressLabels[0].Text, 16) - 0x0010);

                        tbMemoryStartAddress.Text = n.ToString("X4");
                        UpdateMemoryPanel(n, nextInstrAddress);
                    }
                }
            }
        }

        /// <summary>
        /// Change sign flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkFlagS_CheckedChanged(object sender, EventArgs e)
        {
            if (assemblerZ80 != null) assemblerZ80.flagS = chkFlagS.Checked;
        }

        /// <summary>
        /// Change zero flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkFlagZ_CheckedChanged(object sender, EventArgs e)
        {
            if (assemblerZ80 != null) assemblerZ80.flagZ = chkFlagZ.Checked;
        }

        /// <summary>
        /// Change auxiliary carry flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkFlagH_CheckedChanged(object sender, EventArgs e)
        {
            if (assemblerZ80 != null) assemblerZ80.flagH = chkFlagH.Checked;
        }

        /// <summary>
        /// Change parity/overflow flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkFlagPV_CheckedChanged(object sender, EventArgs e)
        {
            if (assemblerZ80 != null) assemblerZ80.flagPV = chkFlagPV.Checked;
        }

        /// <summary>
        /// Change carry flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkFlagC_CheckedChanged(object sender, EventArgs e)
        {
            if (assemblerZ80 != null) assemblerZ80.flagC = chkFlagC.Checked;
        }

        /// <summary>
        /// Show tooltiptext
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_MouseHover(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Instruction instruction = (Instruction)control.Tag;

            toolTip.SetToolTip(control, instruction.Description);
            toolTip.Active = true;
        }

        /// <summary>
        /// Handle keys when running to send to the terminal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxProgram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((assemblerZ80 != null) && (formTerminal != null))
            {
                if (toolStripButtonStop.Enabled)
                {
                    formTerminal.keyBuffer += e.KeyChar;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Ignore keys when running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxProgram_KeyDown(object sender, KeyEventArgs e)
        {
            if ((assemblerZ80 != null) && (formTerminal != null))
            {
                if (toolStripButtonStop.Enabled) e.Handled = true;
            }
        }

        /// <summary>
        /// Handle keys when running to send to the terminal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((assemblerZ80 != null) && (formTerminal != null))
            {
                if (toolStripButtonStop.Enabled)
                {
                    formTerminal.keyBuffer += e.KeyChar;
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region EventHandlers (Menu)

        private void open_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select Assembly File";
//            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            fileDialog.InitialDirectory = Environment.GetEnvironmentVariable("OneDriveConsumer");
            fileDialog.FileName = "";
            fileDialog.Filter = "Z80 assembly|*.asm;*.z80;*.Z80|All Files|*.*";

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                if (tcSources.SelectedTab.Name == "tpProgram")
                {
                    System.IO.StreamReader streamReader;
                    sourceFileProgram = fileDialog.FileName;
                    streamReader = new System.IO.StreamReader(sourceFileProgram);
                    richTextBoxProgram.Text = streamReader.ReadToEnd();
                    streamReader.Close();
                }

                if (tcSources.SelectedTab.Name == "tpCompactFlash")
                {
                    System.IO.StreamReader streamReader;
                    sourceFileCompactFlash = fileDialog.FileName;
                    streamReader = new System.IO.StreamReader(sourceFileCompactFlash);
                    richTextBoxCompactFlash.Text = streamReader.ReadToEnd();
                    streamReader.Close();
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (tcSources.SelectedTab.Name == "tpProgram")
            {
                if (sourceFileProgram == "")
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Title = "Save File As";
                    fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    fileDialog.FileName = "";
                    fileDialog.Filter = "Z80 assembly|*.asm;*.z80;*.Z80|All Files|*.*";

                    if (fileDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        System.IO.StreamWriter streamWriter;
                        sourceFileProgram = fileDialog.FileName;
                        streamWriter = new System.IO.StreamWriter(sourceFileProgram);
                        streamWriter.Write(richTextBoxProgram.Text);
                        streamWriter.Close();
                    }
                } else
                {
                    System.IO.StreamWriter streamWriter;
                    streamWriter = new System.IO.StreamWriter(sourceFileProgram);
                    streamWriter.Write(richTextBoxProgram.Text);
                    streamWriter.Close();
                }
            }

            if (tcSources.SelectedTab.Name == "tpCompactFlash")
            {
                if (sourceFileCompactFlash == "")
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Title = "Save File As";
                    fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    fileDialog.FileName = "";
                    fileDialog.Filter = "Z80 assembly|*.asm;*.z80;*.Z80|All Files|*.*";

                    if (fileDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        System.IO.StreamWriter streamWriter;
                        sourceFileCompactFlash = fileDialog.FileName;
                        streamWriter = new System.IO.StreamWriter(sourceFileCompactFlash);
                        streamWriter.Write(richTextBoxCompactFlash.Text);
                        streamWriter.Close();
                    }
                } else
                {
                    System.IO.StreamWriter streamWriter;
                    streamWriter = new System.IO.StreamWriter(sourceFileCompactFlash);
                    streamWriter.Write(richTextBoxCompactFlash.Text);
                    streamWriter.Close();
                }
            }
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            if (tcSources.SelectedTab.Name == "tpProgram")
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "Save File As";
                fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fileDialog.FileName = "";
                fileDialog.Filter = "Z80 assembly|*.asm;*.z80;*.Z80|All Files|*.*";

                if (fileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    System.IO.StreamWriter streamWriter;
                    sourceFileProgram = fileDialog.FileName;
                    streamWriter = new System.IO.StreamWriter(sourceFileProgram);
                    streamWriter.Write(richTextBoxProgram.Text);
                    streamWriter.Close();
                }
            }

            if (tcSources.SelectedTab.Name == "tpCompactFlash")
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "Save File As";
                fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fileDialog.FileName = "";
                fileDialog.Filter = "Z80 assembly|*.asm;*.z80;*.Z80|All Files|*.*";

                if (fileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    System.IO.StreamWriter streamWriter;
                    sourceFileCompactFlash = fileDialog.FileName;
                    streamWriter = new System.IO.StreamWriter(sourceFileCompactFlash);
                    streamWriter.Write(richTextBoxCompactFlash.Text);
                    streamWriter.Close();
                }
            }
        }

        private void saveBinary_Click(object sender, EventArgs e)
        {
            if (assemblerZ80.programRun == null)
            {
                MessageBox.Show("Nothing yet to save", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save Binary File As";
            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            fileDialog.FileName = "";
            fileDialog.Filter = "Binary|*.bin|All Files|*.*";

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                int start = -1;
                int end = -1;

                // Find start address of code
                for (int i = 0; i < assemblerZ80.RAM.Length; i++)
                {
                    if ((assemblerZ80.RAM[i] != 0) && (start == -1)) start = i;
                }

                // Find end address of code
                for (int i = assemblerZ80.RAM.Length - 1; i >= 0; i--)
                {
                    if ((assemblerZ80.RAM[i] != 0) && (end == -1)) end = i;
                }

                if ((start == -1) || (end == -1))
                {
                    MessageBox.Show("Nothing to save", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // New byte array with only used code 
                byte[] bytes = new byte[end - start + 1];
                for (int i = 0; i < end - start + 1; i++)
                {
                    bytes[i] = assemblerZ80.RAM[start + i];
                }

                // Save binary file
                File.WriteAllBytes(fileDialog.FileName, bytes);

                MessageBox.Show("Binary file saved as\r\n" + fileDialog.FileName, "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void openBinary_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select Binary File";
            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            fileDialog.FileName = "";
            fileDialog.Filter = "Binary|*.bin|All Files|*.*";

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                sourceFileProgram = fileDialog.FileName;
                byte[] bytes = File.ReadAllBytes(sourceFileProgram);

                FormAddresses formAddresses = new FormAddresses();
                formAddresses.ShowDialog();

                FormDisAssembler disAssemblerForm = new FormDisAssembler(bytes, formAddresses.loadAddress, formAddresses.startAddress, formAddresses.useLabels);
                DialogResult dialogResult = disAssemblerForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (timer.Enabled)
                    {
                        timer.Enabled = false;

                        // Enable event handler for updating row/column 
                        richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);
                    }

                    if ((assemblerZ80 != null) && (assemblerZ80.formProgram != null)) assemblerZ80.formProgram.Close();
                    assemblerZ80 = null;
                    UpdateMemoryPanel(0x0000, 0x0000);
                    UpdatePortPanel();
                    UpdateRegisters();
                    UpdateFlags();
                    UpdateInterrupts();
                    ClearDisplay();

                    // Reset color
                    richTextBoxProgram.SelectionStart = 0;
                    richTextBoxProgram.SelectionLength = richTextBoxProgram.Text.Length;
                    richTextBoxProgram.SelectionBackColor = System.Drawing.Color.White;

                    tbSetProgramCounter.Text = "0000";
                    tbMemoryStartAddress.Text = "0000";
                    tbMemoryUpdateByte.Text = "00";
                    numMemoryAddress.Value = 0000;
                    numPort.Value = 0;
                    tbPortUpdateByte.Text = "00";

                    toolStripButtonRun.Enabled = false;
                    toolStripButtonFast.Enabled = false;
                    toolStripButtonStep.Enabled = false;
                    toolStripButtonStop.Enabled = false;

                    lineBreakPoint = -1;

                    Graphics g = pbBreakPoint.CreateGraphics();
                    g.Clear(Color.LightGray);

                    richTextBoxProgram.Text = disAssemblerForm.program;
                }
            }
        }

        private void quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void resetSimulator_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;

                // Enable event handler for updating row/column 
                richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);
            }

            if ((assemblerZ80 != null) && (assemblerZ80.formProgram != null)) assemblerZ80.formProgram.Close();
            assemblerZ80 = null;
            assemblerZ80CompactFlash = null;
            UpdateMemoryPanel(0x0000, 0x0000);
            UpdatePortPanel();
            UpdateRegisters();
            UpdateFlags();
            UpdateInterrupts();
            ClearDisplay();

            // Reset color
            richTextBoxProgram.SelectionStart = 0;
            richTextBoxProgram.SelectionLength = richTextBoxProgram.Text.Length;
            richTextBoxProgram.SelectionBackColor = System.Drawing.Color.White;

            tbSetProgramCounter.Text = "0000";
            tbMemoryStartAddress.Text = "0000";
            tbMemoryUpdateByte.Text = "00";
            numMemoryAddress.Value = 0000;
            numPort.Value = 0;
            tbPortUpdateByte.Text = "00";

            toolStripButtonRun.Enabled = false;
            toolStripButtonFast.Enabled = false;
            toolStripButtonStep.Enabled = false;
            toolStripButtonStop.Enabled = false;

            lineBreakPoint = -1;

            Graphics g = pbBreakPoint.CreateGraphics();
            g.Clear(Color.LightGray);
        }

        private void resetRAM_Click(object sender, EventArgs e)
        {
            assemblerZ80.ClearRam();
            nextInstrAddress = Convert.ToUInt16(tbMemoryStartAddress.Text, 16);
            UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
        }

        private void resetPorts_Click(object sender, EventArgs e)
        {
            assemblerZ80.ClearPorts();
            UpdatePortPanel();
        }

        private void new_Click(object sender, EventArgs e)
        {
            if ((assemblerZ80 != null) && (assemblerZ80.formProgram != null)) assemblerZ80.formProgram.Close();
            assemblerZ80 = null;
            UpdateMemoryPanel(0x0000, 0x0000);
            UpdatePortPanel();
            UpdateRegisters();
            UpdateFlags();
            UpdateInterrupts();
            ClearDisplay();

            richTextBoxProgram.Clear();
            richTextBoxCompactFlash.Clear();
            sourceFileProgram = "";
            sourceFileCompactFlash = "";

            tbSetProgramCounter.Text = "0000";
            tbMemoryStartAddress.Text = "0000";
            tbMemoryUpdateByte.Text = "00";
            numMemoryAddress.Value = 0000;
            numPort.Value = 0;
            tbPortUpdateByte.Text = "00";

            toolStripButtonRun.Enabled = false;
            toolStripButtonFast.Enabled = false;
            toolStripButtonStep.Enabled = false;
            toolStripButtonStop.Enabled = false;

            lineBreakPoint = -1;

            Graphics g = pbBreakPoint.CreateGraphics();
            g.Clear(Color.LightGray);
        }

        private void startDebug_Click(object sender, EventArgs e)
        {
            nextInstrAddress = 0;

            Cursor.Current = Cursors.WaitCursor;

            // Main program
            if (tcSources.SelectedTab.Name == "tpProgram")
            {
                if ((assemblerZ80 != null) && (assemblerZ80.formProgram != null)) assemblerZ80.formProgram.Close();
                assemblerZ80 = new AssemblerZ80(richTextBoxProgram.Lines);

                // Reset compact flash card read/write index
                assemblerZ80.cfIndex = 0;

                try
                {
                    // Run the first Pass of assembler
                    string message = assemblerZ80.FirstPass();
                    if (message != "OK")
                    {
                        MessageBox.Show(this, message, "FIRSTPASS", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Check if a linenumber has been given
                        string[] fields = message.Split(' ');
                        bool result = Int32.TryParse(fields[fields.Length - 1], out int line);
                        if (result)
                        {
                            // Show where the error is (remember the linenumber returned starts with 1 in stead of 0)
                            ChangeColorRTBLine(richTextBoxProgram, line - 1, true);
                        }

                        Cursor.Current = Cursors.Arrow;
                        return;
                    }

                    // Run second pass
                    message = assemblerZ80.SecondPass();
                    if (message != "OK")
                    {
                        MessageBox.Show(this, message, "SECONDPASS", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Show Updated memory
                        UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);

                        // Check if a linenumber has been given
                        string[] fields = message.Split(' ');
                        bool result = Int32.TryParse(fields[fields.Length - 1], out int line);
                        if (result)
                        {
                            // Show where the error is (remember the linenumber returned starts with 1 in stead of 0)
                            ChangeColorRTBLine(richTextBoxProgram, line - 1, true);
                        }

                        Cursor.Current = Cursors.Arrow;
                        return;
                    }

                    // Set startadres of program execution
                    tbMemoryStartAddress.Text = assemblerZ80.startLocation.ToString("X4");
                    tbSetProgramCounter.Text = assemblerZ80.startLocation.ToString("X4");

                    // Show Updated memory
                    UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
                } catch (Exception exception)
                {
                    MessageBox.Show(this, exception.Message, "startDebug_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Cursor.Current = Cursors.Arrow;
                    return;
                }

                nextInstrAddress = Convert.ToUInt16(tbSetProgramCounter.Text, 16);
                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);

                UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
                UpdatePortPanel();
                UpdateRegisters();
                UpdateFlags();
                ClearDisplay();

                toolStripButtonRun.Enabled = true;
                toolStripButtonFast.Enabled = true;
                toolStripButtonStep.Enabled = true;
                toolStripButtonStop.Enabled = false;
            }

            // Compact Flash code
            if (tcSources.SelectedTab.Name == "tpCompactFlash")
            {
                // Create new compact flash image with 512 dir entries
                compactFlash.New(512);

                assemblerZ80CompactFlash = new AssemblerZ80CompactFlash(richTextBoxCompactFlash.Lines);

                try
                {
                    // Run the first Pass of assembler
                    string message = "";
                    int lineNumber = 0;

                    while ((assemblerZ80CompactFlash.lineNumber < richTextBoxCompactFlash.Lines.Count()) && (message != "END"))
                    {
                        message = assemblerZ80CompactFlash.FirstPass(lineNumber);

                        if ((message != "OK") && (message != "END"))
                        {
                            MessageBox.Show(this, message, "FIRSTPASS", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // Check if a linenumber has been given
                            string[] fields = message.Split(' ');
                            bool result = Int32.TryParse(fields[fields.Length - 1], out int line);
                            if (result)
                            {
                                // Show where the error is (remember the linenumber returned starts with 1 in stead of 0)
                                ChangeColorRTBLine(richTextBoxCompactFlash, line - 1, true);
                            }

                            Cursor.Current = Cursors.Arrow;
                            return;
                        }

                        // Run second pass
                        if (message != "END")
                        {
                            message = assemblerZ80CompactFlash.SecondPass(lineNumber);
                            if ((message != "OK") && (message != "END"))
                            {
                                MessageBox.Show(this, message, "SECONDPASS", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                // Check if a linenumber has been given
                                string[] fields = message.Split(' ');
                                bool result = Int32.TryParse(fields[fields.Length - 1], out int line);
                                if (result)
                                {
                                    // Show where the error is (remember the linenumber returned starts with 1 in stead of 0)
                                    ChangeColorRTBLine(richTextBoxCompactFlash, line - 1, true);
                                }

                                Cursor.Current = Cursors.Arrow;
                                return;
                            }

                            // Try to get startaddres of program execution (first line which doesn't point to -1)
                            int startaddress = -1;
                            for (int index = 0; (index < assemblerZ80CompactFlash.RAMprogramLine.Length) && (startaddress == -1); index++)
                            {
                                if (assemblerZ80CompactFlash.RAMprogramLine[index] != -1) startaddress = index;
                            }

                            // Try to get endaddres of program execution (first line from the end which doesn't point to -1)
                            int endaddress = -1;
                            for (int index = assemblerZ80CompactFlash.RAMprogramLine.Length - 1; (index >= 0) && (endaddress == -1); index--)
                            {
                                if (assemblerZ80CompactFlash.RAMprogramLine[index] != -1) endaddress = index + 1;
                            }

                            if (startaddress == -1)
                            {
                                MessageBox.Show(this, "Can't determine start of program", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Cursor.Current = Cursors.Arrow;
                                return;
                            }

                            if (endaddress == -1)
                            {
                                MessageBox.Show(this, "Can't determine end of program", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Cursor.Current = Cursors.Arrow;
                                return;
                            }

                            if ((assemblerZ80CompactFlash.drive != null) && (assemblerZ80CompactFlash.name != null) && (assemblerZ80CompactFlash.type != null))
                            {
                                // Save program to compact flash
                                byte[] bytes = new byte[endaddress - startaddress];
                                for (int i = 0; i < (endaddress - startaddress); i++)
                                {
                                    bytes[i] = assemblerZ80CompactFlash.RAM[startaddress + i];
                                }

                                compactFlash.InsertFile(assemblerZ80CompactFlash.drive, bytes, assemblerZ80CompactFlash.name + "." + assemblerZ80CompactFlash.type);

                                assemblerZ80CompactFlash.drive = null;
                                assemblerZ80CompactFlash.name = null;
                                assemblerZ80CompactFlash.type = null;
                            } else
                            {
                                // Boot code
                                if (endaddress - startaddress > 0x4000)
                                {
                                    MessageBox.Show(this, "The size of the boot program is too large (more then 0x4000 bytes)", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Cursor.Current = Cursors.Arrow;
                                    return;
                                }

                                byte[] boot = new byte[endaddress - startaddress];

                                // Copy to buffer
                                int bootIndex = 0;
                                for (int i = startaddress; i < endaddress; i++)
                                {
                                    boot[bootIndex++] = assemblerZ80CompactFlash.RAM[i];
                                }

                                // Insert buffer in boot sector of drive A
                                compactFlash.InsertFile("A", boot);
                            }
                        }

                        lineNumber = assemblerZ80CompactFlash.lineNumber++;
                    }
                } catch (Exception exception)
                {
                    MessageBox.Show(this, exception.Message, "startDebug_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Cursor.Current = Cursors.Arrow;
                    return;
                }

                // Set first line to green
                ChangeColorRTBLine(richTextBoxCompactFlash, assemblerZ80CompactFlash.RAMprogramLine[0], false);

                MessageBox.Show(this, "No errors in Compact Flash code", "DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Cursor.Current = Cursors.Arrow;
        }

        private void startRun_Click(object sender, EventArgs e)
        {
            toolStripButtonRun.Enabled = false;
            toolStripButtonFast.Enabled = false;
            toolStripButtonStep.Enabled = false;
            toolStripButtonStop.Enabled = true;

            // Disable event handler for updating row/column 
            richTextBoxProgram.SelectionChanged -= richTextBoxProgram_SelectionChanged;

            timer.Interval = Convert.ToInt32(numericUpDownDelay.Value);
            timer.Enabled = true;
        }

        private void startStep_Click(object sender, EventArgs e)
        {
            UInt16 currentInstrAddress = nextInstrAddress;
            string error = assemblerZ80.RunInstruction(currentInstrAddress, ref nextInstrAddress);

            UpdateTerminal();
            UpdateCompactFlash();

            UInt16 startViewAddress = Convert.ToUInt16(memoryAddressLabels[0].Text, 16);

            if (!chkLock.Checked)
            {
                if (nextInstrAddress > startViewAddress + 0x100) startViewAddress = (UInt16)(nextInstrAddress & 0xFFF0);
                if (nextInstrAddress < startViewAddress - 0x100) startViewAddress = (UInt16)(nextInstrAddress & 0xFFF0);
            }

            UpdateMemoryPanel(startViewAddress, nextInstrAddress);
            UpdatePortPanel();
            UpdateRegisters();
            UpdateFlags();
            UpdateInterrupts();

            if ((error == "") && (assemblerZ80.RAMprogramLine[nextInstrAddress] != -1))
            {
                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);

                // Get index of cursor in current program
                int index = richTextBoxProgram.SelectionStart;

                // Get line number
                int line = richTextBoxProgram.GetLineFromCharIndex(index);
                lblLine.Text = (line + 1).ToString();

                int column = richTextBoxProgram.SelectionStart - richTextBoxProgram.GetFirstCharIndexFromLine(line);
                lblColumn.Text = (column + 1).ToString();
            } else if (error == "System Halted")
            {
                toolStripButtonRun.Enabled = false;
                toolStripButtonFast.Enabled = false;
                toolStripButtonStep.Enabled = false;
                toolStripButtonStop.Enabled = false;

                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[currentInstrAddress], false);
                MessageBox.Show(error, "SYSTEM HALTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }

        /// <summary>
        /// Fast run, no updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startFast_Click(object sender, EventArgs e)
        {
            toolStripButtonRun.Enabled = false;
            toolStripButtonFast.Enabled = false;
            toolStripButtonStep.Enabled = false;
            toolStripButtonStop.Enabled = true;

            ClearColorRTBLine(richTextBoxProgram);
            
            string error = "";
            UInt16 currentInstrAddress = nextInstrAddress;

            int counter = 0;
            while (!toolStripButtonFast.Enabled && (error == ""))
            {
                currentInstrAddress = nextInstrAddress;
                error = assemblerZ80.RunInstruction(currentInstrAddress, ref nextInstrAddress);
                if (error == "")
                {
                    UpdateTerminal();
                    UpdateCompactFlash();
                    UpdateInterrupts();
                    Application.DoEvents();

                    counter++;
                    if (counter % 10000 == 0)
                    {
                        this.Focus();
                        Application.DoEvents();
                    }

                    if ((assemblerZ80.RAMprogramLine[nextInstrAddress] == lineBreakPoint) && (lineBreakPoint != -1)) 
                    {
                        toolStripButtonRun.Enabled = true;
                        toolStripButtonFast.Enabled = true;
                        toolStripButtonStep.Enabled = true;
                        toolStripButtonStop.Enabled = false;
                    }

                    if (chkBreakOnExternalCode.Checked && (assemblerZ80.RAMprogramLine[nextInstrAddress] == -1))
                    {
                        toolStripButtonRun.Enabled = true;
                        toolStripButtonFast.Enabled = true;
                        toolStripButtonStep.Enabled = true;
                        toolStripButtonStop.Enabled = false;
                    }

                    toolStripButtonStop.Enabled = true;
                }

                Application.DoEvents();
            }

            UInt16 startViewAddress = Convert.ToUInt16(memoryAddressLabels[0].Text, 16);

            if (!chkLock.Checked)
            {
                if (nextInstrAddress > startViewAddress + 0x100) startViewAddress = (UInt16)(nextInstrAddress & 0xFFF0);
                if (nextInstrAddress < startViewAddress - 0x100) startViewAddress = (UInt16)(nextInstrAddress & 0xFFF0);
            }

            UpdateMemoryPanel(startViewAddress, nextInstrAddress);
            UpdateTerminal();
            UpdatePortPanel();
            UpdateRegisters();
            UpdateFlags();
            UpdateInterrupts();

            if (error == "")
            {
                if (assemblerZ80.RAMprogramLine[nextInstrAddress] == -1)
                {
                    ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[currentInstrAddress], false);
                } else 
                {
                    ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);
                }
                toolStripButtonRun.Enabled = true;
                toolStripButtonFast.Enabled = true;
                toolStripButtonStep.Enabled = true;
                toolStripButtonStop.Enabled = false;
            } else if (error == "System Halted")
            {
                toolStripButtonRun.Enabled = false;
                toolStripButtonFast.Enabled = false;
                toolStripButtonStep.Enabled = false;
                toolStripButtonStop.Enabled = false;

                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[currentInstrAddress], false);
                MessageBox.Show(error, "SYSTEM HALTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else
            {
                toolStripButtonRun.Enabled = false;
                toolStripButtonFast.Enabled = false;
                toolStripButtonStep.Enabled = false;
                toolStripButtonStop.Enabled = false;

                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[currentInstrAddress], true);
                MessageBox.Show(error, "RUNTIME ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Get index of cursor in current program
            int index = richTextBoxProgram.SelectionStart;

            // Get line/column number
            int line = richTextBoxProgram.GetLineFromCharIndex(index);
            lblLine.Text = (line + 1).ToString();

            int column = richTextBoxProgram.SelectionStart - richTextBoxProgram.GetFirstCharIndexFromLine(line);
            lblColumn.Text = (column + 1).ToString();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                timer.Enabled = false;

                // Enable event handler for updating row/column 
                richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);

                ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);

                toolStripButtonRun.Enabled = true;
                toolStripButtonFast.Enabled = true;
                toolStripButtonStep.Enabled = true;
                toolStripButtonStop.Enabled = false;
            }
        }

        private void viewHelp_Click(object sender, EventArgs e)
        {
            FormHelp formHelp = new FormHelp();
            formHelp.ShowDialog();
        }

        private void about_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        #endregion

        #region EventHandlers (Labels)

        private void labelARegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelARegister);
        }

        private void labelBRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelBRegister);
        }

        private void labelCRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelCRegister);
        }

        private void labelDRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelDRegister);
        }

        private void labelERegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelERegister);
        }

        private void labelHRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelHRegister);
        }

        private void labelLRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelLRegister);
        }

        private void labelPCRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelPCRegister);
        }

        private void labelSPRegister_MouseHover(object sender, EventArgs e)
        {
            RegisterHoverBinary(labelSPRegister);
        }

        #endregion

        #region EventHandlers (Buttons)

        /// <summary>
        /// Command buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommand_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag != null)
            {
                Instruction instruction = (Instruction)button.Tag;

                FormCommand command = new FormCommand(instruction.Explanation, instruction.Description, instruction.Mnemonic);
                DialogResult dialogResult = command.ShowDialog();
                
                if (dialogResult == DialogResult.OK)
                {
                    if (richTextBoxProgram.SelectionStart == 0)
                    {
                        richTextBoxProgram.AppendText(command.instruction);
                    } else
                    {
                        richTextBoxProgram.AppendText(Environment.NewLine + command.instruction);
                    }
                }
            }
        }

        /// <summary>
        /// View symbol table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewSymbolTable_Click(object sender, EventArgs e)
        {
            if ((assemblerZ80 != null) && (assemblerZ80.programRun != null))
            {
                string addressSymbolTable = "";

                // Check max length of labels
                int maxLabelSize = 0;
                foreach (KeyValuePair<string, int> keyValuePair in assemblerZ80.addressSymbolTable)
                {
                    if (keyValuePair.Key.Length > maxLabelSize) maxLabelSize = keyValuePair.Key.Length;
                }

                // Add to table
                foreach (KeyValuePair<string, int> keyValuePair in assemblerZ80.addressSymbolTable)
                {
                    addressSymbolTable += keyValuePair.Key;
                    for (int i=keyValuePair.Key.Length; i< maxLabelSize + 1; i++)
                    {
                        addressSymbolTable += " ";
                    }

                    addressSymbolTable += ": " + keyValuePair.Value.ToString("X4") + "\r\n";
                }

                // Create form for display of results                
                Form addressSymbolTableForm = new Form();
                addressSymbolTableForm.Name = "FormSymbolTable";
                addressSymbolTableForm.Text = "SymbolTable";
                addressSymbolTableForm.Icon = Properties.Resources.Z80;
                addressSymbolTableForm.Size = new Size(300, 600);
                addressSymbolTableForm.MinimumSize = new Size(300, 600);
                addressSymbolTableForm.MaximumSize = new Size(300, 600);
                addressSymbolTableForm.MaximizeBox = false;
                addressSymbolTableForm.MinimizeBox = false;
                addressSymbolTableForm.StartPosition = FormStartPosition.CenterScreen;

                // Create button for closing (dialog)form
                Button btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.Location = new Point(204, 530);
                btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
                btnOk.Visible = true;
                btnOk.Click += new EventHandler((object o, EventArgs a) =>
                {
                    addressSymbolTableForm.Close();
                });

                Font font = new Font(FontFamily.GenericMonospace, 10.25F);

                // Sort alphabetically
                string[] tempArray = addressSymbolTable.Split('\n');
                Array.Sort(tempArray, StringComparer.InvariantCulture);
                addressSymbolTable = "";
                foreach (string line in tempArray)
                {
                    addressSymbolTable += line + '\n';
                }

                // Add controls to form
                TextBox textBox = new TextBox();
                textBox.Multiline = true;
                textBox.WordWrap = false;
                textBox.ScrollBars = ScrollBars.Vertical;
                textBox.ReadOnly = true;
                textBox.BackColor = Color.LightYellow;
                textBox.Size = new Size(268, 510);
                textBox.Text = addressSymbolTable;
                textBox.Font = font;
                textBox.BorderStyle = BorderStyle.None;
                textBox.Location = new Point(10, 10);
                textBox.Select(0, 0);

                addressSymbolTableForm.Controls.Add(textBox);
                addressSymbolTableForm.Controls.Add(btnOk);

                // Show form
                addressSymbolTableForm.Show();
            }
        }

        /// <summary>
        /// View program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewProgram_Click(object sender, EventArgs e)
        {
            if ((assemblerZ80 != null) && (assemblerZ80.programRun != null))
            {
                // Create form for display of results                
                Form formProgram = new Form();
                formProgram.Name = "FormProgram";
                formProgram.Text = "Program";
                formProgram.Icon = Properties.Resources.Z80;
                formProgram.Size = new Size(516, 600);
                formProgram.MinimumSize = new Size(500, 600);
                formProgram.MaximizeBox = false;
                formProgram.MinimizeBox = false;
                formProgram.StartPosition = FormStartPosition.CenterScreen;

                // Create button for closing (dialog)form
                Button btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.Location = new Point(400, 530);
                btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
                btnOk.Visible = true;
                btnOk.Click += new EventHandler((object o, EventArgs a) =>
                {
                    formProgram.Close();
                });

                string program = "";
                foreach (string line in assemblerZ80.programView)
                {
                    if ((line != null) && (line != "") && (line != "\r") && (line != "\n") && (line != "\r\n")) program += line + "\r\n";
                }

                Font font = new Font(FontFamily.GenericMonospace, 10.25F);

                // Add controls to form
                TextBox textBox = new TextBox();
                textBox.Multiline = true;
                textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                textBox.WordWrap = false;
                textBox.ScrollBars = ScrollBars.Vertical;
                textBox.ReadOnly = true;
                textBox.BackColor = Color.LightYellow;
                textBox.Size = new Size(486, 510);
                textBox.Text = program;
                textBox.Font = font;
                textBox.BorderStyle = BorderStyle.None;
                textBox.Location = new Point(10, 10);
                textBox.Select(0, 0);

                formProgram.Controls.Add(textBox);
                formProgram.Controls.Add(btnOk);

                // Show form
                formProgram.Show();
            }
        }

        /// <summary>
        /// Set memory start address to view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemoryStartAddress_Click(object sender, EventArgs e)
        {
            UpdateMemoryPanel(GetTextBoxMemoryStartAddress(), nextInstrAddress);
        }

        /// <summary>
        /// Set memory start address to view to Program Counter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewPC_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                UpdateMemoryPanel(assemblerZ80.registerPC, nextInstrAddress);
                tbMemoryStartAddress.Text = assemblerZ80.registerPC.ToString("X4");
            }
        }

        /// <summary>
        /// Set memory start address to view to Stack Pointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewSP_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                UpdateMemoryPanel(assemblerZ80.registerSP, nextInstrAddress);
                tbMemoryStartAddress.Text = assemblerZ80.registerSP.ToString("X4");
            }
        }

        /// <summary>
        /// Previous memory to view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                if (Convert.ToUInt16(memoryAddressLabels[0].Text, 16) >= 0x0100)
                {
                    UInt16 n = (UInt16)(Convert.ToUInt16(memoryAddressLabels[0].Text, 16) - 0x0100);

                    tbMemoryStartAddress.Text = n.ToString("X4");
                    UpdateMemoryPanel(n, nextInstrAddress);
                }
            }
        }

        /// <summary>
        /// Next memory to view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                if (Convert.ToUInt16(memoryAddressLabels[0].Text, 16) < 0xFF00)
                {
                    UInt16 n = (UInt16)(Convert.ToUInt16(memoryAddressLabels[0].Text, 16) + 0x0100);

                    tbMemoryStartAddress.Text = n.ToString("X4");
                    UpdateMemoryPanel(n, nextInstrAddress);
                }
            }
        }

        /// <summary>
        /// Write value to memory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemoryWrite_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                assemblerZ80.RAM[(int)numMemoryAddress.Value] = Convert.ToByte(tbMemoryUpdateByte.Text, 16);

                UInt16 n = (UInt16)(Convert.ToUInt16(memoryAddressLabels[0].Text, 16));
                if (
                    (((UInt16)numMemoryAddress.Value) >= n) &&
                    (((UInt16)numMemoryAddress.Value) < n + 0x100)
                   )
                {
                    UpdateMemoryPanel(n, nextInstrAddress);
                }
            }
        }

        /// <summary>
        /// Clear all ports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearPORT_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                assemblerZ80.ClearPorts();
                UpdatePortPanel();
            }
        }

        /// <summary>
        ///  Write value to port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPortWrite_Click(object sender, EventArgs e)
        {
            if (assemblerZ80 != null)
            {
                assemblerZ80.PORT[(int)numPort.Value] = Convert.ToByte(tbPortUpdateByte.Text, 16);

                UpdatePortPanel();
            }
        }

        /// <summary>
        /// Clear breakpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearBreakPoint_Click(object sender, EventArgs e)
        {
            lineBreakPoint = -1;

            Graphics g = pbBreakPoint.CreateGraphics();
            g.Clear(Color.LightGray);
        }

        #endregion

        #region EventHandlers (RichTextBox)

        private void richTextBoxProgram_SelectionChanged(object sender, EventArgs e)
        {
            // Get index of cursor in current program
            int index = richTextBoxProgram.SelectionStart;

            // Get line number
            int line = richTextBoxProgram.GetLineFromCharIndex(index);
            lblLine.Text = (line + 1).ToString();

            int column = richTextBoxProgram.SelectionStart - richTextBoxProgram.GetFirstCharIndexFromLine(line);
            lblColumn.Text = (column + 1).ToString();
        }

        private void richTextBoxCompactFlash_SelectionChanged(object sender, EventArgs e)
        {
            // Get index of cursor in current program
            int index = richTextBoxCompactFlash.SelectionStart;

            // Get line number
            int line = richTextBoxCompactFlash.GetLineFromCharIndex(index);
            lblLine.Text = (line + 1).ToString();

            int column = richTextBoxCompactFlash.SelectionStart - richTextBoxCompactFlash.GetFirstCharIndexFromLine(line);
            lblColumn.Text = (column + 1).ToString();
        }

        /// <summary>
        /// Program adjusted, remove highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxProgram_TextChanged(object sender, EventArgs e)
        {
            if (toolStripButtonRun.Enabled)
            {
                int pos = richTextBoxProgram.SelectionStart;

                // Reset color
                richTextBoxProgram.SelectionStart = 0;
                richTextBoxProgram.SelectionLength = richTextBoxProgram.Text.Length;
                richTextBoxProgram.SelectionBackColor = System.Drawing.Color.White;

                richTextBoxProgram.SelectionLength = 0;

                richTextBoxProgram.SelectionStart = pos;

                toolStripButtonRun.Enabled = false;
                toolStripButtonFast.Enabled = false;
                toolStripButtonStep.Enabled = false;
                toolStripButtonStop.Enabled = false;
            }

            lineBreakPoint = -1;

            Graphics g = pbBreakPoint.CreateGraphics();
            g.Clear(Color.LightGray);
        }

        /// <summary>
        /// Program adjusted, remove highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxCompactFlash_TextChanged(object sender, EventArgs e)
        {
            if (toolStripButtonRun.Enabled)
            {
                int pos = richTextBoxCompactFlash.SelectionStart;

                // Reset color
                richTextBoxCompactFlash.SelectionStart = 0;
                richTextBoxCompactFlash.SelectionLength = richTextBoxCompactFlash.Text.Length;
                richTextBoxCompactFlash.SelectionBackColor = System.Drawing.Color.White;

                richTextBoxCompactFlash.SelectionLength = 0;

                richTextBoxCompactFlash.SelectionStart = pos;

                toolStripButtonRun.Enabled = false;
                toolStripButtonFast.Enabled = false;
                toolStripButtonStep.Enabled = false;
                toolStripButtonStop.Enabled = false;
            }
        }

        /// <summary>
        /// Mouse button clicked in control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxProgram_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;

            int charIndex = richTextBoxProgram.GetCharIndexFromPosition(new Point(x, y));
            int lineIndex = richTextBoxProgram.GetLineFromCharIndex(charIndex);

            if (assemblerZ80 != null)
            {
                bool found = false;
                for (int address = 0; (address < assemblerZ80.RAMprogramLine.Length) && !found; address++)
                {
                    if (assemblerZ80.RAMprogramLine[address] == lineIndex)
                    {
                        found = true;
                        int startAddress = Convert.ToInt32(memoryAddressLabels[0].Text, 16);

                        int row = (address - startAddress) / 16;
                        int col = (address - startAddress) % 16;

                        foreach (Label lbl in memoryTableLabels)
                        {
                            if (lbl.BackColor != Color.LightGreen) lbl.BackColor = SystemColors.Info;
                        }

                        if ((row >= 0) && (col >= 0) && (row < 16) && (col < 16))
                        {
                            if (memoryTableLabels[row, col].BackColor != Color.LightGreen) memoryTableLabels[row, col].BackColor = SystemColors.GradientInactiveCaption;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mouse enters control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxProgram_MouseEnter(object sender, EventArgs e)
        {
            toolTip.Active = true;
        }

        /// <summary>
        /// Mouse leaves control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxProgram_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(richTextBoxProgram);
            toolTip.Active = false;
        }

        /// <summary>
        /// Disable tooltip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void richTextBoxProgram_MouseMove(object sender, MouseEventArgs e)
        {
            if ((toolTip != null) && (assemblerZ80 != null))
            {
                int x = e.Location.X;
                int y = e.Location.Y;

                int charIndex = richTextBoxProgram.GetCharIndexFromPosition(new Point(x, y));
                int lineIndex = richTextBoxProgram.GetLineFromCharIndex(charIndex);

                bool found = false;
                for (int index = 0; (index < assemblerZ80.RAMprogramLine.Length) && !found; index++)
                {
                    if (assemblerZ80.RAMprogramLine[index] == lineIndex)
                    {
                        found = true;
                        if (toolTip.GetToolTip(richTextBoxProgram) != index.ToString("X4")) 
                        {
                            toolTip.Show(index.ToString("X4"), richTextBoxProgram, -50, richTextBoxProgram.GetPositionFromCharIndex(charIndex).Y, 50000);
                        } 
                    }
                }
            }
        }

        private void richTextBoxProgram_VScroll(object sender, EventArgs e)
        {
            UpdateBreakPoint(lineBreakPoint);
            toolTip.Hide(richTextBoxProgram);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updating the Registers
        /// </summary>
        private void UpdateRegisters()
        {
            if (assemblerZ80 != null)
            {
                labelARegister.Text = assemblerZ80.registerA.ToString("X").PadLeft(2, '0');
                labelBRegister.Text = assemblerZ80.registerB.ToString("X").PadLeft(2, '0');
                labelCRegister.Text = assemblerZ80.registerC.ToString("X").PadLeft(2, '0');
                labelDRegister.Text = assemblerZ80.registerD.ToString("X").PadLeft(2, '0');
                labelERegister.Text = assemblerZ80.registerE.ToString("X").PadLeft(2, '0');
                labelHRegister.Text = assemblerZ80.registerH.ToString("X").PadLeft(2, '0');
                labelLRegister.Text = assemblerZ80.registerL.ToString("X").PadLeft(2, '0');

                labelAaltRegister.Text = assemblerZ80.registerAalt.ToString("X").PadLeft(2, '0');
                labelBaltRegister.Text = assemblerZ80.registerBalt.ToString("X").PadLeft(2, '0');
                labelCaltRegister.Text = assemblerZ80.registerCalt.ToString("X").PadLeft(2, '0');
                labelDaltRegister.Text = assemblerZ80.registerDalt.ToString("X").PadLeft(2, '0');
                labelEaltRegister.Text = assemblerZ80.registerEalt.ToString("X").PadLeft(2, '0');
                labelHaltRegister.Text = assemblerZ80.registerHalt.ToString("X").PadLeft(2, '0');
                labelLaltRegister.Text = assemblerZ80.registerLalt.ToString("X").PadLeft(2, '0');

                labelIXRegister.Text = assemblerZ80.registerIX.ToString("X").PadLeft(4, '0');
                labelIYRegister.Text = assemblerZ80.registerIY.ToString("X").PadLeft(4, '0');

                labelPCRegister.Text = assemblerZ80.registerPC.ToString("X").PadLeft(4, '0');
                labelSPRegister.Text = assemblerZ80.registerSP.ToString("X").PadLeft(4, '0');
            } else
            {
                labelARegister.Text = "00";
                labelBRegister.Text = "00";
                labelCRegister.Text = "00";
                labelDRegister.Text = "00";
                labelERegister.Text = "00";
                labelHRegister.Text = "00";
                labelLRegister.Text = "00";

                labelAaltRegister.Text = "00";
                labelBaltRegister.Text = "00";
                labelCaltRegister.Text = "00";
                labelDaltRegister.Text = "00";
                labelEaltRegister.Text = "00";
                labelHaltRegister.Text = "00";
                labelLaltRegister.Text = "00";

                labelIXRegister.Text = "0000";
                labelIYRegister.Text = "0000";

                labelPCRegister.Text = "0000";
                labelSPRegister.Text = "0000";
            }
        }

        /// <summary>
        /// Update the Flags
        /// </summary>
        private void UpdateFlags()
        {
            if (assemblerZ80 != null)
            {
                chkFlagC.Checked = assemblerZ80.flagC;
                chkFlagPV.Checked = assemblerZ80.flagPV;
                chkFlagH.Checked = assemblerZ80.flagH;
                chkFlagN.Checked = assemblerZ80.flagN;
                chkFlagZ.Checked = assemblerZ80.flagZ;
                chkFlagS.Checked = assemblerZ80.flagS;
                chkFlagCalt.Checked = assemblerZ80.flagCalt;
                chkFlagPValt.Checked = assemblerZ80.flagPValt;
                chkFlagHalt.Checked = assemblerZ80.flagHalt;
                chkFlagNalt.Checked = assemblerZ80.flagNalt;
                chkFlagZalt.Checked = assemblerZ80.flagZalt;
                chkFlagSalt.Checked = assemblerZ80.flagSalt;
            } else
            {
                chkFlagC.Checked = false;
                chkFlagPV.Checked = false;
                chkFlagH.Checked = false;
                chkFlagN.Checked = false;
                chkFlagZ.Checked = false;
                chkFlagS.Checked = false;
                chkFlagCalt.Checked = false;
                chkFlagPValt.Checked = false;
                chkFlagHalt.Checked = false;
                chkFlagNalt.Checked = false;
                chkFlagZalt.Checked = false;
                chkFlagSalt.Checked = false;
            }
        }

        /// <summary>
        /// Update interrupt masks
        /// </summary>
        private void UpdateInterrupts()
        {
            if (assemblerZ80 != null)
            {
                if (assemblerZ80.intrIE)
                {
                    lblInterrupts.BackColor = Color.LightGreen;
                } else
                {
                    lblInterrupts.BackColor = Color.LightPink;
                }

                if (assemblerZ80.im == AssemblerZ80.IM.im0) lblIM.Text = "Interrupt Mode: im0";
                if (assemblerZ80.im == AssemblerZ80.IM.im1) lblIM.Text = "Interrupt Mode: im1";
                if (assemblerZ80.im == AssemblerZ80.IM.im2) lblIM.Text = "Interrupt Mode: im2";
            }
        }

        /// <summary>
        /// Draw memory panel starting from address startAddress, show nextAddress in green
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="nextAddress"></param>
        private void UpdateMemoryPanel(UInt16 startAddress, UInt16 nextAddress)
        {
            if (assemblerZ80 != null)
            {
                // Boundary at address XXX0
                startAddress = (UInt16)(startAddress & 0xFFF0);

                // Check for overflow in display (startaddress + 0xFF larger then 0xFFFF)
                if (startAddress > 0xFF00) startAddress = 0xFF00;

                int i = startAddress;
                int j = 0;

                foreach (Label lbl in memoryAddressLabels)
                {
                    lbl.Text = i.ToString("X").PadLeft(4, '0');
                    i += 0x10;
                }

                i = 0;
                j = 0;

                // MemoryTableLabels, display the memory contents
                foreach (Label lbl in memoryTableLabels)
                {
                    int address = startAddress + (16 * i) + j;
                    lbl.Text = assemblerZ80.RAM[address].ToString("X").PadLeft(2, '0');

                    if (address == nextAddress)
                    {
                        lbl.BackColor = Color.LightGreen;
                    } else
                    if (address == assemblerZ80.registerSP)
                    {
                        lbl.BackColor = Color.LightPink;
                    } else
                    {
                        lbl.BackColor = SystemColors.Info;
                    }

                    j++;
                    if (j == 0x10)
                    {
                        j = 0;
                        i++;
                    }
                }
            } else
            {
                int i = 0;

                foreach (Label lbl in memoryAddressLabels)
                {
                    lbl.Text = i.ToString("X").PadLeft(4, '0');
                    i += 0x10;
                }

                // MemoryTableLabels, display 00
                foreach (Label lbl in memoryTableLabels)
                {
                    lbl.Text = "00";
                    lbl.BackColor = SystemColors.Info;
                }
            }
        }

        /// <summary>
        /// Port panel Update, we can view 256 Bytes at a time, it will be in form of 16 X 16
        /// </summary>
        private void UpdatePortPanel()
        {
            if (assemblerZ80 != null)
            {
                int i = 0;
                int j = 0;

                foreach (Label lbl in portAddressLabels)
                {
                    lbl.Text = i.ToString("X").PadLeft(2, '0');
                    i += 0x10;
                }

                i = 0;
                j = 0;

                // PortTableLabels, display the port contents
                foreach (Label lbl in portTableLabels)
                {
                    lbl.Text = assemblerZ80.PORT[(16 * i) + j].ToString("X").PadLeft(2, '0');

                    j++;
                    if (j == 0x10)
                    {
                        j = 0x00;
                        i++;
                    }
                }
            } else
            {
                // PortTableLabels, display 00
                foreach (Label lbl in portTableLabels)
                {
                    lbl.Text = "00";
                }
            }
        }

        /// <summary>
        /// Clear all of the terminal display
        /// </summary>
        private void ClearDisplay()
        {
            if (formTerminal != null)
            {
                formTerminal.Clear();
                formTerminal.keyBuffer = "";
            }
        }

        /// <summary>
        /// Update terminal (if active)
        /// </summary>
        private void UpdateTerminal()
        {
            if ((assemblerZ80 != null) && (formTerminal != null))
            {
                // Update terminal display if signalled from assembler
                if (assemblerZ80.UpdateDisplay)
                {
                    switch (assemblerZ80.PORT[0x80])
                    {
                        case 12:
                            formTerminal.tbTerminal.Text += "\r\n";
                            break;

                        default:
                            formTerminal.tbTerminal.Text += Convert.ToChar(assemblerZ80.PORT[0x80]);
                            formTerminal.tbTerminal.Select(0, 0);
                            if (formTerminal.tbTerminal.TextLength > 0)
                            {
                                formTerminal.tbTerminal.SelectionStart = formTerminal.tbTerminal.TextLength - 1;
                                formTerminal.tbTerminal.ScrollToCaret();
                            }
                            break;
                    }

                    assemblerZ80.UpdateDisplay = false;

                    // Set SIO control ports status 'buffer empty' and 'char ready' for port A
                    assemblerZ80.PORT[0x82] |= 0x05; 
                }

                // Start serial interrupt handler if characters in buffer
                if (formTerminal.keyBuffer != "")
                {
                    // Next instruction is serial interrupt handler 
                    int address = -1;
                    foreach (KeyValuePair<string, int> keyValuePair in assemblerZ80.addressSymbolTable)
                    {
                        if (keyValuePair.Key == "IO_INTERRUPT_HANDLER") address = keyValuePair.Value;
                    }

                    if (address >= 0)
                    {
                        // Disable further interrupts
                        assemblerZ80.intrIE = false;
                        UpdateInterrupts();

                        // Put first character from the terminal key buffer on port 0 (SIOA_D)
                        byte key = (byte)formTerminal.keyBuffer[0];
                        formTerminal.keyBuffer = formTerminal.keyBuffer.Substring(1);
                        assemblerZ80.PORT[0x80] = key;

                        // Put current Program Counter on stack
                        string temp = assemblerZ80.registerPC.ToString("X4");
                        string hi = temp.Substring(temp.Length - 4, 2);
                        string lo = temp.Substring(temp.Length - 2, 2);
                        assemblerZ80.registerSP--;
                        assemblerZ80.RAM[assemblerZ80.registerSP] = Convert.ToByte(hi, 16);
                        assemblerZ80.registerSP--;
                        assemblerZ80.RAM[assemblerZ80.registerSP] = Convert.ToByte(lo, 16);

                        // Set next instruction to interrupt routine
                        nextInstrAddress = (UInt16)address;

                        // Set SIO control ports status 'buffer empty' and 'char ready' for port A
                        assemblerZ80.PORT[0x82] |= 0x05;
                    } else
                    {
                        timer.Enabled = false;

                        MessageBox.Show(this, "Interrupt Handler with label 'IO_INTERRUPT_HANDLER' not found in code.\r\nCan't handle incoming characters from the terminal", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        formTerminal.keyBuffer = "";

                        // Enable event handler for updating row/column 
                        richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);

                        ChangeColorRTBLine(richTextBoxProgram, assemblerZ80.RAMprogramLine[nextInstrAddress], false);

                        toolStripButtonRun.Enabled = true;
                        toolStripButtonFast.Enabled = true;
                        toolStripButtonStep.Enabled = true;
                        toolStripButtonStop.Enabled = false;
                        return;
                    }
                }

                // Set SIO control ports status 'buffer empty' and 'char ready' for port B (not used)
                assemblerZ80.PORT[0x83] |= 0x05;
            }
        }

        /// <summary>
        /// Update Compact Flash I/O
        /// </summary>
        private void UpdateCompactFlash()
        {
            if ((assemblerZ80 != null) && (compactFlash != null))
            {
                // Read Compact Flash data from address indicated by port 0x13, 0x14 and 0x15 (LBA0, LBA1 and LBA2) to indicate the sector and indexed with a counter 
                if (assemblerZ80.UpdateCompactFlash)
                {
                    int cfAddress = assemblerZ80.PORT[0x15] * 256* 256 * 512 + assemblerZ80.PORT[0x14] * 256 * 512 + assemblerZ80.PORT[0x13] * 512 + assemblerZ80.cfIndex; 
                    assemblerZ80.PORT[0x10] = compactFlash.GetData(cfAddress);
                    assemblerZ80.registerA = compactFlash.GetData(cfAddress);
                    assemblerZ80.cfIndex++;
                }

                assemblerZ80.UpdateCompactFlash = false;
            }
        }

        /// <summary>
        /// get the memory start address from text box
        /// </summary>
        /// <returns></returns>
        private UInt16 GetTextBoxMemoryStartAddress()
        {
            string txtval = tbMemoryStartAddress.Text;
            UInt16 n = Convert.ToUInt16(txtval, 16);    // convert HEX to INT
            return n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="line_number"></param>
        /// <param name="error"></param>
        private void ChangeColorRTBLine(RichTextBox richTextBox, int line_number, bool error)
        {
            if ((line_number >= 0) && (richTextBox.Lines.Length > line_number))
            {
                // No layout events for now (postpone)
                richTextBox.SuspendLayout();

                // Disable certain event handlers completely
                richTextBoxProgram.TextChanged -= richTextBoxProgram_TextChanged;
                richTextBoxProgram.SelectionChanged -= richTextBoxProgram_SelectionChanged;

                // No focus so we won't see flicker from selection changes
                lblSetProgramCounter.Focus();

                // Reset color
                richTextBox.HideSelection = true;
                richTextBox.SelectAll();
                richTextBox.SelectionBackColor = System.Drawing.Color.White;
                richTextBox.DeselectAll();
                richTextBox.HideSelection = false;

                // Get location in RTB
                int firstcharindex = richTextBox.GetFirstCharIndexFromLine(line_number);
                string currentlinetext = richTextBox.Lines[line_number];

                // Select line and color red/green
                richTextBox.SelectionStart = firstcharindex;
                richTextBox.SelectionLength = currentlinetext.Length;
                richTextBox.SelectionBackColor = System.Drawing.Color.LightGreen;
                if (error) richTextBox.SelectionBackColor = System.Drawing.Color.LightPink;

                // Reset selection
                richTextBox.SelectionStart = firstcharindex;
                richTextBox.SelectionLength = 0;

                // Scroll to line (show 1 line before selected line if available)
                if (line_number != 0)
                {
                    firstcharindex = richTextBox.GetFirstCharIndexFromLine(line_number - 1);
                    richTextBox.SelectionStart = firstcharindex;
                }

                richTextBox.ScrollToCaret();

                // Set cursor at selected line
                firstcharindex = richTextBox.GetFirstCharIndexFromLine(line_number);
                richTextBox.SelectionStart = firstcharindex;
                richTextBox.SelectionLength = 0;

                // Set focus again
                richTextBox.Focus();

                // Enable event handler
                richTextBoxProgram.TextChanged += new EventHandler(richTextBoxProgram_TextChanged);
                richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);

                // Resume events 
                richTextBox.ResumeLayout();
            }
        }

        /// <summary>
        /// Clear colors rich text box
        /// </summary>
        /// <param name="richTextBox"></param>
        private void ClearColorRTBLine(RichTextBox richTextBox)
        {
            // No layout events for now (postpone)
            richTextBox.SuspendLayout();

            // Disable certain event handlers completely
            richTextBoxProgram.TextChanged -= richTextBoxProgram_TextChanged;
            richTextBoxProgram.SelectionChanged -= richTextBoxProgram_SelectionChanged;

            // No focus so we won't see flicker from selection changes
            lblSetProgramCounter.Focus();

            // Reset color
            richTextBox.HideSelection = true;
            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = System.Drawing.Color.White;
            richTextBox.DeselectAll();
            richTextBox.HideSelection = false;

            // Set focus again
            richTextBox.Focus();

            // Update breakpoint indicator
            UpdateBreakPoint(lineBreakPoint);

            // Enable event handler
            richTextBoxProgram.TextChanged += new EventHandler(richTextBoxProgram_TextChanged);
            richTextBoxProgram.SelectionChanged += new EventHandler(richTextBoxProgram_SelectionChanged);

            // Resume events 
            richTextBox.ResumeLayout();
        }

        /// <summary>
        /// show tooltip with string binaryval when we hover mouse over a (register) label 
        /// </summary>
        /// <param name="l"></param>
        private void RegisterHoverBinary(Label l)
        {
            string binaryval;
            binaryval = Convert.ToString(Convert.ToInt32(l.Text, 16), 2);

            // change the HEX string to BINARY string
            binaryval = binaryval.PadLeft(8, '0');
            toolTipRegisterBinary.SetToolTip(l, binaryval);
        }

        /// <summary>
        /// Update picturebox with breakpoint
        /// </summary>
        private void UpdateBreakPoint(int line)
        {
            // Clear other breakpoint
            Graphics g = pbBreakPoint.CreateGraphics();
            g.Clear(Color.LightGray);

            if (line >= 0)
            {
                int index = richTextBoxProgram.GetFirstCharIndexFromLine(line);
                if (index > 0)
                {
                    Point point = richTextBoxProgram.GetPositionFromCharIndex(index);
                    g.FillEllipse(Brushes.Red, new Rectangle(1, richTextBoxProgram.Margin.Top + point.Y, 15, 15));
                }
            }
        }

        /// <summary>
        /// Add info for each instruction button
        /// </summary>
        private void InitButtons()
        {
            // All instructions, sorted by name
            Instructions instructions = new Instructions();
            Array.Sort(instructions.Z80MainInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));
            Array.Sort(instructions.Z80BitInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));
            Array.Sort(instructions.Z80IXInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));
            Array.Sort(instructions.Z80IYInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));
            Array.Sort(instructions.Z80MiscInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));
            Array.Sort(instructions.Z80IXBitInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));
            Array.Sort(instructions.Z80IYBitInstructions, (x, y) => x.Mnemonic.CompareTo(y.Mnemonic));

            // Add all instruction buttons
            int numInstruction;

            // Main    
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80MainInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80MainInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 68;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(66, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage1.Controls.Add(button);

                    numInstruction++;
                }
            }

            // Bit
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80BitInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80BitInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 68;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(66, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage2.Controls.Add(button);

                    numInstruction++;
                }
            }

            // IX
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80IXInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80IXInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 72;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(70, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage3.Controls.Add(button);

                    numInstruction++;
                }
            }

            // IY
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80IYInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80IYInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 72;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(70, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage4.Controls.Add(button);

                    numInstruction++;
                }
            }

            // Misc.
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80MiscInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80MiscInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 72;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(70, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage5.Controls.Add(button);

                    numInstruction++;
                }
            }

            // BitIX
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80IXBitInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80IXBitInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 72;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(70, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage6.Controls.Add(button);

                    numInstruction++;
                }
            }

            // BitIY
            numInstruction = 0;
            for (int indexZ80Instructions = 0; indexZ80Instructions < instructions.Z80IYBitInstructions.Length; indexZ80Instructions++)
            {
                Instruction instruction = instructions.Z80IYBitInstructions[indexZ80Instructions];

                if ((instruction.Mnemonic != "-") && (instruction.Size != 0))
                {
                    int x = 2 + (numInstruction % 4) * 72;
                    int y = 2 + (numInstruction / 4) * 25;

                    Button button = new Button();
                    button.BackColor = Color.LightGreen;
                    button.Location = new Point(x, y);
                    button.Name = "btn" + instruction.Opcode.ToString();
                    button.Size = new Size(70, 23);
                    button.Text = instruction.Mnemonic;
                    button.UseVisualStyleBackColor = false;
                    button.Click += new EventHandler(this.btnCommand_Click);
                    button.Tag = instruction;
                    button.MouseHover += new EventHandler(Control_MouseHover);

                    tabPage7.Controls.Add(button);

                    numInstruction++;
                }
            }
        }

        #endregion
    }
}
