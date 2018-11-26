using NicoV4.Properties;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.ComboItems
{
    public class ComboSortVideoModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboSortVideoModel Instance { get; } = new ComboSortVideoModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableSynchronizedCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableSynchronizedCollection<ComboboxItemModel> _Items;

        private ComboSortVideoModel()
        {
            _Items = new ObservableSynchronizedCollection<ComboboxItemModel>
            {
                //new SortItemModel() { Keyword = "-title", Description = "-title" },
                //new SortItemModel() { Keyword = "%2btitle", Description = "+title" },
                //new SortItemModel() { Keyword = "-description", Description = "-description" },
                //new SortItemModel() { Keyword = "%2bdescription", Description = "+description" },
                //new SortItemModel() { Keyword = "-tags", Description = "-tags" },
                //new SortItemModel() { Keyword = "%2btags", Description = "+tags" },
                //new SortItemModel() { Keyword = "-categoryTags", Description = "-categoryTags" },
                //new SortItemModel() { Keyword = "%2bcategoryTags", Description = "+categoryTags" },
                new ComboboxItemModel() { Value = "-viewCounter", Description = Resources.SORT_VIEWRES0 },
                new ComboboxItemModel() { Value = "%2bviewCounter", Description = Resources.SORT_VIEWRES1 },
                new ComboboxItemModel() { Value = "-mylistCounter", Description = Resources.SORT_MYREGRES0 },
                new ComboboxItemModel() { Value = "%2bmylistCounter", Description = Resources.SORT_MYREGRES1 },
                new ComboboxItemModel() { Value = "-commentCounter", Description = Resources.SORT_COMMENTRES0 },
                new ComboboxItemModel() { Value = "%2bcommentCounter", Description = Resources.SORT_COMMENTRES1 },
                new ComboboxItemModel() { Value = "-startTime", Description = Resources.SORT_UPLOAD1 },
                new ComboboxItemModel() { Value = "%2bstartTime", Description = Resources.SORT_UPLOAD0 },
                new ComboboxItemModel() { Value = "-lastCommentTime", Description = Resources.SORT_COMMENT0 },
                new ComboboxItemModel() { Value = "%2blastCommentTime", Description = Resources.SORT_COMMENT1 },
                new ComboboxItemModel() { Value = "-lengthSeconds", Description = Resources.SORT_LENGTH0 },
                new ComboboxItemModel() { Value = "%2blengthSeconds", Description = Resources.SORT_LENGTH1 }
            };
        }
    }
}
