#pragma once

#include "PDFWinRT.h"
#include <stdlib.h>
#include <windows.h>
using namespace Platform;
char *cvt_str_cstr( String ^str );
String ^cvt_cstr_str( const char *str );

namespace RDPDFLib
{
	namespace view
	{
		ref class PDFVPage;
		ref class PDFView;
	}
	namespace pdf
	{
		ref class PDFMatrix;
		ref class PDFOutline;
		ref class PDFPage;
		ref class PDFAnnot;
		ref class PDFDocImage;
		ref class PDFDocFont;
		ref class PDFDocGState;
		ref class PDFImportCtx;
		ref class PDFPageContent;
		ref class PDFDoc;
		public enum class PDF_ERROR
		{
			err_ok,
			err_invalid_para,
			err_open,
			err_password,
			err_encrypt,
			err_bad_file,
		};
		public enum class PDF_RENDER_MODE
		{
			mode_poor = 0,
			mode_normal = 1,
			mode_best = 2,
		};
		public value struct PDFPoint
		{
			float x;
			float y;
		};
		public value struct PDFRect
		{
			float left;
			float top;
			float right;
			float bottom;
		};
		public value struct PDFRef
		{
			unsigned long long ref;
		};
		public ref class PDFGlobal sealed
		{
		public:
			static void SetCMapsPath( String ^cpath, String ^upath )
			{
				char *cp = cvt_str_cstr( cpath );
				char *up = cvt_str_cstr( upath );
				Global_setCMapsPath( cp, up );
				free( cp );
				free( up );
			}
			static Boolean SetCMYKICC(String ^path)
			{
				char *cp = cvt_str_cstr(path);
				bool ret = Global_setCMYKICC(cp);
				free(cp);
				return ret;
			}
			static void FontFileListStart()
			{
				Global_fontfileListStart();
			}
			static void FontFileListAdd( String ^path )
			{
				char *cp = cvt_str_cstr( path );
				Global_fontfileListAdd( cp );
				free( cp );
			}
			static void FontFileListEnd()
			{
				Global_fontfileListEnd();
			}
			static int GetFaceCount()
			{
				return Global_getFaceCount();
			}
			static String ^GetFaceName( int index )
			{
				return cvt_cstr_str(Global_getFaceName(index));
			}
			static Boolean SetDefaultFont( String ^collection, String ^name, Boolean fixed )
			{
				char *cc = cvt_str_cstr(collection);
				char *cn = cvt_str_cstr(name);
				bool ret = Global_setDefaultFont( cc, cn, fixed );
				free( cc );
				free( cn );
				return ret;
			}
			static void LoadStdFont( int index, String ^path )
			{
				char *cp = cvt_str_cstr( path );
				Global_loadStdFont( index, cp );
				free( cp );
			}
			static Boolean SetAnnotFont( String ^name )
			{
				char *cn = cvt_str_cstr( name );
				bool ret = Global_setAnnotFont( cn );
				free( cn );
				return ret;
			}
			static void SetAnnotTransparence( unsigned int color )
			{
				Global_setAnnotTransparency( color );
			}
			static Boolean ActiveLicense( int type, String ^company, String ^email, String ^serial )
			{
				char *scom = cvt_str_cstr( company );
				char *semail = cvt_str_cstr( email );
				char *sser = cvt_str_cstr( serial );
				bool ret = false;
				switch( type )
				{
				case 1:
					ret = Global_activeProfession( scom, semail, sser );
					break;
				case 2:
					ret = Global_activePremium( scom, semail, sser );
					break;
				default:
					ret = Global_activeStandard( scom, semail, sser );
					break;
				}
				free( scom );
				free( semail );
				free( sser );
				return ret;
			}
			static property float ZoomLevel
			{
				float get(){ return zoom_level; }
				void set(float level){ zoom_level = level; }
			}
		private:
			static float zoom_level;
		};
		public ref class PDFPath sealed
		{
		public:
			PDFPath()
			{
				m_path = Path_create();
			}
			void MoveTo( float x, float y )
			{
				Path_moveTo( m_path, x, y );
			}
			void LineTo( float x, float y )
			{
				Path_lineTo( m_path, x, y );
			}
			void CurveTo( float x1, float y1, float x2, float y2, float x3, float y3 )
			{
				Path_curveTo( m_path, x1, y1, x2, y2, x3, y3 );
			}
			void Close()
			{
				Path_closePath( m_path );
			}
			property int NodesCnt
			{
				int get(){return Path_getNodeCount(m_path);}
			}
			int GetOP( int index )
			{
				PDF_POINT pt;
				return Path_getNode( m_path, index, &pt );
			}
			PDFPoint GetPoint( int index )
			{
				PDF_POINT pt;
				Path_getNode( m_path, index, &pt );
				return *(PDFPoint *)&pt;
			}
		private:
			friend PDFMatrix;
			friend PDFPageContent;
			~PDFPath()
			{
				Path_destroy( m_path );
			}
			PDF_PATH m_path;
		};
		public ref class PDFInk sealed
		{
		public:
			PDFInk( float width, unsigned int color )
			{
				m_ink = Ink_create( width, color );
			}
			void Down( float x, float y )
			{
				Ink_onDown( m_ink, x, y );
			}
			void Move( float x, float y )
			{
				Ink_onMove( m_ink, x, y );
			}
			void Up( float x, float y )
			{
				Ink_onUp( m_ink, x, y );
			}
			property int NodesCnt
			{
				int get(){return Ink_getNodeCount(m_ink);}
			}
			int GetOP( int index )
			{
				PDF_POINT pt;
				return Ink_getNode( m_ink, index, &pt );
			}
			PDFPoint GetPoint( int index )
			{
				PDF_POINT pt;
				Ink_getNode( m_ink, index, &pt );
				return *(PDFPoint *)&pt;
			}
		private:
			friend PDFMatrix;
			friend PDFPage;
			~PDFInk()
			{
				Ink_destroy( m_ink );
			}
			PDF_INK m_ink;
		};
		public ref class PDFMatrix sealed
		{
		public:
			PDFMatrix( float scalex, float scaley, float x0, float y0  )
			{
				m_mat = Matrix_createScale( scalex, scaley, x0, y0 );
			}
			PDFMatrix( float xx, float yx, float xy, float yy, float x0, float y0  )
			{
				m_mat = Matrix_create( xx, yx, xy, yy, x0, y0 );
			}
			void Invert()
			{
				Matrix_invert( m_mat );
			}
			void TransformPath( PDFPath ^path )
			{
				Matrix_transformPath( m_mat, path->m_path );
			}
			void TransformInk( PDFInk ^ink )
			{
				Matrix_transformInk( m_mat, ink->m_ink );
			}
			PDFRect TransformRect( PDFRect rect )
			{
				Matrix_transformRect( m_mat, (PDF_RECT *)&rect );
				return rect;
			}
			PDFPoint TransformPoint( PDFPoint point )
			{
				Matrix_transformPoint( m_mat, (PDF_POINT *)&point );
				return point;
			}
		private:
			PDFMatrix()
			{
				m_mat = NULL;
			}
			friend PDFPage;
			friend PDFPageContent;
			friend ref class RDPDFLib::view::PDFVPage;
			~PDFMatrix()
			{
				Matrix_destroy( m_mat );
			}
			PDF_MATRIX m_mat;
		};
		public ref class PDFDIB sealed
		{
		public:
			PDFDIB(int w, int h )
			{
				m_dib = Global_dibGet( NULL, w, h );
			}
			void Resize(int w, int h)
			{
				m_dib = Global_dibGet( m_dib, w, h );
			}
			Boolean SaveJPG(String ^path, int quality)
			{
				const wchar_t *wtxt = path->Data();
				char tmp[512];
				::WideCharToMultiByte(CP_ACP, 0, wtxt, -1, tmp, 512, NULL, NULL);
				return Global_dibSaveJPG(m_dib, tmp, quality);
			}
			property int Width
			{
				int get() {return Global_dibGetWidth( m_dib );}
			}
			property int Height
			{
				int get() {return Global_dibGetHeight( m_dib );}
			}
			property Array<BYTE> ^Data
			{
				Array<BYTE> ^get()
				{
					int w = Global_dibGetWidth( m_dib );
					int h = Global_dibGetHeight( m_dib );
					return ArrayReference<BYTE>((BYTE *)Global_dibGetData(m_dib), w * h * 4 );
				}
			}
		private:
			~PDFDIB()
			{
				Global_dibFree( m_dib );
				m_dib = NULL;
			}
			friend PDFPage;
			PDF_DIB m_dib;
		};
		public ref class PDFBmp sealed
		{
		public:
			PDFBmp(int w, int h )
			{
				m_w = w;
				m_h = h;
				m_dib = ref new WriteableBitmap(w, h);
				m_bmp = Global_lockBitmap( m_dib );
			}
			property int Width
			{
				int get() {return m_w;}
			}
			property int Height
			{
				int get() {return m_h;}
			}
			property WriteableBitmap ^Data
			{
				WriteableBitmap ^get()
				{
					return m_dib;
				}
			}
			void Reset( unsigned int color )
			{
				Global_eraseColor( m_bmp, color );
			}
			Boolean SaveJPG(String ^path, int quality)
			{
				const wchar_t *wtxt = path->Data();
				char tmp[512];
				::WideCharToMultiByte(CP_ACP, 0, wtxt, -1, tmp, 512, NULL, NULL);
				return Global_saveBitmapJPG(m_bmp, tmp, quality);
			}
		private:
			~PDFBmp()
			{
				Global_unlockBitmap( m_bmp );
				m_dib = nullptr;
			}
			friend PDFPage;
			WriteableBitmap ^m_dib;
			PDF_BMP m_bmp;
			int m_w;
			int m_h;
		};
		public interface class PDFStream
		{
		public:
			bool Writeable();
			long long GetLength();
			long long GetPosition();
			bool SetPosition(long long pos);
			int Read( WriteOnlyArray<BYTE> ^buf );
			int Write( const Array<BYTE> ^buf );
			void Close();
			void Flush();
		};
		public ref class PDFObj sealed
		{
		public:
			PDFObj()
			{
				m_obj = NULL;
			}
			property int type
			{
				int get(){ return Obj_getType(m_obj); }
			}
			property int IntVal
			{
				int get(){ return Obj_getInt(m_obj); }
				void set(int v){ Obj_setInt(m_obj, v); }
			}
			property float RealVal
			{
				float get(){ return Obj_getReal(m_obj); }
				void set(float v){ Obj_setReal(m_obj, v); }
			}
			property bool BoolVal
			{
				bool get(){ return Obj_getBoolean(m_obj); }
				void set(bool v){ Obj_setBoolean(m_obj, v); }
			}
			property String ^NameVal
			{
				String ^get()
				{
					const char *cname = Obj_getName(m_obj);
					if (!cname) return nullptr;
					int clen = strlen(cname);
					wchar_t *wsname = (wchar_t *)malloc(sizeof(wchar_t) * (clen + 1));
					MultiByteToWideChar(CP_ACP, 0, cname, -1, wsname, clen + 1);
					String ^ret = ref new String(wsname);
					free(wsname);
					return ret;
				}
				void set(String ^name)
				{
					const wchar_t *wsname = name->Data();
					int wlen = name->Length();
					char *cname = (char *)malloc(wlen * 4 + 4);
					WideCharToMultiByte(CP_ACP, 0, wsname, -1, cname, wlen * 4 + 4, NULL, NULL);
					Obj_setName(m_obj, cname);
					free(cname);
				}
			}
			property String ^AsciiStringVal
			{
				String ^get()
				{
					const char *cname = Obj_getAsciiString(m_obj);
					if (!cname) return nullptr;
					int clen = strlen(cname);
					wchar_t *wsname = (wchar_t *)malloc(sizeof(wchar_t) * (clen + 1));
					MultiByteToWideChar(CP_ACP, 0, cname, -1, wsname, clen + 1);
					String ^ret = ref new String(wsname);
					free(wsname);
					return ret;
				}
				void set(String ^name)
				{
					const wchar_t *wsname = name->Data();
					int wlen = name->Length();
					char *cname = (char *)malloc(wlen * 4 + 4);
					WideCharToMultiByte(CP_ACP, 0, wsname, -1, cname, wlen * 4 + 4, NULL, NULL);
					Obj_setAsciiString(m_obj, cname);
					free(cname);
				}
			}
			property String ^TextStringVal
			{
				String ^get()
				{
					wchar_t *wsname = (wchar_t *)malloc(sizeof(wchar_t) * 65536);
					Obj_getTextString(m_obj, wsname, 65536);
					String ^ret = ref new String(wsname);
					free(wsname);
					return ret;
				}
				void set(String ^name)
				{
					const wchar_t *wsname = name->Data();
					Obj_setTextString(m_obj, wsname);
				}
			}
			property Array<BYTE> ^HexStringVal
			{
				Array<BYTE> ^get()
				{
					int len;
					unsigned char *data = Obj_getHexString(m_obj, &len);
					if (!data) return nullptr;
					return ArrayReference<BYTE>((BYTE *)data, len);
				}
				void set(const Array<BYTE> ^v)
				{
					BYTE *data = v->Data;
					int len = v->Length;
					Obj_setHexString(m_obj, data, len);
				}
			}
			property PDFRef RefVal
			{
				PDFRef get()
				{
					PDFRef ref;
					ref.ref = Obj_getReference(m_obj);
					return ref;
				}
				void set(PDFRef ref)
				{
					Obj_setReference(m_obj, ref.ref);
				}
			}
			void SetDictionary()
			{
				Obj_dictGetItemCount(m_obj);
			}
			int DictGetItemCount()
			{
				return Obj_dictGetItemCount(m_obj);
			}
			String ^DictGetItemTag(int index)
			{
				const char *tag = Obj_dictGetItemName(m_obj, index);
				if (!tag) return nullptr;
				int clen = strlen(tag);
				wchar_t *wsname = (wchar_t *)malloc(sizeof(wchar_t) * (clen + 1));
				MultiByteToWideChar(CP_ACP, 0, tag, -1, wsname, clen + 1);
				String ^ret = ref new String(wsname);
				free(wsname);
				return ret;
			}
			PDFObj ^DictGetItem(int index)
			{
				PDF_OBJ obj = Obj_dictGetItemByIndex(m_obj, index);
				if (!obj) return nullptr;
				PDFObj ^ret = ref new PDFObj();
				ret->m_obj = obj;
				return ret;
			}
			PDFObj ^DictGetItem(String ^tag)
			{
				const wchar_t *wsname = tag->Data();
				int wlen = tag->Length();
				char *cname = (char *)malloc(wlen * 4 + 4);
				WideCharToMultiByte(CP_ACP, 0, wsname, -1, cname, wlen * 4 + 4, NULL, NULL);
				PDF_OBJ obj = Obj_dictGetItemByName(m_obj, cname);
				free(cname);
				if (!obj) return nullptr;
				PDFObj ^ret = ref new PDFObj();
				ret->m_obj = obj;
				return ret;
			}
			void DictSetItem(String ^tag)
			{
				const wchar_t *wsname = tag->Data();
				int wlen = tag->Length();
				char *cname = (char *)malloc(wlen * 4 + 4);
				WideCharToMultiByte(CP_ACP, 0, wsname, -1, cname, wlen * 4 + 4, NULL, NULL);
				Obj_dictSetItem(m_obj, cname);
				free(cname);
			}
			void DictRemoveItem(String ^tag)
			{
				const wchar_t *wsname = tag->Data();
				int wlen = tag->Length();
				char *cname = (char *)malloc(wlen * 4 + 4);
				WideCharToMultiByte(CP_ACP, 0, wsname, -1, cname, wlen * 4 + 4, NULL, NULL);
				Obj_dictRemoveItem(m_obj, cname);
				free(cname);
			}
			void SetArray()
			{
				Obj_arrayClear(m_obj);
			}
			int ArrayGetItemCount()
			{
				return Obj_arrayGetItemCount(m_obj);
			}
			PDFObj ^ArrayGetItem(int index)
			{
				PDF_OBJ obj = Obj_arrayGetItem(m_obj, index);
				if (!obj) return nullptr;
				PDFObj ^ret = ref new PDFObj();
				ret->m_obj = obj;
				return ret;
			}
			void ArrayAppendItem()
			{
				Obj_arrayAppendItem(m_obj);
			}
			void ArrayInsertItem(int index)
			{
				Obj_arrayInsertItem(m_obj, index);
			}
			void ArrayRemoveItem(int index)
			{
				Obj_arrayRemoveItem(m_obj, index);
			}
			void ArrayClear()
			{
				Obj_arrayClear(m_obj);
			}
		private:
			friend ref class PDFDoc;
			PDF_OBJ m_obj;
		};
		public ref class PDFDoc sealed
		{
		public:
			PDFDoc();
			PDF_ERROR Open( IRandomAccessStream ^stream, String ^password );
			PDF_ERROR OpenStream( PDFStream ^stream, String ^password );
			PDF_ERROR OpenPath( String ^path, String ^password );
			PDF_ERROR Create( IRandomAccessStream ^stream )
			{
				PDF_ERR err;
				m_doc = Document_create( stream, &err );
				return (PDF_ERROR)err;
			}
			PDF_ERROR CreateStream( PDFStream ^stream )
			{
				PDF_ERR err;
				m_inner = new PDFStreamInner;
				m_inner->Open( stream );
				m_doc = Document_createForStream( m_inner, &err );
				return (PDF_ERROR)err;
			}
			PDF_ERROR CreatePath( String ^path )
			{
				PDF_ERR err;
				char *cpath = cvt_str_cstr( path );
				m_doc = Document_createForPath( cpath, &err );
				free( cpath );
				return (PDF_ERROR)err;
			}
			void SetCahce( String ^path )
			{
				char *cpath = cvt_str_cstr( path );
				Document_setCache( m_doc, cpath );
				free( cpath );
			}
			Boolean Save();
			void Close();
			float GetPageWidth(int pageno);
			float GetPageHeight(int pageno);
			String ^GetMeta(String ^tag);
			String ^ExportForm();
			PDFOutline ^GetRootOutline();
			Boolean AddRootOutline( String ^label, int dest, float y );
			PDFPage ^GetPage(int pageno);
			PDFDocImage ^NewImageJPEG( String ^path );
			PDFDocImage ^NewImageJPX( String ^path );
			PDFDocFont ^NewFontCID( String ^name, int style );
			PDFDocGState ^NewGSAlpha( int fill_alpha, int stroke_alpha );
			PDFPage ^NewPage( int pageno, float w, float h );
			Boolean RemovePage( int pageno );
			Boolean MovePage( int srcno, int dstno );
			PDFImportCtx ^ImportStart(PDFDoc ^src);
			Boolean ImportPage( PDFImportCtx ^ctx, int srcno, int dstno );
			property int PageCount
			{
				int get() {return Document_getPageCount(m_doc);}
			}
			property int Permission
			{
				int get() {return Document_getPermission( m_doc );}
			}
			property int Perm
			{
				int get() {return Document_getPerm( m_doc );}
			}
			property Boolean CanSave
			{
				Boolean get() {return Document_canSave( m_doc );}
			}
			property Boolean IsEncrypted
			{
				Boolean get() {return Document_isEncrypted( m_doc );}
			}
			PDFObj ^Advance_GetObj(PDFRef ref)
			{
				PDF_OBJ obj = Document_advGetObj(m_doc, ref.ref);
				if (!obj) return nullptr;
				PDFObj ^ret = ref new PDFObj();
				ret->m_obj = obj;
				return ret;
			}
			PDFRef Advance_GetRef()
			{
				PDF_OBJ_REF ref = Document_advGetRef(m_doc);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
			PDFRef Advance_NewIndirect()
			{
				PDF_OBJ_REF ref = Document_advNewIndirectObj(m_doc);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
			PDFRef Advance_NewIndirectAndCopy(PDFObj ^obj)
			{
				PDF_OBJ_REF ref = Document_advNewIndirectObjWithData(m_doc, obj->m_obj);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
			void Advance_Reload()
			{
				Document_advReload(m_doc);
			}
			PDFRef Advance_NewFlateStream(const Array<BYTE> ^src)
			{
				int len = src->Length;
				const unsigned char *data = src->Data;
				PDF_OBJ_REF ref = Document_advNewFlateStream(m_doc, data, len);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
			PDFRef Advance_NewRawStream(const Array<BYTE> ^src)
			{
				int len = src->Length;
				const unsigned char *data = src->Data;
				PDF_OBJ_REF ref = Document_advNewRawStream(m_doc, data, len);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
		private:
			class PDFStreamInner:public IPDFStream
			{
			public:
				void Open( PDFStream ^stream )
				{
					m_stream = stream;
				}
				virtual bool Writeable() const
				{
					return m_stream->Writeable();
				}
				virtual unsigned long long GetLen() const
				{
					return m_stream->GetLength();
				}
				virtual unsigned long long GetPos() const
				{
					return m_stream->GetPosition();
				}
				virtual bool SetPos( unsigned long long off )
				{
					return m_stream->SetPosition(off);
				}
				virtual unsigned int Read( void *pBuf, unsigned int dwBuf )
				{
					ArrayReference<BYTE> tmp((BYTE*)pBuf, dwBuf);
					return m_stream->Read( tmp );
				}
				virtual unsigned int Write( const void *pBuf, unsigned int dwBuf )
				{
					ArrayReference<BYTE> tmp((BYTE*)pBuf, dwBuf);
					return m_stream->Write( tmp );
				}
				virtual void Close()
				{
					m_stream->Close();
					m_stream = nullptr;
				}
				virtual void Flush()
				{
					m_stream->Flush();
				}
			protected:
				PDFStream ^m_stream;
			};
			friend PDFDocFont;
			friend PDFImportCtx;
			friend PDFOutline;
			friend ref class RDPDFLib::view::PDFView;
			~PDFDoc();
			PDF_DOC m_doc;
			PDFStreamInner *m_inner;
		};
		public ref class PDFDocImage sealed
		{
		public:
		private:
			PDFDocImage()
			{
				m_image = NULL;
			}
			friend PDFDoc;
			friend PDFPage;
			PDF_DOC_IMAGE m_image;
		};
		public ref class PDFDocFont sealed
		{
		public:
			property float Ascent
			{
				float get(){return Document_getFontAscent(m_doc->m_doc, m_font);}
			}
			property float Descent
			{
				float get(){return Document_getFontDescent(m_doc->m_doc, m_font);}
			}
		private:
			PDFDocFont()
			{
				m_font = NULL;
			}
			friend PDFDoc;
			friend PDFPage;
			PDFDoc ^m_doc;
			PDF_DOC_FONT m_font;
		};
		public ref class PDFDocGState sealed
		{
		public:
		private:
			PDFDocGState()
			{
				m_gs = NULL;
			}
			friend PDFDoc;
			friend PDFPage;
			PDF_DOC_GSTATE m_gs;
		};
		public ref class PDFImportCtx sealed
		{
		public:
		private:
			PDFImportCtx()
			{
				m_ctx = NULL;
				m_doc = nullptr;
			}
			~PDFImportCtx()
			{
				Document_importEnd( m_doc->m_doc, m_ctx );
			}
			friend PDFDoc;
			friend PDFPage;
			PDFDoc ^m_doc;
			PDF_IMPORTCTX m_ctx;
		};
		public ref class PDFOutline sealed
		{
		public:
			PDFOutline ^GetNext();
			PDFOutline ^GetChild();
			Boolean AddNext( String ^label, int dest, float y );
			Boolean AddChild( String ^label, int dest, float y );
			Boolean RemoveFromDoc();
			property String^label
			{
				String ^get()
				{
					char label[512];
					Document_getOutlineLabel( m_doc->m_doc, m_outline, label, 511 );
					return cvt_cstr_str( label );
				}
			}
			property int dest
			{
				int get()
				{
					return Document_getOutlineDest( m_doc->m_doc, m_outline );
				}
			}
		private:
			PDFOutline()
			{
				m_doc = nullptr;
				m_outline = NULL;
			}
			friend PDFDoc;
			PDFDoc ^m_doc;
			PDF_OUTLINE m_outline;
		};
		public ref class PDFPageGState sealed
		{
		public:
		private:
			PDFPageGState()
			{
				m_gs = NULL;
			}
			friend PDFPage;
			friend PDFPageContent;
			PDF_PAGE_GSTATE m_gs;
		};
		public ref class PDFPageImage sealed
		{
		public:
		private:
			PDFPageImage()
			{
				m_image = NULL;
			}
			friend PDFPage;
			friend PDFPageContent;
			PDF_PAGE_IMAGE m_image;
		};
		public ref class PDFPageFont sealed
		{
		public:
		private:
			PDFPageFont()
			{
				m_font = NULL;
			}
			friend PDFPage;
			friend PDFPageContent;
			PDF_PAGE_FONT m_font;
		};
		public ref class PDFPageContent sealed
		{
		public:
			PDFPageContent()
			{
				m_content = PageContent_create();
			}
			void GSSave()
			{
				PageContent_gsSave( m_content );
			}
			void GSRestore()
			{
				PageContent_gsRestore( m_content );
			}
			void GSSet( PDFPageGState ^gs )
			{
				PageContent_gsSet( m_content, gs->m_gs );
			}
			void GSSetMatrix( PDFMatrix ^mat )
			{
				PageContent_gsSetMatrix( m_content, mat->m_mat );
			}
			void TextBegin()
			{
				PageContent_textBegin( m_content );
			}
			void TextEnd()
			{
				PageContent_textEnd( m_content );
			}
			void DrawImage( PDFPageImage ^img )
			{
				PageContent_drawImage( m_content, img->m_image );
			}
			void DrawText( String ^text )
			{
				PageContent_drawTextW( m_content, text->Data() );
			}
			void StrokePath( PDFPath ^path )
			{
				PageContent_strokePath( m_content, path->m_path );
			}
			void FillPath( PDFPath ^path, Boolean winding )
			{
				PageContent_fillPath( m_content, path->m_path, winding );
			}
			void ClipPath( PDFPath ^path, Boolean winding )
			{
				PageContent_clipPath( m_content, path->m_path, winding );
			}
			void SetFillColor( int color )
			{
				PageContent_setFillColor(m_content, color);
			}
			void SetStrokeColor( int color )
			{
				PageContent_setStrokeColor(m_content, color);
			}
			void SetStrokeCap( int cap )
			{
				PageContent_setStrokeCap(m_content, cap);
			}
			void SetStrokeJoin( int join )
			{
				PageContent_setStrokeJoin(m_content, join);
			}
			void SetStrokeWidth( float w )
			{
				PageContent_setStrokeWidth(m_content, w);
			}
			void SetStrokeMiter( float miter )
			{
				PageContent_setStrokeMiter(m_content, miter);
			}
			void TextSetCharSpace( float space )
			{
				PageContent_textSetCharSpace( m_content, space );
			}
			void TextSetWordSpace( float space )
			{
				PageContent_textSetWordSpace( m_content, space );
			}
			void TextSetLeading( float leading )
			{
				PageContent_textSetLeading( m_content, leading );
			}
			void TextSetRise( float rise )
			{
				PageContent_textSetRise( m_content, rise );
			}
			void TextSetHScale( int scale )
			{
				PageContent_textSetHScale( m_content, scale );
			}
			void TextNextLine()
			{
				PageContent_textNextLine( m_content );
			}
			void TextMove( float x, float y )
			{
				PageContent_textMove( m_content, x, y );
			}
			void TextSetFont( PDFPageFont ^font, float size )
			{
				PageContent_textSetFont( m_content, font->m_font, size );
			}
			void TextSetRenderMode( int mode )
			{
				PageContent_textSetRenderMode( m_content, mode );
			}
		private:
			friend PDFPage;
			~PDFPageContent()
			{
				PageContent_destroy( m_content );
			}
			PDF_PAGECONTENT m_content;
		};
		public ref class PDFFinder sealed
		{
		public:
			int GetCount()
			{
				return Page_findGetCount( m_finder );
			}
			int GetFirstChar( int index )
			{
				return Page_findGetFirstChar( m_finder, index );
			}
		private:
			PDFFinder()
			{
				m_finder = 0;
			}
			~PDFFinder()
			{
				Page_findClose( m_finder );
			}
			friend PDFPage;
			PDF_FINDER m_finder;
		};
		public ref class PDFPage sealed
		{
		public:
			property PDFRect CropBox
			{
				PDFRect get()
				{
					PDF_RECT rc;
					Page_getCropBox( m_page, &rc );
					return *(PDFRect *)&rc;
				}
			}
			property PDFRect MediaBox
			{
				PDFRect get()
				{
					PDF_RECT rc;
					Page_getMediaBox( m_page, &rc );
					return *(PDFRect *)&rc;
				}
			}
			void RenderPrepare( PDFDIB ^dib )
			{
				Page_renderPrepare( m_page, dib->m_dib );
			}
			void RenderPrepare()
			{
				Page_renderPrepare( m_page, NULL );
			}
			Boolean Render( PDFDIB ^dib, PDFMatrix ^mat, Boolean show_annot, PDF_RENDER_MODE mode )
			{
				return Page_render( m_page, dib->m_dib, mat->m_mat, show_annot, (::PDF_RENDER_MODE)mode );
			}
			Boolean RenderToBmp( PDFBmp ^bmp, PDFMatrix ^mat, Boolean show_annot, PDF_RENDER_MODE mode )
			{
				return Page_renderToBmp( m_page, bmp->m_bmp, mat->m_mat, show_annot, (::PDF_RENDER_MODE)mode );
			}
			void RenderCancel()
			{
				Page_renderCancel( m_page );
			}
			Boolean RenderIsFinished()
			{
				return Page_renderIsFinished( m_page );
			}
			PDFPageFont ^AddResFont( PDFDocFont ^font )
			{
				PDF_PAGE_FONT pf = Page_addResFont( m_page, font->m_font );
				if( pf )
				{
					PDFPageFont ^font = ref new PDFPageFont();
					font->m_font = pf;
					return font;
				}
				else return nullptr;
			}
			PDFPageImage ^AddResImage( PDFDocImage ^image )
			{
				PDF_PAGE_IMAGE pf = Page_addResImage( m_page, image->m_image );
				if( pf )
				{
					PDFPageImage ^font = ref new PDFPageImage();
					font->m_image = pf;
					return font;
				}
				else return nullptr;
			}
			PDFPageGState ^AddResGState( PDFDocGState ^gs )
			{
				PDF_PAGE_GSTATE pf = Page_addResGState( m_page, gs->m_gs );
				if( pf )
				{
					PDFPageGState ^font = ref new PDFPageGState();
					font->m_gs = pf;
					return font;
				}
				else return nullptr;
			}
			Boolean AddContent( PDFPageContent ^content, Boolean flush )
			{
				return Page_addContent( m_page, content->m_content, flush );
			}
			void ObjsStart()
			{
				Page_objsStart( m_page );
			}
			int ObjsGetCharCount()
			{
				return Page_objsGetCharCount(m_page);
			}
			PDFRect ObjsGetCharRect( int index )
			{
				PDFRect rect;
				Page_objsGetCharRect( m_page, index, (PDF_RECT *)&rect );
				return rect;
			}
			int ObjsGetCharIndex( float x, float y )
			{
				return Page_objsGetCharIndex( m_page, x, y );
			}
			int ObjsAlignWord( int index, int dir )
			{
				return Page_objsAlignWord( m_page, index, dir );
			}
			String ^ObjsGetCharFontName( int index )
			{
				return cvt_cstr_str( Page_objsGetCharFontName( m_page, index ) );
			}
			String ^ObjsGetString( int from, int to )
			{
				wchar_t *txt = (wchar_t *)malloc( sizeof( wchar_t ) * (to - from + 3) );
				Page_objsGetStringW( m_page, from, to, txt, to - from + 2 );
				String ^ret = ref new String( txt );
				free( txt );
				return ret;
			}
			PDFFinder ^GetFinder( String ^key, Boolean match_case, Boolean whole_word )
			{
				PDF_FINDER find = Page_findOpenW( m_page, key->Data(), match_case, whole_word );
				if( find )
				{
					PDFFinder ^finder = ref new PDFFinder();
					finder->m_finder = find;
					return finder;
				}
				else return nullptr;
			}
			PDFAnnot ^GetAnnot( int index );
			PDFAnnot ^GetAnnot( float x, float y );
			property int AnnotCount
			{
				int get(){return Page_getAnnotCount(m_page);}
			}
			Boolean AddAnnotMarkup( int ci1, int ci2, unsigned int color, int type )
			{
				return Page_addAnnotMarkup2( m_page, ci1, ci2, color, type );
			}
			Boolean AddAnnotGoto( PDFRect rect, int dest, float y )
			{
				return Page_addAnnotGoto2( m_page, (const PDF_RECT *)&rect, dest, y );
			}
			Boolean AddAnnotURI( PDFRect rect, String ^uri )
			{
				char *tmp = cvt_str_cstr( uri );
				bool ret = Page_addAnnotURI2( m_page, (const PDF_RECT *)&rect, tmp );
				free(tmp);
				return ret;
			}
			Boolean AddAnnotPopup(PDFAnnot ^parent, PDFRect rect, bool open);
			Boolean AddAnnotBitmap( WriteableBitmap ^bmp, bool has_alpha, PDFRect rect )
			{
				return Page_addAnnotBitmap2( m_page, bmp, has_alpha, (const PDF_RECT *)&rect );
			}
			Boolean AddAnnotBitmap( WriteableBitmap ^bmp, PDFMatrix ^mat, bool has_alpha, PDFRect rect )
			{
				return Page_addAnnotBitmap( m_page, mat->m_mat, bmp, has_alpha, (const PDF_RECT *)&rect );
			}
			Boolean AddAnnotInk( PDFInk ^ink )
			{
				return Page_addAnnotInk2( m_page, ink->m_ink );
			}
			Boolean AddAnnotLine( float x1, float y1, float x2, float y2, int style1, int style2, float width, unsigned int color, unsigned int icolor )
			{
				PDF_POINT pt1;
				PDF_POINT pt2;
				pt1.x = x1;
				pt1.y = y1;
				pt2.x = x2;
				pt2.y = y2;
				return Page_addAnnotLine2( m_page, &pt1, &pt2, style1, style2, width, color, icolor );
			}
			Boolean AddAnnotRect( PDFRect rect, float width, unsigned int color, unsigned int icolor )
			{
				return Page_addAnnotRect2( m_page, (const PDF_RECT *)&rect, width, color, icolor );
			}
			Boolean AddAnnotEllipse( PDFRect rect, float width, unsigned int color, unsigned int icolor )
			{
				return Page_addAnnotEllipse2( m_page, (const PDF_RECT *)&rect, width, color, icolor );
			}
			Boolean AddAnnotTextNote( float x, float y )
			{
				return Page_addAnnotText2( m_page, x, y );
			}
			Boolean AddAnnotEditbox( PDFRect rect, int line_clr, float line_w, int fill_clr, float tsize, int text_clr )
			{
				return Page_addAnnotEditbox2( m_page, (const PDF_RECT *)&rect, line_clr, line_w, fill_clr, tsize, text_clr );
			}
			Boolean AddAnnotAttachment( PDFRect rect, String ^path, int icon )
			{
				char *tmp = cvt_str_cstr(path);
				bool ret = Page_addAnnotAttachment( m_page, tmp, icon, (const PDF_RECT *)&rect );
				free( tmp );
				return ret;
			}
			void Close()
			{
				if (!m_ref && m_page)
					Page_close(m_page);
				m_page = NULL;
				m_ref = false;
			}
			PDFRef Advance_GetRef()
			{
				PDF_OBJ_REF ref = Page_advGetRef(m_page);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
			void Advance_Reload()
			{
				Page_advReload(m_page);
			}
		private:
			PDFPage()
			{
				//m_doc = nullptr;
				m_page = NULL;
				m_ref = false;
			}
			~PDFPage()
			{
				Close();
			}
			friend PDFDoc;
			friend PDFAnnot;
			friend ref class RDPDFLib::view::PDFVPage;
			bool m_ref;
			//PDFDoc ^m_doc;
			PDF_PAGE m_page;
		};
		public ref class PDFAnnot sealed
		{
		public:
			property int Type
			{
				int get(){return Page_getAnnotType(m_page->m_page, m_annot);}
			}
			property int FieldType
			{
				int get(){return Page_getAnnotFieldType(m_page->m_page, m_annot);}
			}
			property String ^FieldName
			{
				String ^get()
				{
					wchar_t tmp[512] = {0};
					if( Page_getAnnotFieldNameW(m_page->m_page, m_annot, tmp, 511 ) <= 0 ) return nullptr;
					else return ref new String( tmp );
				}
			}
			property String ^FieldFullName
			{
				String ^get()
				{
					wchar_t tmp[512] = {0};
					if( Page_getAnnotFieldFullNameW(m_page->m_page, m_annot, tmp, 511 ) <= 0 ) return nullptr;
					else return ref new String( tmp );
				}
			}
			property String ^FieldFullName2
			{
				String ^get()
				{
					wchar_t tmp[512] = {0};
					if( Page_getAnnotFieldFullName2W(m_page->m_page, m_annot, tmp, 511 ) <= 0 ) return nullptr;
					else return ref new String( tmp );
				}
			}
			property Boolean Locked
			{
				Boolean get(){return Page_isAnnotLocked(m_page->m_page, m_annot);}
			}
			property Boolean LockedContent
			{
				Boolean get(){return Page_isAnnotLockedContent(m_page->m_page, m_annot);}
			}
			property Boolean Hide
			{
				Boolean get(){return Page_isAnnotHide(m_page->m_page, m_annot);}
				void set(Boolean val){Page_setAnnotHide( m_page->m_page, m_annot, val ); }
			}
			property PDFRect Rect
			{
				PDFRect get(){PDFRect rect; Page_getAnnotRect(m_page->m_page, m_annot, (PDF_RECT *)&rect); return rect;}
				void set( PDFRect rect ){Page_setAnnotRect( m_page->m_page, m_annot, (const PDF_RECT *)&rect);}
			}
			property int FillColor
			{
				int get(){return Page_getAnnotFillColor(m_page->m_page, m_annot);}
				void set( int color ){Page_setAnnotFillColor(m_page->m_page, m_annot, color);}
			}
			property int StrokeColor
			{
				int get(){return Page_getAnnotStrokeColor(m_page->m_page, m_annot);}
				void set( int color ){Page_setAnnotStrokeColor(m_page->m_page, m_annot, color);}
			}
			property float StrokeWidth
			{
				float get(){return Page_getAnnotStrokeWidth(m_page->m_page, m_annot);}
				void set( float val ){Page_setAnnotStrokeWidth(m_page->m_page, m_annot, val);}
			}
			property int Icon
			{
				int get(){return Page_getAnnotIcon(m_page->m_page, m_annot);}
				void set( int icon ){Page_setAnnotIcon(m_page->m_page, m_annot, icon);}
			}
			property int Dest
			{
				int get(){return Page_getAnnotDest(m_page->m_page, m_annot);}
			}
			property bool IsURI
			{
				bool get()
				{
					char uri[512];
					return Page_getAnnotURI( m_page->m_page, m_annot, uri, 511 );
				}
			}
			property String ^URI
			{
				String ^get()
				{
					char uri[512];
					if( !Page_getAnnotURI( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
					else return cvt_cstr_str(uri);
				}
			}
			property int IndexInPage
			{
				int get()
				{
					int cur = 0;
					int cnt = m_page->AnnotCount;
					while( cur < cnt )
					{
						PDF_ANNOT tmp = ::Page_getAnnot( m_page->m_page, cur );
						if( tmp == m_annot ) return cur;
						cur++;
					}
					return -1;
				}
			}
			Boolean MoveToPage( PDFPage ^page, PDFRect rect )
			{
				if( !page || !m_page ) return false;
				return Page_moveAnnot( m_page->m_page, page->m_page, m_annot, (const PDF_RECT *)&rect );
			}
			Boolean RemoveFromPage()
			{
				bool ret = Page_removeAnnot( m_page->m_page, m_annot );
				if( ret )
				{
					m_page = nullptr;
					m_annot = NULL;
				}
				return ret;
			}
			property bool IsMovie
			{
				bool get()
				{
					char uri[512];
					return Page_getAnnotMovie( m_page->m_page, m_annot, uri, 511 );
				}
			}
			String ^GetMovieName()
			{
				char uri[512];
				if( !Page_getAnnotMovie( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
				return cvt_cstr_str(uri);
			}
			Boolean GetMovieData( String ^save_path )
			{
				char *tmp = cvt_str_cstr( save_path );
				bool ret = Page_getAnnotMovieData( m_page->m_page, m_annot, tmp );
				free( tmp );
				return ret;
			}
			property bool Is3D
			{
				bool get()
				{
					char uri[512];
					return Page_getAnnot3D( m_page->m_page, m_annot, uri, 511 );
				}
			}
			String ^Get3DName()
			{
				char uri[512];
				if( !Page_getAnnot3D( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
				return cvt_cstr_str(uri);
			}
			Boolean Get3DData( String ^save_path )
			{
				char *tmp = cvt_str_cstr( save_path );
				bool ret = Page_getAnnot3DData( m_page->m_page, m_annot, tmp );
				free( tmp );
				return ret;
			}
			property bool IsAttachment
			{
				bool get()
				{
					char uri[512];
					return Page_getAnnotAttachment( m_page->m_page, m_annot, uri, 511 );
				}
			}
			String ^GetAttachmentName()
			{
				char uri[512];
				if( !Page_getAnnotAttachment( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
				return cvt_cstr_str(uri);
			}
			Boolean GetAttachmentData( String ^save_path )
			{
				char *tmp = cvt_str_cstr( save_path );
				bool ret = Page_getAnnotAttachmentData( m_page->m_page, m_annot, tmp );
				free( tmp );
				return ret;
			}
			property bool IsSound
			{
				bool get()
				{
					char uri[512];
					return Page_getAnnotSound( m_page->m_page, m_annot, uri, 511 );
				}
			}
			String ^GetSoundName()
			{
				char uri[512];
				if( !Page_getAnnotSound( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
				return cvt_cstr_str(uri);
			}
			Array<int> ^GetSoundData( String ^save_path )
			{
				int paras[6];
				char *tmp = cvt_str_cstr( save_path );
				bool ret = Page_getAnnotSoundData( m_page->m_page, m_annot, paras, tmp );
				free( tmp );
				if( !ret ) return nullptr;
				else
				{
					Array<int> ^tmp = ref new Array<int>(6);
					memcpy( tmp->Data, paras, sizeof(int) * 6 );
					return tmp;
				}
			}
			property bool IsPopup
			{
				bool get()
				{
					wchar_t uri[512];
					return Page_getAnnotPopupSubjectW( m_page->m_page, m_annot, uri, 511 );
				}
			}
			property PDFAnnot ^Popup
			{
				PDFAnnot ^get()
				{
					PDFAnnot ^ret = ref new PDFAnnot();
					ret->m_annot = Page_getAnnotPopup(m_page->m_page, m_annot);
					ret->m_page = m_page;
					return ret;
				}
			}
			property Boolean PopupOpen
			{
				Boolean get()
				{
					return Page_getAnnotPopupOpen(m_page->m_page, m_annot);
				}
				void set(Boolean open)
				{
					Page_setAnnotPopupOpen(m_page->m_page, m_annot, open);
				}
			}
			property String ^PopupSubject
			{
				String ^get()
				{
					wchar_t uri[512];
					if( !Page_getAnnotPopupSubjectW( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
					else return ref new String(uri);
				}
				void set( String ^txt )
				{
					Page_setAnnotPopupSubjectW( m_page->m_page, m_annot, txt->Data() );
				}
			}
			property String ^PopupText
			{
				String ^get()
				{
					wchar_t uri[512];
					if( !Page_getAnnotPopupTextW( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
					else return ref new String(uri);
				}
				void set( String ^txt )
				{
					Page_setAnnotPopupTextW( m_page->m_page, m_annot, txt->Data() );
				}
			}
			property String ^PopupLabel
			{
				String ^get()
				{
					wchar_t uri[512];
					if (!Page_getAnnotPopupLabelW(m_page->m_page, m_annot, uri, 511)) return nullptr;
					else return ref new String(uri);
				}
				void set(String ^txt)
				{
					Page_setAnnotPopupLabelW(m_page->m_page, m_annot, txt->Data());
				}
			}
			property String ^EditTextFormat
			{
				String ^get()
				{
					char txt[512];
					wchar_t wtxt[512];
					if( !Page_getAnnotEditTextFormat( m_page->m_page, m_annot, txt, 511 ) ) return nullptr;
					MultiByteToWideChar( CP_ACP, 0, txt, -1, wtxt, 511 );
					return ref new String(wtxt);
				}
			}
			property String ^EditText
			{
				String ^get()
				{
					wchar_t uri[512];
					if( !Page_getAnnotEditTextW( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
					else return ref new String(uri);
				}
				void set( String ^txt )
				{
					Page_setAnnotEditTextW( m_page->m_page, m_annot, txt->Data() );
				}
			}
			property int EditType
			{
				int get(){return Page_getAnnotEditType(m_page->m_page, m_annot);}
			}
			property PDFRect EditTextRect
			{
				PDFRect get()
				{
					PDFRect rect;
					if( !Page_getAnnotEditTextRect( m_page->m_page, m_annot, (PDF_RECT *)&rect ) )
					{
						rect.left = 0;
						rect.top = 0;
						rect.right = 0;
						rect.bottom = 0;
					}
					return rect;
				}
			}
			property float EditTextSize
			{
				float get(){return Page_getAnnotEditTextSize(m_page->m_page, m_annot);}
			}
			property int ComboItemCount
			{
				int get(){return Page_getAnnotComboItemCount(m_page->m_page, m_annot);}
			}
			property int ComboItemSel
			{
				int get(){return Page_getAnnotComboItemSel(m_page->m_page, m_annot);}
				void set(int item){Page_setAnnotComboItem(m_page->m_page, m_annot, item);}
			}
			String ^GetComboItem( int item )
			{
				wchar_t val[512];
				if( Page_getAnnotComboItemW( m_page->m_page, m_annot, item, val, 511 ) )
					return ref new String(val);
				else return nullptr;
			}
			property int ListItemCount
			{
				int get(){return Page_getAnnotListItemCount(m_page->m_page, m_annot);}
			}
			property Array<int> ^ListItemSel
			{
				Array<int> ^get()
				{
					int sels[128];
					int cnt = Page_getAnnotListSels( m_page->m_page, m_annot, sels, 128 );
					Array<int> ^tmp = ref new Array<int>(cnt);
					memcpy( tmp->Data, sels, cnt * sizeof(int) );
					return tmp;
				}
				void set(const Array<int> ^sel)
				{
					Page_setAnnotListSels(m_page->m_page, m_annot, sel->Data, sel->Length);
				}
			}
			String ^GetListItem( int item )
			{
				wchar_t val[512];
				if( Page_getAnnotListItemW( m_page->m_page, m_annot, item, val, 511 ) )
					return ref new String(val);
				else return nullptr;
			}

			int GetCheckStatus()
			{
				return Page_getAnnotCheckStatus( m_page->m_page, m_annot );
			}
			Boolean SetCheckValue(Boolean check)
			{
				return Page_setAnnotCheckValue( m_page->m_page, m_annot, check );
			}
			Boolean SetRadio()
			{
				return Page_setAnnotRadio( m_page->m_page, m_annot );
			}
			Boolean IsResetButton()
			{
				return Page_getAnnotReset( m_page->m_page, m_annot );
			}
			Boolean DoReset()
			{
				return Page_setAnnotReset( m_page->m_page, m_annot );
			}
			property String ^SubmitTarget
			{
				String ^get()
				{
					wchar_t uri[512];
					if( !Page_getAnnotSubmitTargetW( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
					else return ref new String(uri);
				}
			}
			property String ^SubmitPara
			{
				String ^get()
				{
					wchar_t uri[512];
					if( !Page_getAnnotSubmitParaW( m_page->m_page, m_annot, uri, 511 ) ) return nullptr;
					else return ref new String(uri);
				}
			}
			PDFRef Advance_GetRef()
			{
				PDF_OBJ_REF ref = Page_advGetAnnotRef(m_page->m_page, m_annot);
				PDFRef ret;
				ret.ref = ref;
				return ret;
			}
			void Advance_Reload()
			{
				Page_advReloadAnnot(m_page->m_page, m_annot);
			}
		private:
			PDFAnnot()
			{
				m_page = nullptr;
				m_annot = NULL;
			}
			friend PDFPage;
			PDFPage ^m_page;
			PDF_ANNOT m_annot;
		};
	}
}
