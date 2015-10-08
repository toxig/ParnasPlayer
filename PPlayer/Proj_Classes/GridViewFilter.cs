using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Paint;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;

namespace PPlayer
{
    public class GridViewFilterHelper
    {
        private string _ActiveFilter = string.Empty;
        private GridView _View;
        XPaint paint = new XPaint();
        private string[] FilterText;

        public bool Filter_words = false;
        public bool Filter_Plus = false;
        public string ActiveFilter
        {
            get { return _ActiveFilter; }
            set
            {                
                _ActiveFilter = value;
                if (Filter_words) FilterText = _ActiveFilter.Split(' ');
                OnActiveFilterChanged();                
            }
        }

        public GridViewFilterHelper(GridView view)
        {
            _View = view;
            _View.CustomDrawCell += new RowCellCustomDrawEventHandler(_View_CustomDrawCell);
        }

        void _View_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (_ActiveFilter == string.Empty) return;

            if (!Filter_words) // точное совпадение
            {
                if (e.DisplayText.IndexOf(_ActiveFilter) < 0) return;
                DrawSelect(sender, e, _ActiveFilter);
            }
            else // все слова через пробел
            {
                for (int i = 0; i < FilterText.Length; i++)
                {
                    DrawSelect(sender, e, FilterText[i]);
                }                
            }
        }

        void DrawSelect(object sender, RowCellCustomDrawEventArgs e, string text)
        {
            int index = e.DisplayText.IndexOf(text);
            e.Handled = true;
            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            MultiColorDrawStringParams args = new MultiColorDrawStringParams(e.Appearance);
            args.Bounds = e.Bounds;
            args.Text = e.DisplayText;
            args.Appearance.Assign(e.Appearance);
            AppearanceObject apperance = _View.PaintAppearance.SelectedRow;
            CharacterRangeWithFormat defaultRange = new CharacterRangeWithFormat(0, e.DisplayText.Length, e.Appearance.ForeColor, e.Appearance.BackColor);
            CharacterRangeWithFormat range = new CharacterRangeWithFormat(index, text.Length, apperance.ForeColor, apperance.BackColor);
            args.Ranges = new CharacterRangeWithFormat[] { defaultRange, range };
            paint.MultiColorDrawString(e.Cache, args);
        }

        CriteriaOperator CreateFilterCriteria()
        {
            if (!Filter_words)
            { // точное совпадение
                CriteriaOperator[] operators = new CriteriaOperator[_View.VisibleColumns.Count];
                for (int i = 0; i < _View.VisibleColumns.Count; i++)
                {
                    if (Filter_Plus)
                        operators[i] = new BinaryOperator(_View.VisibleColumns[i].FieldName, String.Format("%{0}%", _ActiveFilter), BinaryOperatorType.Like);
                    else
                        operators[i] = new BinaryOperator(_View.VisibleColumns[i].FieldName, String.Format("{0}%", _ActiveFilter), BinaryOperatorType.Like);
                }
                return new GroupOperator(GroupOperatorType.Or, operators);
            }
            else
            { // любое слово                                                
                CriteriaOperator[] levels = new CriteriaOperator[FilterText.Length];

                for (int i = 0; i < FilterText.Length; i++)
                {
                    CriteriaOperator[] operators = new CriteriaOperator[_View.VisibleColumns.Count + 1]; // Тэги + 1
                    for (int j = 0; j < _View.VisibleColumns.Count; j++)
                    {
                        operators[j] = new BinaryOperator(_View.VisibleColumns[j].FieldName, String.Format("%{0}%", FilterText[i]), BinaryOperatorType.Like);
                    }
                    // Тэги
                    operators[_View.VisibleColumns.Count] = new BinaryOperator("TagComments", String.Format("%{0}%", FilterText[i]), BinaryOperatorType.Like);

                    levels[i] = new GroupOperator(GroupOperatorType.Or, operators);
                }
                return new GroupOperator(GroupOperatorType.And, levels);
            }
        }

        private void OnActiveFilterChanged()
        {
            if (_ActiveFilter != "")
            {
                _View.ActiveFilterCriteria = CreateFilterCriteria();
                if (!Filter_Plus) _View.ActiveFilter.Clear();
            }
            else
                _View.ActiveFilter.Clear();
        }
    }
}


