using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cboxResolution.SelectedIndex = 0;
        }
 
        private void btnDowload_Click(object sender, EventArgs e)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
            VideoInfo video = videos.First(p =>
                p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(cboxResolution.Text));
            if (video.RequiresDecryption)
                DownloadUrlResolver.DecryptDownloadUrl(video);
            VideoDownloader downloader = new VideoDownloader(video, Path.Combine(txtPath.Text + "\\", video.Title + video.VideoExtension));
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();
        }

        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                progressBar.Value = (int) e.ProgressPercentage;
                lblPercentage.Text = string.Format("{0:0.##}", e.ProgressPercentage) + @" %";
                progressBar.Update();
            }));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
