using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Streams.Compression
{
    //RLE
    public class CustomCompressionStream : Stream
    {
        private readonly bool read;
        private readonly Stream baseStream;
        private int counter = 0;
        private static int valueByteStock = -1;
        private static int repeatByteStock = -1;

        public CustomCompressionStream(Stream baseStream, bool read)
        {
            this.read = read;
            this.baseStream = baseStream;
            valueByteStock = -1;
            repeatByteStock = -1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int subCounter;
            for (subCounter = 0; subCounter < repeatByteStock; subCounter++)
                buffer[subCounter] = (byte)valueByteStock;
            valueByteStock = -1;
            repeatByteStock = -1;
            for (counter = subCounter; counter < count + offset;)
            {
                var valueByte = baseStream.ReadByte();
                var repeatByte = baseStream.ReadByte();
                if (repeatByte == -1)
                    return valueByte == -1 ? counter : throw new InvalidOperationException();
                for (int i = 0; i < repeatByte; i++)
                {
                    if (counter + offset == buffer.Length)
                    {
                        valueByteStock = valueByte;
                        repeatByteStock = repeatByte - i;
                        if (offset > 0)
                            buffer[buffer.Length - 1] = 0;
                        return counter - offset;
                    }
                    buffer[counter++ + offset] = (byte)valueByte;
                }
            }
            return counter;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++) 
            {
                byte countRepeatByte = 1;
                while (i < buffer.Length - 1 && buffer[i] == buffer[i + 1] && countRepeatByte < byte.MaxValue)
                {
                    countRepeatByte++;
                    i++;
                }
                baseStream.WriteByte(buffer[i]);
                baseStream.WriteByte(i == buffer.Length - 1 ? 
                    (byte)(countRepeatByte - offset) : countRepeatByte);
            }
        }

        public override bool CanRead => read;
        public override bool CanSeek => false;
        public override bool CanWrite => !read;
        public override long Length => throw new NotSupportedException();
        public override long Position
        { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => baseStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
