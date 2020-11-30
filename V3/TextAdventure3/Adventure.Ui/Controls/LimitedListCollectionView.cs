using System;
using System.Collections;
using System.Windows.Data;

namespace Adventure.Ui.Controls
{
    public class LimitedListCollectionView : CollectionView, IEnumerable
    {
        public int Limit { get; set; }

        public override int Count => Math.Min(base.Count, Limit);

        public LimitedListCollectionView(IEnumerable list)
            : base(list) =>
            Limit = int.MaxValue;

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            do
                yield return CurrentItem;
            while (MoveCurrentToNext());
        }

        #endregion

        public override bool MoveCurrentToLast() => base.MoveCurrentToPosition(Count - 1);

        public override bool MoveCurrentToNext() => base.CurrentPosition == Count - 1 ? base.MoveCurrentToPosition(base.Count) : base.MoveCurrentToNext();

        public override bool MoveCurrentToPrevious() => base.IsCurrentAfterLast ? base.MoveCurrentToPosition(Count - 1) : base.MoveCurrentToPrevious();

        public override bool MoveCurrentToPosition(int position) => base.MoveCurrentToPosition(position < Count ? position : base.Count);
    }
}