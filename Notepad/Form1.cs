using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    public partial class Form1 : Form
    {
        
        const string pathCpp = "CppWords.txt";
        const string pathCSharp = "CSharpWords.txt";
        const string pathSQLWords = "SQLWords.txt";
              
        public Form1()
        {
            InitializeComponent();
            this.ContextMenuStrip = contextMenuMain;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;           
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            int i = 0;
            while (i < files.Length)
            {
                if (File.Exists(files[i]))
                {
                    Child child = new Child(File.ReadAllText(files[i], Encoding.Default), files[i])
                    {
                        MdiParent = this
                    };
                    child.Show();
                    ++i;
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void CreateDocument(object sender, EventArgs e)
        {
            Child child = new Child()
            {
                MdiParent = this               
            };           
            child.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
                        
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Files: |*.txt;*.rtf;*.cs;*.h;*.cpp";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                Child child = new Child(File.ReadAllText(dialog.FileName, Encoding.Default), dialog.FileName)
                {
                    MdiParent = this,
                   
                };
                child.Show();
            }
            
        }

        private void TypeIsCpp(object sender, EventArgs e) //выбран тип текста с/с++
        {
            var currForm = this.ActiveMdiChild;
            if (currForm != null)
            {
                Child child = currForm as Child;
                child.LoadVocabulary(pathCpp);
            }
        }

        private void TypeIsCSharp(object sender, EventArgs e)
        {
            var currForm = this.ActiveMdiChild;
            if (currForm != null)
            {
                Child child = currForm as Child;
                child.LoadVocabulary(pathCSharp);
            }

            
        }

        private void TypeIsSQL(object sender, EventArgs e)
        {
            var currForm = this.ActiveMdiChild;
            if (currForm != null)
            {
                Child child = currForm as Child;
                child.LoadVocabulary(pathSQLWords);
            }
        }

        private void EditMenuFirstSevenItems_Click(object sender, EventArgs e)
        {
            var currForm = this.ActiveMdiChild;
            if (currForm != null)
            {
                Child child = currForm as Child;
                string str = sender.ToString();
                switch (str)
                {
                    case "Undo":
                        child.Undo();
                        break;
                    case "Redo":
                        child.Redo();
                        break;
                    case "Cut":
                        child.Cut();
                        break;
                    case "Copy":
                        child.Copy();
                        break;
                    case "Paste":
                        child.Paste();
                        break;
                    case "Delete All":
                        child.DeleteAll();
                        break;
                    case "Select All":
                        child.SelectAll();
                        break;
                }
                
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currForm = this.ActiveMdiChild;
            if(currForm != null)
            {
                Child child = currForm as Child;
                child.SaveAs();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currForm = this.ActiveMdiChild;
            if (currForm != null)
            {
                Child child = currForm as Child;
                child.Save();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var activechild = ActiveMdiChild;
            if(activechild != null)
            {
                FontDialog dialog = new FontDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Child child = activechild as Child; 
                    child.SetFont(dialog);
                }
            }
            
        }

        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Эта программа создана для анализа текста на наличие служебных слов. Для выбора необходимого языка переходите в Format->Type";
            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        
    }
}
