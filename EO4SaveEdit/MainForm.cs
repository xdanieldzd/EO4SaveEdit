﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using EO4SaveEdit.FileHandlers;

namespace EO4SaveEdit
{
    public partial class MainForm : Form
    {
        List<BaseMori4File> dataFiles;

        public MainForm()
        {
            InitializeComponent();

            LoadSaveData(@"E:\[SSD User Data]\Desktop\filer\UserSaveData\20150529080428\00000ea6");

            /*RomFSDataDumper.DumpItemData(
                @"E:\[SSD User Data]\Downloads\EOIV\romfs\Item\equipitemnametable.tbl",
                @"E:\[SSD User Data]\Downloads\EOIV\romfs\Item\equipitemtable.tbl",
                @"C:\Temp\equipitemnametable.xml");*/
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Select folder with 'mori4*.sav' save files.";
            folderBrowserDialog1.SelectedPath = Properties.Settings.Default.LastFolder;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadSaveData(folderBrowserDialog1.SelectedPath);
            }
        }

        private void LoadSaveData(string path)
        {
            Properties.Settings.Default.LastFolder = path;
            dataFiles = new List<BaseMori4File>();

            foreach (string file in Directory.EnumerateFiles(Properties.Settings.Default.LastFolder))
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
                {
                    string signature = Encoding.ASCII.GetString(reader.ReadBytes(8));
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    switch (signature)
                    {
                        case "MOR4OPTI": dataFiles.Add(new Mori4Option(reader)); break;
                        case "MOR4MAPA": dataFiles.Add(new Mori4Map(reader)); break;
                        case "MOR4GAME": dataFiles.Add(new Mori4Game(reader)); break;
                    }
                }
            }

            if (dataFiles.Count == 0)
            {
                MessageBox.Show("No compatible 'mori4*.sav' save files found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                m4Map.Initialize(dataFiles.FirstOrDefault(x => x is Mori4Map) as Mori4Map);
                m4Options.Initialize(dataFiles.FirstOrDefault(x => x is Mori4Option) as Mori4Option);
                characterEditor1.Initialize(dataFiles.FirstOrDefault(x => x is Mori4Game) as Mori4Game);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("EO4 Save Editor\nWritten 2015 by xdaniel", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
