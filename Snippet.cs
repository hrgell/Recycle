// The file used as paste buffer for useful code snippets.

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Bitmap img in ImageList1.Images)
                img?.MakeTransparent(Color.White);
        }
