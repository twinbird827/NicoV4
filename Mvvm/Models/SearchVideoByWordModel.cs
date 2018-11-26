using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm.Service;

namespace NicoV4.Mvvm.Models
{
    public class SearchVideoByWordModel : SearchVideoModel
    {
        public static SearchVideoByWordModel Instance { get; private set; } = new SearchVideoByWordModel();

        private SearchVideoByWordModel() : base(false)
        {

        }

        /// <summary>
        /// 検索ﾜｰﾄﾞ
        /// </summary>
        public string Word
        {
            get { return _Word; }
            set { SetProperty(ref _Word, value); }
        }
        private string _Word = null;

        /// <summary>
        /// ﾀｸﾞ検索 or ｷｰﾜｰﾄﾞ検索
        /// </summary>
        public bool IsTag
        {
            get { return _IsTag; }
            set { SetProperty(ref _IsTag, value); }
        }
        private bool _IsTag = true;

        /// <summary>
        /// ﾘﾐｯﾄ (何件取得するか)
        /// </summary>
        public int Limit
        {
            get { return _Limit; }
            set { SetProperty(ref _Limit, value); }
        }
        private int _Limit = 100;

        /// <summary>
        /// ｵﾌｾｯﾄ (取得する開始位置)
        /// </summary>
        public int Offset
        {
            get { return _Offset; }
            set { SetProperty(ref _Offset, value); }
        }
        private int _Offset = 0;

        /// <summary>
        /// ｿｰﾄ順
        /// </summary>
        public string OrderBy
        {
            get { return _OrderBy; }
            set { SetProperty(ref _OrderBy, value); }
        }
        private string _OrderBy = null;

        /// <summary>
        /// ﾃﾞｰﾀ件数
        /// </summary>
        public double DataLength
        {
            get { return _DataLength; }
            set { SetProperty(ref _DataLength, value); }
        }
        private double _DataLength = 0;

        public override async Task Reload()
        {
            if (string.IsNullOrWhiteSpace(Word))
            {
                ServiceFactory.MessageService.Error("検索ワードが入力されていません。");
                return;
            }

            Videos.Clear();

            string targets = IsTag ? "tagsExact" : "title,description,tags";
            string q = NicoDataConverter.ToUrlEncode(Word);
            string fields = "contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime,lastCommentTime,lengthSeconds,thumbnailUrl";
            string offset = Offset.ToString();
            string limit = Limit.ToString();
            string context = "kaz.server-on.net/NicoV4";
            string sort = OrderBy;
            string url = String.Format("http://api.search.nicovideo.jp/api/v2/video/contents/search?q={0}&targets={1}&fields={2}&_sort={3}&_offset={4}&_limit={5}&_context={6}",
                q, targets, fields, sort, offset, limit, context
            );

            // TODO 入力ﾁｪｯｸ

            var json = await GetJsonAsync(url);

            Videos.Clear();

            foreach (dynamic data in json["data"])
            {
                var video = VideoStatusModel.Instance.GetVideo(NicoDataConverter.ToId(data["contentId"]));
                video.Title = data["title"];
                video.Description = data["description"];
                video.Tags = data["tags"];
                video.CategoryTag = data["categoryTags"];
                video.ViewCounter = data["viewCounter"];
                video.MylistCounter = data["mylistCounter"];
                video.CommentCounter = data["commentCounter"];
                video.StartTime = NicoDataConverter.ToDatetime(data["startTime"]);
                video.LastCommentTime = NicoDataConverter.ToDatetime(data["lastCommentTime"]);
                video.LengthSeconds = data["lengthSeconds"];
                video.ThumbnailUrl = data["thumbnailUrl"];
                //CommunityIcon = data["communityIcon"]

                // 自身に追加
                Videos.Add(video.VideoId);
            }

            DataLength = json["meta"]["totalCount"];

            ServiceFactory.MessageService.Debug(url);
        }
    }
}
