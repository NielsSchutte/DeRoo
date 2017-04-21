using System;
using RDPDFLib.pdf;
using Windows.UI.Xaml.Media.Imaging;

namespace RDPDFLib
{
    namespace view
    {
        public class PDFVCache
        {
            protected PDFDoc m_doc;
            protected int m_pageno;
            public PDFPage m_page = null;
            protected float m_scale = 1;
            public PDFBmp m_dib = null;
            protected int m_dibw = 0;
            protected int m_dibh = 0;
            public int m_status = 0;
            PDFVBmpSet m_bmps = null;
            public PDFVCache(PDFVBmpSet bmps, PDFDoc doc, int pageno, float scale, int w, int h)
            {
                m_doc = doc;
                m_pageno = pageno;
                m_scale = scale;
                m_dibw = w;
                m_dibh = h;
                m_bmps = bmps;
                //int tick = System.Environment.TickCount;
                m_dib = bmps.alloc(m_dibw, m_dibh);
                //int tick1 = System.Environment.TickCount;
                //int tick_n = tick1 - tick;
                m_dib.Reset(0xFFFFFFFF);
                //tick = System.Environment.TickCount - tick1;
                m_status = 0;
            }
            public Boolean UIIsSame(float scale, int w, int h)
            {
                return (m_scale == scale && m_dibw == w && m_dibh == h);
            }
            public void Clear()
            {
                if (m_page != null)
                {
                    m_page.Close();
                    m_page = null;
                }
                m_bmps.dealloc(m_dib);
                m_dib = null;
                m_status = 0;
            }
            public void UIRenderCancel()
            {
                if (m_status == 0)
                {
                    m_status = 2;
                    PDFPage page = m_page;
                    if (page != null)
                    {
                        page.RenderCancel();
                        page = null;
                    }
                }
            }
            public void Render()
            {
                if (m_status == 2 || m_dib == null) return;
                if (m_page == null)
                    m_page = m_doc.GetPage(m_pageno);
                m_page.RenderPrepare();
                if (m_status == 2) return;
                PDFMatrix mat = new PDFMatrix(m_scale, -m_scale, 0, m_dibh);
                m_page.RenderToBmp(m_dib, mat, true, PDFView.renderQuality);
                if (m_status != 2)
                    m_status = 1;
                mat = null;
            }
        }
    }
}