using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV4.Mvvm.Views.Main
{
    public enum MenuItemType
    {
        /// <summary>
        /// 文字による検索
        /// </summary>
        SearchByWord = 0,

        /// <summary>
        /// ﾗﾝｷﾝｸﾞ
        /// </summary>
        Ranking = 1,

        /// <summary>
        /// とりあえずﾏｲﾘｽﾄ
        /// </summary>
        Temporary = 2,

        /// <summary>
        /// ﾏｲﾘｽﾄによる検索
        /// </summary>
        SearchByMylist = 8,

        /// <summary>
        /// ﾏｲﾘｽﾄ(自分)
        /// </summary>
        MylistOfMe = 16,

        /// <summary>
        /// ﾏｲﾘｽﾄ(他人)
        /// </summary>
        MylistOfOther = 4,

        /// <summary>
        /// 設定
        /// </summary>
        Setting = 32
    }
}
