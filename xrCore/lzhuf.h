#ifndef _LZ_H_
#define _LZ_H_

#pragma todo("tsmp: в топку его и все места где он вызывается")

extern XRCORE_API unsigned	_writeLZ		(int hf, void* d, unsigned size);
extern XRCORE_API unsigned	_readLZ			(int hf, void* &d, unsigned size);

extern XRCORE_API void		_compressLZ		(u8** dest, unsigned* dest_sz, void* src, unsigned src_sz);
extern XRCORE_API void		_decompressLZ	(u8** dest, unsigned* dest_sz, void* src, unsigned src_sz);

#endif
