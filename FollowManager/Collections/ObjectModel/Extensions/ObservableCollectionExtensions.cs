using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FollowManager.Collections.ObjectModel.Extensions
{
    public static class ObservableCollectionExtensions
    {
        /// <summary>指定した述語によって定義される条件に一致するすべての要素を削除します。</summary>
        /// <param name="match">
        ///   削除する要素の条件を定義する <see cref="T:System.Predicate`1" /> デリゲート。
        /// </param>
        /// <returns>
        ///   <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> から削除される要素の数。
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="match" /> は <see langword="null" /> です。
        /// </exception>
        public static int RemoveAll<T>(this ObservableCollection<T> observableCollection, Predicate<T> match)
        {
            var list = new List<int>();

            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var removeCounter = 0;

            for (var i = observableCollection.Count - 1; i >= 0; i--)
            {
                if (match(observableCollection[i]))
                {
                    observableCollection.RemoveAt(i);
                    removeCounter++;
                }
            }

            return removeCounter;
        }
    }
}
