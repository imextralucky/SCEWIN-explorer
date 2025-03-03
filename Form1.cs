using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SCEWIN_explorer
{
    public partial class Form1 : Form
    {
        private string[] allLines;

        public Form1()
        {
            InitializeComponent();
            LoadNvramFile();
            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyPreview = true;

            textBoxSearch.TextChanged += new EventHandler(textBoxSearch_TextChanged);
        }

        private void LoadNvramFile()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nvram.txt");

            if (File.Exists(filePath))
            {
                allLines = File.ReadAllLines(filePath);
                DisplayFileContentsFast();
                PopulateSetupQuestionsFast();
            }
            else
            {
                MessageBox.Show("File not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayFileContentsFast()
        {
            if (allLines == null) return;

            richTextBox1.SuspendLayout();
            richTextBox1.Clear();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < allLines.Length; i++)
            {
                sb.AppendLine($"{i + 1}: {allLines[i]}");
            }

            richTextBox1.Text = sb.ToString();
            richTextBox1.ResumeLayout();
        }

        private void PopulateSetupQuestionsFast()
        {
            if (allLines == null) return;

            listBox1.BeginUpdate();
            listBox1.Items.Clear();

            for (int i = 0; i < allLines.Length; i++)
            {
                if (allLines[i].StartsWith("Setup Question\t="))
                {
                    string setupOption = allLines[i].Substring(allLines[i].IndexOf('=') + 1).Trim();
                    string optionOrValue = "N/A";

                    int commentIndex, starIndex, bracketIndex, endIndex;

                    for (int j = i + 1; j < allLines.Length; j++)
                    {
                        if (allLines[j].StartsWith("Options\t="))
                        {
                            int optionStartIndex = allLines[j].IndexOf('=') + 1;

                            commentIndex = allLines[j].IndexOf("//");
                            starIndex = allLines[j].IndexOf("*");
                            bracketIndex = allLines[j].IndexOf("[");

                            if (starIndex != -1 && (commentIndex == -1 || starIndex < commentIndex))
                            {
                                endIndex = commentIndex != -1 ? commentIndex : allLines[j].Length;
                                optionOrValue = allLines[j].Substring(starIndex + 2, endIndex - starIndex - 2).Trim();
                                optionOrValue = optionOrValue.Substring(optionOrValue.IndexOf(']') + 1).Trim();
                                break;
                            }

                            while (j + 1 < allLines.Length && !allLines[j + 1].StartsWith("Setup Question\t="))
                            {
                                j++;
                                commentIndex = allLines[j].IndexOf("//");
                                starIndex = allLines[j].IndexOf("*");
                                bracketIndex = allLines[j].IndexOf("[");

                                if (starIndex != -1 && (commentIndex == -1 || starIndex < commentIndex))
                                {
                                    endIndex = commentIndex != -1 ? commentIndex : allLines[j].Length;
                                    optionOrValue = allLines[j].Substring(starIndex + 2, endIndex - starIndex - 2).Trim();
                                    optionOrValue = optionOrValue.Substring(optionOrValue.IndexOf(']') + 1).Trim();
                                    break;
                                }
                            }

                            break;
                        }
                        else if (allLines[j].StartsWith("Value\t="))
                        {
                            string valueLine = allLines[j];
                            commentIndex = valueLine.IndexOf("//");
                            string valueText = commentIndex != -1 ? valueLine.Substring(0, commentIndex) : valueLine;
                            optionOrValue = valueText.Substring(valueText.IndexOf('=') + 1).Trim();

                            if (optionOrValue.StartsWith("<") && optionOrValue.EndsWith(">"))
                            {
                                optionOrValue = optionOrValue.Trim('<', '>');
                            }
                            break;
                        }

                        if (allLines[j].StartsWith("Setup Question\t="))
                            break;
                    }

                    listBox1.Items.Add($"{i + 1}: {setupOption} ➤ {optionOrValue}");
                }
            }

            listBox1.EndUpdate();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBoxSearch.Text.ToLower();
            PopulateSearchResults(searchText);
        }

        private void PopulateSearchResults(string searchText)
        {
            listBox1.BeginUpdate();
            listBox1.Items.Clear();

            for (int i = 0; i < allLines.Length; i++)
            {
                if (allLines[i].StartsWith("Setup Question\t=") && allLines[i].ToLower().Contains(searchText))
                {
                    string setupOption = allLines[i].Substring(allLines[i].IndexOf('=') + 1).Trim();
                    string optionOrValue = "N/A";

                    int commentIndex, starIndex, endIndex;

                    for (int j = i + 1; j < allLines.Length; j++)
                    {
                        if (allLines[j].StartsWith("Options\t="))
                        {
                            int optionStartIndex = allLines[j].IndexOf('=') + 1;
                            commentIndex = allLines[j].IndexOf("//");
                            starIndex = allLines[j].IndexOf("*");

                            if (starIndex != -1 && (commentIndex == -1 || starIndex < commentIndex))
                            {
                                endIndex = commentIndex != -1 ? commentIndex : allLines[j].Length;
                                optionOrValue = allLines[j].Substring(starIndex + 2, endIndex - starIndex - 2).Trim();
                                optionOrValue = optionOrValue.Substring(optionOrValue.IndexOf(']') + 1).Trim();
                                break;
                            }

                            while (j + 1 < allLines.Length && !allLines[j + 1].StartsWith("Setup Question\t="))
                            {
                                j++;
                                commentIndex = allLines[j].IndexOf("//");
                                starIndex = allLines[j].IndexOf("*");

                                if (starIndex != -1 && (commentIndex == -1 || starIndex < commentIndex))
                                {
                                    endIndex = commentIndex != -1 ? commentIndex : allLines[j].Length;
                                    optionOrValue = allLines[j].Substring(starIndex + 2, endIndex - starIndex - 2).Trim();
                                    optionOrValue = optionOrValue.Substring(optionOrValue.IndexOf(']') + 1).Trim();
                                    break;
                                }
                            }

                            break;
                        }
                        else if (allLines[j].StartsWith("Value\t="))
                        {
                            string valueLine = allLines[j];
                            commentIndex = valueLine.IndexOf("//");
                            string valueText = commentIndex != -1 ? valueLine.Substring(0, commentIndex) : valueLine;
                            optionOrValue = valueText.Substring(valueText.IndexOf('=') + 1).Trim();

                            if (optionOrValue.StartsWith("<") && optionOrValue.EndsWith(">"))
                            {
                                optionOrValue = optionOrValue.Trim('<', '>');
                            }
                            break;
                        }

                        if (allLines[j].StartsWith("Setup Question\t="))
                            break;
                    }

                    listBox1.Items.Add($"{i + 1}: {setupOption} ➤ {optionOrValue}");
                }
            }

            listBox1.EndUpdate();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            int selectedLineNumber = int.Parse(listBox1.SelectedItem.ToString().Split(':')[0]);
            int selectedIndex = selectedLineNumber - 1;

            if (selectedIndex >= 0 && selectedIndex < richTextBox1.Lines.Length)
            {
                string selectedLineText = richTextBox1.Lines[selectedIndex];

                int firstCharIndex = richTextBox1.Text.IndexOf(selectedLineText, StringComparison.Ordinal);
                int lineLength = selectedLineText.Length;

                richTextBox1.SelectionStart = firstCharIndex;
                richTextBox1.SelectionLength = lineLength;
                richTextBox1.Focus();
                richTextBox1.ScrollToCaret();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R)
            {
                LoadNvramFile();
                e.SuppressKeyPress = true;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}