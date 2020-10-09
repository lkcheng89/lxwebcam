#define Camera

using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ASCOM.LxWebcam
{
    public partial class Camera
    {
        [ComVisible(false)]
        public class Webcam : ISampleGrabberCB, IComparer<VideoInfoHeader>
        {
            private ICaptureGraphBuilder2 captureGraphBuilder;
            private IFilterGraph2 filterGraph;
            private IBaseFilter videoCapture;
            private IMediaControl mediaControl;
            private DsROTEntry rotEntry;
            private LinkedList<Frame> frames;

            public Webcam(string name)
            {
                this.Name = name;
                this.MediaType = null;
                this.Width = 0;
                this.Height = 0;
                this.FrameRate = 0;
                this.Backtrace = 15;

                /////

                this.captureGraphBuilder = null;
                this.filterGraph = null;
                this.mediaControl = null;
                this.rotEntry = null;
                this.videoCapture = null;
                this.frames = null;
            }

            public string Name { get; set; }
            public VideoInfoHeader MediaType { get; set; }
            public int Width { get; private set; }
            public int Height { get; private set; }
            public double FrameRate { get; private set; }
            public int Backtrace { get; set; }

            public void Open(bool run = true)
            {
                DsDevice device = null;

                foreach (DsDevice d in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                {
                    if (d.Name == this.Name)
                    {
                        device = d; break;
                    }
                }

                if (device == null)
                {
                    throw new NullReferenceException("DsDevice");
                }

                try
                {
                    // Create

                    this.captureGraphBuilder = new CaptureGraphBuilder2() as ICaptureGraphBuilder2;
                    if (this.captureGraphBuilder == null) throw new NullReferenceException("ICaptureGraphBuilder2");

                    this.filterGraph = new FilterGraph() as IFilterGraph2;
                    if (this.filterGraph == null) throw new NullReferenceException("IFilterGraph2");

                    this.mediaControl = this.filterGraph as IMediaControl;
                    if (this.mediaControl == null) throw new NullReferenceException("IMediaControl");

                    // Filter Graph (Video Capture -> Sample Grabber -> Null Renderer)

                    int hr = this.captureGraphBuilder.SetFiltergraph(this.filterGraph);
                    if (hr < 0) throw new COMException("ICaptureGraphBuilder2::SetFiltergraph", hr);

                    // Video Capture

                    hr = this.filterGraph.AddSourceFilterForMoniker(device.Mon, null, device.Name, out this.videoCapture);
                    if (hr < 0) throw new COMException("IFilterGraph2::AddSourceFilterForMoniker", hr);

                    if (run)
                    {
                        hr = this.captureGraphBuilder.FindInterface(PinCategory.Capture, DirectShowLib.MediaType.Video, this.videoCapture, typeof(IAMStreamConfig).GUID, out object intrface);
                        if (hr < 0) throw new COMException("ICaptureGraphBuilder2::FindInterface::IAMStreamConfig", hr);

                        IAMStreamConfig streamConfig = intrface as IAMStreamConfig;
                        if (streamConfig == null) throw new NullReferenceException("IAMStreamConfig");

                        hr = streamConfig.GetFormat(out AMMediaType media);
                        if (hr < 0) throw new COMException("IAMStreamConfig::GetFormat", hr);

                        if (this.MediaType == null)
                        {
                            this.MediaType = new VideoInfoHeader();
                            Marshal.PtrToStructure(media.formatPtr, this.MediaType);
                            DsUtils.FreeAMMediaType(media); media = null;
                        }
                        else
                        {
                            Marshal.StructureToPtr(this.MediaType, media.formatPtr, false);
                            hr = streamConfig.SetFormat(media);
                            DsUtils.FreeAMMediaType(media); media = null;
                            if (hr < 0) throw new COMException("IAMStreamConfig::SetFormat", hr);
                        }

                        this.Width = this.MediaType.BmiHeader.Width;
                        this.Height = this.MediaType.BmiHeader.Height;
                        this.FrameRate = 10000000.0 / this.MediaType.AvgTimePerFrame;

                        // Sample Grabber

                        ISampleGrabber sampleGrabber = new SampleGrabber() as ISampleGrabber;
                        media = new AMMediaType();
                        media.majorType = DirectShowLib.MediaType.Video;
                        media.subType = MediaSubType.RGB24;
                        media.formatType = FormatType.VideoInfo;
                        hr = sampleGrabber.SetMediaType(media);
                        DsUtils.FreeAMMediaType(media); media = null;
                        if (hr < 0) throw new COMException("ISampleGrabber::SetMediaType", hr);

                        hr = sampleGrabber.SetCallback(this, 1);
                        if (hr < 0) throw new COMException("ISampleGrabber::SetCallback", hr);

                        hr = this.filterGraph.AddFilter(sampleGrabber as IBaseFilter, "SampleGrabber");
                        if (hr < 0) throw new COMException("IFilterGraph2::AddFilter::SampleGrabber", hr);

                        // Null Renderer

                        NullRenderer nullRenderer = new NullRenderer();
                        hr = this.filterGraph.AddFilter(nullRenderer as IBaseFilter, "NullRenderer");
                        if (hr < 0) throw new COMException("IFilterGraph2::AddFilter::NullRenderer", hr);

                        hr = this.captureGraphBuilder.RenderStream(PinCategory.Capture, DirectShowLib.MediaType.Video, this.videoCapture, sampleGrabber as IBaseFilter, nullRenderer as IBaseFilter);
                        if (hr < 0) throw new COMException("ICaptureGraphBuilder2::RenderStream", hr);

                        // ROT (Running Object Table) Entry

                        this.rotEntry = new DsROTEntry(this.filterGraph);

                        // Frames

                        this.frames = new LinkedList<Frame>();

                        for (int b = 0; b < this.Backtrace; b++)
                        {
                            this.frames.AddLast(new Frame());
                        }

                        // Run Filter Graph

                        hr = this.mediaControl.Run();
                        if (hr < 0) throw new COMException("IMediaControl::Run", hr);
                    }
                }
                catch (Exception e)
                {
                    this.Close(); throw e;
                }
            }

            public void Close()
            {
                if (this.rotEntry != null)
                {
                    this.rotEntry.Dispose();
                    this.rotEntry = null;
                }

                if (this.mediaControl != null)
                {
                    this.mediaControl.StopWhenReady();

                    Marshal.ReleaseComObject(this.mediaControl);
                    this.mediaControl = null;
                }

                this.videoCapture = null;

                if (this.filterGraph != null)
                {
                    Marshal.ReleaseComObject(this.filterGraph);
                    this.filterGraph = null;
                }

                if (this.captureGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(this.captureGraphBuilder);
                    this.captureGraphBuilder = null;
                }

                this.frames = null;
            }

            public int Compare(VideoInfoHeader mediaType1, VideoInfoHeader mediaType2)
            {
                int size1 = mediaType1.BmiHeader.Width * mediaType1.BmiHeader.Height;
                int size2 = mediaType2.BmiHeader.Width * mediaType2.BmiHeader.Height;

                if (size1 < size2) return 1;
                if (size1 > size2) return -1;

                size1 = mediaType1.BmiHeader.ImageSize;
                size2 = mediaType2.BmiHeader.ImageSize;

                if (size1 < size2) return 1;
                if (size1 > size2) return -1;

                return 0;
            }

            public List<VideoInfoHeader> GetMediaTypes()
            {
                if (this.videoCapture == null) throw new NullReferenceException("IBaseFilter::VideoCapture");

                List<VideoInfoHeader> mediaTypes = new List<VideoInfoHeader>();

                IPin pin = DsFindPin.ByCategory(this.videoCapture, PinCategory.Capture, 0);
                if (pin == null) throw new NullReferenceException("IPin");

                int hr = pin.EnumMediaTypes(out IEnumMediaTypes enumMediaTypes);
                if (hr < 0) throw new COMException("IPin::EnumMediaTypes", hr);

                AMMediaType[] nextMediaTypes = new AMMediaType[1];
                hr = enumMediaTypes.Next(1, nextMediaTypes, IntPtr.Zero);
                if (hr < 0) throw new COMException("IEnumMediaTypes::Next", hr);
                AMMediaType nextMediaType = nextMediaTypes[0];

                while (nextMediaType != null)
                {
                    VideoInfoHeader mediaType = new VideoInfoHeader();
                    Marshal.PtrToStructure(nextMediaType.formatPtr, mediaType);
                    DsUtils.FreeAMMediaType(nextMediaType);

                    if (mediaType.BmiHeader.Width > 0 && mediaType.BmiHeader.Height > 0 && mediaType.BmiHeader.BitCount > 0)
                    {
                        mediaTypes.Add(mediaType);
                    }

                    hr = enumMediaTypes.Next(1, nextMediaTypes, IntPtr.Zero);
                    if (hr < 0) throw new COMException("IEnumMediaTypes::Next", hr);
                    nextMediaType = nextMediaTypes[0];
                }

                mediaTypes.Sort(this); return mediaTypes;
            }

            public bool Capture(out int[,,] image, int startX, int startY, int numX, int numY, bool now = false)
            {
                if (!now)
                {
                    Thread.Sleep((int)(500.0 / this.FrameRate * this.Backtrace + 0.5));
                }

                lock (this)
                {
                    int stride = (this.Width * 3 + 3) & (-4);
                    int length = stride * this.Height;

                    Frame bestFrame = null;

                    foreach (Frame frame in this.frames)
                    {
                        if (frame.Data.Length != length)
                        {
                            continue;
                        }

                        if (double.IsNaN(frame.Goodness))
                        {
                            frame.Goodness = 0;

                            for (int y = 0, s = 0; y < this.Height; y++, s += stride)
                            {
                                for (int x = 0, i = s; x < this.Width; x++)
                                {
                                    frame.Goodness += frame.Data[i++];
                                    frame.Goodness += frame.Data[i++];
                                    frame.Goodness += frame.Data[i++];
                                }
                            }
                        }

                        if (bestFrame == null || bestFrame.Goodness < frame.Goodness)
                        {
                            bestFrame = frame;
                        }
                    }

                    if (bestFrame == null)
                    {
                        image = null;
                    }
                    else
                    {
                        image = new int[numX, numY, 3];

                        for (int sy = this.Height - 1, dy = sy - startY, s = 0; sy >= 0; sy--, dy--, s += stride)
                        {
                            for (int sx = 0, dx = sx - startX, i = s; sx < this.Width; sx++, dx++)
                            {
                                byte data1 = bestFrame.Data[i++];
                                byte data2 = bestFrame.Data[i++];
                                byte data3 = bestFrame.Data[i++];

                                if (0 <= dx && dx < numX && 0 <= dy && dy < numY)
                                {
                                    image[dx, dy, 0] = data1;
                                    image[dx, dy, 1] = data2;
                                    image[dx, dy, 2] = data3;
                                }
                            }
                        }
                    }
                }

                return (image != null);
            }

            public int SampleCB(double SampleTime, IMediaSample pSample)
            {
                throw new NotImplementedException();
            }

            public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
            {
                lock (this)
                {
                    LinkedListNode<Frame> node = this.frames.First;

                    if (node != null)
                    {
                        Frame frame = node.Value;

                        if (frame.Data.Length != BufferLen)
                        {
                            frame.Data = new byte[BufferLen];
                        }

                        Marshal.Copy(pBuffer, frame.Data, 0, BufferLen);
                        frame.Timestamp = SampleTime;
                        frame.Goodness = double.NaN;

                        this.frames.RemoveFirst();
                        this.frames.AddLast(node);
                    }
                }

                return 0;
            }

            /////

            public static VideoInfoHeader Parse(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                List<byte> bytes = new List<byte>();

                foreach (string s in value.Split(' '))
                {
                    if (byte.TryParse(s, NumberStyles.HexNumber, null, out byte b))
                    {
                        bytes.Add(b);
                    }
                }

                int cb = Marshal.SizeOf(typeof(VideoInfoHeader));

                if (cb != bytes.Count)
                {
                    return null;
                }

                IntPtr ptr = IntPtr.Zero;

                try
                {
                    ptr = Marshal.AllocHGlobal(cb);
                }
                catch
                {
                    return null;
                }

                Marshal.Copy(bytes.ToArray(), 0, ptr, cb);
                VideoInfoHeader mediaType = Marshal.PtrToStructure(ptr, typeof(VideoInfoHeader)) as VideoInfoHeader;
                Marshal.FreeHGlobal(ptr);

                return mediaType;
            }

            public static string ToString(VideoInfoHeader mediaType)
            {
                if (mediaType == null)
                {
                    return string.Empty;
                }

                int cb = Marshal.SizeOf(typeof(VideoInfoHeader));
                IntPtr ptr = IntPtr.Zero;

                try
                {
                    ptr = Marshal.AllocHGlobal(cb);
                }
                catch
                {
                    return string.Empty;
                }

                byte[] bytes = new byte[cb];
                Marshal.StructureToPtr(mediaType, ptr, false);
                Marshal.Copy(ptr, bytes, 0, cb);
                Marshal.FreeHGlobal(ptr);

                StringBuilder stringBuilder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(' ');
                    }

                    stringBuilder.AppendFormat("{0:X02}", b);
                }

                return stringBuilder.ToString();
            }

            [ComVisible(false)]
            public class Frame
            {
                public Frame()
                {
                    this.Data = new byte[0];
                    this.Timestamp = double.NaN;
                    this.Goodness = double.NaN;
                }

                public byte[] Data { get; set; }
                public double Timestamp { get; set; }
                public double Goodness { get; set; }
            }
        }
    }
}
