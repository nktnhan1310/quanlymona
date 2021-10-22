using QuanLy.Entities.Newfeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLy.Core.Hubs
{
    public interface IPostCommentHub
    {
        Task BroadcastMessage(int postComments);
    }
}
