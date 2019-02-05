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
    public class ComboRankCategoryModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboRankCategoryModel Instance { get; } = new ComboRankCategoryModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableSynchronizedCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableSynchronizedCollection<ComboboxItemModel> _Items;

        private ComboRankCategoryModel()
        {
            _Items = new ObservableSynchronizedCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "all", Description = Resources.RANK_CATEGORY_ALL },
                new ComboboxItemModel() { Value = "music", Description = Resources.RANK_CATEGORY_MUSIC },
                new ComboboxItemModel() { Value = "ent", Description = Resources.RANK_CATEGORY_ENT },
                new ComboboxItemModel() { Value = "anime", Description = Resources.RANK_CATEGORY_ANIME },
                new ComboboxItemModel() { Value = "game", Description = Resources.RANK_CATEGORY_GAME },
                new ComboboxItemModel() { Value = "animal", Description = Resources.RANK_CATEGORY_ANIMAL },
                new ComboboxItemModel() { Value = "que", Description = Resources.RANK_CATEGORY_QUE },
                new ComboboxItemModel() { Value = "radio", Description = Resources.RANK_CATEGORY_RADIO },
                new ComboboxItemModel() { Value = "sport", Description = Resources.RANK_CATEGORY_SPORT },
                new ComboboxItemModel() { Value = "politics", Description = Resources.RANK_CATEGORY_POLITICS },
                new ComboboxItemModel() { Value = "chat", Description = Resources.RANK_CATEGORY_CHAT },
                new ComboboxItemModel() { Value = "science", Description = Resources.RANK_CATEGORY_SCIENCE },
                new ComboboxItemModel() { Value = "history", Description = Resources.RANK_CATEGORY_HISTORY },
                new ComboboxItemModel() { Value = "cooking", Description = Resources.RANK_CATEGORY_COOKING },
                new ComboboxItemModel() { Value = "nature", Description = Resources.RANK_CATEGORY_NATURE },
                new ComboboxItemModel() { Value = "diary", Description = Resources.RANK_CATEGORY_DIARY },
                new ComboboxItemModel() { Value = "dance", Description = Resources.RANK_CATEGORY_DANCE },
                new ComboboxItemModel() { Value = "sing", Description = Resources.RANK_CATEGORY_SING },
                new ComboboxItemModel() { Value = "play", Description = Resources.RANK_CATEGORY_PLAY },
                new ComboboxItemModel() { Value = "lecture", Description = Resources.RANK_CATEGORY_LECTURE },
                new ComboboxItemModel() { Value = "tw", Description = Resources.RANK_CATEGORY_TW },
                new ComboboxItemModel() { Value = "other", Description = Resources.RANK_CATEGORY_OTHER },
                new ComboboxItemModel() { Value = "test", Description = Resources.RANK_CATEGORY_TEST },
                new ComboboxItemModel() { Value = "r18", Description = Resources.RANK_CATEGORY_R18 },
            };
        }
    }
}
