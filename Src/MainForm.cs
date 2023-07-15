using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace CPM
{
    public partial class MainForm : Form
    {
        #region Members

        // Filename (argument of mainform and general purpose)
        private string fileName;
        private string fileNameContainer;

        #endregion

        #region Constructor

        public MainForm(string fileName = "")
        {
            InitializeComponent();

            this.fileName = fileName;
            this.fileNameContainer = "";
            if (fileName.ToLower().EndsWith("img")) fileNameContainer = fileName; 
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Main form loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            cbDirEntries.SelectedIndex = 0;

            if (fileName != "")
            {
                // Read image file into byte array
                byte[] bytes = File.ReadAllBytes(fileName);

                // Check if this is a single diskimage
                if (fileName.ToLower().EndsWith(".cpm"))
                {
                    // Create new diskimage object
                    int max_dir = Convert.ToInt32(cbDirEntries.Text);
                    byte drive = (byte)('A' + lbImages.Items.Count);
                    DiskImage diskImage = new DiskImage(fileName, Convert.ToChar(drive).ToString(), bytes, max_dir);

                    // Add the new object (image) to the listbox
                    lbImages.Items.Add(diskImage);
                } else
                {
                    // If not, assume a container of 8MB disk images (2MB at the end ?)
                    int parts = bytes.Length / 0x00800000;
                    if (bytes.Length % 0x00800000 != 0) parts++;

                    for (int part = 0; part < parts; part++)
                    {
                        int bytes_left = bytes.Length - (part * 0x00800000);
                        if (bytes_left > 0x00800000) bytes_left = 0x00800000;

                        byte[] bytes_image = new byte[bytes_left];
                        Array.Copy(bytes, part * 0x00800000, bytes_image, 0, bytes_left);

                        // Create new diskimage object
                        int max_dir = Convert.ToInt32(cbDirEntries.Text);
                        byte drive = (byte)('A' + lbImages.Items.Count);
                        DiskImage diskImage = new DiskImage(fileName, Convert.ToChar(drive).ToString(), bytes_image, max_dir);

                        // Add the new object (image) to the listbox
                        lbImages.Items.Add(diskImage);
                    }
                }

                // Show files in this image
                lbImages.SelectedIndex = lbImages.Items.Count - 1;
                ShowImageFiles((DiskImage)lbImages.SelectedItem);
            }
        }

        /// <summary>
        /// Show help form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Click(object sender, EventArgs e)
        {
            FormHelp formHelp = new FormHelp();
            formHelp.ShowDialog();
        }

        /// <summary>
        /// Show about form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        /// <summary>
        /// New image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, EventArgs e)
        {
            // Create new empty image, standard 8MB. Only 2Mb if last one (P)
            int max_dir = Convert.ToInt32(cbDirEntries.Text);
            char drive = (char)('A' + lbImages.Items.Count);
            byte[] bytes;

            if (drive > 'P')
            {
                MessageBox.Show("No more room for images (max = drive 'P')", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (drive == 'P')
            {
                bytes = new byte[0x00200000];
            } else
            {
                bytes = new byte[0x00800000];
            }

            DiskImage diskImage = new DiskImage(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + drive.ToString() + ".CPM", drive.ToString(), bytes, max_dir);

            // Set dir entries to empty
            for (int i = 0; i < max_dir; i++)
            {
                if (diskImage.boot)
                {
                    diskImage.bytes[DiskImage.BOOTSIZE + i * 32] = 0xE5;
                } else
                {
                    diskImage.bytes[i * 32] = 0xE5;
                }
            }

            // Update file info
            diskImage.SetFileInfo();

            // Add the new object (image) to the listbox
            lbImages.Items.Add(diskImage);
            lbImages.SelectedIndex = lbImages.Items.Count - 1;
        }

        /// <summary>
        /// Open image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select Image File";
            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            fileDialog.FileName = "";
            fileDialog.Multiselect = false;
            fileDialog.Filter = "CPM Disk Image (Container)|*.img;*.cpm|All Files|*.*";

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                fileName = fileDialog.FileName;

                // Read image file into byte array
                byte[] bytes = File.ReadAllBytes(fileName);

                // Check if this is a single diskimage
                if (fileName.ToLower().EndsWith(".cpm"))
                {
                    if (lbImages.Items.Count >= 16)
                    {
                        MessageBox.Show("No more room for images (max = drive 'P')", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Create new diskimage object
                    int max_dir = Convert.ToInt32(cbDirEntries.Text);
                    byte drive = (byte)('A' + lbImages.Items.Count);
                    DiskImage diskImage = new DiskImage(fileName, Convert.ToChar(drive).ToString(), bytes, max_dir);

                    // Add the new object (image) to the listbox
                    lbImages.Items.Add(diskImage);
                } else
                {
                    // If not, assume a container of 8MB disk images (2MB image at the end ?)
                    lbImages.Items.Clear();
                    btnBoot.Visible = false;

                    fileNameContainer = fileName;
                    string folderName = "";
                    string[] temp = fileName.Split('\\');
                    for (int i = 0; i < temp.Length - 1; i++) folderName += temp[i] + "\\";

                    int parts = bytes.Length / 0x00800000;
                    if (bytes.Length % 0x00800000 != 0) parts++;

                    for (int part=0; part< parts; part++)
                    {
                        int bytes_left = bytes.Length - (part * 0x00800000);
                        if (bytes_left > 0x00800000) bytes_left = 0x00800000;

                        byte[] bytes_image = new byte[bytes_left];
                        Array.Copy(bytes, part * 0x00800000, bytes_image, 0, bytes_left);

                        // Create new diskimage object
                        int max_dir = Convert.ToInt32(cbDirEntries.Text);
                        byte drive = (byte)('A' + lbImages.Items.Count);

                        DiskImage diskImage = new DiskImage(folderName + Convert.ToChar(drive).ToString() + ".CPM", Convert.ToChar(drive).ToString(), bytes_image, max_dir);

                        // Add the new object (image) to the listbox
                        lbImages.Items.Add(diskImage);
                    }
                }

                // Show files in this image
                lbImages.SelectedIndex = lbImages.Items.Count - 1;
                ShowImageFiles((DiskImage)lbImages.SelectedItem);
            }
        }

        /// <summary>
        /// Close image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, EventArgs e)
        {
            if (lbImages.SelectedItem != null)
            {
                DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
                lbImages.Items.Remove(diskImage);
                btnBoot.Visible = false;

                for (int i=0; i<lbImages.Items.Count; i++)
                {
                    // Assign disk name (volume)
                    ((DiskImage)lbImages.Items[i]).volume = Convert.ToChar(((byte)('A' + i))).ToString();
                    
                    // Redraw
                    lbImages.Items[i] = lbImages.Items[i];
                }

                ShowImageFiles(null);
                if (lbImages.Items.Count > 0) lbImages.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Save image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            if (lbImages.SelectedItem != null)
            {
                DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
                fileName = diskImage.file_name;

                if (fileName == "")
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Title = "Save File As";
                    fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    fileDialog.FileName = "";
                    fileDialog.Filter = "CPM Disk Image|*.cpm|All Files|*.*";

                    if (fileDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        fileName = fileDialog.FileName;

                        byte[] bytes = diskImage.bytes;

                        // Save binary file
                        File.WriteAllBytes(fileDialog.FileName, bytes);

                        MessageBox.Show("File saved as '" + fileName + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } else
                {
                    byte[] bytes = diskImage.bytes;

                    // Save binary file
                    File.WriteAllBytes(fileName, bytes);

                    MessageBox.Show("File saved as '" + fileName + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Save image file as
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAs_Click(object sender, EventArgs e)
        {
            if (lbImages.SelectedItem != null)
            {
                DiskImage diskImage = (DiskImage)lbImages.SelectedItem;

                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "Save File As";
                fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fileDialog.FileName = "";
                fileDialog.Filter = "CPM Disk Image|*.cpm|All Files|*.*";

                if (fileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    fileName = fileDialog.FileName;

                    byte[] bytes = diskImage.bytes;

                    // Save binary file
                    File.WriteAllBytes(fileDialog.FileName, bytes);

                    // Update name in list
                    diskImage.file_name = fileName;
                    lbImages.Items[lbImages.SelectedIndex] = diskImage;

                    MessageBox.Show("File saved as '" + fileName + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Save all files contained in the selected image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFiles_Click(object sender, EventArgs e)
        {
            if (lbImages.SelectedItem != null)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderDialog.SelectedPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                folderDialog.Description = "Select the directory for saving the binary files.";

                if (folderDialog.ShowDialog() != DialogResult.Cancel)
                {
                    DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
                    string[] temp1 = diskImage.file_name.Split('\\');
                    string[] temp2 = temp1[temp1.Length - 1].Split('.');
                    string imageFile = temp2[0];    

                    string folder = folderDialog.SelectedPath;
                    Directory.CreateDirectory(folder + "\\" + imageFile + "\\");

                    // If boot disk then save binary file
                    if (diskImage.boot)
                    {
                        byte[] binary = new byte[diskImage.boot_index];
                        for (int i=0; i < diskImage.boot_index; i++)
                        {
                            binary[i] = diskImage.bytes[i];
                        }

                        File.WriteAllBytes(folder + "\\" + imageFile + "\\boot.bin", binary);
                    }

                    foreach (DataGridViewRow row in dgvFiles.Rows)
                    {
                        string fileName = row.Cells["file"].Value.ToString().Trim();
                        string fileType = row.Cells["type"].Value.ToString().Trim();
                        int index = Convert.ToInt32(row.Cells["index"].Value);

                        byte[] data = diskImage.GetFileData(index);

                        if (data != null)
                        {
                            try
                            { 
                                File.WriteAllBytes(folder + "\\" + imageFile + "\\" + fileName + "." + fileType, data);
                            } catch (Exception exception)
                            {
                                MessageBox.Show("Can't save files '" + fileName + "." + fileType + "': " + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    MessageBox.Show("Files saved in '" + folder + "\\" + imageFile + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Insert files in an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Insert_Click(object sender, EventArgs e)
        {
            if (lbImages.SelectedItem != null)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "Select File(s)";
                fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fileDialog.FileName = "";
                fileDialog.Multiselect = true;
                fileDialog.Filter = "All Files|*.*";

                if (fileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    // Get diskimage where to put this file on
                    DiskImage diskImage = (DiskImage)lbImages.SelectedItem;

                    foreach (string filePath in fileDialog.FileNames)
                    {
                        InsertFile(diskImage, filePath);
                    }

                    // Show files in this image
                    ShowImageFiles((DiskImage)lbImages.SelectedItem);
                }
            }
        }

        /// <summary>
        /// Save container of images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveContainer_Click(object sender, EventArgs e)
        {
            if (lbImages.Items.Count > 0)
            {
                if (fileNameContainer == "")
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Title = "Save File As";
                    fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    fileDialog.FileName = "";
                    fileDialog.Filter = "IMG Disk Image Container|*.img|All Files|*.*";

                    if (fileDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        fileNameContainer = fileDialog.FileName;

                        int size = 0;
                        foreach (DiskImage diskImage in lbImages.Items)
                        {
                            size += diskImage.bytes.Length;
                        }

                        byte[] bytes = new byte[size];
                        int index = 0;
                        foreach (DiskImage diskImage in lbImages.Items)
                        {
                            for (int i=0; i<diskImage.bytes.Length; i++)
                            {
                                bytes[index++] = diskImage.bytes[i];
                            }
                        }

                        // Save binary file
                        File.WriteAllBytes(fileNameContainer, bytes);

                        MessageBox.Show("File saved as '" + fileNameContainer + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } else
                {
                    int size = 0;
                    foreach (DiskImage diskImage in lbImages.Items)
                    {
                        size += diskImage.bytes.Length;
                    }

                    byte[] bytes = new byte[size];
                    int index = 0;
                    foreach (DiskImage diskImage in lbImages.Items)
                    {
                        for (int i = 0; i < diskImage.bytes.Length; i++)
                        {
                            bytes[index++] = diskImage.bytes[i];
                        }
                    }

                    // Save binary file
                    File.WriteAllBytes(fileNameContainer, bytes);

                    MessageBox.Show("File saved as '" + fileNameContainer + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Save container of images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveContainerAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save File As";
            fileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            fileDialog.FileName = "";
            fileDialog.Filter = "IMG Disk Image Container|*.img|All Files|*.*";

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                fileNameContainer = fileDialog.FileName;

                int size = 0;
                foreach (DiskImage diskImage in lbImages.Items)
                {
                    size += diskImage.bytes.Length;
                }

                byte[] bytes = new byte[size];
                int index = 0;
                foreach (DiskImage diskImage in lbImages.Items)
                {
                    for (int i = 0; i < diskImage.bytes.Length; i++)
                    {
                        bytes[index++] = diskImage.bytes[i];
                    }
                }

                // Save binary file
                File.WriteAllBytes(fileNameContainer, bytes);

                MessageBox.Show("File saved as '" + fileNameContainer + "'", "SAVED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Read raw image from a remote drive (usually CF card)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readRAWImageFromCF_Click(object sender, EventArgs e)
        {
            // get all removable drives 
            Drives drives = new Drives();
            Dictionary<Drive, string> removableDrives;

            try
            {
                removableDrives = drives.GetDrives("Removable Media");
            } catch (Exception ex)
            {
                MessageBox.Show("Can't read removable media:\r\n" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (removableDrives.Count == 0)
            {
                MessageBox.Show("No removable drives found", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create form for display of results                
            Form readForm = new Form();
            readForm.Name = "FormRead";
            readForm.Text = "Choose Drive/Size";
            readForm.Icon = Properties.Resources.icon;
            readForm.Size = new Size(300, 200);
            readForm.MinimumSize = new Size(300, 200);
            readForm.MaximumSize = new Size(300, 200);
            readForm.MaximizeBox = false;
            readForm.MinimizeBox = false;
            readForm.StartPosition = FormStartPosition.CenterScreen;

            // Create info label
            Label label = new Label();
            label.Size = new Size(260, 28);
            label.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label.Text = "Choose the drive and size to read:";
            label.Location = new Point(10, 10);

            // Add combobox for device to read
            ComboBox comboBoxDrive = new ComboBox();
            comboBoxDrive.Size = new Size(260, 22);
            comboBoxDrive.Location = new Point(10, 40);

            // Set combobox datasource to the dictionary 
            comboBoxDrive.DataSource = new BindingSource(removableDrives, null);
            comboBoxDrive.DisplayMember = "Value";
            comboBoxDrive.ValueMember = "Key";

            // Add combobox for size to read
            ComboBox comboBoxSize = new ComboBox();
            comboBoxSize.Size = new Size(60, 22);
            comboBoxSize.Location = new Point(10, 80);
            comboBoxSize.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSize.Items.AddRange(new object[] {
                "64MB",
                "128MB",
                "FULL"
            });
            comboBoxSize.SelectedIndex = 1;

            // Create button for OK
            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(200, 120);
            btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOk.Visible = true;
            btnOk.DialogResult = DialogResult.OK;

            // Create button for Cancel
            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(10, 120);
            btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            btnCancel.Visible = true;
            btnCancel.DialogResult = DialogResult.Cancel;

            readForm.Controls.Add(label);
            readForm.Controls.Add(comboBoxDrive);
            readForm.Controls.Add(comboBoxSize);
            readForm.Controls.Add(btnOk);
            readForm.Controls.Add(btnCancel);

            // Show form
            DialogResult dialogResult = readForm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Drive drive = ((KeyValuePair<Drive, string>)comboBoxDrive.SelectedItem).Key;

                // Get disk size
                Int64 totalSizeDisk = drive.diskGeometry.Cylinders * drive.diskGeometry.TracksPerCylinder * drive.diskGeometry.SectorsPerTrack * drive.diskGeometry.BytesPerSector;

                Int64 numBytes = 0;
                switch (comboBoxSize.SelectedIndex)
                {
                    case 0:
                        numBytes = 64 * 1024 * 1024;
                        break;

                    case 1:
                        numBytes = 128 * 1024 * 1024;
                        break;

                    case 2:
                        numBytes = totalSizeDisk;
                        break;

                    default:
                        numBytes = totalSizeDisk;
                        break;
                }

                if (numBytes > 128 * 1024 * 1024)
                {
                    MessageBox.Show("Can't read full disk, it is larger then 128MB.\r\nReading first 128MB", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numBytes = 128 * 1024 * 1024;
                }

                if (numBytes > totalSizeDisk)
                {
                    MessageBox.Show("This disk is actual smaller then 128MB.\r\nReading full disk", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numBytes = totalSizeDisk;
                }

                Cursor.Current = Cursors.WaitCursor;

                // Read image file into byte array
                byte[] bytes = drive.ReadBytes(0, (Int32)numBytes);

                // Assume a container of 8MB disk images (probably a smaller image at the end for drive 'P')
                lbImages.Items.Clear();
                btnBoot.Visible = false;

                fileNameContainer = fileName;
                string folderName = "";
                string[] temp = fileName.Split('\\');
                for (int i = 0; i < temp.Length - 1; i++) folderName += temp[i] + "\\";

                int parts = bytes.Length / 0x00800000;
                if (bytes.Length % 0x00800000 != 0) parts++;
                for (int part = 0; part < parts; part++)
                {
                    int bytes_left = bytes.Length - (part * 0x00800000);
                    if (bytes_left > 0x00800000) bytes_left = 0x00800000;

                    byte[] bytes_image = new byte[bytes_left];
                    Array.Copy(bytes, part * 0x00800000, bytes_image, 0, bytes_left);

                    // Create new diskimage object
                    int max_dir = Convert.ToInt32(cbDirEntries.Text);
                    byte volume = (byte)('A' + lbImages.Items.Count);

                    DiskImage diskImage = new DiskImage(folderName + Convert.ToChar(volume).ToString() + ".CPM", Convert.ToChar(volume).ToString(), bytes_image, max_dir);

                    // Add the new object (image) to the listbox
                    lbImages.Items.Add(diskImage);
                }

                // Close filehandle/stream
                drive.Close();

                Cursor.Current = Cursors.Arrow;
                MessageBox.Show("Done", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Save raw image to a remote drive (usually CF card)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveRAWToCF_Click(object sender, EventArgs e)
        {
            if (lbImages.Items.Count > 0)
            {
                // get all removable drives 
                Drives drives = new Drives();
                Dictionary<Drive, string> removableDrives;

                try
                {
                    removableDrives = drives.GetDrives("Removable Media");
                } catch (Exception ex)
                {
                    MessageBox.Show("Can't read removable media:\r\n" + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (removableDrives.Count == 0)
                {
                    MessageBox.Show("No removable drives found", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create form for display of results                
                Form driveForm = new Form();
                driveForm.Name = "FormDrive";
                driveForm.Text = "Choose Device";
                driveForm.Icon = Properties.Resources.icon;
                driveForm.Size = new Size(300, 200);
                driveForm.MinimumSize = new Size(300, 200);
                driveForm.MaximumSize = new Size(300, 200);
                driveForm.MaximizeBox = false;
                driveForm.MinimizeBox = false;
                driveForm.StartPosition = FormStartPosition.CenterScreen;

                // Create warning label
                Label label = new Label();
                label.Size = new Size(260, 60);
                label.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
                label.Text = "WARNING:\r\nALL DATA ON THIS DRIVE WILL BE DELETED !";
                label.Location = new Point(10, 40);

                // Create button for OK
                Button btnOk = new Button();
                btnOk.Text = "Write";
                btnOk.Location = new Point(200, 120);
                btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
                btnOk.Visible = true;
                btnOk.DialogResult = DialogResult.OK;

                // Create button for Cancel
                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Location = new Point(10, 120);
                btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                btnCancel.Visible = true;
                btnCancel.DialogResult = DialogResult.Cancel;

                // Add controls to form
                ComboBox comboBox = new ComboBox();
                comboBox.Size = new Size(260, 22);
                comboBox.Location = new Point(10, 10);

                // Set combobox datasource to the dictionary 
                comboBox.DataSource = new BindingSource(removableDrives, null);
                comboBox.DisplayMember = "Value";
                comboBox.ValueMember = "Key";

                driveForm.Controls.Add(comboBox);
                driveForm.Controls.Add(label);
                driveForm.Controls.Add(btnOk);
                driveForm.Controls.Add(btnCancel);

                // Show form
                DialogResult dialogResult = driveForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (comboBox.SelectedItem == null)
                    {
                        MessageBox.Show("No disk selected", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Drive drive = ((KeyValuePair<Drive, string>)comboBox.SelectedItem).Key;

                    // Check size if it fits
                    Int64 totalSizeImages = 0;
                    Int64 totalSizeDisk = drive.diskGeometry.Cylinders * drive.diskGeometry.TracksPerCylinder * drive.diskGeometry.SectorsPerTrack * drive.diskGeometry.BytesPerSector;
                    foreach (DiskImage diskImage in lbImages.Items)
                    {
                        totalSizeImages += diskImage.bytes.Length;
                    }

                    if (totalSizeImages > totalSizeDisk)
                    {
                        MessageBox.Show("Container is to large for selected disk:\r\n" + totalSizeImages + " Bytes (Container)\r\n" + totalSizeDisk + " Bytes (Disk)", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int offset = 0;
                    Cursor.Current = Cursors.WaitCursor;

                    int index = 0;
                    FormProgress progress = new FormProgress(index, lbImages.Items.Count);
                    progress.Show();

                    try
                    {
                        foreach (DiskImage diskImage in lbImages.Items)
                        {
                            drive.WriteBytes(diskImage.bytes, offset, diskImage.bytes.Length);
                            offset += diskImage.bytes.Length;
                            progress.SetValue(index++, lbImages.Items.Count);
                        }
                    } catch (Exception ex)
                    {
                        Cursor.Current = Cursors.Arrow;
                        MessageBox.Show("Error writing to disk: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Close filehandle/stream
                    drive.Close();

                    Cursor.Current = Cursors.Arrow;
                    progress.Close();

                    MessageBox.Show("Done", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Close program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Show boot sector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBoot_Click(object sender, EventArgs e)
        {
            // Create form for display of results                
            Form bootForm = new Form();
            bootForm.Name = "FormBoot";
            bootForm.Text = "Boot 'Sector'";
            bootForm.Icon = Properties.Resources.icon;
            bootForm.Size = new Size(680, 600);
            bootForm.MinimumSize = new Size(680, 600);
            bootForm.MaximumSize = new Size(680, 600);
            bootForm.MaximizeBox = false;
            bootForm.MinimizeBox = false;
            bootForm.StartPosition = FormStartPosition.CenterScreen;

            // Create button for closing (dialog)form
            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(584, 530);
            btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOk.Visible = true;
            btnOk.Click += new EventHandler((object o, EventArgs a) =>
            {
                bootForm.Close();
            });

            Font font = new Font(FontFamily.GenericMonospace, 10.25F);

            // Compose 
            DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
            string boot = "";
            string ascii = "";
            for (int address = 0; address < DiskImage.BOOTSIZE; address++)
            {
                if (address % 16 == 0)
                {
                    if (address != 0) boot += "  " + ascii + "\r\n";
                    ascii = "";
                    boot += address.ToString("X").PadLeft(4, '0') + ":   ";
                }

                boot += diskImage.bytes[address].ToString("X").PadLeft(2, '0') + " ";
                if ((diskImage.bytes[address] < 128) && (diskImage.bytes[address] >= 32)) ascii += Convert.ToChar(diskImage.bytes[address]); else ascii += ".";
            }

            // Add controls to form
            TextBox textBox = new TextBox();
            textBox.Multiline = true;
            textBox.WordWrap = false;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.ReadOnly = true;
            textBox.BackColor = Color.LightYellow;
            textBox.Size = new Size(648, 510);
            textBox.Text = boot;
            textBox.Font = font;
            textBox.BorderStyle = BorderStyle.None;
            textBox.Location = new Point(10, 10);
            textBox.Select(0, 0);

            bootForm.Controls.Add(textBox);
            bootForm.Controls.Add(btnOk);

            // Show form
            bootForm.Show();
        }

        /// <summary>
        /// Image file selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show files in this image
            if (lbImages.SelectedItem != null)
            {
                ShowImageFiles((DiskImage)lbImages.SelectedItem);
            }
        }
        
        /// <summary>
        /// Delete file from image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dgvFiles.CurrentCell != null)
                {
                    DiskImage diskImage = (DiskImage)lbImages.SelectedItem;
                    int index = Convert.ToInt32(dgvFiles.Rows[dgvFiles.CurrentCell.RowIndex].Cells["index"].Value);
                    diskImage.DeleteFile(index);
                    ShowImageFiles(diskImage);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Show files in this image
        /// </summary>
        /// <param name="diskImage"></param>
        private void ShowImageFiles(DiskImage diskImage)
        {
            dgvFiles.Rows.Clear();
            dgvFiles.Columns.Clear();

            if (diskImage == null) return;

            // Show size
            lblSize.Text = "Size: " + (diskImage.bytes.Length / 0x100000).ToString() + " MB";

            // Show boot (if assigned)
            btnBoot.Visible = false;
            if (diskImage.boot) btnBoot.Visible = true;

            // Create font for header text
            Font font = new Font("Tahoma", 9.75F, FontStyle.Bold);

            // Fill datagridview with info
            dgvFiles.Columns.Add("index", "Index");
            dgvFiles.Columns.Add("number", "User Number");
            dgvFiles.Columns.Add("file", "File");
            dgvFiles.Columns.Add("type", "Type");
            dgvFiles.Columns.Add("extend_high", "Extend Counter High");
            dgvFiles.Columns.Add("extend_low", "Extend Counter Low");
            dgvFiles.Columns.Add("count", "Record Count");
            dgvFiles.Columns["count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvFiles.Columns.Add("size", "Extent Size");
            dgvFiles.Columns["size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvFiles.Columns.Add("block", "Block Pointers");

            foreach (DataGridViewColumn column in dgvFiles.Columns)
            {
                column.HeaderCell.Style.Font = font;
            }

            for (int i = 0; i < diskImage.max_dir; i++)
            {
                // If user number is not E5 then file entry not empty
                if (diskImage.files[i].user_number != 0xE5)
                {
                    dgvFiles.Rows.Add();
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["index"].Value = i;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["number"].Value = diskImage.files[i].user_number;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["file"].Value = diskImage.files[i].file_name.Trim();
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["type"].Value = diskImage.files[i].file_type.Trim();
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["extend_high"].Value = diskImage.files[i].extend_counter_high;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["extend_low"].Value = diskImage.files[i].extend_counter_low;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["count"].Value = diskImage.files[i].record_count;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["size"].Value = diskImage.files[i].extent_size;
                    dgvFiles.Rows[dgvFiles.Rows.Count - 1].Cells["block"].Value = (diskImage.files[i].block_pointers[ 0] + diskImage.files[i].block_pointers[ 1] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 2] + diskImage.files[i].block_pointers[ 3] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 4] + diskImage.files[i].block_pointers[ 5] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 6] + diskImage.files[i].block_pointers[ 7] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[ 8] + diskImage.files[i].block_pointers[ 9] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[10] + diskImage.files[i].block_pointers[11] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[12] + diskImage.files[i].block_pointers[13] * 256).ToString() + ", " +
                                                                                  (diskImage.files[i].block_pointers[14] + diskImage.files[i].block_pointers[15] * 256).ToString();
                }
            }
        }

        /// <summary>
        /// Insert a file in a diskimage
        /// </summary>
        /// <param name="diskImage"></param>
        /// <param name="filePath"></param>
        public void InsertFile(DiskImage diskImage, string filePath)
        {
            string[] temp = filePath.Split('\\');
            string[] temp_name = temp[temp.Length - 1].Split('.');
            string name = temp_name[0];
            string type = "";
            if (temp_name.Length > 1) type = temp_name[1];

            // If hex file on disk A and this is a boot disk, then convert to binary and insert
            if (diskImage.boot && (type.ToLower() == "hex"))
            {
                string hex = File.ReadAllText(filePath);
                byte[] binary = HexToByteArray(hex);
                if (binary == null)
                {
                    MessageBox.Show("Error in this hex file", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert into boot 'sector'
                diskImage.InsertBoot(binary);
                MessageBox.Show("File inserted, free boot 'sector' space: " + (DiskImage.BOOTSIZE - diskImage.boot_index).ToString(), "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If bin file on disk A and this is a boot disk, then insert 
            if (diskImage.boot && (type.ToLower() == "bin"))
            {
                byte[] binary = File.ReadAllBytes(filePath);
                if (binary == null)
                {
                    MessageBox.Show("Error in this bin file", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert into boot 'sector'
                diskImage.InsertBoot(binary);
                MessageBox.Show("File inserted, free boot 'sector' space: " + (DiskImage.BOOTSIZE - diskImage.boot_index).ToString(), "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the number of files is lower then max
            if (diskImage.num_files >= diskImage.max_dir)
            {
                MessageBox.Show("Too many files on this disk", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check Folder/File names for duplicates
            bool found = false;
            foreach (DiskImage.FILE file in diskImage.files)
            {
                if (file.user_number != 0xE5)
                {
                    if ((file.file_name.Trim() == name.Trim()) && (file.file_type.Trim() == type.Trim())) found = true;
                }
            }

            if (found)
            {
                MessageBox.Show("Already a file with the same name/type present: " + name + "." + type + "\r\nChanging user number for this file", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Read image file into byte array
            byte[] bytes = File.ReadAllBytes(filePath);

            // Insert
            diskImage.InsertFile(name, type, bytes);
        }

        /// <summary>
        /// Convert hex file to binary
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public byte[] HexToByteArray(string hex)
        {
            List<byte> bytes = new List<byte>();

            hex = hex.Replace('\r', '\n');
            hex = hex.Replace("\n\n", "\n");

            // Divide in lines
            string[] lines = hex.Split('\n');

            foreach (string line in lines)
            {
                // Skip empty lines (needs colon, number of entries, start address and code)
                if (line.Length > 8)
                {
                    // Skip all before and at colon
                    int index = line.IndexOf(':');
                    if (index < 0) index = 0;
                    if (index >= line.Length - 1)
                    {
                        return null;
                    }

                    string hexString = line.Substring(index + 1);

                    // Check validity of the line    
                    if (hexString.Length % 2 != 0)
                    {
                        return null;
                    }

                    // Check for end-of-file record
                    if (hexString == "00000001FF")
                    {
                        return bytes.ToArray();
                    }

                    for (index = 0; index < hexString.Length / 2; index++)
                    {
                        string byteValue = hexString.Substring(index * 2, 2);

                        // Skip number, address, code at the start and the checksum at the end
                        if ((index > 3) && (index < hexString.Length / 2 - 1))
                        {
                            bytes.Add(byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                        }
                    }
                }
            }

            return bytes.ToArray();
        }

        #endregion
    }
}
