//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;
//using System.Windows.Forms;
//using System.Drawing.Imaging;
//using System.IO;
/////Algorithms Project
/////Intelligent Scissors
/////

//namespace ImageQuantization
//{
//    /// <summary>
//    /// Holds the pixel color in 3 byte values: red, green and blue
//    /// </summary>
//    public struct RGBPixel
//    {
//        public byte red, green, blue;
//    }
//    public struct RGBPixelD
//    {
//        public double red, green, blue;
//    }
    
  
//    /// <summary>
//    /// Library of static functions that deal with images
//    /// </summary>
//    public class ImageOperations
//    {
//        /// <summary>
//        /// Open an image and load it into 2D array of colors (size: Height x Width)
//        /// </summary>
//        /// <param name="ImagePath">Image file path</param>
//        /// <returns>2D array of colors</returns>
//        public static RGBPixel[,] OpenImage(string ImagePath)
//        {
//            Bitmap original_bm = new Bitmap(ImagePath);
//            int Height = original_bm.Height;
//            int Width = original_bm.Width;

//            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

//            unsafe
//            {
//                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
//                int x, y;
//                int nWidth = 0;
//                bool Format32 = false;
//                bool Format24 = false;
//                bool Format8 = false;

//                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
//                {
//                    Format24 = true;
//                    nWidth = Width * 3;
//                }
//                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
//                {
//                    Format32 = true;
//                    nWidth = Width * 4;
//                }
//                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
//                {
//                    Format8 = true;
//                    nWidth = Width;
//                }
//                int nOffset = bmd.Stride - nWidth;
//                byte* p = (byte*)bmd.Scan0;
//                for (y = 0; y < Height; y++)
//                {
//                    for (x = 0; x < Width; x++)
//                    {
//                        if (Format8)
//                        {
//                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
//                            p++;
//                        }
//                        else
//                        {
//                            Buffer[y, x].red = p[2];
//                            Buffer[y, x].green = p[1];
//                            Buffer[y, x].blue = p[0];
//                            if (Format24) p += 3;
//                            else if (Format32) p += 4;
//                        }
//                    }
//                    p += nOffset;
//                }
//                original_bm.UnlockBits(bmd);
//            }

//            return Buffer;
//        }
        
//        /// <summary>
//        /// Get the height of the image 
//        /// </summary>
//        /// <param name="ImageMatrix">2D array that contains the image</param>
//        /// <returns>Image Height</returns>
//        public static int GetHeight(RGBPixel[,] ImageMatrix)
//        {
//            return ImageMatrix.GetLength(0);
            
//        }

//        /// <summary>
//        /// Get the width of the image 
//        /// </summary>
//        /// <param name="ImageMatrix">2D array that contains the image</param>
//        /// <returns>Image Width</returns>
//        public static int GetWidth(RGBPixel[,] ImageMatrix)
//        {
//            return ImageMatrix.GetLength(1);
//        }

//        /// <summary>
//        /// Display the given image on the given PictureBox object
//        /// </summary>
//        /// <param name="ImageMatrix">2D array that contains the image</param>
//        /// <param name="PicBox">PictureBox object to display the image on it</param>
//        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
//        {
//            // Create Image:
//            //==============
//            int Height = ImageMatrix.GetLength(0);
//            int Width = ImageMatrix.GetLength(1);

//            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

//            unsafe
//            {
//                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
//                int nWidth = 0;
//                nWidth = Width * 3;
//                int nOffset = bmd.Stride - nWidth;
//                byte* p = (byte*)bmd.Scan0;
//                for (int i = 0; i < Height; i++)
//                {
//                    for (int j = 0; j < Width; j++)
//                    {
//                        p[2] = ImageMatrix[i, j].red;
//                        p[1] = ImageMatrix[i, j].green;
//                        p[0] = ImageMatrix[i, j].blue;
//                        p += 3;
//                    }

//                    p += nOffset;
//                }
//                ImageBMP.UnlockBits(bmd);
//            }
//            PicBox.Image = ImageBMP;
//        }


//       /// <summary>
//       /// Apply Gaussian smoothing filter to enhance the edge detection 
//       /// </summary>
//       /// <param name="ImageMatrix">Colored image matrix</param>
//       /// <param name="filterSize">Gaussian mask size</param>
//       /// <param name="sigma">Gaussian sigma</param>
//       /// <returns>smoothed color image</returns>
//        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
//        {
//            int Height = GetHeight(ImageMatrix);
//            int Width = GetWidth(ImageMatrix);

//            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
//            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

           
//            // Create Filter in Spatial Domain:
//            //=================================
//            //make the filter ODD size
//            if (filterSize % 2 == 0) filterSize++;

//            double[] Filter = new double[filterSize];

//            //Compute Filter in Spatial Domain :
//            //==================================
//            double Sum1 = 0;
//            int HalfSize = filterSize / 2;
//            for (int y = -HalfSize; y <= HalfSize; y++)
//            {
//                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
//                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
//                Sum1 += Filter[y + HalfSize];
//            }
//            for (int y = -HalfSize; y <= HalfSize; y++)
//            {
//                Filter[y + HalfSize] /= Sum1;
//            }

