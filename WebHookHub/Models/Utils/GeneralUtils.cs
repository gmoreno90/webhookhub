﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace WebHookHub.Models.Utils
{
    public static class GeneralUtils
    {
        public static string getVersionImage(string strVersion)
        {
            string svgContent = "<svg width=\"89.5\" height=\"20.0\" xmlns=\"http://www.w3.org/2000/svg\">" +
                       "  <linearGradient id=\"a\" x2=\"0\" y2=\"100%\">" +
                       "    <stop offset=\"0.0\" stop-opacity=\"0.0\" stop-color=\"#000\" />" +
                       "    <stop offset=\"1.0\" stop-opacity=\"0.2\" stop-color=\"#000\" />" +
                       "  </linearGradient>" +
                       "  <clipPath id=\"c\">" +
                       "    <rect width=\"89.5\" height=\"20.0\" rx=\"3.0\" />" +
                       "  </clipPath>" +
                       "  <g clip-path=\"url(#c)\">" +
                       "    <rect width=\"21.5\" height=\"20.0\" fill=\"#555555\" />" +
                       "    <rect width=\"68.9\" height=\"20.0\" fill=\"#4EC820\" x=\"21.6\" />" +
                       "    <rect width=\"89.5\" height=\"20.0\" fill=\"url(#a)\" />" +
                       "  </g>" +
                       "  <svg width=\"12\" height=\"12\" viewBox=\"0 0 12 12\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\" x=\"5\" y=\"4\">" +
                       "    <g>" +
                       "      <path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M0 9H1V11H3L3 12H0V9Z\" fill=\"#fff\" />" +
                       "      <path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M0.666656 4H3.7352L6.20309 0.444336C6.38861 0.166992 6.70068 0 7.03479 0H11.5C11.7762 0 12 0.224609 12 0.5V4.96484C12 5.29883 11.8332 5.61133 11.5553 5.79688L8 8.26465V11.333C8 11.7012 7.70154 12 7.33334 12H5L4 11L5.25 9.75L4.25 8.75L2.99997 10L1.99997 9L3.25 7.75L2.25 6.75L1 8L0 7V4.66699C0 4.29883 0.298462 4 0.666656 4ZM10.5 3C10.5 3.82812 9.82843 4.5 9.00003 4.5C8.1716 4.5 7.50003 3.82812 7.50003 3C7.50003 2.17188 8.1716 1.5 9.00003 1.5C9.82843 1.5 10.5 2.17188 10.5 3Z\" fill=\"#fff\" />" +
                       "    </g>" +
                       "  </svg>" +
                       "  <g fill=\"#fff\" text-anchor=\"middle\" font-family=\"DejaVu Sans,Verdana,Geneva,sans-serif\" font-size=\"11\">" +
                       "    <text x=\"55.3\" y=\"15.0\" fill=\"#000\" fill-opacity=\"0.3\">" + strVersion + "</text>" +
                       "    <text x=\"55.3\" y=\"14.0\" fill=\"#fff\">" + strVersion + "</text>" +
                       "  </g>" +
                       "</svg>";
            return svgContent;
        }
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
