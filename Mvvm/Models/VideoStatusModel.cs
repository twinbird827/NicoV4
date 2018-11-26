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
    public class VideoStatusModel : BindableBase
    {
        public static VideoStatusModel Instance { get; private set; } = JsonConverter.Deserialize<VideoStatusModel>(Variables.VideoStatusPath);

        private VideoStatusModel()
        {
            NewVideos = new List<string>();
            SeeVideos = new List<string>();
        }

        // ****************************************************************************************************
        // ﾌﾟﾛﾊﾟﾃｨ定義
        // ****************************************************************************************************

        /// <summary>
        /// ｱｲﾃﾑ構成
        /// </summary>
        public ObservableSynchronizedCollection<VideoModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<VideoModel> _Videos = new ObservableSynchronizedCollection<VideoModel>();

        /// <summary>
        /// NEWﾘｽﾄ
        /// </summary>
        public List<string> NewVideos { get; set; }

        /// <summary>
        /// SEEﾘｽﾄ
        /// </summary>
        public List<string> SeeVideos { get; set; }

        // ****************************************************************************************************
        // 公開ﾒｿｯﾄﾞ定義
        // ****************************************************************************************************

        public VideoModel GetVideo(string id)
        {
            var video = Videos.FirstOrDefault(v => v.VideoId == id);

            if (video == null)
            {
                video = new VideoModel()
                {
                    VideoId = id
                };
                Videos.Add(video);
            }

            return video;
        }

        protected override void OnDisposed()
        {
            // ﾌﾟﾛﾊﾟﾃｨに設定された内容を外部ﾌｧｲﾙに保存します。
            JsonConverter.Serialize(Variables.VideoStatusPath, this);

            Videos = null;
            NewVideos = null;
            SeeVideos = null;

            base.OnDisposed();
        }
    }
}
