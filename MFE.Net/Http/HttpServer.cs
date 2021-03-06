﻿using MFE.Core;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace MFE.Net.Http
{
    public delegate void RequestEventHandler(HttpListenerRequest request);
    public delegate void ResponseEventHandler(HttpListenerResponse response);
    public delegate void GETRequestEventHandler(string path, Hashtable parameters, HttpListenerResponse response);

    public class HttpServer
    {
        #region Fields
        private static Encoding encoder = Encoding.UTF8;
        private static char[] fwdSlashDelim = { '\\' };
        private HttpListener listener;
        private bool isStopped = true;

        #region Emulator Certificate
        // The following data is a self-signed certificate in .PFX format that is used by this sample when deployed to the emulator.
        private static byte[] m_emulatorCertData = new byte[] {
        0x30, 0x82, 0x0a, 0x73, 0x02, 0x01, 0x03, 0x30, 0x82, 0x0a, 
        0x33, 0x06, 0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 
        0x07, 0x01, 0xa0, 0x82, 0x0a, 0x24, 0x04, 0x82, 0x0a, 0x20, 
        0x30, 0x82, 0x0a, 0x1c, 0x30, 0x82, 0x06, 0x15, 0x06, 0x09, 
        0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x07, 0x01, 0xa0, 
        0x82, 0x06, 0x06, 0x04, 0x82, 0x06, 0x02, 0x30, 0x82, 0x05, 
        0xfe, 0x30, 0x82, 0x05, 0xfa, 0x06, 0x0b, 0x2a, 0x86, 0x48, 
        0x86, 0xf7, 0x0d, 0x01, 0x0c, 0x0a, 0x01, 0x02, 0xa0, 0x82, 
        0x04, 0xfe, 0x30, 0x82, 0x04, 0xfa, 0x30, 0x1c, 0x06, 0x0a, 
        0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x0c, 0x01, 0x03, 
        0x30, 0x0e, 0x04, 0x08, 0x10, 0x8f, 0xb2, 0x4b, 0x95, 0x7e, 
        0x94, 0x37, 0x02, 0x02, 0x07, 0xd0, 0x04, 0x82, 0x04, 0xd8, 
        0xfc, 0xc9, 0xde, 0x58, 0x78, 0x73, 0x14, 0xa8, 0xf8, 0xd6, 
        0xed, 0x28, 0x53, 0x9e, 0x7a, 0xd3, 0x31, 0x33, 0x7c, 0xea, 
        0xe9, 0x1a, 0xdd, 0x46, 0x62, 0x66, 0x65, 0xf7, 0xbb, 0x8f, 
        0xfd, 0xb2, 0xd0, 0x81, 0x68, 0xfe, 0x24, 0x51, 0x30, 0xa7, 
        0xf3, 0x45, 0x90, 0x82, 0x21, 0xfe, 0x49, 0x32, 0xdc, 0xe2, 
        0x51, 0xf0, 0x5a, 0xec, 0x05, 0xee, 0x48, 0x36, 0x94, 0x6b, 
        0xc5, 0xa2, 0x07, 0x24, 0xaf, 0xec, 0x6f, 0x4b, 0xb5, 0x7e, 
        0x9d, 0x15, 0x54, 0xbc, 0x2d, 0x1f, 0xe1, 0x63, 0x87, 0x11, 
        0x7e, 0xa7, 0x45, 0x61, 0x52, 0x67, 0xbc, 0xb1, 0x64, 0x1b, 
        0xa2, 0x18, 0xb5, 0x1d, 0xf5, 0xca, 0x48, 0x7a, 0xf7, 0x95, 
        0xce, 0x78, 0x00, 0x2b, 0x78, 0x61, 0x98, 0x07, 0xff, 0xab, 
        0x29, 0xc6, 0xc7, 0x05, 0x69, 0x33, 0x68, 0xad, 0x9b, 0xc7, 
        0x64, 0x38, 0x56, 0x6c, 0x43, 0xd9, 0x24, 0xfd, 0x6c, 0x9e, 
        0xbd, 0xd3, 0xe3, 0x77, 0x7b, 0xe6, 0x33, 0x9b, 0xa9, 0x49, 
        0x14, 0xab, 0xa2, 0x62, 0x75, 0xe8, 0xe8, 0xd7, 0xe4, 0x59, 
        0x66, 0x8d, 0xcc, 0x67, 0xcc, 0x57, 0x90, 0x4c, 0x52, 0x5a, 
        0x56, 0x8d, 0xea, 0x79, 0xc8, 0x5a, 0x2d, 0xc1, 0xdb, 0xd4, 
        0x68, 0x34, 0x8d, 0x6c, 0xbf, 0x4c, 0x5f, 0xa9, 0x0c, 0x58, 
        0x3e, 0xc2, 0x68, 0x89, 0x91, 0xee, 0xc5, 0xcb, 0x13, 0x38, 
        0x5f, 0x9c, 0x90, 0x3f, 0x0a, 0x28, 0x03, 0xca, 0xa1, 0x60, 
        0xa8, 0xfd, 0x96, 0xa3, 0x02, 0xf1, 0xd8, 0x51, 0x46, 0x9d, 
        0xec, 0xb9, 0xbb, 0x9e, 0x35, 0x02, 0x8a, 0x63, 0x15, 0xfb, 
        0xe5, 0xdc, 0x25, 0xeb, 0xc6, 0xe7, 0x71, 0xeb, 0xcf, 0x4e, 
        0xc3, 0x30, 0xf8, 0xc9, 0x77, 0xc8, 0xb2, 0x74, 0x68, 0x37, 
        0xa9, 0x13, 0x30, 0x86, 0x20, 0x0c, 0xb6, 0x30, 0x96, 0x78, 
        0x25, 0xb5, 0xc0, 0x49, 0x1e, 0xda, 0xa2, 0x4b, 0xb2, 0xbb, 
        0xae, 0x77, 0x95, 0x1f, 0x97, 0x45, 0x08, 0x24, 0x1c, 0xf7, 
        0xfc, 0x36, 0xee, 0x4a, 0xbf, 0xc0, 0x60, 0x16, 0x13, 0x1b, 
        0x25, 0xe8, 0x7f, 0x97, 0x46, 0x70, 0xf6, 0x75, 0xd8, 0x6c, 
        0xbc, 0xf6, 0xe1, 0x7e, 0xe5, 0xb3, 0x83, 0x69, 0xa4, 0xd3, 
        0x4a, 0xe5, 0xef, 0x19, 0x1c, 0x71, 0xc3, 0xd2, 0x09, 0x12, 
        0xb0, 0xed, 0xe8, 0x0f, 0xb9, 0x38, 0x1e, 0x23, 0x8c, 0xce, 
        0x68, 0x62, 0x7c, 0x42, 0xce, 0xec, 0xd4, 0x38, 0x05, 0x9f, 
        0x1c, 0x2c, 0xb1, 0x7c, 0x3e, 0xfb, 0xc4, 0x00, 0xc8, 0xca, 
        0xa9, 0xbc, 0xbd, 0xee, 0xc6, 0xe4, 0xec, 0xff, 0x82, 0xb1, 
        0x89, 0xe6, 0x5b, 0xab, 0x57, 0x9e, 0x19, 0x51, 0x90, 0x21, 
        0x76, 0xcb, 0x57, 0xa9, 0x38, 0x26, 0x38, 0x55, 0xa8, 0x63, 
        0x8c, 0xa3, 0x54, 0x32, 0x1f, 0xa0, 0x23, 0x0e, 0x58, 0x32, 
        0x84, 0xc6, 0x54, 0xf5, 0x7b, 0x70, 0xd6, 0xbc, 0xc3, 0x63, 
        0x9b, 0x43, 0x49, 0x44, 0x9f, 0x26, 0x90, 0x9c, 0x6a, 0x93, 
        0x99, 0x20, 0xc2, 0x60, 0x51, 0x8a, 0x0d, 0x0c, 0x29, 0xe1, 
        0x37, 0x6d, 0xab, 0x77, 0x8d, 0xd3, 0x9d, 0xac, 0xc9, 0xa2, 
        0xc0, 0x41, 0x9d, 0x07, 0xaf, 0xda, 0x84, 0x5d, 0xc4, 0x18, 
        0x43, 0x10, 0xce, 0x48, 0xb7, 0xf7, 0x6c, 0x77, 0x5d, 0x2c, 
        0xd3, 0x88, 0x72, 0xf1, 0xe3, 0x10, 0x80, 0xbc, 0x5d, 0xf5, 
        0x02, 0xc9, 0x0e, 0xfb, 0xfb, 0xf8, 0xfb, 0x83, 0xe1, 0xa6, 
        0x4a, 0xb5, 0xae, 0xb7, 0x4e, 0xe3, 0xbe, 0xe9, 0xd4, 0xf4, 
        0xd3, 0x0c, 0x41, 0x80, 0x99, 0xce, 0x55, 0x3c, 0x4f, 0x35, 
        0x41, 0x7e, 0x4c, 0x1d, 0x97, 0x9c, 0x48, 0xde, 0xb5, 0xe5, 
        0x23, 0x34, 0xb2, 0x7c, 0xfa, 0xe1, 0x29, 0x75, 0xb6, 0x3b, 
        0x34, 0x51, 0x0f, 0xe9, 0xc9, 0x55, 0xe3, 0x1d, 0xb5, 0xf4, 
        0x1f, 0xed, 0xf6, 0xf0, 0x2b, 0xa2, 0x03, 0x95, 0x6e, 0xe9, 
        0xc6, 0xbe, 0x1a, 0x81, 0x39, 0x33, 0x74, 0xd8, 0xf2, 0x72, 
        0xc1, 0x36, 0x4b, 0x1d, 0x3c, 0x09, 0xd2, 0x43, 0x0c, 0x88, 
        0x5b, 0x95, 0xca, 0xa0, 0x20, 0x0f, 0x52, 0x8d, 0x98, 0x6a, 
        0x87, 0xe7, 0x87, 0x6d, 0xed, 0x95, 0x14, 0xab, 0xa2, 0xf1, 
        0x9b, 0x45, 0xe7, 0x78, 0x4f, 0x8b, 0x27, 0x1d, 0xa9, 0x17, 
        0x42, 0x30, 0x85, 0x77, 0x2e, 0xd6, 0x02, 0x7e, 0x95, 0xda, 
        0xcf, 0x42, 0xbd, 0xec, 0x96, 0x25, 0xdb, 0xc8, 0x9b, 0x2a, 
        0x7c, 0xe4, 0x00, 0x03, 0x70, 0xf4, 0x93, 0x1d, 0x8b, 0x45, 
        0x19, 0x5b, 0xf5, 0xbc, 0x6a, 0x1f, 0xaf, 0xe1, 0x8d, 0x4e, 
        0x6d, 0x0e, 0xef, 0x78, 0x34, 0xf4, 0x24, 0xcb, 0x9b, 0x05, 
        0xf0, 0xcc, 0x9e, 0x5e, 0x53, 0x8a, 0xba, 0xc4, 0xf7, 0xbf, 
        0xf5, 0x13, 0xe0, 0x00, 0x51, 0xea, 0x22, 0xa4, 0xec, 0x24, 
        0x83, 0x88, 0x74, 0xc7, 0x39, 0xe0, 0xb2, 0xa6, 0x13, 0xc8, 
        0xdb, 0xad, 0xe6, 0x09, 0x01, 0x1b, 0x0a, 0x79, 0xb1, 0xc3, 
        0xbc, 0x8c, 0x45, 0xd7, 0x8a, 0x66, 0x04, 0x90, 0xb3, 0x7b, 
        0x75, 0x24, 0xbe, 0xa4, 0x61, 0xdc, 0x0a, 0x46, 0xd9, 0x28, 
        0x47, 0x26, 0x0e, 0xad, 0xb8, 0xf9, 0xae, 0xf9, 0x16, 0xc2, 
        0x88, 0x9c, 0x8d, 0xfb, 0xec, 0xee, 0x16, 0xcd, 0x1d, 0xe5, 
        0x9d, 0x9d, 0x48, 0xaa, 0xd8, 0x88, 0x2c, 0x10, 0x62, 0x61, 
        0x57, 0x79, 0xe3, 0x31, 0x8a, 0x1f, 0x2b, 0x9f, 0xa8, 0x31, 
        0x9c, 0xeb, 0x36, 0x2c, 0xb7, 0x48, 0x50, 0x73, 0xc0, 0x90, 
        0xc9, 0x2c, 0xd7, 0xbe, 0x90, 0xa0, 0x57, 0xde, 0x6f, 0x54, 
        0xc3, 0x88, 0xe3, 0x87, 0x22, 0xf5, 0x16, 0x1a, 0x30, 0x42, 
        0x3c, 0xd1, 0x19, 0xbd, 0xb1, 0xaf, 0x7c, 0x63, 0xfc, 0xdf, 
        0x7a, 0x4b, 0xe6, 0x54, 0x22, 0x77, 0x4a, 0x1e, 0x3b, 0x09, 
        0x8e, 0x13, 0xb0, 0x1f, 0x16, 0xb9, 0xab, 0x27, 0x27, 0x4f, 
        0xa3, 0x6a, 0xe0, 0xb9, 0x5f, 0x5d, 0x3d, 0x07, 0x30, 0x0f, 
        0xc6, 0xb7, 0xe8, 0x62, 0xdd, 0x02, 0x43, 0x9c, 0x99, 0xc8, 
        0xc0, 0xff, 0xb0, 0xe5, 0x77, 0xe2, 0x81, 0xd4, 0x1e, 0xcc, 
        0xe6, 0x54, 0x83, 0xbb, 0x60, 0xfc, 0xa0, 0xc2, 0x51, 0x66, 
        0xa7, 0xf6, 0x43, 0x72, 0x6b, 0x13, 0xd1, 0x39, 0xb7, 0xf5, 
        0x0e, 0x68, 0x50, 0xa1, 0xe8, 0x44, 0xb3, 0x76, 0x45, 0xe6, 
        0x46, 0x13, 0xda, 0xd8, 0xd0, 0x27, 0x7e, 0x2d, 0xaf, 0xec, 
        0x61, 0xe0, 0xe3, 0xeb, 0x0b, 0xcf, 0x59, 0x77, 0x18, 0x65, 
        0xd2, 0x4d, 0x9b, 0x70, 0xb0, 0x26, 0xb7, 0x3c, 0xfd, 0x22, 
        0xb9, 0xb6, 0xc9, 0x5c, 0xe3, 0xfd, 0xae, 0xf8, 0xbe, 0x70, 
        0x7b, 0xdc, 0x24, 0x7e, 0x92, 0xf7, 0x33, 0xbd, 0xd9, 0x7c, 
        0xf4, 0x26, 0x12, 0xd3, 0x0f, 0xc3, 0x65, 0x60, 0x0a, 0x07, 
        0xa8, 0x39, 0xf7, 0x0e, 0x23, 0x68, 0xca, 0x73, 0x0e, 0xf3, 
        0xb3, 0x20, 0x3c, 0x63, 0xf3, 0x60, 0x91, 0xae, 0x16, 0xdc, 
        0x1e, 0x92, 0x8d, 0xe6, 0xff, 0x7f, 0x4b, 0xe6, 0xa6, 0xfa, 
        0x75, 0x50, 0x7b, 0x05, 0x4e, 0xc0, 0x76, 0xe8, 0x72, 0x75, 
        0x58, 0xd4, 0xf3, 0x58, 0x02, 0xa9, 0x1b, 0xc8, 0xb3, 0x33, 
        0x49, 0x79, 0xb3, 0x1d, 0x22, 0x49, 0x28, 0x9e, 0x84, 0x15, 
        0xb3, 0x82, 0x09, 0x67, 0x30, 0xd4, 0x9f, 0x70, 0x9c, 0x8f, 
        0x86, 0xad, 0xdc, 0x40, 0x1f, 0x29, 0x7b, 0x3c, 0x2f, 0x58, 
        0xfd, 0xc2, 0xfa, 0xfe, 0x32, 0x28, 0xfb, 0x67, 0x01, 0x21, 
        0x5c, 0x4b, 0x8f, 0x19, 0xcd, 0xbc, 0xfe, 0x74, 0xd2, 0xb1, 
        0x36, 0x56, 0x5e, 0x9f, 0xd0, 0x1b, 0x36, 0xc7, 0xaf, 0xc3, 
        0xa0, 0x88, 0xf1, 0x68, 0xed, 0x22, 0xe5, 0x88, 0x78, 0x88, 
        0x8f, 0x72, 0xfd, 0x90, 0x44, 0xb0, 0xd6, 0x77, 0xab, 0x98, 
        0x55, 0xcb, 0xe8, 0x6a, 0xb5, 0x66, 0x28, 0x18, 0xf1, 0xaf, 
        0xde, 0x24, 0x82, 0xe1, 0x5a, 0x0a, 0x20, 0x36, 0x1e, 0x07, 
        0xa9, 0x79, 0x36, 0x48, 0xab, 0xee, 0xbe, 0x99, 0x7f, 0xc6, 
        0xdc, 0xa6, 0x83, 0x60, 0x9b, 0x75, 0x4b, 0xfb, 0x90, 0x8d, 
        0x04, 0x11, 0x7e, 0xa8, 0x94, 0x43, 0x2a, 0xec, 0x1b, 0x9c, 
        0xa9, 0x0b, 0x7a, 0x43, 0x8f, 0x1e, 0xb1, 0x7b, 0x67, 0x72, 
        0xd5, 0x48, 0x93, 0xdc, 0xe0, 0x49, 0xca, 0x39, 0xe0, 0x2d, 
        0xaa, 0x44, 0x67, 0xd1, 0x28, 0x49, 0x62, 0x8b, 0x40, 0x41, 
        0x72, 0x49, 0xce, 0x59, 0x32, 0x44, 0x69, 0x22, 0xde, 0xd7, 
        0xe7, 0x9c, 0xab, 0x66, 0xb3, 0x71, 0xe2, 0xc1, 0xee, 0x43, 
        0x0f, 0x3d, 0x2a, 0xe6, 0xae, 0xbc, 0x37, 0x64, 0x5a, 0xf3, 
        0x99, 0x5d, 0xab, 0x3a, 0x2c, 0xd4, 0x6d, 0x2e, 0xb8, 0x5e, 
        0xd8, 0x94, 0x86, 0x64, 0xad, 0x82, 0xb0, 0xe5, 0x76, 0xf2, 
        0xbc, 0x78, 0x45, 0xb1, 0x63, 0x0e, 0x25, 0x1c, 0xd8, 0xd3, 
        0x14, 0xc6, 0x18, 0x07, 0x7e, 0x0b, 0xc7, 0x06, 0x76, 0xbb, 
        0x89, 0x44, 0xeb, 0x84, 0x1c, 0xf8, 0x9c, 0xfe, 0x9e, 0x0f, 
        0xac, 0x4f, 0xd8, 0xe6, 0xbd, 0x29, 0x90, 0xeb, 0xe9, 0x54, 
        0xb8, 0xb6, 0x15, 0x9a, 0x44, 0x0d, 0x52, 0xda, 0xc9, 0x67, 
        0xd3, 0x83, 0xd7, 0x52, 0x3f, 0xbd, 0x2a, 0x6c, 0x55, 0x6c, 
        0x08, 0x63, 0xb4, 0x61, 0xac, 0xe8, 0x34, 0x4e, 0x06, 0xa1, 
        0xb8, 0x89, 0xb5, 0x13, 0xe4, 0x93, 0xad, 0xc6, 0xa8, 0xf5, 
        0x31, 0x81, 0xe8, 0x30, 0x0d, 0x06, 0x09, 0x2b, 0x06, 0x01, 
        0x04, 0x01, 0x82, 0x37, 0x11, 0x02, 0x31, 0x00, 0x30, 0x13, 
        0x06, 0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x09, 
        0x15, 0x31, 0x06, 0x04, 0x04, 0x01, 0x00, 0x00, 0x00, 0x30, 
        0x57, 0x06, 0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 
        0x09, 0x14, 0x31, 0x4a, 0x1e, 0x48, 0x00, 0x65, 0x00, 0x35, 
        0x00, 0x62, 0x00, 0x61, 0x00, 0x34, 0x00, 0x35, 0x00, 0x63, 
        0x00, 0x36, 0x00, 0x2d, 0x00, 0x33, 0x00, 0x61, 0x00, 0x31, 
        0x00, 0x61, 0x00, 0x2d, 0x00, 0x34, 0x00, 0x39, 0x00, 0x62, 
        0x00, 0x35, 0x00, 0x2d, 0x00, 0x62, 0x00, 0x33, 0x00, 0x32, 
        0x00, 0x35, 0x00, 0x2d, 0x00, 0x35, 0x00, 0x36, 0x00, 0x37, 
        0x00, 0x65, 0x00, 0x39, 0x00, 0x61, 0x00, 0x36, 0x00, 0x37, 
        0x00, 0x62, 0x00, 0x30, 0x00, 0x65, 0x00, 0x39, 0x30, 0x69, 
        0x06, 0x09, 0x2b, 0x06, 0x01, 0x04, 0x01, 0x82, 0x37, 0x11, 
        0x01, 0x31, 0x5c, 0x1e, 0x5a, 0x00, 0x4d, 0x00, 0x69, 0x00, 
        0x63, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x73, 0x00, 0x6f, 0x00, 
        0x66, 0x00, 0x74, 0x00, 0x20, 0x00, 0x52, 0x00, 0x53, 0x00, 
        0x41, 0x00, 0x20, 0x00, 0x53, 0x00, 0x43, 0x00, 0x68, 0x00, 
        0x61, 0x00, 0x6e, 0x00, 0x6e, 0x00, 0x65, 0x00, 0x6c, 0x00, 
        0x20, 0x00, 0x43, 0x00, 0x72, 0x00, 0x79, 0x00, 0x70, 0x00, 
        0x74, 0x00, 0x6f, 0x00, 0x67, 0x00, 0x72, 0x00, 0x61, 0x00, 
        0x70, 0x00, 0x68, 0x00, 0x69, 0x00, 0x63, 0x00, 0x20, 0x00, 
        0x50, 0x00, 0x72, 0x00, 0x6f, 0x00, 0x76, 0x00, 0x69, 0x00, 
        0x64, 0x00, 0x65, 0x00, 0x72, 0x30, 0x82, 0x03, 0xff, 0x06, 
        0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x07, 0x06, 
        0xa0, 0x82, 0x03, 0xf0, 0x30, 0x82, 0x03, 0xec, 0x02, 0x01, 
        0x00, 0x30, 0x82, 0x03, 0xe5, 0x06, 0x09, 0x2a, 0x86, 0x48, 
        0x86, 0xf7, 0x0d, 0x01, 0x07, 0x01, 0x30, 0x1c, 0x06, 0x0a, 
        0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x0c, 0x01, 0x06, 
        0x30, 0x0e, 0x04, 0x08, 0x61, 0xa4, 0xc1, 0x1b, 0x25, 0xf3, 
        0x59, 0xa7, 0x02, 0x02, 0x07, 0xd0, 0x80, 0x82, 0x03, 0xb8, 
        0xba, 0x27, 0x3f, 0x33, 0x49, 0x3e, 0x7a, 0xa4, 0xc8, 0x15, 
        0x83, 0x12, 0x97, 0x7c, 0xc2, 0x18, 0xad, 0x15, 0x60, 0x74, 
        0xd8, 0xb7, 0x64, 0x79, 0x50, 0x4e, 0xa2, 0xb5, 0x4f, 0xc7, 
        0x8b, 0xd7, 0x67, 0x11, 0xbf, 0xa9, 0xef, 0xea, 0x4c, 0xef, 
        0x31, 0xb4, 0x54, 0xe7, 0xf8, 0xcb, 0xc8, 0xca, 0x17, 0x59, 
        0xe9, 0x15, 0xaf, 0x1b, 0xe8, 0xfe, 0xd0, 0xc5, 0x4a, 0xa9, 
        0xf7, 0x5b, 0x0c, 0x77, 0xea, 0xae, 0x25, 0x99, 0x5a, 0xdc, 
        0x2b, 0x30, 0x25, 0x8c, 0x53, 0xa3, 0x39, 0x53, 0x5b, 0xf3, 
        0x25, 0xe7, 0xc2, 0xe2, 0x13, 0x2e, 0x9f, 0x47, 0xea, 0xf7, 
        0x86, 0xad, 0x45, 0x8b, 0x75, 0x73, 0x2a, 0xef, 0xf8, 0x43, 
        0x45, 0xb6, 0x2d, 0xb5, 0x8d, 0xb0, 0x0e, 0xfa, 0x93, 0x5e, 
        0xb5, 0xe8, 0xb7, 0xaf, 0x8a, 0xfd, 0xbc, 0x7a, 0xcd, 0xc9, 
        0xa7, 0x5c, 0x3c, 0x35, 0xd9, 0xec, 0x39, 0x39, 0x80, 0x4a, 
        0xe4, 0x34, 0xe1, 0x8f, 0x17, 0xa0, 0x97, 0xa5, 0xcf, 0x7c, 
        0xe4, 0x2e, 0x9a, 0xc6, 0x16, 0xa5, 0x85, 0x6d, 0x1c, 0xd7, 
        0x44, 0xbe, 0xf3, 0xea, 0xd5, 0xfd, 0xd1, 0x35, 0x38, 0x94, 
        0x40, 0xa6, 0xd2, 0x0e, 0xae, 0xd7, 0x5f, 0xef, 0xcb, 0x9b, 
        0xf6, 0x10, 0x8c, 0xe1, 0xd5, 0x9d, 0xf1, 0xf3, 0x75, 0x4d, 
        0x3f, 0x32, 0x15, 0xc7, 0xcf, 0xe8, 0x07, 0xfa, 0xcd, 0x48, 
        0x56, 0x7e, 0xf9, 0xd2, 0xb2, 0x60, 0x6f, 0x8d, 0x70, 0x6e, 
        0xc0, 0xb4, 0x87, 0xb8, 0x43, 0xdc, 0x35, 0x22, 0x2f, 0xc0, 
        0x61, 0xbd, 0x6b, 0x9c, 0xfe, 0xe1, 0xeb, 0xaa, 0x6a, 0xbc, 
        0x30, 0x88, 0x5a, 0x6d, 0xfb, 0xd3, 0x87, 0x22, 0xca, 0xbd, 
        0x3f, 0x1a, 0xf7, 0x0e, 0xf4, 0xf9, 0x6e, 0xd0, 0xf7, 0x01, 
        0xea, 0x78, 0x2d, 0x7c, 0x50, 0x26, 0xd9, 0xca, 0xfa, 0x7c, 
        0xe1, 0x16, 0x22, 0xb8, 0xb7, 0x9c, 0x94, 0x18, 0xb8, 0xd8, 
        0x4e, 0xac, 0xbd, 0xe8, 0x80, 0xe7, 0x4f, 0x9a, 0x9b, 0xb7, 
        0x06, 0x96, 0x32, 0xd6, 0xde, 0xd5, 0xe6, 0x8f, 0x45, 0x5d, 
        0x7e, 0xc2, 0xe6, 0xff, 0x71, 0x9b, 0x66, 0x67, 0x89, 0xbf, 
        0x85, 0x11, 0xf1, 0xeb, 0x13, 0x42, 0xa9, 0x66, 0x09, 0x43, 
        0x01, 0x66, 0x9b, 0x94, 0xb5, 0x16, 0x64, 0xcd, 0xf1, 0x32, 
        0x3f, 0x46, 0x6d, 0x39, 0x29, 0x9c, 0xd4, 0xcf, 0xff, 0x1b, 
        0x1c, 0xe9, 0x5a, 0xe0, 0xd2, 0xca, 0x26, 0x73, 0xa0, 0x9e, 
        0xbe, 0x76, 0xdf, 0x12, 0x20, 0x4e, 0x65, 0xcc, 0xd3, 0x22, 
        0x28, 0xe4, 0x20, 0x87, 0xab, 0xf3, 0xfb, 0x1e, 0x3d, 0x14, 
        0x1f, 0x84, 0x7d, 0x3f, 0x5e, 0xf6, 0xce, 0x95, 0xfe, 0xe1, 
        0xe4, 0x55, 0xb6, 0xa7, 0xf8, 0xa4, 0x28, 0x38, 0x7a, 0x78, 
        0xd3, 0xd6, 0x51, 0xfc, 0x62, 0x5b, 0x49, 0x53, 0xec, 0x53, 
        0x06, 0x9f, 0xa1, 0x71, 0x45, 0x60, 0x0f, 0x2a, 0x13, 0x6f, 
        0xc4, 0x64, 0x58, 0x43, 0x6f, 0x56, 0x0c, 0xb5, 0x73, 0x4d, 
        0x6f, 0x45, 0x9a, 0x3a, 0x19, 0xd7, 0x5c, 0xa1, 0xf7, 0x8e, 
        0x94, 0x02, 0x14, 0x4e, 0x7d, 0x01, 0x70, 0x34, 0x98, 0xeb, 
        0x1d, 0x30, 0x47, 0x58, 0x81, 0xfb, 0x20, 0xb2, 0x3c, 0xaf, 
        0x5e, 0x50, 0x41, 0xca, 0x20, 0xf1, 0xbd, 0x6d, 0x29, 0xd6, 
        0x92, 0x33, 0x27, 0xc0, 0x34, 0x6b, 0x5a, 0x6c, 0x55, 0xe3, 
        0x4f, 0x22, 0x14, 0x6b, 0x37, 0x53, 0xca, 0xa8, 0x6c, 0xf7, 
        0xf5, 0xaa, 0xdb, 0x42, 0xa7, 0x56, 0x81, 0x8b, 0x72, 0xc5, 
        0xf9, 0x07, 0x70, 0x97, 0x18, 0x04, 0xea, 0x6b, 0x33, 0x5a, 
        0xdf, 0xc7, 0xfa, 0xf8, 0x01, 0x0f, 0x9b, 0xfc, 0xb6, 0x06, 
        0xc1, 0x95, 0xb8, 0x32, 0xa1, 0xd0, 0x6f, 0xd8, 0xd5, 0xe8, 
        0xba, 0x95, 0xf5, 0x6d, 0x2c, 0x5a, 0x20, 0x0a, 0xe2, 0xfe, 
        0x4e, 0xa7, 0x5c, 0xf2, 0x6d, 0xc5, 0x09, 0x0d, 0xe7, 0x5f, 
        0x90, 0xa9, 0x54, 0x7c, 0x91, 0x60, 0x50, 0x45, 0xf9, 0x48, 
        0x7b, 0x90, 0xf8, 0xba, 0x99, 0x86, 0xbe, 0x05, 0xda, 0x2d, 
        0xa9, 0xae, 0xfd, 0xba, 0x71, 0x5d, 0xf6, 0xa1, 0x1f, 0x01, 
        0x70, 0xd0, 0x3e, 0xfd, 0x11, 0x8d, 0x82, 0xfa, 0x23, 0x5e, 
        0xf8, 0x3f, 0x57, 0x44, 0x0a, 0xbf, 0xd4, 0x59, 0x4b, 0xe8, 
        0x61, 0x82, 0x96, 0x59, 0xe7, 0xd8, 0xae, 0x53, 0x9b, 0x57, 
        0x69, 0x70, 0xf8, 0xde, 0xa0, 0x55, 0x8b, 0xc5, 0x48, 0x21, 
        0x60, 0x35, 0xdc, 0xb6, 0x1b, 0xeb, 0x42, 0x89, 0x25, 0x8f, 
        0xa6, 0xab, 0xe0, 0x97, 0x1b, 0x37, 0x3e, 0x34, 0xc5, 0xc5, 
        0x86, 0xf4, 0x74, 0xa1, 0x53, 0x67, 0xef, 0x5b, 0x41, 0x11, 
        0xb8, 0xfd, 0xed, 0xf6, 0x59, 0x5c, 0x7e, 0xf3, 0xbd, 0x43, 
        0x76, 0x9a, 0x1d, 0x62, 0x01, 0xda, 0x9c, 0x6c, 0xb9, 0x96, 
        0xa9, 0x0e, 0xcb, 0xf0, 0xe1, 0x65, 0x23, 0x5b, 0x31, 0x9e, 
        0x27, 0x16, 0x79, 0x1b, 0x78, 0xeb, 0x09, 0x47, 0xdb, 0x0e, 
        0xba, 0x9f, 0x6b, 0x5d, 0x0f, 0xf5, 0x1b, 0xbb, 0x84, 0x2b, 
        0x46, 0x9f, 0x6b, 0xcb, 0x65, 0x2d, 0xeb, 0x61, 0xa6, 0xe9, 
        0x55, 0xb7, 0x59, 0xf0, 0x5d, 0x02, 0xc5, 0x56, 0xbf, 0x5b, 
        0x6e, 0xa7, 0x88, 0x4f, 0x9f, 0x3b, 0x9d, 0x64, 0x58, 0x79, 
        0x99, 0x04, 0x43, 0xba, 0xe4, 0xc3, 0x63, 0x22, 0x77, 0xad, 
        0xc8, 0xef, 0x1a, 0x4e, 0xc4, 0x0b, 0xaa, 0xd1, 0x76, 0xaf, 
        0x63, 0x48, 0x42, 0xf6, 0x68, 0x5f, 0x34, 0xd7, 0xbf, 0x41, 
        0x58, 0xce, 0x50, 0xb9, 0x9e, 0x60, 0x5d, 0x4d, 0xb3, 0x42, 
        0xa0, 0x31, 0xbd, 0xf6, 0x95, 0x7e, 0x8d, 0xd4, 0x10, 0xb8, 
        0x48, 0xee, 0x7e, 0x53, 0x7d, 0x18, 0xc8, 0x20, 0x77, 0xfd, 
        0xa1, 0xce, 0x3c, 0x99, 0xd8, 0xcc, 0x2e, 0x58, 0xc8, 0xa4, 
        0x47, 0x4c, 0x2b, 0xab, 0xb5, 0x80, 0x54, 0x57, 0x95, 0xd4, 
        0x96, 0x7d, 0xd1, 0x0c, 0xe0, 0x14, 0x97, 0x98, 0xcb, 0x48, 
        0x3f, 0x52, 0x08, 0x9c, 0xc6, 0x0e, 0xa9, 0xa5, 0xa5, 0x3d, 
        0x4f, 0xa8, 0xb4, 0xf6, 0x7b, 0x6c, 0xf3, 0x69, 0x9c, 0x7d, 
        0x28, 0xf8, 0xf8, 0x99, 0x6a, 0xad, 0x35, 0xdb, 0x14, 0xe4, 
        0x3c, 0xfc, 0x92, 0xb0, 0xb6, 0x68, 0x7c, 0xa1, 0xe9, 0x55, 
        0x41, 0x91, 0x05, 0xf2, 0xb4, 0xd7, 0x7c, 0x33, 0xc4, 0x72, 
        0xfb, 0xde, 0xd7, 0x2d, 0x4f, 0xfb, 0xa1, 0x46, 0xe6, 0x30, 
        0xd8, 0x73, 0xf3, 0x19, 0x9e, 0x0b, 0x82, 0xef, 0x62, 0xac, 
        0xd5, 0x1e, 0x7d, 0x25, 0xb7, 0xd9, 0xd8, 0x04, 0xe3, 0x3d, 
        0x90, 0x50, 0x3b, 0x91, 0xfe, 0x34, 0x89, 0x04, 0x9e, 0x5b, 
        0xac, 0x26, 0x1e, 0xa0, 0xb8, 0x02, 0xf0, 0x91, 0x06, 0xe2, 
        0xc3, 0xf9, 0x49, 0x7d, 0xe8, 0xe5, 0xd4, 0x9e, 0xad, 0x03, 
        0x78, 0xf2, 0xba, 0x06, 0xcd, 0x70, 0xf9, 0xa9, 0xf1, 0xba, 
        0x62, 0xcc, 0xfb, 0x7a, 0xaf, 0xde, 0xbf, 0xbf, 0x92, 0x38, 
        0x76, 0x16, 0x3c, 0x5d, 0xbe, 0xc6, 0x6a, 0x55, 0xa4, 0x8b, 
        0x71, 0x00, 0xd4, 0x9e, 0xac, 0x0c, 0x16, 0x5a, 0x0f, 0x1d, 
        0x5d, 0xa0, 0x18, 0x9a, 0x9c, 0x33, 0x7d, 0xf3, 0x61, 0xb4, 
        0x4d, 0x4c, 0x30, 0x37, 0x30, 0x1f, 0x30, 0x07, 0x06, 0x05, 
        0x2b, 0x0e, 0x03, 0x02, 0x1a, 0x04, 0x14, 0xc5, 0xf9, 0x1d, 
        0xbf, 0x65, 0xa1, 0x0e, 0xe0, 0x50, 0x17, 0x1c, 0x78, 0x47, 
        0x1a, 0x91, 0xd4, 0xe9, 0x58, 0x28, 0x92, 0x04, 0x14, 0x97, 
        0x1e, 0xb6, 0x49, 0xb8, 0x3e, 0x03, 0x6d, 0xef, 0xbf, 0xe4, 
        0x41, 0x34, 0x44, 0x24, 0xa9, 0xc7, 0xcd, 0x35, 0xc9, 
            };

        #endregion //EmulatorCertificate
        #endregion

        #region Events
        public event GETRequestEventHandler OnGetRequest;

        public event RequestEventHandler OnRequest;
        public event ResponseEventHandler OnResponse;
        #endregion

        #region Public methods
        public void Start(string prefix, int port)
        {
            if (listener != null)
                return;

            listener = new HttpListener(prefix, port);

            // Loads certificate if it is https server.
            if (prefix == "https")
            {
                // SKU == 3 indicates the device is the emulator.
                // The emulator and the device use different certificate types.  The emulator requires the use of a .PFX certficate, whereas the device simply
                // requires a CER certificate with appended private key. In addtion, the .PFX certificate requires a password ("NetMF").
                if (Utils.IsEmulator)
                    listener.HttpsCert = new X509Certificate(m_emulatorCertData, "NetMF");
                else
                {
                    string serverCertAsString = Resource1.GetString(Resource1.StringResources.cert_device_microsoft_com);
                    listener.HttpsCert = new X509Certificate(Encoding.UTF8.GetBytes(serverCertAsString));
                }
            }

            listener.Start();
            isStopped = false;

            new Thread(() =>
            {
                while (!isStopped)
                {
                    try
                    {
                        if (!listener.IsListening)
                            listener.Start();

                        HttpListenerContext context = listener.GetContext();

                        // see http://netmf.codeplex.com/workitem/2157
                        //new Thread(() =>
                        {
                            try
                            {
                                if (OnRequest != null)
                                    OnRequest(context.Request);

                                switch (context.Request.HttpMethod.ToUpper())
                                {
                                    case "GET": ProcessClientGetRequest(context); break;
                                    case "POST": ProcessClientPostRequest(context); break;
                                }

                                // for test:
                                //SendStream(Encoding.UTF8.GetBytes("<html><body>" + DateTime.Now + "</body></html>"), "text/html", context.Response);

                                context.Close();
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.Message);
                            }
                        }
                        //).Start();
                    }
                    //catch (InvalidOperationException ex)
                    //{
                    //    listener.Stop();
                    //    Thread.Sleep(1000);
                    //}
                    //catch (ObjectDisposedException ex)
                    //{
                    //    listener.Start();
                    //}
                    //catch (SocketException ex)
                    //{
                    //    if (ex.ErrorCode == 10053)
                    //        listener.Stop();
                    //}
                    catch (Exception ex)
                    {
                        //Thread.Sleep(1000);
                        //if (context != null)
                        //    context.Close();
                    }
                }

                // stopped
                listener.Close();
                listener = null;
            }) { Priority = ThreadPriority.Normal }.Start();
        }
        public void Stop()
        {
            isStopped = true;
        }

        public void SendFile(string path, HttpListenerResponse response)
        {
            if (!File.Exists(path))
                Response404(response);
            else
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = DefineContentType(path);

                //uint ram = Debug.GC(false);
                //Debug.Print("RAM: " + ram);

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    long fileLength = fs.Length;
                    response.ContentLength64 = fileLength;

                    int bufferSize = 1024 * 64;
                    byte[] buf = new byte[bufferSize];
                    for (long bytesSent = 0; bytesSent < fileLength; )
                    {
                        long bytesToRead = fileLength - bytesSent;
                        bytesToRead = bytesToRead < bufferSize ? bytesToRead : bufferSize;

                        int bytesRead = fs.Read(buf, 0, (int)bytesToRead);
                        response.OutputStream.Write(buf, 0, bytesRead);
                        bytesSent += bytesRead;

                        if (OnResponse != null)
                            OnResponse(response);
                    }

                    fs.Close();
                }
            }
        }
        public void SendStream(byte[] data, string contentType, HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = contentType;
            response.ContentLength64 = (long)data.Length;
            
            response.OutputStream.Write(data, 0, data.Length);

            if (OnResponse != null)
                OnResponse(response);
        }

        public static string DefineContentType(string path)
        {
            string ext = Path.GetExtension(path).ToLower();

            switch (ext)
            {
                case ".htm":
                case ".html":
                case ".htmls":
                    return "text/html";
                case ".js":
                    return "text/javascript";
                //return "application/javascript";
                case ".jpe":
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".ico":
                    return "image/x-icon";
                case ".pdf":
                    return "application/pdf";
                case ".svg":
                    return "image/svg+xml";
                case ".css":
                    return "text/css";
                case ".xml":
                    return "text/xml";
                case ".json":
                    return "application/json";
                case ".arj":
                case ".lzh":
                case ".exe":
                case ".rar":
                case ".tar":
                case ".zip":
                    return "application/octet-stream";
                case ".mid":
                case ".midi":
                    return "application/x-midi";
                case ".mp3":
                    return "audio/mpeg";
                case ".swf":
                    return "application/x-shockwave-flash";
                default:
                    return "text/plain";
            }
        }
        #endregion

        #region Private methods
        private void ProcessClientGetRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string query = Utils.StringReplace(request.RawUrl, "/", "\\");

            string path = query;
            Hashtable parameters = new Hashtable();
            int idx = query.IndexOf("?");
            if (idx != -1)
            {
                path = query.Substring(0, idx);
                string[] pairs = query.Substring(idx + 1).Split(new Char[] { '&' });
                foreach (string pair in pairs)
                {
                    if (!Utils.StringIsNullOrEmpty(pair))
                    {
                        string[] s = pair.Split(new Char[] { '=' });
                        parameters.Add(s[0], s[1]);
                    }
                }
            }

            if (path == "\\")
                path = "\\index.html";

            if (path.ToLower() == "\\admin")
                ProcessPasswordProtectedArea(request, response);
            else
                if (OnGetRequest != null)
                    OnGetRequest(path, parameters, response);
        }
        private void ProcessClientPostRequest(HttpListenerContext context)
        {
            const int BUFFER_SIZE = 1024;

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            // Allocates buffer for reading of message body
            byte[] postdata = new byte[BUFFER_SIZE];

            // Now reads the posted data. The content length should be supplied. 
            // It is error not to have content length with post request.
            if (request.ContentLength64 > 0)
            {
                Debug.Print("Request Headers:");
                Debug.Print(request.Headers.ToString());

                VolumeInfo[] vis = VolumeInfo.GetVolumes();
                bool fHasVolume = false;

                if (vis.Length > 0)
                {
                    for (int i = 0; i < vis.Length; i++)
                    {
                        if (vis[i].Name.ToLower() == "root")
                        {
                            if (!vis[i].IsFormatted)
                                vis[i].Format(0);
                            fHasVolume = true;
                            break;
                        }
                        else if (vis[i].Name.ToLower() == "winfs")
                        {
                            if (!vis[i].IsFormatted)
                                vis[i].Format(0);
                            fHasVolume = true;
                            break;
                        }
                    }
                }

                FileStream fs = null;

                if (fHasVolume)
                {
                    try
                    {
                        fs = new FileStream("\\ROOT\\Upload.txt", FileMode.Create);
                    }
                    catch
                    {
                        try
                        {
                            fs = new FileStream("\\WINFS\\Upload.txt", FileMode.Create);
                        }
                        catch
                        {
                            fHasVolume = false;
                        }
                    }
                }

                long totalBytesReceived = 0;
                long contLen = request.ContentLength64;
                if (fHasVolume)
                {
                    while (totalBytesReceived < contLen)
                    {
                        int bytesToRead = (int)(contLen - totalBytesReceived);
                        bytesToRead = System.Math.Min(bytesToRead, BUFFER_SIZE); // Limit to buffer size

                        int dataRead = request.InputStream.Read(postdata, 0, bytesToRead);
                        if (dataRead == 0) // Definitely some error. Means file incomplete.
                            break;

                        fs.Write(postdata, 0, dataRead);

                        totalBytesReceived += dataRead;
                    };

                    fs.Close();
                }

                // Sends response:
                string strResp = "<HTML><BODY>.Net Micro Framework Example HTTP Server<p>";

                // Print requested verb, URL and version.. Adds information from the request.
                strResp += "HTTP Method: " + request.HttpMethod + "<br> Requested URL: \"" + request.RawUrl +
                    "<br> HTTP Version: " + request.ProtocolVersion + "\"<p>";

                strResp += "Amount of data received in message body: " + totalBytesReceived + "<br>";
                strResp += "Data of message body is discarded (if there is no filesystem). Please review HTTP Server sample code to add processing of data";
                strResp += "</BODY></HTML>";

                response.StatusCode = (int)HttpStatusCode.OK;
                //response.RedirectLocation = "http://localhost/WINFS/DpwsDevice/";
                response.ContentType = "text/html";
                byte[] messageBody = Encoding.UTF8.GetBytes(strResp);
                response.OutputStream.Write(messageBody, 0, messageBody.Length);
            }
            else // Content length is missing, send error back
            {
                // Sends response:
                string strResp = "<HTML><BODY>Content length is missing in Post request</BODY></HTML>";
                byte[] messageBody = Encoding.UTF8.GetBytes(strResp);
                response.ContentType = "text/html";
                response.OutputStream.Write(messageBody, 0, messageBody.Length);
            }
        }
        private void ProcessPasswordProtectedArea(HttpListenerRequest request, HttpListenerResponse response)
        {
            string strResp = "<html><body>.Net MF Example HTTP Server. Secure Area<p>";
            if (request.Credentials != null)
            { // Parse and Decode string.
                Debug.Print("User Name : " + request.Credentials.UserName);
                Debug.Print("Password : " + request.Credentials.Password);
                if (request.Credentials.UserName != "gothicmaestro" || request.Credentials.Password != "Kotyara75")
                {
                    strResp += "Authentication failed. <br>You supplied <p>User Name: " + request.Credentials.UserName +
                               "<br>Password: " + request.Credentials.Password;
                    strResp += "<p>Please look at root page for required credentials";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Headers.Add("WWW-Authenticate: Basic realm=\".Net MF Example of Secure Area\"");
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    strResp += "Authentication Succeeded.";
                }
            }
            else
            {
                strResp += "<p> Authentication Required<p> Please look at root page for required credentials";

                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Headers.Add("WWW-Authenticate: Basic realm=\".Net MF Example of Secure Area\"");
            }
            strResp += "</body></html>";

            byte[] buffer = encoder.GetBytes(strResp);
            response.ContentType = "text/html";
            response.OutputStream.Write(buffer, 0, buffer.Length);

            if (OnResponse != null)
                OnResponse(response);
        }
        private void Response404(HttpListenerResponse response)
        {
            //string strResp = "<HTML><BODY>.Net Micro Framework Example HTTP Server<p>";
            //// Print requested verb, URL and version.. Adds information from the request.
            //strResp += "HTTP Method: " + request.HttpMethod + "<br> Requested URL: \"" + request.RawUrl +
            //    "<br> HTTP Version: " + request.ProtocolVersion + "\"<p>";
            //// Information about the path that we access.
            //strResp += "File to access " + strFilePath + "<p>";
            //if (Directory.Exists(strFilePath)) // If directory present - iterate over it and 
            //    strResp += FillDirectoryContext(strFilePath, request);
            //else // Neither File or directory exists, prints that directory is not found.
            //    strResp += "Directory: \"" + strFilePath + "\" Does not exists";
            //strResp += "</BODY></HTML>";


            string strResp = "HTTP/1.1 404 File Not Found\r\n";
            response.ContentType = "text/html";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            
            byte[] buffer = encoder.GetBytes(strResp);
            response.OutputStream.Write(buffer, 0, buffer.Length);

            if (OnResponse != null)
                OnResponse(response);
        }

        private static string FillDirectoryContext(string strFilePath, HttpListenerRequest request)
        {
            string strRet = "Context of directory: \"" + strFilePath + "\"<p>";

            try
            {
                string[] dirs = Directory.GetDirectories(strFilePath);
                foreach (string strDirPath in dirs)
                {
                    // dirs keep full path. We need only relative path in directory.
                    // So we need to split by '\' and take the last directory,
                    string[] splitDir = strDirPath.Split(fwdSlashDelim);
                    string strDir = splitDir[splitDir.Length - 1];
                    string strUrl = request.RawUrl;
                    if (strUrl[strUrl.Length - 1] != '/')
                    {
                        strUrl += '/';
                    }
                    strRet += "Dir  - " +
                    "<A HREF=\"" + strUrl + strDir + "\">" + strDir + "</A><br>";
                }

                string[] files = Directory.GetFiles(strFilePath);
                foreach (string locFilePath in files)
                {
                    string[] splitFile = locFilePath.Split(fwdSlashDelim);
                    string strFile = splitFile[splitFile.Length - 1];
                    string strUrl = request.RawUrl;
                    if (strUrl[strUrl.Length - 1] != '/')
                    {
                        strUrl += '/';
                    }

                    strRet += "File - " +
                    "<A HREF=\"" + strUrl + strFile + "\">" + strFile + "</A><br>";
                }
            }
            catch
            {
            }

            // Following line adds file post capabilities
            strRet += "<p>Below is example of sending file data to HTTP server";
            strRet += Resource1.GetString(Resource1.StringResources.PostForm);
            // Next line shows link protected by password.
            strRet += "<br>Below is example of protected link.";
            strRet += "<br><A HREF=\"/PasswordProtected\">Password Protected Secure Area</A>";
            strRet += "<br>Use following credentials to access it:<br> User Name: Igor<br> Password: MyPassword<br>";

            return strRet;
        }
        #endregion
    }
}
