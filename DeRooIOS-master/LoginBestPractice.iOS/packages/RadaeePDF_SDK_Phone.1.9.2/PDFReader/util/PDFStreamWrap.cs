using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDPDFLib.pdf;
using System.IO;

namespace RDPDFLib.util
{
    class PDFStreamWrap : PDFStream
    {
        private Stream m_stream = null;
        private bool m_fix = false;
        public void Open(Stream stream)
        {
            if (stream != null && stream.CanSeek && stream.CanRead)
            {
                m_stream = stream;
                string type_name = m_stream.GetType().ToString();
                m_fix = (type_name.LastIndexOf("NativeFileStream") > 0);
            }
        }
        public virtual void Close()
        {
            if (m_stream != null)
            {
                //m_stream.Close();
                m_stream.Dispose();
                m_stream = null;
            }
        }
        public virtual void Flush()
        {
            if (m_stream != null)
                m_stream.Flush();
        }
        public virtual long GetLength()
        {
            if (m_stream == null) return 0;
            return m_stream.Length;
        }
        public virtual long GetPosition()
        {
            if (m_stream == null) return 0;
            return m_stream.Position;
        }
        public virtual int Read(byte[] buf)
        {
            if (m_stream == null) return 0;
            int read = m_stream.Read(buf, 0, buf.Length);
            return read;
        }
        public virtual bool SetPosition(long pos)
        {
            if (m_stream == null) return false;
            string type_name = m_stream.GetType().ToString();
            if (m_fix)
            {
                ulong uoffset = (ulong)pos;
                ulong fix = ((uoffset & 0xffffffffL) << 32) | ((uoffset & 0xffffffff00000000L) >> 32);
                m_stream.Seek((long)fix, SeekOrigin.Begin);
            }
            else
                m_stream.Seek(pos, SeekOrigin.Begin);
            return true;
        }
        public virtual int Write(byte[] buf)
        {
            if (m_stream == null) return 0;
            long prev = m_stream.Position;
            m_stream.Write(buf, 0, buf.Length);
            return (int)(m_stream.Position - prev);
        }
        public virtual bool Writeable()
        {
            if (m_stream == null) return false;
            return m_stream.CanWrite;
        }
    }
}
