using Aki.Launcher.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Aki.Launcher.MiniCommon
{

    public static class ImageRequest
	{
		public static string ImageCacheFolder = $"{LauncherSettingsProvider.Instance.GamePath}\\Aki_Data\\Launcher\\Image_Cache";

		private static List<string> CachedRoutes = new List<string>();

		private static string LauncherRoute = "/files/launcher/";
		public static void CacheBackgroundImage() => CacheImage($"{LauncherRoute}bg.png", Path.Combine(ImageCacheFolder, "bg.png"));
		public static void CacheSideImage(string Side)
		{
			if (Side == null || string.IsNullOrWhiteSpace(Side) || Side.ToLower() == "unknown") return;

			string SideImagePath = Path.Combine(ImageCacheFolder, "side.png");

			CacheImage($"{LauncherRoute}side_{Side.ToLower()}.png", SideImagePath);
		}

		private static void CacheImage(string route, string filePath)
		{
			Directory.CreateDirectory(ImageCacheFolder);

			if (String.IsNullOrWhiteSpace(route) || CachedRoutes.Contains(route)) //Don't want to request the image if it was already cached.
			{
				return;
			}

			string sessionID = null;
			string remote = "https://127.0.0.1";
			string type = "GET";
			string data = null;
			bool compress = false;

			using Stream s = new Request(sessionID, remote).Send(route, type, data, compress);

			using MemoryStream ms = new MemoryStream();

			s.CopyTo(ms);

			if (ms.Length == 0) return;

			using Image image = Image.FromStream(ms);

			image.Save(filePath);

			CachedRoutes.Add(route);
		}
	}
}
