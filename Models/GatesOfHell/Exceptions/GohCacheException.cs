using System.Text;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohCacheException : GohException {
        private static readonly string s_message = "Load cache {0}error.";

        protected override byte ExceptionTypeCode => 7;

        public GohCacheException(string? message = null, string? cacheName = null, Exception? inner = null) : base(GetFullErrorMessage(message, cacheName), inner) { }

        public static GohCacheException CacheNotFount(string cacheName) {
            return new GohCacheException("Cache not found.", cacheName);
        }

        private static string GetFullErrorMessage(string? message = null, string? cacheName = null) {
            var fullMessage = new StringBuilder();

            if (cacheName != null) {
                fullMessage.Append(string.Format(s_message, $"\"{cacheName}\" "));
            } else {
                fullMessage.Append(s_message);
            }

            if (message != null) {
                fullMessage.Append(' ');
                fullMessage.Append(message);
            }

            return fullMessage.ToString();
        }
    }
}
