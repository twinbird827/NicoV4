using NicoV4.Common;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV4.Mvvm.Models
{
    public class MylistStatusModel : BindableBase
    {
        public static MylistStatusModel Instance { get; private set; } = JsonConverter.Deserialize<MylistStatusModel>(Variables.MylistStatusPath);

        private MylistStatusModel()
        {
            Favorites = new List<string>();
        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        /// <summary>
        /// ｱｲﾃﾑ構成
        /// </summary>
        public ObservableSynchronizedCollection<SearchVideoByMylistModel> Mylists
        {
            get { return _Mylists; }
            set { SetProperty(ref _Mylists, value); }
        }
        private ObservableSynchronizedCollection<SearchVideoByMylistModel> _Mylists = new ObservableSynchronizedCollection<SearchVideoByMylistModel>();

        /// <summary>
        /// SEEﾘｽﾄ
        /// </summary>
        public List<string> Favorites { get; set; }

        public void AddFavorites(string id)
        {
            if (!Favorites.Any(f => f == id))
            {
                Favorites.Add(id);
            }
        }

        protected override void OnDisposed()
        {
            // ﾌﾟﾛﾊﾟﾃｨに設定された内容を外部ﾌｧｲﾙに保存します。
            JsonConverter.Serialize(Variables.MylistStatusPath, this);

            Mylists = null;
            Favorites = null;

            base.OnDisposed();
        }
    }
}
