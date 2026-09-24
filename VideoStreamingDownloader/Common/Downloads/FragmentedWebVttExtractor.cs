using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class FragmentedWebVttExtractor
{
    public static void Extract(
        string inputPath,
        string outputPath)
    {
        using (var stream = File.OpenRead(inputPath))
        {
            // Your file has TrackID 1 according to MP4Box.
            uint subtitleTrackId = FindWebVttTrackId(stream);

            int timescale = FindTrackTimescale(
                stream,
                subtitleTrackId);

            var cues = new List<VttCue>();

            foreach (var moof in FindTopLevelBoxes(stream, "moof"))
            {
                // Find the following mdat.
                stream.Position = moof.Offset + moof.Size;

                Mp4Box mdat = ReadNextBox(stream);

                if (mdat == null || mdat.Type != "mdat")
                    continue;

                ExtractMoofSamples(
                    stream,
                    moof,
                    mdat,
                    subtitleTrackId,
                    timescale,
                    cues);
            }

            cues.Sort(
                delegate (VttCue a, VttCue b)
                {
                    return a.Start.CompareTo(b.Start);
                });

            WriteVtt(outputPath, cues);
        }
    }

    // ================================================================
    // moof / traf / trun
    // ================================================================

    private static void ExtractMoofSamples(
        Stream stream,
        Mp4Box moof,
        Mp4Box mdat,
        uint subtitleTrackId,
        int timescale,
        List<VttCue> cues)
    {
        foreach (var traf in FindChildren(moof, "traf"))
        {
            Mp4Box tfhd = FindChild(traf, "tfhd");

            if (tfhd == null)
                continue;

            TfhdInfo track = ReadTfhd(stream, tfhd);

            if (track.TrackId != subtitleTrackId)
                continue;

            Mp4Box tfdt = FindChild(traf, "tfdt");

            ulong decodeTime = tfdt != null
                ? ReadTfdt(stream, tfdt)
                : 0UL;

            // Position used when trun doesn't specify data_offset.
            long? nextDataPosition = null;

            foreach (var trun in FindChildren(traf, "trun"))
            {
                TrunInfo run = ReadTrun(stream, trun);

                long dataPosition;

                if (run.HasDataOffset)
                {
                    // trun.data_offset is signed and relative to moof.
                    dataPosition = checked(
                        moof.Offset + run.DataOffset);
                }
                else if (nextDataPosition.HasValue)
                {
                    // Continue immediately after the previous trun.
                    dataPosition = nextDataPosition.Value;
                }
                else if (track.BaseDataOffset != 0)
                {
                    dataPosition = checked(
                        (long)track.BaseDataOffset);
                }
                else
                {
                    // Default for fragmented MP4 when no explicit
                    // base-data-offset is supplied.
                    dataPosition = checked(
                        moof.Offset + moof.Size);
                }

                long samplePosition = dataPosition;

                foreach (var sample in run.Samples)
                {
                    uint duration =
                        sample.Duration.HasValue
                            ? sample.Duration.Value
                            : track.DefaultSampleDuration;

                    uint size =
                        sample.Size.HasValue
                            ? sample.Size.Value
                            : track.DefaultSampleSize;

                    if (size == 0)
                    {
                        decodeTime += duration;
                        continue;
                    }

                    if (samplePosition < mdat.Offset ||
                        samplePosition + size >
                            mdat.Offset + mdat.Size)
                    {
                        throw new InvalidDataException(
                            "Sample outside mdat. " +
                            "Position=" + samplePosition +
                            ", Size=" + size +
                            ", mdat=" + mdat.Offset +
                            "-" + (mdat.Offset + mdat.Size));
                    }

                    stream.Position = samplePosition;

                    byte[] sampleData =
                        new byte[checked((int)size)];

                    ReadExactly(stream, sampleData);

                    VttCueData cueData =
                        ExtractVttCue(sampleData);

                    if (cueData != null)
                    {
                        long presentationTime =
                            checked(
                                (long)decodeTime +
                                sample.CompositionTimeOffset);

                        TimeSpan start =
                            TimeSpan.FromSeconds(
                                (double)presentationTime / timescale);

                        TimeSpan end =
                            TimeSpan.FromSeconds(
                                (double)(
                                    presentationTime +
                                    duration) / timescale);

                        cues.Add(
                            new VttCue
                            {
                                Start = start,
                                End = end,
                                Text = cueData.Text,
                                Settings = cueData.Settings
                            });
                    }

                    samplePosition += size;
                    decodeTime += duration;
                }

                // Next trun without data_offset continues from
                // the end of this trun's sample data.
                nextDataPosition = samplePosition;
            }
        }
    }

    // ================================================================
    // tfhd
    // ================================================================

    private static TfhdInfo ReadTfhd(
        Stream stream,
        Mp4Box box)
    {
        stream.Position = box.Offset + 8;

        uint versionFlags = ReadUInt32BE(stream);

        uint flags = versionFlags & 0x00FFFFFF;

        uint trackId = ReadUInt32BE(stream);

        ulong baseDataOffset = 0;

        uint defaultSampleDuration = 0;
        uint defaultSampleSize = 0;

        // base-data-offset-present
        if ((flags & 0x000001) != 0)
        {
            baseDataOffset = ReadUInt64BE(stream);
        }

        // sample-description-index-present
        if ((flags & 0x000002) != 0)
        {
            ReadUInt32BE(stream);
        }

        // default-sample-duration-present
        if ((flags & 0x000008) != 0)
        {
            defaultSampleDuration = ReadUInt32BE(stream);
        }

        // default-sample-size-present
        if ((flags & 0x000010) != 0)
        {
            defaultSampleSize = ReadUInt32BE(stream);
        }

        // default-sample-flags-present
        if ((flags & 0x000020) != 0)
        {
            ReadUInt32BE(stream);
        }

        return new TfhdInfo
        {
            TrackId = trackId,
            BaseDataOffset = baseDataOffset,
            DefaultSampleDuration = defaultSampleDuration,
            DefaultSampleSize = defaultSampleSize
        };
    }

    // ================================================================
    // tfdt
    // ================================================================

    private static ulong ReadTfdt(
        Stream stream,
        Mp4Box box)
    {
        stream.Position = box.Offset + 8;

        uint versionFlags = ReadUInt32BE(stream);

        byte version =
            (byte)(versionFlags >> 24);

        if (version == 1)
            return ReadUInt64BE(stream);

        return ReadUInt32BE(stream);
    }

    // ================================================================
    // trun
    // ================================================================

    private static TrunInfo ReadTrun(
        Stream stream,
        Mp4Box box)
    {
        stream.Position = box.Offset + 8;

        uint versionFlags = ReadUInt32BE(stream);

        byte version =
            (byte)(versionFlags >> 24);

        uint flags =
            versionFlags & 0x00FFFFFF;

        uint sampleCount =
            ReadUInt32BE(stream);

        int dataOffset = 0;

        bool hasDataOffset =
            (flags & 0x000001) != 0;

        if (hasDataOffset)
        {
            dataOffset = ReadInt32BE(stream);
        }

        // first_sample_flags-present
        if ((flags & 0x000004) != 0)
        {
            ReadUInt32BE(stream);
        }

        bool hasDuration =
            (flags & 0x000100) != 0;

        bool hasSize =
            (flags & 0x000200) != 0;

        bool hasFlags =
            (flags & 0x000400) != 0;

        bool hasCompositionOffset =
            (flags & 0x000800) != 0;

        var samples = new List<TrunSample>(
            checked((int)sampleCount));

        for (int i = 0; i < sampleCount; i++)
        {
            uint? duration = null;
            uint? size = null;
            int compositionOffset = 0;

            if (hasDuration)
                duration = ReadUInt32BE(stream);

            if (hasSize)
                size = ReadUInt32BE(stream);

            if (hasFlags)
                ReadUInt32BE(stream);

            if (hasCompositionOffset)
            {
                uint value = ReadUInt32BE(stream);

                compositionOffset =
                    version == 0
                        ? checked((int)value)
                        : ReadSignedInt32(value);
            }

            samples.Add(
                new TrunSample
                {
                    Duration = duration,
                    Size = size,
                    CompositionTimeOffset = compositionOffset
                });
        }

        return new TrunInfo
        {
            HasDataOffset = hasDataOffset,
            DataOffset = dataOffset,
            Samples = samples
        };
    }

    private static int ReadSignedInt32(uint value)
    {
        return unchecked((int)value);
    }

    // ================================================================
    // WebVTT sample parsing
    // ================================================================

    private static VttCueData ExtractVttCue(byte[] sample)
    {
        using (var stream = new MemoryStream(sample))
        {
            while (stream.Position + 8 <= stream.Length)
            {
                long boxStart = stream.Position;

                uint size32 = ReadUInt32BE(stream);
                string type = ReadType(stream);

                long boxSize;

                if (size32 == 1)
                {
                    boxSize =
                        checked((long)ReadUInt64BE(stream));
                }
                else if (size32 == 0)
                {
                    boxSize =
                        stream.Length - boxStart;
                }
                else
                {
                    boxSize = size32;
                }

                if (boxSize < 8 ||
                    boxStart + boxSize > stream.Length)
                {
                    break;
                }

                long boxEnd = boxStart + boxSize;

                if (type == "vttc")
                {
                    return ExtractVttcCue(
                        stream,
                        boxEnd);
                }

                stream.Position = boxEnd;
            }
        }

        return null;
    }

    private static VttCueData ExtractVttcCue(
        Stream stream,
        long end)
    {
        string text = null;
        string settings = null;

        while (stream.Position + 8 <= end)
        {
            long boxStart = stream.Position;

            uint size32 = ReadUInt32BE(stream);
            string type = ReadType(stream);

            long boxSize;

            if (size32 == 1)
            {
                boxSize =
                    checked((long)ReadUInt64BE(stream));
            }
            else if (size32 == 0)
            {
                boxSize = end - boxStart;
            }
            else
            {
                boxSize = size32;
            }

            if (boxSize < 8 ||
                boxStart + boxSize > end)
            {
                break;
            }

            long boxEnd = boxStart + boxSize;

            int length =
                checked((int)(
                    boxEnd - stream.Position));

            byte[] data = new byte[length];

            ReadExactly(stream, data);

            switch (type)
            {
                case "payl":
                    text = DecodeVttText(data);
                    break;

                case "sttg":
                    settings = DecodeVttText(data);
                    break;
            }

            stream.Position = boxEnd;
        }

        if (String.IsNullOrWhiteSpace(text))
            return null;

        return new VttCueData
        {
            Text = text,
            Settings = settings ?? ""
        };
    }

    private static string DecodeVttText(
        byte[] data)
    {
        return Encoding.UTF8
            .GetString(data)
            .Trim('\0', '\r', '\n', ' ');
    }

    // ================================================================
    // Find WebVTT track
    // ================================================================

    private static uint FindWebVttTrackId(
        Stream stream)
    {
        Mp4Box moov =
            FindTopLevelBox(stream, "moov");

        if (moov == null)
            throw new InvalidDataException(
                "moov box not found.");

        foreach (var trak in FindChildren(moov, "trak"))
        {
            Mp4Box mdia =
                FindChild(trak, "mdia");

            Mp4Box hdlr =
                FindChild(mdia, "hdlr");

            string handler =
                ReadHandlerType(hdlr);

            if (handler != "text")
                continue;

            Mp4Box minf =
                FindChild(mdia, "minf");

            Mp4Box stbl =
                FindChild(minf, "stbl");

            Mp4Box stsd =
                FindChild(stbl, "stsd");

            if (stsd != null &&
                IsWebVttTrack(stsd))
            {
                Mp4Box tkhd =
                    FindChild(trak, "tkhd");

                if (tkhd == null)
                    continue;

                return ReadTrackId(tkhd);
            }
        }

        throw new InvalidDataException(
            "No WebVTT track found.");
    }

    private static bool IsWebVttTrack(
        Mp4Box stsd)
    {
        stsd.Stream.Position =
            stsd.Offset + 8;

        byte[] data =
            new byte[checked((int)(
                stsd.Size - 8))];

        ReadExactly(stsd.Stream, data);

        return Encoding.ASCII
            .GetString(data)
            .IndexOf(
                "wvtt",
                StringComparison.Ordinal) >= 0;
    }

    // ================================================================
    // Track ID
    // ================================================================

    private static uint ReadTrackId(
        Mp4Box tkhd)
    {
        tkhd.Stream.Position =
            tkhd.Offset + 8;

        uint versionFlags =
            ReadUInt32BE(tkhd.Stream);

        byte version =
            (byte)(versionFlags >> 24);

        if (version == 1)
        {
            // version + flags
            // creation_time: 8
            // modification_time: 8
            // track_ID: 4
            tkhd.Stream.Position =
                tkhd.Offset + 28;
        }
        else
        {
            // version + flags
            // creation_time: 4
            // modification_time: 4
            // track_ID: 4
            tkhd.Stream.Position =
                tkhd.Offset + 20;
        }

        return ReadUInt32BE(tkhd.Stream);
    }

    // ================================================================
    // mdhd timescale
    // ================================================================

    private static int FindTrackTimescale(
        Stream stream,
        uint trackId)
    {
        Mp4Box moov =
            FindTopLevelBox(stream, "moov");

        if (moov == null)
            throw new InvalidDataException(
                "moov box not found.");

        foreach (var trak in FindChildren(moov, "trak"))
        {
            Mp4Box tkhd =
                FindChild(trak, "tkhd");

            if (tkhd == null)
                continue;

            if (ReadTrackId(tkhd) != trackId)
                continue;

            Mp4Box mdia =
                FindChild(trak, "mdia");

            Mp4Box mdhd =
                FindChild(mdia, "mdhd");

            if (mdhd == null)
                throw new InvalidDataException(
                    "mdhd not found.");

            mdhd.Stream.Position =
                mdhd.Offset + 8;

            uint versionFlags =
                ReadUInt32BE(mdhd.Stream);

            byte version =
                (byte)(versionFlags >> 24);

            if (version == 1)
            {
                mdhd.Stream.Position =
                    mdhd.Offset + 28;
            }
            else
            {
                mdhd.Stream.Position =
                    mdhd.Offset + 20;
            }

            return checked(
                (int)ReadUInt32BE(mdhd.Stream));
        }

        throw new InvalidDataException(
            "Track " + trackId + " not found.");
    }

    // ================================================================
    // MP4 box helpers
    // ================================================================

    private static Mp4Box FindTopLevelBox(
        Stream stream,
        string type)
    {
        stream.Position = 0;

        while (stream.Position + 8 <= stream.Length)
        {
            Mp4Box box = ReadNextBox(stream);

            if (box == null)
                break;

            if (box.Type == type)
                return box;

            stream.Position =
                box.Offset + box.Size;
        }

        return null;
    }

    private static IEnumerable<Mp4Box>
        FindTopLevelBoxes(
            Stream stream,
            string type)
    {
        stream.Position = 0;

        while (stream.Position + 8 <= stream.Length)
        {
            Mp4Box box =
                ReadNextBox(stream);

            if (box == null)
                yield break;

            if (box.Type == type)
                yield return box;

            stream.Position =
                box.Offset + box.Size;
        }
    }

    private static Mp4Box ReadNextBox(
        Stream stream)
    {
        long offset =
            stream.Position;

        if (stream.Length - offset < 8)
            return null;

        uint size32 =
            ReadUInt32BE(stream);

        string type =
            ReadType(stream);

        long size;

        if (size32 == 1)
        {
            size =
                checked((long)ReadUInt64BE(stream));
        }
        else if (size32 == 0)
        {
            size =
                stream.Length - offset;
        }
        else
        {
            size = size32;
        }

        if (size < 8 ||
            offset + size > stream.Length)
        {
            throw new InvalidDataException(
                "Invalid MP4 box '" + type +
                "' at " + offset +
                ", size=" + size);
        }

        return new Mp4Box
        {
            Offset = offset,
            Size = size,
            Type = type,
            Stream = stream
        };
    }

    private static Mp4Box FindChild(
        Mp4Box parent,
        string type)
    {
        if (parent == null)
            return null;

        Stream stream =
            parent.Stream;

        long position =
            parent.Offset + 8;

        long end =
            parent.Offset + parent.Size;

        stream.Position = position;

        while (stream.Position + 8 <= end)
        {
            Mp4Box box =
                ReadBoxWithin(stream, end);

            if (box == null)
                return null;

            if (box.Type == type)
                return box;

            stream.Position =
                box.Offset + box.Size;
        }

        return null;
    }

    private static IEnumerable<Mp4Box>
        FindChildren(
            Mp4Box parent,
            string type)
    {
        Stream stream =
            parent.Stream;

        long end =
            parent.Offset + parent.Size;

        stream.Position =
            parent.Offset + 8;

        while (stream.Position + 8 <= end)
        {
            Mp4Box box =
                ReadBoxWithin(stream, end);

            if (box == null)
                yield break;

            if (box.Type == type)
                yield return box;

            stream.Position =
                box.Offset + box.Size;
        }
    }

    private static Mp4Box ReadBoxWithin(
        Stream stream,
        long parentEnd)
    {
        long offset =
            stream.Position;

        if (parentEnd - offset < 8)
            return null;

        uint size32 =
            ReadUInt32BE(stream);

        string type =
            ReadType(stream);

        long size;

        if (size32 == 1)
        {
            if (parentEnd - stream.Position < 8)
                throw new InvalidDataException(
                    "Invalid extended MP4 box.");

            size =
                checked((long)ReadUInt64BE(stream));
        }
        else if (size32 == 0)
        {
            size =
                parentEnd - offset;
        }
        else
        {
            size = size32;
        }

        if (size < 8 ||
            offset + size > parentEnd)
        {
            throw new InvalidDataException(
                "Invalid MP4 box '" + type +
                "' at " + offset +
                ", size=" + size);
        }

        return new Mp4Box
        {
            Offset = offset,
            Size = size,
            Type = type,
            Stream = stream
        };
    }

    // ================================================================
    // Handler
    // ================================================================

    private static string ReadHandlerType(
        Mp4Box hdlr)
    {
        if (hdlr == null)
            return "";

        // hdlr:
        // version + flags = 4
        // pre_defined = 4
        // handler_type = 4
        hdlr.Stream.Position =
            hdlr.Offset + 16;

        return ReadType(hdlr.Stream);
    }

    // ================================================================
    // Binary readers
    // ================================================================

    private static uint ReadUInt32BE(
        Stream stream)
    {
        byte[] buffer = new byte[4];

        ReadExactly(stream, buffer);

        return ((uint)buffer[0] << 24)
             | ((uint)buffer[1] << 16)
             | ((uint)buffer[2] << 8)
             | buffer[3];
    }

    private static int ReadInt32BE(
        Stream stream)
    {
        return unchecked(
            (int)ReadUInt32BE(stream));
    }

    private static ulong ReadUInt64BE(
        Stream stream)
    {
        byte[] buffer = new byte[8];

        ReadExactly(stream, buffer);

        ulong value = 0;

        for (int i = 0; i < 8; i++)
            value = (value << 8) | buffer[i];

        return value;
    }

    private static string ReadType(
        Stream stream)
    {
        byte[] buffer = new byte[4];

        ReadExactly(stream, buffer);

        return Encoding.ASCII.GetString(buffer);
    }

    // .NET Framework 4.8 does not have Stream.ReadExactly().
    private static void ReadExactly(
        Stream stream,
        byte[] buffer)
    {
        int offset = 0;

        while (offset < buffer.Length)
        {
            int read =
                stream.Read(
                    buffer,
                    offset,
                    buffer.Length - offset);

            if (read == 0)
            {
                throw new EndOfStreamException(
                    "Unexpected end of stream.");
            }

            offset += read;
        }
    }

    // ================================================================
    // VTT output
    // ================================================================

    private static void WriteVtt(
        string path,
        List<VttCue> cues)
    {
        using (var writer = new StreamWriter(
            path,
            false,
            new UTF8Encoding(false)))
        {
            writer.WriteLine("WEBVTT");
            writer.WriteLine();

            foreach (var cue in cues)
            {
                writer.WriteLine(
                    FormatTime(cue.Start) +
                    " --> " +
                    FormatTime(cue.End) +
                    (String.IsNullOrWhiteSpace(cue.Settings)
                        ? ""
                        : " " + cue.Settings));

                writer.WriteLine(cue.Text);
                writer.WriteLine();
            }
        }
    }

    private static string FormatTime(
        TimeSpan time)
    {
        // WebVTT timestamps can exceed 24 hours,
        // so don't use time.Hours.
        int hours =
            (int)time.TotalHours;

        return String.Format(
            "{0:00}:{1:00}:{2:00}.{3:000}",
            hours,
            time.Minutes,
            time.Seconds,
            time.Milliseconds);
    }

    // ================================================================
    // Models
    // ================================================================

    private sealed class Mp4Box
    {
        public Stream Stream { get; set; }
        public string Type { get; set; }
        public long Offset { get; set; }
        public long Size { get; set; }
    }

    private sealed class TfhdInfo
    {
        public uint TrackId { get; set; }

        public ulong BaseDataOffset { get; set; }

        public uint DefaultSampleDuration { get; set; }

        public uint DefaultSampleSize { get; set; }
    }

    private sealed class TrunInfo
    {
        public bool HasDataOffset { get; set; }

        public int DataOffset { get; set; }

        public List<TrunSample> Samples { get; set; }
            = new List<TrunSample>();
    }

    private sealed class TrunSample
    {
        public uint? Duration { get; set; }

        public uint? Size { get; set; }

        public int CompositionTimeOffset { get; set; }
    }

    private sealed class VttCue
    {
        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public string Text { get; set; }

        public string Settings { get; set; }
    }

    private sealed class VttCueData
    {
        public string Text { get; set; }

        public string Settings { get; set; }
    }
}
