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
            int returnVal = -1;
            returnVal = string.Compare(((ListViewItem)x).SubItems[idx].Text, ((ListViewItem)y).SubItems[idx].Text);
            if (order == SortOrder.Descending)
                returnVal *= -1;
            return returnVal;
        }
    }
}
