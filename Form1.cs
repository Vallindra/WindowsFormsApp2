using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap DownscaleImage(Bitmap originalImage)
        {
            double scalingFactor = double.Parse(textBox1.Text);
            label1.Text = "Enter scaling percentage";
            if (scalingFactor > 0)
            {

                int newWidth = (int)(originalImage.Width * scalingFactor / 100);
                int newHeight = (int)(originalImage.Height * scalingFactor / 100);

                Bitmap scaledImage = new Bitmap(newWidth, newHeight);

                for (int x = 0; x < newWidth; x++)
                {
                    for (int y = 0; y < newHeight; y++)
                    {
                        int originalX = (int)(x * (originalImage.Width - 1) / (newWidth - 1));
                        int originalY = (int)(y * (originalImage.Height - 1) / (newHeight - 1));

                        Color originalColor = originalImage.GetPixel(originalX, originalY);
                        scaledImage.SetPixel(x, y, originalColor);
                    }
                }
                pictureBox1.Image = scaledImage;
                return scaledImage;
            }
            else { label1.Text = "no"; return originalImage; }
        }
        //**********************************************************************************************************************
        // plan is to divide the image first
        // then work simultaniously on them with a modded version of the simple downscaler
        // and finally put the image slices back together
        public Bitmap TDownscaleImage(Bitmap originalImage)
            
        {
            double scalingFactor = double.Parse(textBox1.Text);
            label1.Text = "Enter scaling percentage";
            if (scalingFactor > 0)
            {

                int newWidth = (int)(originalImage.Width * scalingFactor / 100);
                int newHeight = (int)(originalImage.Height * scalingFactor / 100);

                Bitmap scaledImage = new Bitmap(newWidth, newHeight);

                for (int x = 0; x < newWidth; x++)
                {
                    for (int y = 0; y < newHeight; y++)
                    {
                        int originalX = (int)(x * (originalImage.Width - 1) / (newWidth - 1));
                        int originalY = (int)(y * (originalImage.Height - 1) / (newHeight - 1));

                        Color originalColor = originalImage.GetPixel(originalX, originalY);
                        scaledImage.SetPixel(x, y, originalColor);
                    }
                }
                //pictureBox1.Image = scaledImage;
                return scaledImage;
            }
            else { label1.Text = "no"; }return originalImage; }

        public Bitmap CombineImages(Bitmap[] imageSections)
        {
            if (imageSections.Length < 4 || imageSections.Any(section => section == null))
            {
                return null;
            }

            int totalWidth = imageSections[0].Width + imageSections[1].Width + imageSections[2].Width + imageSections[3].Width;
            int totalHeight = imageSections[0].Height + imageSections[1].Height + imageSections[2].Height + imageSections[3].Height;

            
            Bitmap combinedImage = new Bitmap(totalWidth, totalHeight);

            using (Graphics g = Graphics.FromImage(combinedImage))
            {
                // narejdane
                g.DrawImage(imageSections[0], new Point(0,0));
                g.DrawImage(imageSections[1], new Point(imageSections[0].Width,0));
                g.DrawImage(imageSections[2], new Point(imageSections[1].Width*2,0));
                g.DrawImage(imageSections[3], new Point(imageSections[2].Width*3,0));
            }

            return combinedImage;
        }
        private const int ThreadCount = 4;


        public void DivideImage(Bitmap originalImage)
        {
            
            int sectionWidth = originalImage.Width / ThreadCount;
            int sectionHeight = originalImage.Height / ThreadCount;

            Bitmap[] imageSections = new Bitmap[ThreadCount];
            Thread[] workers = new Thread[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)
            {
                int startX = i * sectionWidth;
                int startY = 0;
                int width = sectionWidth;
                int height = originalImage.Height;

                
                if (i == ThreadCount - 1)
                {
                    width = originalImage.Width - startX;
                }

                
                imageSections[i] = originalImage.Clone(new Rectangle(startX, startY, width, height), originalImage.PixelFormat);

                
                int sectionIndex = i;
                workers[i] = new Thread(() =>
                {
                    imageSections[sectionIndex] = TDownscaleImage(imageSections[sectionIndex]);
                });
                workers[i].Start();
            }

            
            foreach (Thread worker in workers)
            {
                worker.Join();
            }

            
            pictureBox1.Image = CombineImages(imageSections);
        }
//********************************************************************************************************************
        private void label1_Click(object sender, EventArgs e)
        {
            // bruh
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string imgFileName =  openFileDialog1.FileName;
                pictureBox1.Image=Image.FromFile(imgFileName);
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // ....ok
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch time= new Stopwatch();
            time.Start();
            Bitmap image = new Bitmap(pictureBox1.Image);
            DownscaleImage(image);
            time.Stop();
            label2.Text= "Normal exec time: " + time.ElapsedMilliseconds;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            Bitmap image = new Bitmap(pictureBox1.Image);
            DivideImage(image);
            time.Stop();
            label3.Text = "Cool exec time: " + time.ElapsedMilliseconds;
        }
    }
}
