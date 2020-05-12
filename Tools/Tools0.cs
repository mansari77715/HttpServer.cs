using HttpUI.Server;
using System.Net;

namespace HttpUI.Tools
{
    public static class Tools0
    {

        public delegate void OnResult(HttpListenerContext responce, HttpData datas);
        public delegate void OnResult0(string path);
        public delegate bool OnResult1(HttpListenerContext responce, HttpData datas);

    }

}