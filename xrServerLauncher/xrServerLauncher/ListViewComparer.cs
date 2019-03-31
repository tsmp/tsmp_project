using System;
using System.Collections;
using System.Windows.Forms;

namespace S.E.R.V.E.R___Shadow_Of_Chernobyl_1._0006
{
    public class ListViewComparer : IComparer
    {
        private int idx;
        private SortOrder order;
        public ListViewComparer()
        {
            idx = 0;
            order = SortOrder.Ascending;
        }
        public ListViewComparer(int column, SortOrder order)
        {
            idx = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int rtn = -1;
            try
            {
                rtn = string.Compare(((ListViewItem)x).SubItems[idx].Text, ((ListViewItem)y).SubItems[idx].Text);               
            }
            catch(Exception)
            {
                rtn = 0;
            }
            if (order == SortOrder.Descending)
                rtn *= -1;
            return rtn;
        }
    }
}
