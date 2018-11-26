using NicoV4.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV4.Mvvm.Models
{
    public class SearchVideoByTemporaryModel : SearchVideoModel
    {
        public static SearchVideoByTemporaryModel Instance { get; private set; } = new SearchVideoByTemporaryModel();

        private SearchVideoByTemporaryModel() : base(true)
        {
            Reload().ConfigureAwait(false);
        }

        public override async Task Reload()
        {
            const string url = "http://www.nicovideo.jp/api/deflist/list";

            var json = await GetJsonAsync(url);

            Videos.Clear();

            foreach (var item in json["mylistitem"])
            {
                var video = VideoStatusModel.Instance.GetVideo((string)item["item_id"]);

                video.VideoUrl = item["item_id"];
                video.Title = item["item_data"]["title"];
                video.Description = item["description"];
                //video.Tags = data["tags"];
                //video.CategoryTag = data["categoryTags"];
                video.ViewCounter = long.Parse(item["item_data"]["view_counter"]);
                video.MylistCounter = long.Parse(item["item_data"]["mylist_counter"]);
                video.CommentCounter = long.Parse(item["item_data"]["num_res"]);
                video.StartTime = NicoDataConverter.FromUnixTime((long)item["create_time"]);
                //video.LastCommentTime = Converter.item(data["lastCommentTime"]);
                video.LengthSeconds = long.Parse(item["item_data"]["length_seconds"]);
                video.ThumbnailUrl = item["item_data"]["thumbnail_url"];
                video.LastResBody = item["item_data"]["last_res_body"];
                //video.CommunityIcon = data["communityIcon"];

                Videos.Add(video.VideoId);
            }
        }

        public async Task AddVideo(string id)
        {

        }

        public async Task DeleteVideo(string id)
        {

        }
    }
}
