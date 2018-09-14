using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        HuffmanNode Red_Root ;
        HuffmanNode Green_Root ;
        HuffmanNode Blue_Root ;
        HuffmanTree Red_Huffman;
        HuffmanTree Green_Huffman;
        HuffmanTree Blue_Huffman;
        int red_bytes_in_file = 0;
        int green_bytes_in_file = 0;
        int blue_bytes_in_file = 0;

        RGBPixel[,] ImageMatrix;
        int[] powersoftwo = {128, 64, 32, 16, 8, 4, 2, 1 };
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

        }
        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        public void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        public static long LFSR(long initial_seed,int tap,int length_initial_seed ,int shifted_tap ,int shifted_length)
        {
            long extractedbits = initial_seed;
            long value_of_last_bit;
            long value_of_tap_position;
            uint xor;
            for (int i = 0 ; i < 8 ; i++)
            {
                extractedbits = extractedbits & (shifted_length);
                extractedbits = extractedbits >> length_initial_seed;

                value_of_last_bit = extractedbits; // convert because extracted bits is long and value is int

                extractedbits = 0;
                extractedbits = initial_seed;
                extractedbits = extractedbits & (shifted_tap);
                extractedbits = extractedbits >> tap;

                value_of_tap_position = extractedbits;

                extractedbits = 0;
                extractedbits = initial_seed;
                xor = Convert.ToUInt32(value_of_last_bit ^ value_of_tap_position);

                extractedbits <<= 1;
                extractedbits |= xor;
                initial_seed = extractedbits;
            }

            return extractedbits;
        }
        public void Encryption_Decryption(ref RGBPixel[,] img, long initial, int tap, int length_initialseed)
        {
            int img_Height = ImageOperations.GetHeight(img);
            int img_Width = ImageOperations.GetWidth(img);
            int Shift_Length = 1 << length_initialseed;
            int Shift_Tap = 1 << tap;
            for (int i = 0; i < img_Height; i++)
            {
                for (int j = 0; j < img_Width; j++)
                {
                    long initial_seed_1 = LFSR(initial, tap, length_initialseed, Shift_Tap, Shift_Length);
                    img[i, j].red = (Byte)(img[i, j].red ^ (initial_seed_1 & 255));

                    long initial_seed_2 = LFSR(initial_seed_1, tap, length_initialseed, Shift_Tap, Shift_Length);
                    img[i, j].green = (Byte)(img[i, j].green ^ (initial_seed_2 & 255));

                    long initial_seed_3 = LFSR(initial_seed_2, tap, length_initialseed, Shift_Tap, Shift_Length);
                    img[i, j].blue = (Byte)(img[i, j].blue ^ (initial_seed_3 & 255));

                    initial = initial_seed_3;
                }
            }
            ImageOperations.DisplayImage(img, pictureBox2);
        }

        public static StringBuilder LFSR_Big_Seed(ref StringBuilder seed , ref int tap ,ref int last_bit )
        {
            for(int z = 0 ; z < 8 ; z++)
            {
                if(seed[z] == seed[tap])
                {
                    seed.Append("0");
                    last_bit++;
                    tap++;
                }
                else
                {
                    seed.Append("1");
                    last_bit++;
                    tap++;
                }
            }
            return seed;
        }

        public void Encryption_Decryption_with_Big_seed(ref RGBPixel[,] img,ref StringBuilder initial ,ref int tap,ref int last_bit)
        {
            int img_Height = ImageOperations.GetHeight(img);
            int img_Width = ImageOperations.GetWidth(img);
            for (int i = 0; i < img_Height; i++)
            {
                for (int j = 0; j < img_Width; j++)
                {
                    StringBuilder redseed = LFSR_Big_Seed(ref initial,ref tap,ref last_bit);
                    int redextracted = 0;
                    int counter = 0;
                    for(int a = redseed.Length - 8 ; a <= redseed.Length - 1 ; a++)
                    {
                      redextracted += (redseed[a]-48) * powersoftwo[counter];
                      counter++;
                    }
                    img[i, j].red = Convert.ToByte(img[i, j].red ^ redextracted);
                    counter = 0;
                    int greenextracted = 0;
                    StringBuilder greenseed = LFSR_Big_Seed(ref redseed, ref tap, ref last_bit);
                    for (int a = greenseed.Length - 8; a <= greenseed.Length - 1; a++)
                    {
                        greenextracted += (greenseed[a] - 48) * powersoftwo[counter];
                        counter++;
                    }
                    img[i, j].green = Convert.ToByte(img[i, j].green ^ greenextracted);
                    counter = 0;
                    int blueextracted = 0;
                    StringBuilder blueseed = LFSR_Big_Seed(ref greenseed, ref tap, ref last_bit);
                    for (int a = blueseed.Length - 8; a <= greenseed.Length - 1; a++)
                    {
                        blueextracted += (blueseed[a] - 48) * powersoftwo[counter];
                        counter++;
                    }
                    img[i, j].blue = Convert.ToByte(img[i, j].blue ^ blueextracted);
                    counter = 0;
                    initial = blueseed;
                }
            }

            ImageOperations.DisplayImage(img, pictureBox2);
        }


        private void btn_encrypt_Click(object sender, EventArgs e)
        {
            Stopwatch SWE = new Stopwatch();
            SWE.Start();
            string initial_seed = txt_initialseed.Text;
            long initial_seed_L;
           
            if (initial_seed.Length <= 64)
            {
                bool result = long.TryParse(initial_seed, out initial_seed_L);
                if (result == true)
                {
                    Encryption_Decryption(ref ImageMatrix, Convert.ToInt64(txt_initialseed.Text, 2), Convert.ToInt32(txt_tapposition.Text), txt_initialseed.Text.Length - 1);
                }
                else
                {
                    long total = 0;
                    for (int i = 0; i < initial_seed.Length; i++)
                    {
                        total += initial_seed[i];
                    }
                    string binary = Convert.ToString(total, 2);
                    int binary_length = binary.Length - 1;
                    Encryption_Decryption(ref ImageMatrix,total, Convert.ToInt32(txt_tapposition.Text), binary.Length-1);
                }

            }
            else
            {
                StringBuilder seed = new StringBuilder(); 
                for(int i = 0 ; i < initial_seed.Length ; i++)
                {
                    seed.Append(initial_seed[i]);
                }
                int tap =seed.Length - 1 - Convert.ToInt32(txt_tapposition.Text);
                int lastbit = 0;
                Encryption_Decryption_with_Big_seed(ref ImageMatrix , ref seed , ref tap , ref lastbit);
            }
               
                SWE.Stop();
            lbl_Encrypt.Text = SWE.Elapsed.ToString();
        }

        private void btn_decrypt_Click(object sender, EventArgs e)
        {
            Stopwatch SWD = new Stopwatch();
            SWD.Start();
            string initial_seed = txt_initialseed.Text;
            long initial_seed_L;

            if (initial_seed.Length <= 64)
            {
                bool result = long.TryParse(initial_seed, out initial_seed_L);
                if (result == true)
                {
                    Encryption_Decryption(ref ImageMatrix, Convert.ToInt64(txt_initialseed.Text, 2), Convert.ToInt32(txt_tapposition.Text), txt_initialseed.Text.Length - 1);
                }
                else
                {
                    long total = 0;
                    for (int i = 0; i < initial_seed.Length; i++)
                    {
                        total += initial_seed[i];
                    }
                    string binary = Convert.ToString(total, 2);
                    int binary_length = binary.Length - 1;
                    Encryption_Decryption(ref ImageMatrix, total, Convert.ToInt32(txt_tapposition.Text), binary.Length - 1);
                }

            }
            else
            {
                StringBuilder seed = new StringBuilder();
                for (int i = 0; i < initial_seed.Length; i++)
                {
                    seed.Append(initial_seed[i]);
                }
                int tap = seed.Length - 1 - Convert.ToInt32(txt_tapposition.Text);
                int lastbit = 0;
                Encryption_Decryption_with_Big_seed(ref ImageMatrix, ref seed, ref tap, ref lastbit);
            }
            SWD.Stop();

            lbl_Decrypt.Text = SWD.Elapsed.ToString();
        }



        Image File;
        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog SF = new SaveFileDialog();
            SF.Filter = "JPG(*.JPG)|*.jpg";

            File = pictureBox2.Image;

            if (SF.ShowDialog() == DialogResult.OK)
            {
                File.Save(SF.FileName);
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void lbl_Decrypt_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            long[] Red_array = new long[256];
            long[] Green_array = new long[256];
            long[] Blue_array = new long[256];
            Priority_Queue Red_priority = new Priority_Queue();
            Priority_Queue Green_priority = new Priority_Queue();
            Priority_Queue Blue_priority = new Priority_Queue();

             Red_Huffman = new HuffmanTree();
             Green_Huffman = new HuffmanTree();
             Blue_Huffman = new HuffmanTree();

            List<HuffmanNode> Red_Nodes = new List<HuffmanNode>();
            List<HuffmanNode> Green_Nodes = new List<HuffmanNode>();
            List<HuffmanNode> Blue_Nodes = new List<HuffmanNode>();
         
            Red_Root = new HuffmanNode();
            Green_Root = new HuffmanNode();
            Blue_Root = new HuffmanNode();

            int Height = ImageOperations.GetHeight(ImageMatrix);
            int width = ImageOperations.GetWidth(ImageMatrix);

            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Red_array[ImageMatrix[h, w].red]++;
                    Green_array[ImageMatrix[h, w].green]++;
                    Blue_array[ImageMatrix[h, w].blue]++;
                }
            }

            for (int k = 0; k < 256; k++)
            {
                if (Red_array[k] > 0)
                {
                    Red_Nodes.Add(new HuffmanNode { value = Convert.ToByte(k), freq = Red_array[k], CoLoR_CoDe = "" });
                }
                if (Green_array[k] > 0)
                {
                    Green_Nodes.Add(new HuffmanNode { value = Convert.ToByte(k), freq = Green_array[k], CoLoR_CoDe = "" });
                }
                if (Blue_array[k] > 0)
                {
                    Blue_Nodes.Add(new HuffmanNode { value = Convert.ToByte(k), freq = Blue_array[k], CoLoR_CoDe = "" });
                }
            }
            Red_priority.Build_Min_Heap(ref Red_Nodes);
            Green_priority.Build_Min_Heap(ref Green_Nodes);
            Blue_priority.Build_Min_Heap(ref Blue_Nodes);

            Red_Root = Red_Huffman.BuildHuffmanTree(ref Red_Nodes);
            Green_Root = Green_Huffman.BuildHuffmanTree(ref Green_Nodes);
            Blue_Root = Blue_Huffman.BuildHuffmanTree(ref Blue_Nodes);

            FileStream Trunc_Red_File = new FileStream("Red_Info.txt", FileMode.Truncate);
            Trunc_Red_File.Close();
            FileStream Red_File = new FileStream("Red_Info.txt", FileMode.Append);
            StreamWriter Red_Writer = new StreamWriter(Red_File);
            Red_Writer.WriteLine("--R--");
            Red_Writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
            Red_Writer.Close();
            Red_File.Close();
            Red_Huffman.PrintCodes(Red_Root, "", "Red_Info.txt");

            FileStream Trunc_Green_File = new FileStream("Green_Info.txt", FileMode.Truncate);
            Trunc_Green_File.Close();
            FileStream Green_File = new FileStream("Green_Info.txt", FileMode.Append);
            StreamWriter Green_Writer = new StreamWriter(Green_File);
            Green_Writer.WriteLine("--G--");
            Green_Writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
            Green_Writer.Close();
            Green_File.Close();
            Green_Huffman.PrintCodes(Green_Root, "", "Green_Info.txt");

            FileStream Trunc_Blue_File = new FileStream("Blue_Info.txt", FileMode.Truncate);
            Trunc_Blue_File.Close();
            FileStream Blue_File = new FileStream("Blue_Info.txt", FileMode.Append);
            StreamWriter Blue_Writer = new StreamWriter(Blue_File);
            Blue_Writer.WriteLine("--B--");
            Blue_Writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
            Blue_Writer.Close();
            Blue_File.Close();
            Blue_Huffman.PrintCodes(Blue_Root, "", "Blue_Info.txt");

            FileStream truncate_total_info = new FileStream("Total_Info.dat", FileMode.Truncate);
            truncate_total_info.Close();

            
            sp.Stop();
            lbl_construct_tree.Text = sp.Elapsed.ToString();
            MessageBox.Show("Constructed !");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HuffmanTree compression = new HuffmanTree();
            double CRBCompression = 0;
            double CRACompression = 0;

            compression.Compression(ref ImageMatrix , ref CRBCompression, ref CRACompression , ref red_bytes_in_file , ref green_bytes_in_file , ref blue_bytes_in_file);
            
            //save Trees in file
            Red_Huffman = new HuffmanTree();
            Green_Huffman = new HuffmanTree();
            Blue_Huffman = new HuffmanTree(); 

            Red_Huffman.PrintHuffmanTree(Red_Root);
            Green_Huffman.PrintHuffmanTree(Green_Root);
            Blue_Huffman.PrintHuffmanTree(Blue_Root);
            // save width and height and initial seed and tap position in file
            FileStream FN = new FileStream("Total_Info.dat", FileMode.Append);
            BinaryWriter FNR = new BinaryWriter(FN);
            FNR.Write(ImageOperations.GetHeight(ImageMatrix));
            FNR.Write(ImageOperations.GetWidth(ImageMatrix));
            FNR.Write(txt_initialseed.Text);
            FNR.Write(txt_tapposition.Text);
            FNR.Close();
            FN.Close();
            sw.Stop();
            compression_lbl.Text = sw.Elapsed.ToString();
            
            MessageBox.Show("Compressed !");
            MessageBox.Show("Original Size = " + CRBCompression.ToString());
            MessageBox.Show("Compression Ratio After Compression = " + CRACompression.ToString());
            MessageBox.Show("stored bytes red" + red_bytes_in_file.ToString());
            MessageBox.Show("stored bytes green" + green_bytes_in_file.ToString());
            MessageBox.Show("stored bytes blue" + blue_bytes_in_file.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringBuilder red_seq = new StringBuilder();
            StringBuilder green_seq = new StringBuilder();
            StringBuilder blue_seq = new StringBuilder();
            string re;
            string gr;
            string bl;


            FileStream FOX = new FileStream("Total_Info.dat", FileMode.Open);
            BinaryReader BRR = new BinaryReader(FOX);

            for(int i = 0 ; i < red_bytes_in_file ; i++)
            {
               re = Convert.ToString(BRR.ReadInt32() , 2);
               red_seq.Append(re.PadLeft(32 , '0')); 
                
            }
            for (int i = 0; i < green_bytes_in_file; i++)
            {
                gr = Convert.ToString(BRR.ReadInt32(), 2);
                green_seq.Append(gr.PadLeft(32 , '0'));
            }
           
            for (int i = 0; i < blue_bytes_in_file; i++)
            {
                bl = Convert.ToString(BRR.ReadInt32(), 2);
                blue_seq.Append(bl.PadLeft(32 , '0'));
            }
            BRR.Close();
            FileStream Ftrunc = new FileStream("Total_Info.dat", FileMode.Truncate);
            Ftrunc.Close();
            FOX.Close();
            
            HuffmanTree decomp = new HuffmanTree();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ImageOperations.DisplayImage(decomp.Decompression(ref ImageMatrix, Red_Root, Green_Root, Blue_Root, red_seq, green_seq, blue_seq) ,  pictureBox2);
            sw.Stop();
            decomp_lbl.Text = sw.Elapsed.ToString();
            MessageBox.Show("decompressed ");
        }
    }
}