using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DesktopClient
{
    public class TouchGrid
    {
        private readonly DataGridView _View;
        int startDragRowHandle = -1;
        int topRowIndex = -1;
        private bool _IsDragging;

        public bool IsDragging
        {
            get { return _IsDragging; }
            set { _IsDragging = value; }
        }
        public TouchGrid(DataGridView view)
        {
            _View = view;
            InitViewProperties();
        }
        private void InitViewProperties()
        {
            _View.Cursor = Cursors.Hand;
            _View.MultiSelect = false;
            _View.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _View.MouseDown += _View_MouseDown;
            _View.MouseMove += _View_MouseMove;
            _View.MouseUp += _View_MouseUp;
            _View.Layout += new LayoutEventHandler(_View_Layout);
            _View.DataError += new DataGridViewDataErrorEventHandler(_View_DataError);
        }

        private void _View_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        DataGridView.HitTestInfo myHitTest;
        private int GetRowUnderCursor(Point location)
        {
            myHitTest = _View.HitTest(location.X, location.Y);
            return myHitTest.RowIndex;
        }
        void _View_Layout(object sender, LayoutEventArgs e)
        {
            try
            {
                IsDragging = false;
            }
            catch { }
        }

        private void DoScroll(int delta)
        {
            if (delta == 0)
                return;
            if (delta < 0 && _View.FirstDisplayedScrollingRowIndex == 0)
                delta = 0;
            _View.FirstDisplayedScrollingRowIndex += delta;
        }

        private void _View_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                IsDragging = false;
            }
            catch { }
        }
        private void _View_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (IsDragging)
                {
                    int newRow = GetRowUnderCursor(e.Location);
                    if (newRow < 0)
                        return;
                    int delta = startDragRowHandle - newRow;
                    DoScroll(delta);
                }
            }
            catch { }
        }

        private void _View_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                IsDragging = true;
                startDragRowHandle = GetRowUnderCursor(e.Location);
                topRowIndex = _View.FirstDisplayedScrollingRowIndex;
            }
            catch { }
        }
    }
}
