using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QueryCapture
{
    public partial class frmSSASQueryCapture : Form
    {
        QueryTraceProcessor qtProc;
        int queryCnt;
        public frmSSASQueryCapture()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "Tracing";
            queryCnt = 0;
            
            try
            {
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    qtProc = new QueryTraceProcessor();
                    qtProc.Start(txtServer.Text, saveFileDialog1.FileName);
                    qtProc.QueryCaptured += new QueryCapturedDelegate( qtProc_QueryCaptured);
                }
            }
            catch (Exception ex)
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                MessageBox.Show(ex.Message, "The following error occurred", MessageBoxButtons.OK, MessageBoxIcon.Stop);   
            }
            
        }

        void qtProc_QueryCaptured()
        {
            queryCnt++;
            lblQueries.Text = queryCnt == 1? string.Format("{0} query captured", queryCnt) : string.Format("{0} queries captured", queryCnt);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            lblStatus.Text = "Idle";
            qtProc.Stop();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnStop.Enabled)
            {
                qtProc.Stop();
            }
            this.Close();
        }


    }
}