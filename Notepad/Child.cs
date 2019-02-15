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
    public partial class Child : Form
    {       
        const char enter = '\n'; //без явного указания этого проклятого символа не работает часть логики приложения       
        const string symbolsPath = "symbols.txt";
        const int word_size = 15; //средняя длина слов
        bool keyCtrl = false;
        StringBuilder word = new StringBuilder(word_size); //содержит текущее слово
        List<string> list = new List<string>(); //текущий подключенный словарь
        List<string> symbols = new List<string>(); //служебные символы
        public bool Changed { get; set; } //несохраненные изменения в документе

        string FileName; //было первое сохранение документа
        public Child()
        {
            InitializeComponent();
            Changed = false;
            symbols.AddRange(File.ReadAllLines(symbolsPath));
            richTextBox.AcceptsTab = true; //принимает Tab
            richTextBox.ContextMenuStrip = ContextMenu;            
        }


        public Child(string text, string path) : this()
        {
            richTextBox.Text = text;
            FileName = path;
        }

        
        public void LoadVocabulary(string path) //загрузка выбранного словаря
        {
            list.Clear();
            try
            {
                list.AddRange(File.ReadAllLines(path));
            }
            catch(Exception ex)
            {
                this.Text = ex.Message;
            }

            if (list.Count > 0)
            {
                VerifyText(); //проверка текста после смены словаря
            }
            
        }

        private void VerifyText() //проверка всего содержимого richBox'a
        {
            Changed = true;

            richTextBox.SelectAll();
            richTextBox.SelectionColor = Color.Black;
            richTextBox.SelectionStart = 0; //установка каретки в начало текста
                        
            var ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
            while (richTextBox.SelectionStart!= richTextBox.TextLength)
            {
                int startPos = 0;
                if (richTextBox.SelectionStart != 0)
                {
                    startPos = richTextBox.SelectionStart;
                }
                
                int endPos = 0;
                while (!(symbols.Exists(s => s == ch.ToString()) || ch==enter) && richTextBox.SelectionStart != richTextBox.TextLength)
                {
                    ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                    endPos++;
                    richTextBox.SelectionStart++;
                }
                {

                }
                
                word.Clear();

                richTextBox.Select(startPos, endPos - 1);
                
                word.Append(richTextBox.SelectedText);
               
                if (list.Exists(s => s == word.ToString()))
                {
                    richTextBox.SelectionColor = Color.Blue; //установка цвета для выделенного слова
                    richTextBox.DeselectAll(); //снятие выделения с текста                  
                    richTextBox.SelectionColor = Color.Black; //установка цвета по умолчанию
                }

                richTextBox.SelectionStart += endPos;

                while((symbols.Exists(s => s == ch.ToString()) || ch==enter) && richTextBox.SelectionStart != richTextBox.TextLength)
                {
                    ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                    richTextBox.SelectionStart++;
                }
                
                if (richTextBox.SelectionStart != richTextBox.TextLength)
                {
                    richTextBox.SelectionStart--;
                }
                else break;
            }
            richTextBox.ClearUndo();
        }

        private void richTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            Changed = true;
            
            if(!keyCtrl)
            {
                keyCtrl = false;
                int currPosition = richTextBox.SelectionStart; //получение текущей позиции курсора
                char ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(currPosition - 1));
                int length = 0;

                if (symbols.Exists(s => s == ch.ToString()) || ch==enter) //введенный символ является разделителем
                {
                    if (richTextBox.SelectionStart > 0)
                    {
                        richTextBox.SelectionStart--; //сдвиг влево на одну позицию (до введенного символа-разделителя)
                    }
                   
                    ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart - 1));
                    while (richTextBox.SelectionStart != 0 && !(symbols.Exists(s => s == ch.ToString()) || ch==enter))
                    {
                        richTextBox.SelectionStart--;
                        ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                        length++;
                    }

                    richTextBox.Select(++richTextBox.SelectionStart, length - 1);

                    if (list.Exists(s => s == richTextBox.SelectedText)) //до введенного символа-разделителся текст явл.служебным
                    {
                        richTextBox.SelectionColor = Color.Blue;

                    }
                    else
                    {
                        richTextBox.SelectionColor = Color.Black;
                    }
                    richTextBox.DeselectAll();

                    if (richTextBox.SelectionStart > 0)
                    {
                        richTextBox.SelectionStart = currPosition - 1;
                    }
                    
                    richTextBox.Select(richTextBox.SelectionStart, 1);
                    richTextBox.SelectionColor = Color.Black;
                    richTextBox.DeselectAll();

                    ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                    while ((symbols.Exists(s => s == ch.ToString()) || ch==enter) && richTextBox.SelectionStart != richTextBox.TextLength)
                    {
                        richTextBox.SelectionStart++;
                        ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                    }

                    length = 0;
                    while (richTextBox.SelectionStart != richTextBox.TextLength && !(symbols.Exists(s => s == ch.ToString()) || ch==enter))
                    {
                        richTextBox.SelectionStart++;
                        ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                        length++;
                    }
                    richTextBox.Select(currPosition, length);

                    if (list.Exists(s => s == richTextBox.SelectedText))
                    {
                        richTextBox.SelectionColor = Color.Blue;
                    }
                    else
                    {
                        richTextBox.SelectionColor = Color.Black;
                    }
                    richTextBox.DeselectAll();

                    richTextBox.SelectionStart = currPosition;
                }
                else //введенный символ не является разделителем
                {
                    
                    ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(currPosition - 1));
                    while (richTextBox.SelectionStart != 0 && !(symbols.Exists(s => s == ch.ToString()) || ch==enter))
                    {
                        richTextBox.SelectionStart--;
                        ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                    }

                    length = 0;
                    int startPosition = ++richTextBox.SelectionStart;
                    ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                    while (richTextBox.SelectionStart != richTextBox.TextLength && !(symbols.Exists(s => s == ch.ToString()) || ch==enter))
                    {
                        richTextBox.SelectionStart++;
                        ch = richTextBox.GetCharFromPosition(richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart));
                        length++;
                    }
                    richTextBox.Select(startPosition, length);

                    if (list.Exists(s => s == richTextBox.SelectedText))
                    {
                        richTextBox.SelectionColor = Color.Blue;
                    }
                    else
                    {
                        richTextBox.SelectionColor = Color.Black;
                    }
                    richTextBox.DeselectAll();

                    richTextBox.SelectionStart = currPosition;
                }
            }
            
                
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Undo();
        }

        public void Undo()
        {
            richTextBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Redo();
        }

        public void Redo()
        {
            richTextBox.Redo();
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Cut();
            VerifyText();
        }

        public void Cut()
        {
            richTextBox.Cut();
            VerifyText();
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Copy();
        }

        public void Copy()
        {
            richTextBox.Copy();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Paste();
            VerifyText();
        }

        public void Paste()
        {
            richTextBox.Paste();
            VerifyText();
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectAll();
        }

        public void SelectAll()
        {
            richTextBox.SelectAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Clear();
            
        }

        public void DeleteAll()
        {
            richTextBox.Clear();
        }
        private void richTextBox_KeyDown(object sender, KeyEventArgs e) //проверка нажатия кнопки ctrl
        {
            if(e.Modifiers== Keys.Control)
            {
                keyCtrl = true;
            }
        }

        public void SaveAs()
        {           
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Document |*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileName = dialog.FileName;
                File.WriteAllLines(FileName, richTextBox.Lines);
            }
            Changed = false;
        }

        public void Save()
        {
            if (FileName == null)
            {
                this.SaveAs();
            }        
            else
            {
                File.WriteAllLines(FileName, richTextBox.Lines);
            }
            Changed = false;
        }

        public void SetFont(FontDialog dialog) //установка цвета, шрифта, размера в richTextBox
        {
            Changed = true;
            richTextBox.SelectionFont = dialog.Font;
        }
    }
}
