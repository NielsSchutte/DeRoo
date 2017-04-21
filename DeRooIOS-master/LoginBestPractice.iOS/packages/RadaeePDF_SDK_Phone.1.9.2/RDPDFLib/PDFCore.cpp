#include "pch.h"
#include "PDFCore.h"

using namespace RDPDFLib::pdf;

float PDFGlobal::zoom_level = 3;

char *cvt_str_cstr( String ^str )
{
	if( !str ) return NULL;
	const wchar_t *wstr = str->Data();
	int wlen = str->Length();
	char *data = (char *)malloc( (wlen + 1) * 2 );
	int len = ::WideCharToMultiByte( CP_ACP, 0, wstr, wlen, data, (wlen + 1) * 2, NULL, NULL );
	data[len] = 0;
	return data;
}
String ^cvt_cstr_str( const char *str )
{
	if( !str || !str[0] ) return nullptr;
	int len1 = strlen( str ) + 2;
	wchar_t *wtxt = (wchar_t *)malloc( sizeof(wchar_t) * len1 );
	int len = MultiByteToWideChar( CP_ACP, 0, str, -1, wtxt, len1 );
	wtxt[len] = 0;
	String ^ret = ref new String( wtxt );
	free( wtxt );
	return ret;
}

PDFDoc::PDFDoc()
{
	m_doc = NULL;
	m_inner = NULL;
}
PDFDoc::~PDFDoc()
{
	Close();
}

PDF_ERROR PDFDoc::Open( IRandomAccessStream ^stream, String ^password )
{
	PDF_ERR err;
	char *pswd = cvt_str_cstr( password );
	m_doc = Document_open( stream, pswd, &err );
	free( pswd );
	if( m_doc ) return PDF_ERROR::err_ok;
	else return (PDF_ERROR)err;
}

PDF_ERROR PDFDoc::OpenStream( PDFStream ^stream, String ^password )
{
	PDF_ERR err;
	char *pswd = cvt_str_cstr( password );
	m_inner = new PDFStreamInner;
	m_inner->Open( stream );
	m_doc = Document_openStream( m_inner, pswd, &err );
	free( pswd );
	if( m_doc ) return PDF_ERROR::err_ok;
	else return (PDF_ERROR)err;
}

PDF_ERROR PDFDoc::OpenPath( String ^path, String ^password )
{
	PDF_ERR err;
	char *cpath = cvt_str_cstr(path);
	char *pswd = cvt_str_cstr( password );
	m_doc = Document_openPath( cpath, pswd, &err );
	free( pswd );
	free( cpath );
	if( m_doc ) return PDF_ERROR::err_ok;
	else return (PDF_ERROR)err;
}

String ^PDFDoc::GetMeta(String ^tag)
{
	if( !tag ) return nullptr;
	char *ctag = cvt_str_cstr( tag );
	wchar_t meta[1024];
	Document_getMetaW( m_doc, ctag, meta, 1023 );
	free( ctag );
	return ref new String(meta);
}

String ^PDFDoc::ExportForm()
{
	char txt[2048];
	if( !Document_exportForm( m_doc, txt, 2048 ) ) return nullptr;
	wchar_t wtxt[2048];
	::MultiByteToWideChar( CP_UTF8, 0, txt, -1, wtxt, 2047 );
	return ref new String(wtxt);
}

PDFPage ^PDFDoc::GetPage(int pageno)
{
	PDF_PAGE page = Document_getPage( m_doc, pageno );
	if( page )
	{
		PDFPage ^pg = ref new PDFPage();
		pg->m_page = page;
		//pg->m_doc = this;
		return pg;
	}
	else
		return nullptr;
}

PDFOutline ^PDFDoc::GetRootOutline()
{
	PDF_OUTLINE outline = Document_getOutlineNext( m_doc, NULL );
	if( outline )
	{
		PDFOutline ^otl =  ref new PDFOutline();
		otl->m_doc = this;
		otl->m_outline = outline;
		return otl;
	}
	else return nullptr;
}

Boolean PDFDoc::AddRootOutline( String ^label, int dest, float y )
{
	return Document_addOutlineNextW( m_doc, NULL, label->Data(), dest, y );
}

Boolean PDFDoc::Save()
{
	return Document_save(m_doc);
}

void PDFDoc::Close()
{
	Document_close(m_doc);
	m_doc = NULL;
	if( m_inner )
	{
		m_inner->Close();
		delete m_inner;
	}
	m_inner = NULL;
}

float PDFDoc::GetPageWidth(int pageno)
{
	return Document_getPageWidth( m_doc, pageno );
}

float PDFDoc::GetPageHeight(int pageno)
{
	return Document_getPageHeight( m_doc, pageno );
}

PDFDocImage ^PDFDoc::NewImageJPEG( String ^path )
{
	char *cpath = cvt_str_cstr( path );
	PDF_DOC_IMAGE image = Document_newImageJPEG( m_doc, cpath );
	free( cpath );
	if( image )
	{
		PDFDocImage ^img = ref new PDFDocImage();
		img->m_image = image;
		return img;
	}
	else return nullptr;
}