//            //Filter Original Image Vertically:
//            //=================================
//            int ii, jj;
//            RGBPixelD Sum;
//            RGBPixel Item1;
//            RGBPixelD Item2;

//            for (int j = 0; j < Width; j++)
//                for (int i = 0; i < Height; i++)
//                {
//                    Sum.red = 0;
//                    Sum.green = 0;
//                    Sum.blue = 0;
//                    for (int y = -HalfSize; y <= HalfSize; y++)
//                    {
//                        ii = i + y;
//                        if (ii >= 0 && ii < Height)
//                        {
//                            Item1 = ImageMatrix[ii, j];
//                            Sum.red += Filter[y + HalfSize] * Item1.red;
//                            Sum.green += Filter[y + HalfSize] * Item1.green;
//                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
//                        }
//                    }
//                    VerFiltered[i, j] = Sum;
//                }

//            //Filter Resulting Image Horizontally:
//            //===================================
//            for (int i = 0; i < Height; i++)
//                for (int j = 0; j < Width; j++)
//                {
//                    Sum.red = 0;
//                    Sum.green = 0;
//                    Sum.blue = 0;
//                    for (int x = -HalfSize; x <= HalfSize; x++)
//                    {
//                        jj = j + x;
//                        if (jj >= 0 && jj < Width)
//                        {
//                            Item2 = VerFiltered[i, jj];
//                            Sum.red += Filter[x + HalfSize] * Item2.red;
//                            Sum.green += Filter[x + HalfSize] * Item2.green;
//                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
//                        }
//                    }
//                    Filtered[i, j].red = (byte)Sum.red;
//                    Filtered[i, j].green = (byte)Sum.green;
//                    Filtered[i, j].blue = (byte)Sum.blue;
//                }

//            return Filtered;
//        }


//    }

//    public class Priority_Queue
//    {
//        public void Min_Heapify(ref List<HuffmanNode> data, int i)
//        {
//            int smallest;
//            int left = 2 * i + 1;
//            int right = 2 * i + 2;
//            if ((left <= data.Count - 1) && (data[left].freq < data[i].freq))
//            {
//                smallest = left;
//            }
//            else
//            {
//                smallest = i;
//            }
//            if ((right <= data.Count - 1) && (data[right].freq < data[smallest].freq))
//            {
//                smallest = right;
//            }
//            if (smallest != i)
//            {
//                long tmp_freq = data[i].freq;
//                data[i].freq = data[smallest].freq;
//                data[smallest].freq = tmp_freq;

//                Byte tmp_value = data[i].value;
//                data[i].value = data[smallest].value;
//                data[smallest].value = tmp_value;

//                HuffmanNode left_n = data[i].Left_Node;
//                data[i].Left_Node = data[smallest].Left_Node;
//                data[smallest].Left_Node = left_n;

//                HuffmanNode right_n = data[i].Right_Node;
//                data[i].Right_Node = data[smallest].Right_Node;
//                data[smallest].Right_Node = right_n;

//                Min_Heapify(ref data, smallest);
//            }

//        }

//        public void Build_Min_Heap(ref List<HuffmanNode> build_data)
//        {
//            double size = build_data.Count / 2;
//            for (int j = (int)Math.Floor(size); j >= 0; j--)
//            {
//                Min_Heapify(ref build_data, j);
//            }

//        }

//        public HuffmanNode Extract_Min(ref List<HuffmanNode> data)
//        {
//            HuffmanNode Min = new HuffmanNode();
//            Min.value = data[0].value;
//            Min.freq = data[0].freq;
//            Min.Left_Node = data[0].Left_Node;
//            Min.Right_Node = data[0].Right_Node;
//            Min.CoLoR_CoDe = data[0].CoLoR_CoDe;


//            data[0].value = data[data.Count - 1].value;
//            data[0].freq = data[data.Count - 1].freq;
//            data[0].Left_Node = data[data.Count - 1].Left_Node;
//            data[0].Right_Node = data[data.Count - 1].Right_Node;
//            data[0].CoLoR_CoDe = data[data.Count - 1].CoLoR_CoDe;

//            data.Remove(data[data.Count - 1]);
//            Min_Heapify(ref data, 0);
//            return Min;
//        }


//        public void Insert_Color(ref List<HuffmanNode> data_insert, HuffmanNode new_color)
//        {
//            data_insert.Add(new_color);
//            double index_to_new_color = data_insert.Count - 1;

//            int parent;
//            //parent is floor length / 2 if index to new color is odd
//            //else parent is floor length / 2 if index is even
//            while (index_to_new_color > 0)
//            {
//                if (index_to_new_color % 2 == 0)
//                {
//                    parent = ((int)Math.Floor(index_to_new_color / 2) - 1);
//                }
//                else
//                {
//                    parent = (int)Math.Floor(index_to_new_color / 2);
//                }
//                if (data_insert[parent].freq > data_insert[(int)index_to_new_color].freq)
//                {
//                    long tmp_freq = data_insert[parent].freq;
//                    data_insert[parent].freq = data_insert[(int)index_to_new_color].freq;
//                    data_insert[(int)index_to_new_color].freq = tmp_freq;

