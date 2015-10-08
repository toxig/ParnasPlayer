using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;


namespace PPlayer
{
    public partial class Mod_Rich_Edit : DevExpress.XtraEditors.XtraUserControl
    {
        public bool v_need_reload = false;        

        private string _v_FilePath = "";
        public string v_FilePath
        {
            get
            {
                return _v_FilePath;
            }
            set 
            {
                _v_FilePath = value;

                v_need_reload = false;                

                if (value != "")
                {                    
                    RTE_Text.LoadDocument(_v_FilePath);
                }
                else
                {
                    RTE_Text.Text = "";
                }                
            }
        }        

        public Mod_Rich_Edit()
        {
            InitializeComponent();
            StateRefresh();
        }

        private void iClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("Очистить текст карточки?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                RTE_Text.ResetText();
        }

        private void iOpen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFile();
        }

        public void OpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Все форматы (*.rtf; *.txt; *.htm; *.html; *.mht; *.odt; *.docx; *.xml)|*.rtf; *.txt; *.htm; *.html; *.mht; *.odt; *.docx; *.xml";
            dlg.Filter += "|Html страница (*.htm; *.html; *.mht)|*.htm; *.html; *.mht";
            dlg.Filter += "|MS Word 2007 (*.docx)|*.docx";                      
            dlg.Filter += "|RTF документ (*.rtf)|*.rtf";
            dlg.Filter += "|Open Office документ(*.odt)|*.odt";
            dlg.Filter += "|Текстовый файл (*.txt)|*.txt";              
            dlg.Filter += "|XML документ (*.xml)|*.xml";            
            dlg.Filter += "|Все файлы *.*|*.*";
            dlg.Title = "Выбор файла";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    RTE_Text.LoadDocument(dlg.FileName);
                }
                catch (Exception exc)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Ошибка загрузки документа\n" +
                        exc.Message,
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );                    
                }                
            }            
        }

        private void StateRefresh()
        {
            iUndo.Enabled = RTE_Text.CanUndo;
            iRedo.Enabled = RTE_Text.CanRedo;
            iClear.Enabled = !RTE_Text.Text.Equals("");
        }

        private void RTE_Text_RtfTextChanged(object sender, EventArgs e)
        {
            StateRefresh();
        }

        private void iUndo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RTE_Text.Undo();
        }

        private void iRedo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RTE_Text.Redo();
        }


        private void iBold_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {                                
            /*RTE_Text.Document.Selection.BeginUpdateDocumen();
            RTE_Text.Document.Selection.EndUpdateDocument();

                //SelectionFont = new Font(SelectFont, rtPadFontStyle());        
            //RTE_Text.Document.Selection.BeginUpdateDocument*/
        }

        private void fileSaveItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveData();
        }

        public bool SaveData()
        {
            string file_name = System.IO.Path.GetFileNameWithoutExtension(v_FilePath);
            string file_dir = System.IO.Path.GetDirectoryName(v_FilePath);
            string new_file_path = file_dir + "\\" + file_name + ".rtf";

            try
            {
                RTE_Text.SaveDocument(new_file_path, DevExpress.XtraRichEdit.DocumentFormat.Rtf);
                file_name = new_file_path;                
            }
            catch 
            {
                XtraMessageBox.Show("Ошибка сохранения данных в файл: \n" + new_file_path, "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            v_need_reload = true;
            return true;
        }

        private void sbtn_Close_Click(object sender, EventArgs e)
        {

        }

        #region Font

        /*
        protected Font SelectFont
        {            
            get
            {
                if (CurrentRichTextBox != null)
                    return CurrentRichTextBox.SelectionFont;
                return null;
            }
        } 

                protected Font SelectFont {
            get {
                if(CurrentRichTextBox != null)
                    return CurrentRichTextBox.SelectionFont;
                return null;
            }
        }

        void ShowFontDialog() {
            if(CurrentRichTextBox == null) return;
            Font dialogFont = null;
            if(SelectFont != null)
                dialogFont = (Font)SelectFont.Clone();
            else dialogFont = CurrentRichTextBox.Font;
            XtraFontDialog dlg = new XtraFontDialog(dialogFont);
            if(dlg.ShowDialog() == DialogResult.OK) {
                CurrentRichTextBox.SelectionFont = dlg.ResultFont;
                beiFontSize.EditValue = dlg.ResultFont.Size;
            }
        }
        private void iFont_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            ShowFontDialog();
        }
        private void iFontColor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            if(CurrentRichTextBox == null) return;
            CurrentRichTextBox.SelectionColor = cp.ResultColor;
        }*/
        #endregion     
    }
}
