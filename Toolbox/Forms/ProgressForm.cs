using System;
using System.Windows.Forms;
using System.Drawing;

namespace Toolbox.Forms
{
    public class ProgressForm : Form
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        private Label timeLabel;
        private int totalSteps;
        private DateTime startTime;

        public int TotalSteps
        {
            get { return totalSteps; }
            set
            {
                totalSteps = value;
                progressBar.Maximum = value;
            }
        }

        public ProgressForm()
        {
            InitializeComponent();
            startTime = DateTime.Now;
        }

        private void InitializeComponent()
        {
            this.Text = "处理进度";
            this.Size = new Size(400, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 50);
            progressBar.Size = new Size(360, 23);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Minimum = 0;

            statusLabel = new Label();
            statusLabel.Location = new Point(20, 20);
            statusLabel.Size = new Size(360, 20);
            statusLabel.Text = "准备开始...";

            timeLabel = new Label();
            timeLabel.Location = new Point(20, 80);
            timeLabel.Size = new Size(360, 20);
            timeLabel.Text = "已用时间: 0秒";

            this.Controls.Add(progressBar);
            this.Controls.Add(statusLabel);
            this.Controls.Add(timeLabel);
        }

        public void UpdateProgress(int currentStep, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(currentStep, status)));
                return;
            }

            progressBar.Value = currentStep;
            statusLabel.Text = status;
            
            // 更新已用时间
            TimeSpan elapsed = DateTime.Now - startTime;
            timeLabel.Text = $"已用时间: {elapsed.TotalSeconds:F1}秒";
            
            Application.DoEvents();
        }

        public string GetTotalTime()
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            return $"{elapsed.TotalSeconds:F1}秒";
        }
    }
} 