//                    Byte tmp_value = data_insert[parent].value;
//                    data_insert[parent].value = data_insert[(int)index_to_new_color].value;
//                    data_insert[(int)index_to_new_color].value = tmp_value;

//                    HuffmanNode left_n = data_insert[parent].Left_Node;
//                    data_insert[parent].Left_Node = data_insert[(int)index_to_new_color].Left_Node;
//                    data_insert[(int)index_to_new_color].Left_Node = left_n;

//                    HuffmanNode right_n = data_insert[parent].Right_Node;
//                    data_insert[parent].Right_Node = data_insert[(int)index_to_new_color].Right_Node;
//                    data_insert[(int)index_to_new_color].Right_Node = right_n;

//                    index_to_new_color = parent;
//                }
//                else
//                {
//                    break;
//                }

//            }
//        }
//    }


//    public class HuffmanNode
//    {
//        public Byte value;
//        public long freq;
//        public string CoLoR_CoDe { set; get; }
//        public HuffmanNode Right_Node { get; set; }
//        public HuffmanNode Left_Node { get; set; }

//        public HuffmanNode() { }

//        public HuffmanNode(Byte _value, long _freq, string color_code)
//        {
//            this.value = _value;
//            this.freq = _freq;
//            this.CoLoR_CoDe = color_code;
//            Right_Node = null;
//            Left_Node = null;
//        }
//    }

//    public class HuffmanTree
//    {
//        public long TotalBits { set; get; }
//        public HuffmanNode BuildHuffmanTree(ref List<HuffmanNode> Data)
//        {
//            Priority_Queue new_priority = new Priority_Queue();
//            HuffmanNode Parent_node = null;
//            while (Data.Count != 1)
//            {
//                HuffmanNode Right = new_priority.Extract_Min(ref Data);
//                Right.CoLoR_CoDe = "";
//                HuffmanNode Left = new_priority.Extract_Min(ref Data);
//                Left.CoLoR_CoDe = "";

//                Parent_node = new HuffmanNode(1, Right.freq + Left.freq, "$");
//                Parent_node.Left_Node = Left;
//                Parent_node.Right_Node = Right;
//                new_priority.Insert_Color(ref Data, Parent_node);
//            }
//            return Parent_node;
//        }
//        public void PrintHuffmanTree(HuffmanNode Root)
//        {
//            if (Root != null)
//            {
//                PrintHuffmanTree(Root.Left_Node);
//                PrintHuffmanTree(Root.Right_Node);
//            }
//        }

//        public void PrintCodes(HuffmanNode Root, string str, string file_name)
//        {
//            if (Root == null)
//                return;

//            if ((Root.CoLoR_CoDe != "$") && (Root.Left_Node == null) && (Root.Right_Node == null))
//            {
//                FileStream infofile = new FileStream(file_name, FileMode.Append);
//                StreamWriter writeinfo = new StreamWriter(infofile);
//                Root.CoLoR_CoDe = str;
//                TotalBits = str.Length * Root.freq;
//                writeinfo.WriteLine("{0} - {1} - {2} - {3} ", Root.value, Root.freq, Root.CoLoR_CoDe, TotalBits);
//                writeinfo.Close();
//            }
//            PrintCodes(Root.Left_Node, str + "0", file_name);
//            PrintCodes(Root.Right_Node, str + "1", file_name);
//        }
//        public void Compression(ref RGBPixel[,] img , ref double CRBCompression, ref double CRACompression)
//        {
//            string line;
//            string[] tokens = new string[4];
//            int Height = ImageOperations.GetHeight(img);
//            int width = ImageOperations.GetWidth(img);
//            Dictionary<byte, string> R = new Dictionary<byte, string>();
//            Dictionary<byte, string> G = new Dictionary<byte, string>();
//            Dictionary<byte, string> B = new Dictionary<byte, string>();

//            double Total_Bits = 0;

//            /* Red */

//            // Read the file and display it line by line.  
//            System.IO.StreamReader file = new System.IO.StreamReader("Red_Info.txt");

//            //To discard the first two lines
//            file.ReadLine();
//            file.ReadLine();

//            //Loop until we reach eof (end of file)
//            while ((line = file.ReadLine()) != null)
//            {
//                tokens = line.Split(new[] { " - " }, StringSplitOptions.None);
//                R.Add(Convert.ToByte(tokens[0]), tokens[2]);
//                Total_Bits += Convert.ToInt64(tokens[3]);
//            }

//            file.Close();

//            /* Green */

//            // Read the file and display it line by line.  
//            file = new System.IO.StreamReader("Green_Info.txt");

//            //To discard the first two lines
//            file.ReadLine();
//            file.ReadLine();

//            //Loop until we reach eof (end of file)
//            while ((line = file.ReadLine()) != null)
//            {
//                tokens = line.Split(new[] { " - " }, StringSplitOptions.None);
//                G.Add(Convert.ToByte(tokens[0]), tokens[2]);
//                Total_Bits += Convert.ToInt64(tokens[3]);
//            }

