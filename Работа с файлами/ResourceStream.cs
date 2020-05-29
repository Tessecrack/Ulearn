using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        private Stream testStream;
        private byte[] byteKey;
        private bool finish = false;
        private bool seekValue = false;        

        public ResourceReaderStream(Stream stream, string key)
        {
            testStream = new BufferedStream(stream, 1024);
            this.byteKey = Encoding.ASCII.GetBytes(key);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (finish) return 0;
            if (!seekValue) SeekValue();
            seekValue = true;
            return ReadFieldValue(buffer, offset, count);
        }

        private void SeekValue()
        {
            if (SeekKey()) 
                while(true)
                {
                    var byteFromStream = testStream.ReadByte();
                    if (byteFromStream == 1) return;
                }
        }

        private bool SeekKey()
        {
            var data = new List<byte>();
            while(true)
            {
                var byteFromStream = testStream.ReadByte();
                if (IsEndStream(byteFromStream)) return false;
                data.Add((byte)byteFromStream);
                if (data.SequenceEqual(byteKey)) return true;
                if (data.Count == byteKey.Length) data.RemoveAt(0);
            }
        }

        private int ReadFieldValue(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var byteFromStream = ParseByte();
                if (IsEndStream(byteFromStream)) return i;
                buffer[i + offset] = (byte)byteFromStream;
            }
            return count;
        }
        private bool IsEndStream(int byteFromStream)
        {
            if (byteFromStream == -1)
            {
                finish = true;
                return true;
            }
            return false;
        }
        private int ParseByte()
        {
            int currentByte = testStream.ReadByte();
            int nextByte;
            if (currentByte == 0) nextByte = testStream.ReadByte();
            else nextByte = currentByte;
            if (currentByte == -1 || nextByte == -1 || currentByte == 0 && nextByte == 1) return -1;
            return nextByte;
        }

        #region
        public override void Flush()
            => testStream.Flush();
        public override long Seek(long offset, SeekOrigin origin)
            => testStream.Seek(offset, origin);
        public override void SetLength(long value) => testStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => testStream.Write(buffer, offset, count);
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => testStream.Length;
        public override long Position
        { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion
    }

}
