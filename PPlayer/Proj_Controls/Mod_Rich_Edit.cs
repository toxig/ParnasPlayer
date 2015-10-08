using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;


namespace PPlayer
{
    public partial class Mod_Rich_Edit : DevExpress.XtraEditors.XtraUserControl
    {
        public bool v_need_reload = false;
        public bool v_need_save = false;
        private int v_changes_count;
        private int v_changes_saved;

        private string v_FilePath_new = "";

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

                if (value != "")
                {                    
                    RTE_Text.LoadDocument(_v_FilePath);                    
                }
                else
                {
                    RTE_Text.Text = "";
                }

                v_need_reload = false; 
                v_need_save = false;
                v_changes_count = 0;
                v_changes_saved = 0;
                StateRefresh();
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
                    v_FilePath_new = dlg.FileName;
                    RTE_Text.LoadDocument(v_FilePath_new);                    
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
                    v_FilePath_new = "";
                }                
            }            
        }

        private void StateRefresh()
        {
            iUndo.Enabled = RTE_Text.CanUndo;            
            iRedo.Enabled = RTE_Text.CanRedo;
            iClear.Enabled = !RTE_Text.Text.Equals("");

            if (
                    (
                    (!RTE_Text.CanUndo && !RTE_Text.CanRedo)
                    ||
                    (!RTE_Text.CanUndo && v_changes_saved == 0)
                    || 
                    (v_changes_saved == v_changes_count)
                    ) 
                    &&
                    (v_FilePath_new == "")
                )
            { // нет изменений
                v_need_save = false;
                iSave.Enabled = false;
            }
            else
            { // есть изменения
                v_need_save = true;
                iSave.Enabled = true;                
            }
        }

        private void RTE_Text_RtfTextChanged(object sender, EventArgs e)
        {
            v_changes_count++;
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
            StateRefresh();
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

            if (!RTE_Text.CanUndo) v_changes_count = 0;
            v_changes_saved = v_changes_count;

            v_FilePath_new = "";

            v_need_reload = true;
            v_need_save = false;            

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
