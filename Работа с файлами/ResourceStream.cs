using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        private readonly char[] separators = new char[] { '\0', '\u0001' };
        private readonly Stream stream;
        private readonly string key;
        private bool seek = false;
        private bool returned = false;
        private readonly int bufferSize = Constants.BufferSize;
        public ResourceReaderStream(Stream stream, string key)
        {
            this.stream = new BufferedStream(stream, bufferSize);
            this.key = key;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!seek) SeekValue();
            if (!returned)
                return ReadFieldValue(buffer, offset, count);
            return 0;
        }

        private void SeekValue()
        {
            var byteList = new List<byte>();
            while (true)
            {
                var byteRead = (byte)stream.ReadByte();
                byteList.Add(byteRead);
                if (byteList.Count > key.Length)
                    byteList.RemoveAt(0);
                if (Encoding.ASCII.GetString(byteList.ToArray()) == key || byteRead == 255)
                {
                    stream.ReadByte();
                    stream.ReadByte();
                    seek = true;
                    break;
                }
            }
        }

        private int ReadFieldValue(byte[] buffer, int offset, int count)
        {
            int counter;
            for (counter = offset; counter < count + offset;)
            {
                var readByte1 = (byte)stream.ReadByte();
                var readByte2 = (byte)stream.ReadByte();
                if (readByte1 == separators[1] || readByte2 == separators[1] || readByte1 == 255)
                {
                    if (counter > 0 && buffer[counter - 1] == separators[0])
                        counter--;
                    break;
                }
                if (!(readByte1 == readByte2 && readByte1 == separators[0]))
                    buffer[counter++] = readByte1;
                buffer[counter++] = readByte2;
                if (counter >= count + offset - 1) return counter + 1;
            }
            returned = true;
            return counter;
        }

        public override void Flush() { }
        public override bool CanRead => throw new NotImplementedException();
        public override bool CanSeek => throw new NotImplementedException();
        public override bool CanWrite => throw new NotImplementedException();
        public override long Length => throw new NotImplementedException();
        public override long Position
        { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
    }
}
