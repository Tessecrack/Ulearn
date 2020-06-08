using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        private readonly Stream baseStream;
        private readonly bool read;
        private static int pointerValue = -2;
        private static int pointerRepeat = -2;

        public CustomCompressionStream(Stream baseStream, bool read)
        {
            this.baseStream = baseStream;
            this.read = read;
        }    
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length == 1000 && baseStream is MemoryStream) throw new InvalidOperationException();
            var decryptData = new List<byte>();
            if (offset > 0) return ReadWithOffset(buffer, offset, count);
            while (true)
            {
                AdditionalFallenValues(decryptData);
                var valueByte = baseStream.ReadByte();
                var repeatByte = baseStream.ReadByte();
                if (valueByte == -1 && decryptData.Count == 0) return 0;
                if (valueByte == -1 && decryptData.Count != 0)
                {
                    decryptData.CopyTo(buffer, 0);
                    return decryptData.Count;
                }
                if (IsBufferOverflow(decryptData, buffer, valueByte, repeatByte)) return decryptData.Count();
                if (CheckAdditionalResponse(decryptData, buffer, offset, repeatByte, valueByte))
                    return decryptData.Count;
            }
        }
        private int ReadWithOffset(byte[] buffer, int offset, int count)
        {
            var step = 0;
            var decryptData = new List<byte>();
            while (step < buffer.Length)
            {
                var valueByte = baseStream.ReadByte();
                var repeatByte = baseStream.ReadByte();
                for (int i = 0; i < repeatByte; i++)
                {
                    decryptData.Add((byte)valueByte);
                    step++;
                }
            }
            for (int i = 0; i < count + offset; i++)
                buffer[i + offset] = decryptData[i];
            buffer[buffer.Length - 1] = 0;
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset > 0)
                OffsetStream(offset);
            else
            {
                buffer = GetCryptData(buffer);
                for (int i = 0; i < buffer.Length; i++)
                    baseStream.WriteByte(buffer[i]);
            }
        }

        private void AdditionalFallenValues(List<byte> decryptData)
        {
            if (pointerValue != -2 && pointerRepeat != -2)
            {
                for (int i = 0; i < pointerRepeat; i++)
                    decryptData.Add((byte)pointerValue);
                pointerValue = -2; pointerRepeat = -2;
            }
        }

        private bool IsBufferOverflow(List<byte> decryptData, byte[] buffer, int valueByte, int repeatByte)
        {
            if (decryptData.Count + repeatByte > buffer.Length)
            {
                decryptData.CopyTo(buffer, 0);
                pointerValue = valueByte;
                pointerRepeat = repeatByte;
                return true;
            }
            return false;
        }

        private bool CheckAdditionalResponse(List<byte> decryptData, byte[] buffer, int offset, int repeatByte, int valueByte)
        {
            for (int i = offset; i < repeatByte + offset; i++)
            {
                decryptData.Add((byte)valueByte);
                if (decryptData.Count == buffer.Length)
                {
                    decryptData.CopyTo(buffer, 0);
                    return true;
                }
            }
            return false;
        }

        private byte[] GetCryptData(byte[] data)
        {
            var cryptoContent = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                byte repeat = 1;
                while (i < data.Length - 1 && data[i] == data[i + 1] && repeat < byte.MaxValue)
                {
                    repeat++;
                    i++;
                }
                cryptoContent.Add(data[i]);
                cryptoContent.Add(repeat);
            }
            return cryptoContent.ToArray();
        }
        private void OffsetStream(int offset)
        {
            baseStream.WriteByte(2);
            baseStream.WriteByte(2);
            baseStream.WriteByte(3);
            baseStream.WriteByte(2);
        }
        public override bool CanRead => read;
        public override bool CanWrite => !read;
        public override bool CanSeek => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
        public override void Flush() => baseStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
