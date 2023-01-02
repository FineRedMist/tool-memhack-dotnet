using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MemHack2
{
    /// <summary>
    /// From http://www.wpf-tutorial.com/listview-control/listview-how-to-column-sorting/
    /// </summary>
    public class SortState
    {
        private GridViewColumnHeader? mSortColumn = null;
        private SortAdorner? mSortAdorner = null;

        public void OnColumnHeader_Click(ListView listView, object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader? column = (sender as GridViewColumnHeader);
            if(column == null) 
            {
                return;
            }
            string? sortBy = column?.Tag.ToString();
            if (mSortColumn != null)
            {
                AdornerLayer.GetAdornerLayer(mSortColumn).Remove(mSortAdorner);
                listView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (mSortColumn == column && mSortAdorner != null && mSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            mSortColumn = column;
            mSortAdorner = new SortAdorner(column!, newDir);
            AdornerLayer.GetAdornerLayer(mSortColumn).Add(mSortAdorner);
            listView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }


    public class SortAdorner : Adorner
    {
        private static readonly Geometry ascGeometry =
                Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static readonly Geometry descGeometry =
                Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform
                    (
                            AdornedElement.RenderSize.Width - 15,
                            (AdornedElement.RenderSize.Height - 5) / 2
                    );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }
    }
}