//            file.Close();

//            /* Blue */

//            // Read the file and display it line by line.  
//            file = new System.IO.StreamReader("Blue_Info.txt");

//            //To discard the first two lines
//            file.ReadLine();
//            file.ReadLine();

//            //Loop until we reach eof (end of file)
//            while ((line = file.ReadLine()) != null)
//            {
//                tokens = line.Split(new[] { " - " }, StringSplitOptions.None);
//                B.Add(Convert.ToByte(tokens[0]), tokens[2]);
//                Total_Bits += Convert.ToInt64(tokens[3]);
//            }

//            file.Close();

//            CRBCompression = width * Height * 3;
//            CRACompression = ((Total_Bits / 8 ) / CRBCompression) * 100;

//            StringBuilder Red_String = new StringBuilder();
//            StringBuilder Green_String = new StringBuilder();
//            StringBuilder Blue_String = new StringBuilder();

//            for (int h = 0; h < Height; h++)
//                for (int w = 0; w < width; w++)
//                {
//                    Red_String.Append(R[img[h, w].red]);
//                    Green_String.Append(G[img[h, w].green]);
//                    Blue_String.Append(B[img[h, w].blue]);
//                }

//            /* Red */

//            Stream stream = new FileStream("Red.dat", FileMode.OpenOrCreate);
//            BinaryWriter bw = new BinaryWriter(stream);

//            byte[] arr1 = Encoding.ASCII.GetBytes(Red_String.ToString());

//            foreach (var b in arr1)
//            {
//                bw.Write(b);
//            }

//            bw.Flush();
//            bw.Close();

//            /* Green */

//            stream = new FileStream("Green.dat", FileMode.OpenOrCreate);
//            bw = new BinaryWriter(stream);

//            byte[] arr2 = Encoding.ASCII.GetBytes(Green_String.ToString());

//            foreach (var b in arr2)
//            {
//                bw.Write(b);
//            }

//            /* Blue */

//            stream = new FileStream("Blue.dat", FileMode.OpenOrCreate);
//            bw = new BinaryWriter(stream);

//            byte[] arr3 = Encoding.ASCII.GetBytes(Blue_String.ToString());

//            foreach (var b in arr3)
//            {
//                bw.Write(b);
//            }

//            bw.Flush();
//            bw.Close();

//            MessageBox.Show("Total bits = " + (Total_Bits).ToString());
//        }
//    }
//}