PDFDocImage ^PDFDoc::NewImageJPX( String ^path )
{
	char *cpath = cvt_str_cstr( path );
	PDF_DOC_IMAGE image = Document_newImageJPX( m_doc, cpath );
	free( cpath );
	if( image )
	{
		PDFDocImage ^img = ref new PDFDocImage();
		img->m_image = image;
		return img;
	}
	else return nullptr;
}

PDFDocFont ^PDFDoc::NewFontCID( String ^name, int style )
{
	char *fname = cvt_str_cstr( name );
	PDF_DOC_FONT font = Document_newFontCID( m_doc, fname, style );
	free( fname );
	if( font )
	{
		PDFDocFont ^fnt = ref new PDFDocFont();
		fnt->m_font = font;
		fnt->m_doc = this;
		return fnt;
	}
	else return nullptr;
}

PDFDocGState ^PDFDoc::NewGSAlpha( int fill_alpha, int stroke_alpha )
{
	PDF_DOC_GSTATE gs = Document_newGState( m_doc );
	if( gs )
	{
		Document_setGStateFillAlpha( m_doc, gs, fill_alpha );
		Document_setGStateStrokeAlpha( m_doc, gs, stroke_alpha );
		PDFDocGState ^state = ref new PDFDocGState();
		state->m_gs = gs;
		return state;
	}
	else return nullptr;
}

Boolean PDFDoc::RemovePage( int pageno )
{
	return Document_removePage( m_doc, pageno );
}

Boolean PDFDoc::MovePage( int srcno, int dstno )
{
	return Document_movePage( m_doc, srcno, dstno );
}

PDFPage ^PDFDoc::NewPage( int pageno, float w, float h )
{
	PDF_PAGE pg = Document_newPage( m_doc, pageno, w, h );
	if( pg )
	{
		PDFPage ^page = ref new PDFPage();
		//page->m_doc = this;
		page->m_page = pg;
		return page;
	}
	else return nullptr;
}

PDFImportCtx ^PDFDoc::ImportStart(PDFDoc ^src)
{
	PDF_IMPORTCTX ctx = Document_importStart( m_doc, src->m_doc );
	if( ctx )
	{
		PDFImportCtx ^ictx = ref new PDFImportCtx();
		ictx->m_doc = this;
		ictx->m_ctx = ctx;
		return ictx;
	}
	else return nullptr;
}

Boolean PDFDoc::ImportPage( PDFImportCtx ^ctx, int srcno, int dstno )
{
	return Document_importPage( m_doc, ctx->m_ctx, srcno, dstno );
}

PDFOutline ^PDFOutline::GetNext()
{
	PDF_OUTLINE otl = Document_getOutlineNext( m_doc->m_doc, m_outline );
	if( otl )
	{
		PDFOutline ^outline = ref new PDFOutline();
		outline->m_doc = m_doc;
		outline->m_outline = otl;
		return outline;
	}
	else return nullptr;
}

PDFOutline ^PDFOutline::GetChild()
{
	PDF_OUTLINE otl = Document_getOutlineChild( m_doc->m_doc, m_outline );
	if( otl )
	{
		PDFOutline ^outline = ref new PDFOutline();
		outline->m_doc = m_doc;
		outline->m_outline = otl;
		return outline;
	}
	else return nullptr;
}

Boolean PDFOutline::AddNext( String ^label, int dest, float y )
{
	return Document_addOutlineNextW( m_doc->m_doc, m_outline, label->Data(), dest, y );
}

Boolean PDFOutline::AddChild( String ^label, int dest, float y )
{
	return Document_addOutlineChildW( m_doc->m_doc, m_outline, label->Data(), dest, y );
}

Boolean PDFOutline::RemoveFromDoc()
{
	bool ret = Document_removeOutline( m_doc->m_doc, m_outline );
	if( ret )
	{
		m_outline = NULL;
		m_doc = nullptr;
	}
	return ret;
}

PDFAnnot ^PDFPage::GetAnnot( int index )
{
	PDF_ANNOT annot = Page_getAnnot( m_page, index );
	if( annot )
	{
		PDFAnnot ^annot1 = ref new PDFAnnot();
		annot1->m_annot = annot;
		annot1->m_page = this;
		return annot1;
	}
	else return nullptr;
}
PDFAnnot ^PDFPage::GetAnnot( float x, float y )
{
	PDF_ANNOT annot = Page_getAnnotFromPoint( m_page, x, y );
	if( annot )
	{
		PDFAnnot ^annot1 = ref new PDFAnnot();
		annot1->m_annot = annot;
		annot1->m_page = this;
		return annot1;
	}
	else return nullptr;
}
Boolean PDFPage::AddAnnotPopup(PDFAnnot ^parent, PDFRect rect, bool open)
{
	return Page_addAnnotPopup(m_page, parent->m_annot, (const PDF_RECT *)&rect, open);
}
