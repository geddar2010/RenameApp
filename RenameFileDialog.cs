using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RenameApp
{
    public partial class RenameFileDialog : Form
    {
        public RenameFileDialog(string fileName, DateTime creationDate)
        {
            InitializeComponent();
            this.tbFileName.Text = fileName;
            this.dtpCreationDate.Value = creationDate;
        }
    }
}
