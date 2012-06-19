using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Data2Serial
{
    public partial class Form1 : Form
    {
        int lineCount = 0;
        String file = "";
        LinkedList<String> lines = new LinkedList<string>();
        byte[][] db;
        //System.IO.TextReader reader;
        System.IO.Ports.SerialPort rs = new System.IO.Ports.SerialPort();
        //System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(doLineWork));
        bool delay = false;

        public Form1(string[] args)
        {
            InitializeComponent();
            String version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = "Data2Serial v" + version;
            addList("Application opened v" + version);
            linkLabel1.Enabled = false;
            if (args.Length > 0)
            {
                if (System.IO.File.Exists(args[0]))
                {
                    file = args[0];
                    addList(file + " is selected!");
                    button1.Enabled = false;
                    button1.Text = "Selected!";
                    linkLabel1.Enabled = true;
                }
                else
                {
                    addList(file + " does not exist!");
                    
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            file = openFileDialog1.FileName;
            if (file.Length > 1)
            {
                addList(file + " is selected!");
                button1.Enabled = false;
                button1.Text = "Selected!";
                linkLabel1.Enabled = true;
                System.IO.TextReader tr = new System.IO.StreamReader(file);
                String a;
                //int i = 0;
                while ((a = tr.ReadLine()) != null)
                {
                    lines.AddLast(a);
                    lineCount++;
                }
                db = new byte[lines.Count][];
                for (int i = 0; i < lines.Count; i++)
                {
                    db[i] = StringToByteArray(lines.ElementAt(i));
                }
                tr.Close();
                progressBar1.Maximum = lineCount;
                lineCount = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lineCount = 0;
            progressBar1.Value = lineCount;
            bool valid = true;
            if (textBox2.Text.Length < 3)
            {
                MessageBox.Show(this, "Please enter a valid baud rate (>100)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                valid = false;
                return;
            }
            if (file.Length < 1)
            {
                MessageBox.Show(this, "Please select a file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                valid = false;
                return;
            }
            if (comboBox4.SelectedItem == null)
            {
                MessageBox.Show(this, "Please select a port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                valid = false;
                return;
            }
            if (textBox1.Text.Length < 1 && checkBox1.Checked)
            {
                MessageBox.Show(this, "Please enter a delay > 0 ms!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                valid = false;
                return;
            }
            if(valid)
            {
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                    button2.Text = "Connect";
                    this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
                    linkLabel1.Enabled = true;
                    button5.Enabled = true;
                    checkBox1.Enabled = true;
                    if (checkBox1.Checked)
                    {
                        textBox1.Enabled = true;
                    }
                    return;
                    
                }
                rs.BaudRate = int.Parse(textBox2.Text);
                rs.PortName = comboBox4.SelectedItem.ToString();

                //reader = new System.IO.StreamReader(file);

                if (comboBox2.SelectedIndex == 0)
                {
                    rs.Parity = System.IO.Ports.Parity.None;
                }
                else if (comboBox2.SelectedIndex == 1)
                {
                    rs.Parity = System.IO.Ports.Parity.Odd;
                }
                else if (comboBox2.SelectedIndex == 2)
                {
                    rs.Parity = System.IO.Ports.Parity.Even;
                }
                else if (comboBox2.SelectedIndex == 3)
                {
                    rs.Parity = System.IO.Ports.Parity.Mark;
                }
                else if (comboBox2.SelectedIndex == 4)
                {
                    rs.Parity = System.IO.Ports.Parity.Space;
                }

                if (comboBox1.SelectedIndex == 0)
                {
                    rs.DataBits = 5;
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    rs.DataBits = 6;
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    rs.DataBits = 7;
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    rs.DataBits = 8;
                }
                if (comboBox3.SelectedIndex == 0)
                {
                    rs.StopBits = System.IO.Ports.StopBits.None;
                }
                else if (comboBox3.SelectedIndex == 1)
                {
                    rs.StopBits = System.IO.Ports.StopBits.One;
                }
                else if (comboBox3.SelectedIndex == 2)
                {
                    rs.StopBits = System.IO.Ports.StopBits.OnePointFive;
                }
                else if (comboBox3.SelectedIndex == 3)
                {
                    rs.StopBits = System.IO.Ports.StopBits.Two;
                }
                rs.ReadTimeout = 500;
                rs.WriteTimeout = 500;
                rs.Handshake = System.IO.Ports.Handshake.None;
                
                if(!backgroundWorker1.IsBusy)
                {
                    button2.Text = "Cancel";
                    this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
                    linkLabel1.Enabled = false;
                    button5.Enabled = false;
                    checkBox1.Enabled = false;
                    if (checkBox1.Checked)
                    {
                        textBox1.Enabled = false;
                    }

                    backgroundWorker1.RunWorkerAsync();
                }
                
            }
        }


        private void addList(String line)
        {
            this.listBox1.Items.Insert(listBox1.Items.Count, DateTime.Now.ToString("HH:mm:ss.fff") + " | " + line);
            this.listBox1.SetSelected(listBox1.Items.Count - 1, true);
        }

        private void saveLog(String saveLocation)
        {
            if (saveLocation.Length > 0)
            {
                System.IO.TextWriter tw = new System.IO.StreamWriter(saveLocation);
                for (int i = listBox1.Items.Count; i > 0; i--)
                {
                    tw.WriteLine(listBox1.GetItemText(listBox1.Items[listBox1.Items.Count - i]));
                }
                tw.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            saveLog(saveFileDialog1.FileName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 3;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            checkOpenPorts();
            //MessageBox.Show(checkOpenPorts());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            file = "";
            button1.Enabled = true;
            button1.Text = "Select File";
            this.linkLabel1.Enabled = false;
            progressBar1.Value = 0;
            label7.Text = "%0";
            lines = new LinkedList<string>();
            lineCount = 0;
            db = null;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           // String line;
            int i = 0;
            while (lines.Count != i)
            {
                if (rs.IsOpen)
                {
                    doOutput(i);
                    i++;
                }
                else
                {
                    try
                    {
                        rs.Open();
                        Invoke((MethodInvoker)(() =>
                        {
                            addList("Line is open!");
                        }));
                        doOutput(i);
                        i++;
                    }
                    catch (Exception ioex)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            addList("Error: " + ioex.Message);
                            button2.Text = "Connect";
                            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
                            linkLabel1.Enabled = true;
                            button5.Enabled = true;
                        }));
                        break;
                    }
                }
                
                if ((backgroundWorker1.CancellationPending == true))
                {
                    e.Cancel = true;
                    //reader.Close();
                    break;
                }
            }
            if (lines.Count == i)
            {
                Invoke((MethodInvoker)(() =>
                {
                    addList("File sent!");
                    button2.Text = "Connect";
                    this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
                    linkLabel1.Enabled = true;
                    button5.Enabled = true;
                    checkBox1.Enabled = true;
                    textBox1.Enabled = true;
                    progressBar1.Value = 0;
                    label7.Text = "%0";
                }));
            }
            if (rs.IsOpen)
            {
                rs.Close();
                Invoke((MethodInvoker)(() =>
                {
                    addList("Line closed!");
                    linkLabel1.Enabled = true;
                }));
            }
        }

        private void doOutput(int index)
        {
            if (delay)
            {
                System.Threading.Thread.Sleep(int.Parse(textBox1.Text));
            }
            if (radioButton2.Checked)
            {
                rs.Write(db[index], 0, db[index].Length);

                Invoke((MethodInvoker)(() =>
                {
                    addList("Sent " + ++lineCount);
                    progressBar1.Value++;// = lineCount;
                    label7.Text = "%" + (int)(((double)(lineCount)) / progressBar1.Maximum * 100);
                }));
                //String read = "";*/
                if (checkBox2.Checked)
                {
                    int sent = db[index].Length;
                    if (checkBox1.Checked)
                    {
                        System.Threading.Thread.Sleep(int.Parse(textBox1.Text));
                    }
                    try
                    {
                        char[] buffer = new char[sent];
                        rs.Read(buffer, 0, sent);
                        if (buffer.Length > 0)
                        {
                            Invoke((MethodInvoker)(() =>
                            {
                                addList("Read " + new String(buffer));
                            }));
                        }
                    }
                    catch (TimeoutException)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            addList("Missing " + index.ToString());
                        }));
                    }
                }
            }
            else
            {
                try
                {
                    
                    rs.WriteLine(lines.ElementAt(index) +"\r");
                    
                }
                catch (Exception)
                {
                }
                Invoke((MethodInvoker)(() =>
                {
                    addList("Sent " + lineCount + 1 ); // line);
                    lineCount++;
                    progressBar1.Value = lineCount;
                    label7.Text = "%" + (int)((double)(lineCount) / progressBar1.Maximum * 100);
                }));
                if (checkBox2.Checked)
                {
                    String read = "";
                    try
                    {
                        while ((read = rs.ReadLine()) != null)
                        {
                            Invoke((MethodInvoker)(() =>
                            {
                                addList("Read " + read);
                            }));
                        }
                    }
                    catch (TimeoutException)
                    {
                        if (read != lines.ElementAt(index))
                        {
                            Invoke((MethodInvoker)(() =>
                            {
                                addList("Missing " + lines.ElementAt(index));
                            }));
                        }
                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Text = "Connect";
        }

        public byte[] StringToByteArray(string hex)
        {
            //hex = hex.Replace(' ','');

            try
            {
                byte[] asd = Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
                return asd;
            }
            catch (Exception)
            {
            }
            return null;
        }

        private String stringToHex(String convertThis)
        {
            string hex = "";
            foreach (char c in convertThis)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        private void checkOpenPorts()
        {
            comboBox4.Items.Clear();
            var portNames = System.IO.Ports.SerialPort.GetPortNames();

            foreach (var port in portNames)
            {
                //Try for every portName and break on the first working
                try {
                    rs.PortName = port;
                    rs.Open();
                    //openPorts += port + "|";
                    comboBox4.Items.Add(port);
                    comboBox4.SelectedIndex = 0;
                    rs.Close();
                }
                catch {

                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            checkOpenPorts();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.Enabled = true;
                delay = true;
            }
            else
            {
                textBox1.Enabled = false;
                delay = false;
            }
        }

        /*private byte[] stringHex2Byte(String a)
        {
            int az = new int(a,16);
            return 1;
        }*/

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            string allowedCharacterSet = "0123456789\b";
            //Allowed character set

            if (allowedCharacterSet.Contains(e.KeyChar.ToString()))
            {

            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string allowedCharacterSet = "0123456789\b";
            //Allowed character set

            if (allowedCharacterSet.Contains(e.KeyChar.ToString()))
            {

            }
            else
            {
                e.Handled = true;
            }
        }

    }
}
