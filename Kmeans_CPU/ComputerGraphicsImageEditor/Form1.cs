using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphicsImageEditor
{
    public partial class ImageEditor : Form
    {
        private Image currentImage = null;
        private const int brightnessConstant = 15;
        private float contrastConstant = 75;
        private const double gammaConstant = 0.9;
        public List<List<int>> kernel = new List<List<int>> { new List<int> {1,1,1},
                                                              new List<int> {1,1,1},
                                                              new List<int> {1,1,1}};
        public bool useDefault = false;
        public int divisor = 9;
        public int offset = 0;
        public int anchorXIndex = 1;
        public int anchorYIndex = 1;

        //Variables related to k-means clustering
        List<ColorDataPoint> colorInputData = new List<ColorDataPoint>();
        List<ColorDataPoint> clusters = new List<ColorDataPoint>();
        int numOfClusters = 0;
        public ImageEditor()
        {
            InitializeComponent();
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.WorkerReportsProgress = true;
            List<int> widths = new List<int> { 3, 4, 5, 6, 7, 8, 9 };
            List<int> heights = new List<int> { 3, 4, 5, 6, 7, 8, 9 };
            List<string> convolutionTypes = new List<string> { "blur", "gaussian", "sharpen", "mean removal", "edge detection", "emboss" };
            kernelWidthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            convolutionTemplateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            kernelWidthComboBox.DataSource = widths;
            kernelHeightComboBox.DataSource = heights;
            convolutionTemplateComboBox.DataSource = convolutionTypes;
        }
        public static int clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff" +
                            "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|";
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Please select an image to edit.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                currentImage = Image.FromFile(dialog.FileName);
                pictureBox1.Image = currentImage;
                originalImageBox.Image = currentImage;
                currentImage = createNonIndexedImage(currentImage);
            }
        }
        private void brightnessPlusButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap bmp = (Bitmap)currentImage;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        Color newPixel = Color.FromArgb(clamp(pixel.R + brightnessConstant, 0, 255),
                                                        clamp(pixel.G + brightnessConstant, 0, 255),
                                                        clamp(pixel.B + brightnessConstant, 0, 255));
                        bmp.SetPixel(i, j, newPixel);
                    }
                }
                currentImage = bmp;
                pictureBox1.Image = currentImage;
            }
        }

        private void brightnessMinusButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap bmp = (Bitmap)currentImage;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        Color newPixel = Color.FromArgb(clamp(pixel.R - brightnessConstant, 0, 255),
                                                        clamp(pixel.G - brightnessConstant, 0, 255),
                                                        clamp(pixel.B - brightnessConstant, 0, 255));
                        bmp.SetPixel(i, j, newPixel);
                    }
                }

                currentImage = bmp;
                pictureBox1.Image = currentImage;
            }
        }

        private void contrastButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                contrastConstant = ((contrastConstant + 100) / 100);
                contrastConstant = (contrastConstant * contrastConstant);
                Bitmap bmp = (Bitmap)currentImage;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        Color newPixel = Color.FromArgb(clamp((int)(contrastConstant * (pixel.R - 0.5) + 0.5), 0, 255),
                                                        clamp((int)(contrastConstant * (pixel.G - 0.5) + 0.5), 0, 255),
                                                        clamp((int)(contrastConstant * (pixel.B - 0.5) + 0.5), 0, 255));
                        bmp.SetPixel(i, j, newPixel);
                    }
                }

                currentImage = bmp;
                pictureBox1.Image = currentImage;
            }
        }

        private void negationButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap bmp = (Bitmap)currentImage;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        Color newPixel = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                        bmp.SetPixel(i, j, newPixel);

                    }
                }

                currentImage = bmp;
                pictureBox1.Image = currentImage;
            }
        }

        private void editKernelButton_Click(object sender, EventArgs e)
        {
            KernelForm kf = new KernelForm((int)kernelWidthComboBox.SelectedItem, (int)kernelHeightComboBox.SelectedItem, this);
            kf.Show();
        }

        public string getSelectedConvolution()
        {
            return (string)convolutionTemplateComboBox.SelectedItem;
        }

        private void performConvolutionButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                if (useDefault)
                {
                    divisor = 0;
                    foreach (List<int> l in kernel)
                    {
                        foreach (int i in l)
                        {
                            divisor += i;
                        }
                    }

                }


                divisor = (divisor == 0 ? 1 : divisor);

                Bitmap bmp = (Bitmap)currentImage;
                Bitmap copy = new Bitmap(bmp.Width, bmp.Height);

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        //Now we calculate the new pixel value by looping through the kernel
                        int rSum = 0, gSum = 0, bSum = 0;
                        for (int i = 0; i < kernel.Count; i++)
                        {
                            for (int j = 0; j < kernel[i].Count; j++)
                            {
                                // We overlay the anchor of the kernel matrix on top of the current pixel x,y pixel, and calculate the convolution                         
                                int correspondingXIndex = x - anchorXIndex + j;
                                int correspondingYIndex = y - anchorYIndex + i;
                                int rPixelVal = 0, gPixelVal = 0, bPixelVal = 0;

                                //Now we check if this corresponding pixel location is out of bounds, and handle that case if it is

                                /*The possible cases are the following: 
                                **Corresponding pixel is in the bounds of the image
                                **Both X and Y are out of bounds, so we split this case into 8 subcases, if pixel is closest to one of the 4 corners or one of the 4 edges                             
                                **X is out of bounds but Y is not
                                **Y is out of bounds but X is not
                                */


                                //Pixel is in the bounds of the image
                                if (correspondingXIndex >= 0 && correspondingXIndex <= bmp.Width - 1 && correspondingYIndex >= 0 && correspondingYIndex <= bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(correspondingXIndex, correspondingYIndex);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }

                                //X and Y are out of bounds and closest to top left corner
                                else if (correspondingXIndex < 0 && correspondingYIndex < 0)
                                {
                                    Color tempPixel = bmp.GetPixel(0, 0);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X and Y are out of bounds and closest to top right corner
                                else if (correspondingXIndex > bmp.Width - 1 && correspondingYIndex < 0)
                                {
                                    Color tempPixel = bmp.GetPixel(bmp.Width - 1, 0);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X and Y are out of bounds and closest to bottom left corner
                                else if (correspondingXIndex < 0 && correspondingYIndex > bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(0, bmp.Height - 1);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X and Y are out of bounds and closest to bottom right corner
                                else if (correspondingXIndex > bmp.Width - 1 && correspondingYIndex > bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(bmp.Width - 1, bmp.Height - 1);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X is out of bounds and less than 0 but Y is within the bounds of the image
                                else if (correspondingXIndex < 0 && correspondingYIndex >= 0 && correspondingYIndex <= bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(0, correspondingYIndex);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X is out of bounds and greater than width but Y is within the bounds of the image
                                else if (correspondingXIndex > bmp.Width - 1 && correspondingYIndex >= 0 && correspondingYIndex <= bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(bmp.Width - 1, correspondingYIndex);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //Y is out of bounds and less than 0 but X is within the bounds of the image
                                else if (correspondingYIndex < 0 && correspondingXIndex >= 0 && correspondingXIndex <= bmp.Width - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(correspondingXIndex, 0);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //Y is out of bounds and greater than height but X is within the bounds of the image
                                else if (correspondingYIndex > bmp.Height - 1 && correspondingXIndex >= 0 && correspondingXIndex <= bmp.Width - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(correspondingXIndex, bmp.Height - 1);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }

                                rSum += (rPixelVal *= kernel[i][j]);
                                gSum += (gPixelVal *= kernel[i][j]);
                                bSum += (bPixelVal *= kernel[i][j]);



                            }//kernel columns
                        }//kernel rows


                        rSum = clamp((rSum / divisor) + offset, 0, 255);
                        gSum = clamp((gSum / divisor) + offset, 0, 255);
                        bSum = clamp((bSum / divisor) + offset, 0, 255);
                        copy.SetPixel(x, y, Color.FromArgb(rSum, gSum, bSum));
                    }
                }

                currentImage = copy;
                pictureBox1.Image = currentImage;
            }
        }

        private void gammaButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Stopwatch s = new Stopwatch();
                Bitmap bmp = (Bitmap)currentImage;
                s.Start();
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        Color newPixel = Color.FromArgb(clamp((int)Math.Pow(pixel.R, gammaConstant), 0, 255),
                                                        clamp((int)Math.Pow(pixel.R, gammaConstant), 0, 255),
                                                        clamp((int)Math.Pow(pixel.R, gammaConstant), 0, 255));
                        bmp.SetPixel(i, j, newPixel);
                    }
                }
                s.Stop();
                MessageBox.Show("Exec time: " + s.Elapsed.ToString());
                currentImage = bmp;
                pictureBox1.Image = currentImage;
            }
        }



        private void convolutionTemplateComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (convolutionTemplateComboBox.SelectedItem.ToString())
            {
                case "blur":
                    //Fill 3x3 matrix with all ones
                    kernel.Clear();
                    kernel = new List<List<int>> { new List<int> { 1, 1, 1 }, new List<int> { 1, 1, 1 }, new List<int> { 1, 1, 1 } };
                    divisor = 9;
                    break;
                case "gaussian":
                    kernel.Clear();
                    kernel = new List<List<int>> { new List<int> { 0, 1, 0 }, new List<int> { 1, 4, 1 }, new List<int> { 0, 1, 0 } };
                    divisor = 8;
                    break;
                case "sharpen":
                    kernel.Clear();
                    kernel = new List<List<int>> { new List<int> { 0, -1, 0 }, new List<int> { -1, 5, -1 }, new List<int> { 0, -1, 0 } };
                    divisor = 1;
                    break;
                case "mean removal":
                    kernel.Clear();
                    kernel = new List<List<int>> { new List<int> { -1, -1, -1 }, new List<int> { -1, 9, -1 }, new List<int> { -1, -1, -1 } };
                    divisor = 1;
                    break;
                case "edge detection":
                    //diagonal
                    kernel.Clear();
                    kernel = new List<List<int>> { new List<int> { -1, 0, 0 }, new List<int> { 0, 1, 0 }, new List<int> { 0, 0, 0 } };
                    divisor = 1;
                    break;
                case "emboss":
                    //southeast emboss
                    kernel.Clear();
                    kernel = new List<List<int>> { new List<int> { -1, -1, 0 }, new List<int> { -1, 1, 1 }, new List<int> { 0, 1, 1 } };
                    divisor = 1;
                    break;
                default:
                    break;
            }
            kernelWidthComboBox.SelectedItem = 3;
            kernelHeightComboBox.SelectedItem = 3;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void medianFilterButton_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap bmp = (Bitmap)currentImage;
                Bitmap copy = new Bitmap(bmp.Width, bmp.Height);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color pixel = bmp.GetPixel(x, y);
                        Color[] neighbors = new Color[9];
                        int count = 0;
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                int correspondingXIndex = x - 1 + j;
                                int correspondingYIndex = y - 1 + i;
                                int rPixelVal = 0, gPixelVal = 0, bPixelVal = 0;

                                //Now we check if this corresponding pixel location is out of bounds, and handle that case if it is

                                /*The possible cases are the following: 
                                **Corresponding pixel is in the bounds of the image
                                **Both X and Y are out of bounds, so we split this case into 8 subcases, if pixel is closest to one of the 4 corners or one of the 4 edges                             
                                **X is out of bounds but Y is not
                                **Y is out of bounds but X is not
                                */

                                //Pixel is in the bounds of the image
                                if (correspondingXIndex >= 0 && correspondingXIndex <= bmp.Width - 1 && correspondingYIndex >= 0 && correspondingYIndex <= bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(correspondingXIndex, correspondingYIndex);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }

                                //X and Y are out of bounds and closest to top left corner
                                else if (correspondingXIndex < 0 && correspondingYIndex < 0)
                                {
                                    Color tempPixel = bmp.GetPixel(0, 0);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X and Y are out of bounds and closest to top right corner
                                else if (correspondingXIndex > bmp.Width - 1 && correspondingYIndex < 0)
                                {
                                    Color tempPixel = bmp.GetPixel(bmp.Width - 1, 0);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X and Y are out of bounds and closest to bottom left corner
                                else if (correspondingXIndex < 0 && correspondingYIndex > bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(0, bmp.Height - 1);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X and Y are out of bounds and closest to bottom right corner
                                else if (correspondingXIndex > bmp.Width - 1 && correspondingYIndex > bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(bmp.Width - 1, bmp.Height - 1);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X is out of bounds and less than 0 but Y is within the bounds of the image
                                else if (correspondingXIndex < 0 && correspondingYIndex >= 0 && correspondingYIndex <= bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(0, correspondingYIndex);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //X is out of bounds and greater than width but Y is within the bounds of the image
                                else if (correspondingXIndex > bmp.Width - 1 && correspondingYIndex >= 0 && correspondingYIndex <= bmp.Height - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(bmp.Width - 1, correspondingYIndex);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //Y is out of bounds and less than 0 but X is within the bounds of the image
                                else if (correspondingYIndex < 0 && correspondingXIndex >= 0 && correspondingXIndex <= bmp.Width - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(correspondingXIndex, 0);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }
                                //Y is out of bounds and greater than height but X is within the bounds of the image
                                else if (correspondingYIndex > bmp.Height - 1 && correspondingXIndex >= 0 && correspondingXIndex <= bmp.Width - 1)
                                {
                                    Color tempPixel = bmp.GetPixel(correspondingXIndex, bmp.Height - 1);
                                    rPixelVal = tempPixel.R;
                                    gPixelVal = tempPixel.G;
                                    bPixelVal = tempPixel.B;
                                }

                                neighbors[count] = Color.FromArgb(rPixelVal, gPixelVal, bPixelVal);
                                count++;
                            }
                        }
                        Array.Sort<Color>(neighbors, SortColors);
                        copy.SetPixel(x, y, neighbors[4]);
                    }
                }

                currentImage = copy;
                pictureBox1.Image = currentImage;

            }
        }

        private int SortColors(Color a, Color b)
        {
            if (a.R < b.R)
                return 1;
            else if (a.R > b.R)
                return -1;
            else
            {
                if (a.G < b.G)
                    return 1;
                else if (a.G > b.G)
                    return -1;
                else
                {
                    if (a.B < b.B)
                        return 1;
                    else if (a.B > b.B)
                        return -1;
                }
            }

            return 0;
        }

        private void ditheringButton_Click(object sender, EventArgs e)
        {
            int k = 4;
            double t = getAverageThreshold();
            MessageBox.Show("Threshold: " + t);
            List<int> intensities = new List<int>();

            for (int l = 0; l < k; l++)
            {
                intensities.Add((int)((l * 255) / (k - 1)));
            }

            if (currentImage != null)
            {
                Bitmap bmp = (Bitmap)currentImage;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        //Converting to grayscale
                        Color pixel = bmp.GetPixel(i, j);
                        Color newPixel = Color.FromArgb(clamp((int)((0.3 * pixel.R) + (0.58 * pixel.G) + (0.12 * pixel.B)), 0, 255),
                                                        clamp((int)((0.3 * pixel.R) + (0.58 * pixel.G) + (0.12 * pixel.B)), 0, 255),
                                                        clamp((int)((0.3 * pixel.R) + (0.58 * pixel.G) + (0.12 * pixel.B)), 0, 255));

                        //End of converting to grayscale
                        int jIndex = (int)((newPixel.R * (k - 1)) / 255);

                        if (jIndex == k - 1)
                        {
                            jIndex = k - 2;
                        }
                        int ti = intensities[jIndex] + (int)(t * (intensities[jIndex + 1] - intensities[jIndex]));
                        if (newPixel.R >= ti)
                        {
                            newPixel = Color.FromArgb(intensities[jIndex + 1], intensities[jIndex + 1], intensities[jIndex + 1]);
                        }
                        else
                        {
                            newPixel = Color.FromArgb(intensities[jIndex], intensities[jIndex], intensities[jIndex]);
                        }
                        bmp.SetPixel(i, j, newPixel);
                    }
                }
                currentImage = bmp;
                pictureBox1.Image = currentImage;
            }
        }

        public float getAverageThreshold()
        {
            float counter = 0.0f;
            if (currentImage != null)
            {
                Bitmap bmp = (Bitmap)currentImage;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        //Converting to grayscale
                        Color pixel = bmp.GetPixel(i, j);
                        counter += clamp((int)((0.3 * pixel.R) + (0.58 * pixel.G) + (0.12 * pixel.B)), 0, 255);
                    }
                }
                return (counter / (bmp.Width * bmp.Height)) / 255;
            }
            return 0.5f;
        }

        //Images like .gif has indexed pixel format which throw an exception, this will convert image to non-indexed
        public Bitmap createNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }


        private void kMeansButton_Click(object sender, EventArgs e)
        {
            /*
            if (currentImage != null)
            {
                initializeKMeansData();
                perFormClustering();
            }
            */
            backgroundWorker1.RunWorkerAsync();
        }

        private void initializeKMeansData()
        {
            if (currentImage != null)
            {

                Bitmap bmp = (Bitmap)currentImage;
                this.colorInputData = new List<ColorDataPoint>();
                this.clusters = new List<ColorDataPoint>();
                this.numOfClusters = int.Parse(kMeansTxtBox.Text);
                Random random = new Random(this.numOfClusters);
                for(int x = 0; x < numOfClusters; x++)
                {
                    this.clusters.Add(new ColorDataPoint { Cluster = x });
                }

                //Cluster must have at least 1 data point
                int count = 0;

                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixel = bmp.GetPixel(i, j);
                        if (count < this.numOfClusters)
                        {
                            colorInputData.Add(new ColorDataPoint { ColorValue = Color.FromArgb(pixel.R, pixel.G, pixel.B), Cluster = count });
                            count++;
                        }
                        else
                        {
                            colorInputData.Add(new ColorDataPoint { ColorValue = Color.FromArgb(pixel.R, pixel.G, pixel.B), Cluster = random.Next(this.numOfClusters)});
                        }

                    }
                }
                /*
                for (int k = this.numOfClusters; k < colorInputData.Count; k++)
                {
                    colorInputData[k].Cluster = random.Next(0, this.numOfClusters);
                }
                */
                //MessageBox.Show(colorInputData.Count.ToString());


            }
        }

        private bool emptyClusterExists(List<ColorDataPoint> data)
        {
            var groups = data.GroupBy(x => x.Cluster).OrderBy(x => x.Key).Select(g => new { Count = g.Count() });
            foreach (var item in groups)
            {
                if (item.Count == 0)
                    return true;
            }
            return false;
        }

        private bool updateColorDataPointsMeans()
        {
            if (emptyClusterExists(this.colorInputData))
            {
                return false;
            }

            int rSum = 0, gSum = 0, bSum = 0, clusterIndex = 0;
            var colorDataOrderedByCluster = this.colorInputData.GroupBy(x => x.Cluster).OrderBy(x => x.Key);

            foreach (var group in colorDataOrderedByCluster)
            {
                foreach (var item in group)
                {
                    rSum += item.ColorValue.R;
                    gSum += item.ColorValue.G;
                    bSum += item.ColorValue.B;
                }
                this.clusters[clusterIndex].ColorValue = Color.FromArgb(rSum / group.Count(), gSum / group.Count(), bSum / group.Count());
                clusterIndex++;
                rSum = 0;
                gSum = 0;
                bSum = 0;
            }

            return true;

        }
        public int eucledianDistance(ColorDataPoint p1, ColorDataPoint p2)
        {
            return (int)Math.Sqrt(Math.Pow((p1.ColorValue.R - p2.ColorValue.R), 2)
                                + Math.Pow((p1.ColorValue.G - p2.ColorValue.G), 2)
                                + Math.Pow((p1.ColorValue.B - p2.ColorValue.B), 2));
        }
        //returns true if at least one color joined a new cluster
        //returns false if no cluster changed or a cluster becomes empty
        private bool updateClusterColors()
        {
            bool hasChanged = false;
            int[] distances = new int[this.numOfClusters];
            string loopTime = "", emptyClusterTime = "";
            int size = this.colorInputData.Count;
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < this.numOfClusters; j++)
                {
                    distances[j] = eucledianDistance(colorInputData[i], this.clusters[j]);
                    
                }
               
                int minIndex = IndexOfMin(distances.ToList());
                if (minIndex != colorInputData[i].Cluster)
                {
                    hasChanged = true;
                    colorInputData[i].Cluster = minIndex;
                }
            }

            if (emptyClusterExists(this.colorInputData))
            {
                return false;
            }
            //MessageBox.Show("Loop time: " + loopTime + "\n empty cluster time: " + emptyClusterTime);
            return hasChanged;
        }

        public void writeCurrentImageToFile(string filename)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename))
            {
                
                Bitmap bmp = (Bitmap)currentImage;
                //write width and height
                //file.WriteLine(bmp.Width);
                //file.WriteLine(bmp.Height);
                int count = 1;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for(int j = 0; j < bmp.Height; j++)
                    {
                        string rgbStr = "";
                        Color c = bmp.GetPixel(i, j);
                        rgbStr += (count + " ");
                        rgbStr += (c.R + " ");
                        rgbStr += (c.G + " ");
                        rgbStr += (c.B + '\n');
                        file.WriteLine(rgbStr);
                        count++;
                    }
                }
            }
        }
        private int IndexOfMin(IList<int> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            if (self.Count == 0)
            {
                throw new ArgumentException("List is empty.", "self");
            }

            int min = self[0];
            int minIndex = 0;

            for (int i = 1; i < self.Count; ++i)
            {
                if (self[i] < min)
                {
                    min = self[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }
        public void perFormClustering()
        {
            bool hasChanged = true;
            bool noEmptyClusters = true;
            bool writeToFile = true;
            int maxNumOfIterations = 100;
            int numOfIters = 0;
            Stopwatch s = new Stopwatch();
            s.Start();
            while (noEmptyClusters && hasChanged && numOfIters < maxNumOfIterations)
            {
                
                noEmptyClusters = this.updateColorDataPointsMeans();               
                hasChanged = this.updateClusterColors();
                numOfIters++;
                backgroundWorker1.ReportProgress((int)((numOfIters / maxNumOfIterations) * 100));
            }
            s.Stop();
            MessageBox.Show("K-Means clustering complete. Num of iterations: " + numOfIters + " Num of clusters (k): "
                           + this.numOfClusters + "\n Execution time: " + s.Elapsed.ToString()
                           + "Num of pixels: " + this.colorInputData.Count);
            s.Reset();
            int count = 0;
            Bitmap bmp = (Bitmap)currentImage;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp.SetPixel(i, j, this.clusters[this.colorInputData[count].Cluster].ColorValue);
                    count++;
                }
            }
            //currentImage = bmp;
            pictureBox1.Image = currentImage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (currentImage != null)
            {
                initializeKMeansData();
                perFormClustering();
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        public void loadImageFromFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            string width = lines[0];
            string height = lines[1];
            char[] delimiterChars = { ' ' };
            Bitmap bmp = new Bitmap(int.Parse(width), int.Parse(height));
            int count = 2;
            for(int i = 0; i < bmp.Width; i++)
            {
                for(int j = 0; j < bmp.Height; j++)
                {
                    string[] rgb = lines[count].Split(delimiterChars);
                    Color c = Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                    bmp.SetPixel(i, j, c);
                    count++;
                }
            }
            currentImage = bmp;
            pictureBox1.Image = currentImage;
        }
        private void WriteImageButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "image.txt";
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                writeCurrentImageToFile(savefile.FileName);
            }
        }

        private void ReadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Please select a valid image text file.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                loadImageFromFile(dialog.FileName);
            }
        }
    }
}
