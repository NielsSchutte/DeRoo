using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using RDPDFLib.pdf;

namespace RDPDFLib.view
{
    public class PDFVBmpSet
    {
        private struct PDFBMP
        {
            public PDFBmp bmp;
            public bool used;
        }
        private PDFBMP[] m_bmps;
        private int m_bmps_cnt;
        private int m_mem_used;
        private int m_unsed_cnt;
        private int MEM_LIMIT = (2 << 20);
        public PDFVBmpSet()
        {
            m_bmps = new PDFBMP[256];
            m_bmps_cnt = 0;
            m_mem_used = 0;
            m_unsed_cnt = 0;
            int cur = 0;
            int cnt = 256;
            while (cur < cnt)
            {
                m_bmps[cur].used = false;
                m_bmps[cur].bmp = null;
                cur++;
            }
        }
        public void setlimit(int size)
        {
            if (size < (2 << 20))
                size = (2 << 20);
            else MEM_LIMIT = size;
        }
        private void flush()
        {
            if (m_unsed_cnt > 2 || m_mem_used > MEM_LIMIT)
            {
                int cur = 0;
                int cnt = m_bmps_cnt;
                while (cur < cnt)
                {
                    if (m_bmps[cur].bmp != null && !m_bmps[cur].used)
                    {
                        PDFBmp bmp = m_bmps[cur].bmp;
                        m_mem_used -= bmp.Width * bmp.Height;
                        m_unsed_cnt--;
                        bmp = null;
                        m_bmps[cur].bmp = null;
                        cnt--;
                        while (cur < cnt)
                        {
                            m_bmps[cur] = m_bmps[cur + 1];
                            cur++;
                        }
                        m_bmps[cur].bmp = null;
                        m_bmps[cur].used = false;
                        m_bmps_cnt--;
                        break;
                    }
                    cur++;
                }
            }
        }
        public PDFBmp alloc(int w, int h)
        {
            int cur = 0;
            int cnt = m_bmps_cnt;
            PDFBmp bmp;
            while (cur < cnt)
            {
                bmp = m_bmps[cur].bmp;
                if (bmp != null && !m_bmps[cur].used && bmp.Width == w && bmp.Height == h)
                {
                    m_bmps[cur].used = true;
                    m_unsed_cnt--;
                    return bmp;
                }
                cur++;
            }
            m_mem_used += w * h;
            bmp = new PDFBmp(w, h);
            m_bmps[cnt].bmp = bmp;
            m_bmps[cnt].used = true;
            m_bmps_cnt++;
            flush();
            return bmp;
        }
        public void popup(PDFBmp bmp)
        {
            int cur = 0;
            int cnt = m_bmps_cnt;
            while (cur < cnt)
            {
                if (bmp == m_bmps[cur].bmp)
                {
                    m_mem_used -= bmp.Width * bmp.Height;
                    m_unsed_cnt--;
                    bmp = null;
                    m_bmps[cur].bmp = null;
                    cnt--;
                    while (cur < cnt)
                    {
                        m_bmps[cur] = m_bmps[cur + 1];
                        cur++;
                    }
                    m_bmps[cur].bmp = null;
                    m_bmps[cur].used = false;
                    m_bmps_cnt--;
                    break;
                }
                cur++;
            }
        }
        public void dealloc(PDFBmp bmp)
        {
            if (bmp == null) return;
            int cur = 0;
            int cnt = m_bmps_cnt;
            while (cur < cnt)
            {
                if (bmp == m_bmps[cur].bmp)
                {
                    m_bmps[cur].used = false;
                    m_unsed_cnt++;
                    flush();
                    break;
                }
                cur++;
            }
        }
    }
}