using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }


    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);

        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }


    }

    public class Priority_Queue
    {
        public void Min_Heapify(ref List<HuffmanNode> data, int i)
        {
            int smallest;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            if ((left <= data.Count - 1) && (data[left].freq < data[i].freq))
            {
                smallest = left;
            }
            else
            {
                smallest = i;
            }
            if ((right <= data.Count - 1) && (data[right].freq < data[smallest].freq))
            {
                smallest = right;
            }
            if (smallest != i)
            {
                long tmp_freq = data[i].freq;
                data[i].freq = data[smallest].freq;
                data[smallest].freq = tmp_freq;

                Byte tmp_value = data[i].value;
                data[i].value = data[smallest].value;
                data[smallest].value = tmp_value;

                HuffmanNode left_n = data[i].Left_Node;
                data[i].Left_Node = data[smallest].Left_Node;
                data[smallest].Left_Node = left_n;

                HuffmanNode right_n = data[i].Right_Node;
                data[i].Right_Node = data[smallest].Right_Node;
                data[smallest].Right_Node = right_n;

                string color_code = data[i].CoLoR_CoDe;
                data[i].CoLoR_CoDe = data[smallest].CoLoR_CoDe;
                data[smallest].CoLoR_CoDe = color_code;


                Min_Heapify(ref data, smallest);
            }

        }

        public void Build_Min_Heap(ref List<HuffmanNode> build_data)
        {
            double size = build_data.Count / 2;
            for (int j = (int)Math.Floor(size); j >= 0; j--)
            {
                Min_Heapify(ref build_data, j);
            }

        }

        public HuffmanNode Extract_Min(ref List<HuffmanNode> data)
        {
            HuffmanNode Min = new HuffmanNode();
            Min.value = data[0].value;
            Min.freq = data[0].freq;
            Min.Left_Node = data[0].Left_Node;
            Min.Right_Node = data[0].Right_Node;
            Min.CoLoR_CoDe = data[0].CoLoR_CoDe;


            data[0].value = data[data.Count - 1].value;
            data[0].freq = data[data.Count - 1].freq;
            data[0].Left_Node = data[data.Count - 1].Left_Node;
            data[0].Right_Node = data[data.Count - 1].Right_Node;
            data[0].CoLoR_CoDe = data[data.Count - 1].CoLoR_CoDe;

            data.RemoveAt(data.Count - 1);
            Min_Heapify(ref data, 0);
            return Min;
        }


        public void Insert_Color(ref List<HuffmanNode> data_insert, HuffmanNode new_color)
        {
            data_insert.Add(new_color);
            double index_to_new_color = data_insert.Count - 1;

            int parent;
            //parent is floor length / 2 if index to new color is odd
            //else parent is floor length / 2 if index is even
            while (index_to_new_color > 0)
            {
                if (index_to_new_color % 2 == 0)
                {
                    parent = ((int)Math.Floor(index_to_new_color / 2) - 1);
                }
                else
                {
                    parent = (int)Math.Floor(index_to_new_color / 2);
                }
                if (data_insert[parent].freq > data_insert[(int)index_to_new_color].freq)
                {
                    long tmp_freq = data_insert[parent].freq;
                    data_insert[parent].freq = data_insert[(int)index_to_new_color].freq;
                    data_insert[(int)index_to_new_color].freq = tmp_freq;

                    string tmp_colorcode = data_insert[parent].CoLoR_CoDe;
                    data_insert[parent].CoLoR_CoDe = data_insert[(int)index_to_new_color].CoLoR_CoDe;
                    data_insert[(int)index_to_new_color].CoLoR_CoDe = tmp_colorcode;

                    Byte tmp_value = data_insert[parent].value;
                    data_insert[parent].value = data_insert[(int)index_to_new_color].value;
                    data_insert[(int)index_to_new_color].value = tmp_value;

                    HuffmanNode left_n = data_insert[parent].Left_Node;
                    data_insert[parent].Left_Node = data_insert[(int)index_to_new_color].Left_Node;
                    data_insert[(int)index_to_new_color].Left_Node = left_n;

                    HuffmanNode right_n = data_insert[parent].Right_Node;
                    data_insert[parent].Right_Node = data_insert[(int)index_to_new_color].Right_Node;
                    data_insert[(int)index_to_new_color].Right_Node = right_n;



                    index_to_new_color = parent;
                }
                else
                {
                    break;
                }

            }
        }
    }


    public class HuffmanNode
    {
        public Byte value;
        public long freq;
        public string CoLoR_CoDe { set; get; }
        public HuffmanNode Right_Node { get; set; }
        public HuffmanNode Left_Node { get; set; }

        public HuffmanNode() { }

        public HuffmanNode(Byte _value, long _freq, string color_code)
        {
            this.value = _value;
            this.freq = _freq;
            this.CoLoR_CoDe = color_code;
            Right_Node = null;
            Left_Node = null;
        }
    }
    public class HuffmanTree
    {
        public long TotalBits { set; get; }
        public int R_index = 0;
        public int G_index = 0;
        public int B_index = 0;
        public int Red_Store = 0;
        public int Green_Store = 0;
        public int Blue_Store = 0;
        public int Needed_Bits = 0;
        public int Needed_Bits_L = 0;
        public int Immediate_Red = 0;
        public int Immediate_Green = 0;
        public int Immediate_Blue = 0;
        public HuffmanNode BuildHuffmanTree(ref List<HuffmanNode> Data)
        {
            Priority_Queue new_priority = new Priority_Queue();
            HuffmanNode Parent_node = null;
            while (Data.Count != 1)
            {
                HuffmanNode Right = new_priority.Extract_Min(ref Data);
                HuffmanNode Left = new_priority.Extract_Min(ref Data);

                Parent_node = new HuffmanNode(1 , Right.freq + Left.freq, "$");
                Parent_node.Left_Node = Left;
                Parent_node.Right_Node = Right;
                new_priority.Insert_Color(ref Data, Parent_node);
            }
            return Parent_node;
        }
        public void PrintHuffmanTree(HuffmanNode Root)
        {
            if (Root == null)
            {
                FileStream FSS = new FileStream("Total_Info.dat", FileMode.Append);
                BinaryWriter WI = new BinaryWriter(FSS);
                WI.Write('!');
                WI.Close();
                return;
            }


            FileStream FS = new FileStream("Total_Info.dat", FileMode.Append);
            BinaryWriter BWI = new BinaryWriter(FS);   
                BWI.Write(Root.value);
                BWI.Close();
                FS.Close();

                PrintHuffmanTree(Root.Left_Node);
                PrintHuffmanTree(Root.Right_Node);
                
            
        }

        public void PrintCodes(HuffmanNode Root, string str, string file_name)
        {
            if (Root == null)
                return;

            if ((Root.Left_Node == null) && (Root.Right_Node == null))
            {
                FileStream infofile = new FileStream(file_name, FileMode.Append);
                StreamWriter writeinfo = new StreamWriter(infofile);
                Root.CoLoR_CoDe = str;
                TotalBits = str.Length * Root.freq;
                writeinfo.WriteLine("{0} - {1} - {2} - {3} ", Root.value, Root.freq, Root.CoLoR_CoDe, TotalBits);
                writeinfo.Close();
                infofile.Close();
            }
            PrintCodes(Root.Left_Node, str + "0", file_name);
            PrintCodes(Root.Right_Node, str + "1", file_name);
        }

        public void Compression(ref RGBPixel[,] img, ref double CRBCompression, ref double CRACompression, ref int red_bytes_in_file, ref int green_bytes_in_file, ref int blue_bytes_in_file)
        {
            string line;
            string[] tokens = new string[4];
            int Height = ImageOperations.GetHeight(img);
            int width = ImageOperations.GetWidth(img);
            
            
            Dictionary<byte, KeyValuePair<int , int>> R = new Dictionary<byte, KeyValuePair<int , int>>();
            Dictionary<byte, KeyValuePair<int , int>> G = new Dictionary<byte, KeyValuePair<int , int>>();
            Dictionary<byte, KeyValuePair<int , int>> B = new Dictionary<byte, KeyValuePair<int , int>>();
            KeyValuePair<int, int> Red_pair;
            KeyValuePair<int, int> Green_pair;
            KeyValuePair<int, int> Blue_pair;
            double Total_Bits = 0;

            
            /* Red */

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader("Red_Info.txt");

            //To discard the first two lines
            file.ReadLine();
            file.ReadLine();

            //Loop until we reach eof (end of file)
            
            while ((line = file.ReadLine()) != null)
            {
                tokens = line.Split(new[] { " - " }, StringSplitOptions.None);
                Red_pair = new KeyValuePair<int, int>(Convert.ToInt32(tokens[2] , 2) , tokens[2].Length);
                R.Add(Byte.Parse(tokens[0]) , Red_pair);
                Total_Bits += Convert.ToInt64(tokens[3]);
            }
            
            file.Close();

            /* Green */

            // Read the file and display it line by line.  
            file = new System.IO.StreamReader("Green_Info.txt");

            //To discard the first two lines
            file.ReadLine();
            file.ReadLine();

            //Loop until we reach eof (end of file)
            while ((line = file.ReadLine()) != null)
            {
                tokens = line.Split(new[] { " - " }, StringSplitOptions.None);
                Green_pair = new KeyValuePair<int,int>(Convert.ToInt32(tokens[2] , 2 ) , tokens[2].Length);
                G.Add(Convert.ToByte(tokens[0]), Green_pair);
                Total_Bits += Convert.ToInt64(tokens[3]);
            }
            

            file.Close();

            /* Blue */

            // Read the file and display it line by line.  
            file = new System.IO.StreamReader("Blue_Info.txt");

            //To discard the first two lines
            file.ReadLine();
            file.ReadLine();

            //Loop until we reach eof (end of file)
            while ((line = file.ReadLine()) != null)
            {
                tokens = line.Split(new[] { " - " }, StringSplitOptions.None);
                Blue_pair = new KeyValuePair<int, int>(Convert.ToInt32(tokens[2], 2), tokens[2].Length);
                B.Add(Convert.ToByte(tokens[0]), Blue_pair);
                Total_Bits += Convert.ToInt64(tokens[3]);
            }

            file.Close();
            
            CRBCompression = ImageOperations.GetWidth(img) * ImageOperations.GetHeight(img) * 3;
            CRACompression = ((Total_Bits / 8) / CRBCompression) * 100;
            StringBuilder RSB = new StringBuilder();
            

            StringBuilder GSB = new StringBuilder();
            

            StringBuilder BSB = new StringBuilder();
            


            FileStream fsTruncate = new FileStream("Red.dat", FileMode.Create);
            FileStream fs1Truncate = new FileStream("Blue.dat", FileMode.Create);
            FileStream fs2Truncate = new FileStream("Green.dat", FileMode.Create);
            fsTruncate.Close();
            fs1Truncate.Close();
            fs2Truncate.Close();

            FileStream fs = new FileStream("Red.dat", FileMode.Append);
            FileStream fs1 = new FileStream("Blue.dat", FileMode.Append);
            FileStream fs2 = new FileStream("Green.dat", FileMode.Append);
            BinaryWriter rwriter = new BinaryWriter(fs);
            BinaryWriter gwriter = new BinaryWriter(fs2);
            BinaryWriter bwriter = new BinaryWriter(fs1);

        
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    // get red , green and blue code and its value it take O(1)
                    Red_pair = R[img[h, w].red];
                    Green_pair = G[img[h, w].green];
                    Blue_pair = B[img[h, w].blue];

                    //Red Part
                    if (R_index + Red_pair.Value < 32) 
                    {
                        Red_Store <<= Red_pair.Value; //O(1)
                        Red_Store |= Red_pair.Key; // O(1)
                        R_index += Red_pair.Value; // O(1)
                    }
                    else if (R_index + Red_pair.Value == 32) 
                    {
                        Red_Store <<= Red_pair.Value; //O(1)
                        Red_Store |= Red_pair.Key; //O(1)
                        R_index = 0;
                        rwriter.Write(Red_Store);//Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
                        red_bytes_in_file += 1; // number of eight bytes stored to red
                        Red_Store = 0;
                    }
                    else if (R_index + Red_pair.Value > 32)  //10010100    29   10111111  11100000  
                    {
                        // if R_index = 61 , and Red_Pair.value = 4 , so this value will exceed long size (65) so we need only 3 bits
                        // which we calculate it in needed_bits then and number of ones to get this bits
                        // meaning if we need 3 bits so we should generate number with three ones 111 = 7 and if we need 4 bits should generate 1111 = 15
                        // so to calculte this use this equation (2^needed_bits - 1) 
                        Immediate_Red = Red_pair.Key;
                        Needed_Bits = 32 - R_index;//3
                        Needed_Bits = (2 << (Needed_Bits - 1)) - 1;//7 // apply eq 2^bits - 1 to get the value of n bits and take O(1)
                        Needed_Bits_L = Needed_Bits;
                        Needed_Bits_L <<= (Red_pair.Value + R_index - 32 ); // make shift to get the last n bits ,a and make shift with (R_index - Needed_Bits)
                        // Because if i have for example 62 bits in R_index and coming bits is 4 so needed bits
                        // to reach long size is 2 bits so we should extract last 2 bits mean that i should 
                        // create this number 110000....0000 number of zeros is 60  , and 60 come from 62 - 2 
                        // take O(1)

                        Immediate_Red &= Needed_Bits_L; // O(1) // now i have the last n bits that i need  but my value followed by zeros :(
                        Immediate_Red >>= (Red_pair.Value  + R_index - 32); // O(1) // so make shift right to it to get the exact value
                        Red_Store <<= (32-R_index); //O(1) // now i have the value that if i add to Red_Store will not exceed its size so shif Red_Store with this value
                        Red_Store |= Immediate_Red; //O(1) // and make or with Immediate to concatenate between between them

                        // Now red store ready to store in file :)
                        rwriter.Write(Red_Store);//Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
                        red_bytes_in_file += 1; // number of eight bytes stored to red
                        // but remaining Bits of Red_pair.Key is lost so i should maintains it  
                        Immediate_Red = Red_pair.Key;
                        Immediate_Red = Immediate_Red & (~Needed_Bits_L); //O(1)// Gets the value of remaining bits 
                        Red_Store = Immediate_Red;
                        Needed_Bits = 0;
                        Needed_Bits_L = 0;
                        R_index = R_index + Red_pair.Value - 32;
                    }

                    //Green part
                    if (G_index + Green_pair.Value < 32)
                    {
                        Green_Store <<= Green_pair.Value;
                        Green_Store |= Green_pair.Key;
                        G_index += Green_pair.Value;
                    }
                    else if (G_index + Green_pair.Value == 32)
                    {
                        Green_Store <<= Green_pair.Value;
                        Green_Store |= Green_pair.Key;
                        G_index = 0;
                        gwriter.Write(Green_Store);
                        green_bytes_in_file += 1; // number of eight bytes stored to green
                        Green_Store = 0;
                    }
                    else if (G_index + Green_pair.Value > 32)
                    {

                        Immediate_Green = Green_pair.Key;
                        Needed_Bits = 32 - G_index;
                        Needed_Bits = (2 << (Needed_Bits - 1)) - 1; //O(1)
                        Needed_Bits_L = Needed_Bits;
                        Needed_Bits_L <<= (Green_pair.Value + G_index - 32); //O(1)


                        Immediate_Green &= Needed_Bits_L; // O(1)
                        Immediate_Green >>= (Green_pair.Value + G_index - 32); // O(1)
                        Green_Store <<= (32-G_index); // O(1)
                        Green_Store |= Immediate_Green; // O(1)

                        // Now red store ready to store in file :)
                        gwriter.Write(Green_Store); // Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
                        green_bytes_in_file += 1;// number of eight bytes stored to green
                        // but remaining Bits of Red_pair.Key is lost so i should maintains it  
                        Immediate_Green = Green_pair.Key;
                        Immediate_Green = Immediate_Green & (~Needed_Bits_L); // Gets the value of remaining bits  takes O(1)
                        Green_Store = Immediate_Green;
                        Needed_Bits = 0;
                        Needed_Bits_L = 0;
                        G_index = G_index + Green_pair.Value - 32;
                    }

                    //Blue part
                    if (B_index + Blue_pair.Value < 32)
                    {
                        Blue_Store <<= Blue_pair.Value; // O(1)
                        Blue_Store |= Blue_pair.Key;
                        B_index += Blue_pair.Value;
                    }
                    else if (B_index + Blue_pair.Value == 32)
                    {
                        Blue_Store <<= Blue_pair.Value; // O(1)
                        Blue_Store |= Blue_pair.Key;
                        B_index = 0;
                        bwriter.Write(Blue_Store); // Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
                        blue_bytes_in_file += 1;// number of eight bytes stored to blue
                        Blue_Store = 0;
                    }
                    else if (B_index + Blue_pair.Value > 32)
                    {
                        Immediate_Blue = Blue_pair.Key;
                        Needed_Bits = 32 - B_index;
                        Needed_Bits = (2 << (Needed_Bits - 1)) - 1; //O(1)
                        Needed_Bits_L = Needed_Bits;
                        Needed_Bits_L <<= (Blue_pair.Value +  B_index - 32); // O(1)

                        Immediate_Blue &= Needed_Bits_L; // O(1)
                        Immediate_Blue >>= (Blue_pair.Value + B_index - 32); // O(1)
                        Blue_Store <<= (32-B_index); // O(1)
                        Blue_Store |= Immediate_Blue; // O(1)

                        // Now red store ready to store in file :)
                        bwriter.Write(Blue_Store);//Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
                        blue_bytes_in_file += 1;// number of eight bytes stored to green
                        // but remaining Bits of Red_pair.Key is lost so i should maintains it  
                        Immediate_Blue = Blue_pair.Key;
                        Immediate_Blue = Immediate_Blue & (~Needed_Bits_L); // O(1) 
                        Blue_Store = Immediate_Blue;
                        Needed_Bits = 0;
                        Needed_Bits_L = 0;
                        B_index = B_index + Blue_pair.Value - 32;
                    }

                }
            }
             //At the end pixel if there is value in it so store it
            if (Red_Store > 0)
            {
                rwriter.Write(Red_Store);
                red_bytes_in_file += 1;
            }
            if (Green_Store > 0)
            {
                gwriter.Write(Green_Store);
                green_bytes_in_file += 1;
            }
            if (Blue_Store > 0)
            {
                bwriter.Write(Blue_Store);
                blue_bytes_in_file += 1;
            }
            rwriter.Close();
            gwriter.Close();
            bwriter.Close();
            fs.Close();
            fs1.Close();
            fs2.Close();

            FileStream final = new FileStream("Total_Info.dat", FileMode.Append);
            
            FileStream red = new FileStream("Red.dat", FileMode.Open);
            FileStream green = new FileStream("Green.dat", FileMode.Open);
            FileStream blue = new FileStream("Blue.dat", FileMode.Open);

            red.CopyTo(final);
            green.CopyTo(final);
            blue.CopyTo(final);
            
            final.Close();
            red.Close();
            green.Close();
            blue.Close();

            File.Delete("Red.dat");
            File.Delete("Green.dat");
            File.Delete("Blue.dat");

            //MessageBox.Show("total bytes " + (Total_Bits / 8).ToString());

        }

        public RGBPixel[,] Decompression(ref RGBPixel[,] imgmatrix, HuffmanNode Root_Red, HuffmanNode Root_Green, HuffmanNode Root_Blue, StringBuilder Red_sequencebits, StringBuilder Green_sequencebits, StringBuilder Blue_sequencebits)
        {

            int height = ImageOperations.GetHeight(imgmatrix);
            int width = ImageOperations.GetWidth(imgmatrix);
            long red_sequence = Red_sequencebits.Length;
            long green_sequence = Green_sequencebits.Length;
            long blue_sequence = Blue_sequencebits.Length;
            int index_red_seq = 0;
            int index_green_seq = 0;
            int index_blue_seq = 0;

            HuffmanNode Copy_Root_Red = Root_Red;
            HuffmanNode Copy_Root_Green = Root_Green;
            HuffmanNode Copy_Root_Blue = Root_Blue;

            for (int h = 0; h < height; h++ )
            {
                for (int w = 0; w < width; w++)
                {

                    // get next red value
                    while (index_red_seq < Red_sequencebits.Length)
                    {
                        if (Red_sequencebits[index_red_seq] == '0')
                        {
                            Copy_Root_Red = Copy_Root_Red.Left_Node;
                            index_red_seq++;
                        }
                        else if (Red_sequencebits[index_red_seq] == '1')
                        {
                            Copy_Root_Red = Copy_Root_Red.Right_Node;
                            index_red_seq++;
                        }
                        if (Copy_Root_Red.CoLoR_CoDe != "$")
                        {
                            imgmatrix[h, w].red = Copy_Root_Red.value;
                            break;
                        }
                    }
                    Copy_Root_Red = Root_Red;

                    //// get next green value
                    while (index_green_seq < Green_sequencebits.Length)
                    {
                            if (Green_sequencebits[index_green_seq] == '0')
                            {
                                Copy_Root_Green = Copy_Root_Green.Left_Node;
                                index_green_seq++;
                            }
                            else if (Green_sequencebits[index_green_seq] == '1')
                            {
                                Copy_Root_Green = Copy_Root_Green.Right_Node;
                                index_green_seq++;
                            }
                            if(Copy_Root_Green.CoLoR_CoDe != "$")
                            {
                                imgmatrix[h, w].green = Copy_Root_Green.value;
                                break;
                            }

                    }
                    Copy_Root_Green = Root_Green;

                    //get next blue value

                    while (index_blue_seq < Blue_sequencebits.Length)
                    {

                            if (Blue_sequencebits[index_blue_seq] == '0')
                            {
                                Copy_Root_Blue = Copy_Root_Blue.Left_Node;
                                index_blue_seq++;
                            }
                            else if (Blue_sequencebits[index_blue_seq] == '1')
                            {
                                Copy_Root_Blue = Copy_Root_Blue.Right_Node;
                                index_blue_seq++;
                            }
                            if(Copy_Root_Blue.CoLoR_CoDe != "$")
                            {
                                imgmatrix[h, w].blue = Copy_Root_Blue.value;
                                break;
                            }

                    }
                    Copy_Root_Blue = Root_Blue;
                }   
            }
                
            return imgmatrix;
            
        }
    }
}

