/*
 * libgpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
 *  Copyright (C) 2008 Daniel Mueller <daniel@danm.de>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Libgpgme.Interop
{
    internal partial class libgpgme
    {
		internal static string REQUIRE_GPGME = "1.1.6";
        internal static string GNUPG_DIRECTORY = @"C:\Program Files\GNU\GnuPG";
        internal static string GNUPG_LIBNAME = @"libgpgme-11.dll";
		internal static bool USE_LFS_ON_UNIX = true;
		
		internal static bool use_lfs = false;
        internal static bool IsWindows = false;
		internal static string gpgme_version_str = null;
        internal static GpgmeVersion gpgme_version = null;

        internal const int GPGME_ERR_SOURCE_DEFAULT = (int)gpg_err_source_t.GPG_ERR_SOURCE_USER_1;

        internal static int gpgme_err_make(gpg_err_source_t source, gpg_err_code_t code)
        {
            return libgpgerror.gpg_err_make(source, code);
        }

        internal static int gpgme_error(gpg_err_code_t code)
        {
            return gpgme_err_make((gpg_err_source_t)GPGME_ERR_SOURCE_DEFAULT, code);
        }

        internal static gpg_err_code_t gpgme_err_code(int err)
        {
            return libgpgerror.gpg_err_code(err);
        }

        internal static gpg_err_source_t gpgme_err_source(int err)
        {
            return libgpgerror.gpg_err_source(err);
        }

        /* Windows: add GNUPG directory as library path */
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetDllDirectory(string lpPathName);


        /* Check that the library fulfills the version requirement.  Note:
           This is here only for the case where a user takes a pointer from
           the old version of this function.  The new version and macro for
           run-time checks are below.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_check_version(
            [In] IntPtr req_version); // const char*

        /* Check that the library fulfills the version requirement and check
           for struct layout mismatch involving bitfields.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_check_version_internal(
            [In] IntPtr req_version, // const char *
            [In] IntPtr offset_sig_validity); // size_t

        /* Return a pointer to a string containing a description of the error
           code in the error value ERR.  This function is not thread safe.  
           
           const char *gpgme_strerror (gpgme_error_t err) */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static IntPtr gpgme_strerror(
            [In] int err);


        /* Return the error string for ERR in the user-supplied buffer BUF of
           size BUFLEN.  This function is, in contrast to gpg_strerror,
           thread-safe if a thread-safe strerror_r() function is provided by
           the system.  If the function succeeds, 0 is returned and BUF
           contains the string describing the error.  If the buffer was not
           large enough, ERANGE is returned and BUF contains as much of the
           beginning of the error string as fits into the buffer. 
         
           int gpgme_strerror_r(gpg_error_t err, char* buf, size_t buflen); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_strerror_r(
            [In]  int err,
            [Out] out IntPtr buf,
            [In]  UIntPtr buflen);

        /* Return a pointer to a string containing a description of the error
           source in the error value ERR.  
           
           const char *gpgme_strsource (gpgme_error_t err); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static IntPtr gpgme_strsource(
            [In] int err);


        /* Retrieve the error code for the system error ERR.  This returns
           GPG_ERR_UNKNOWN_ERRNO if the system error is not mapped (report
           this).  
           
           gpgme_err_code_t gpgme_err_code_from_errno(int err); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static gpg_err_code_t gpgme_err_code_from_errno(
            [In] int err);

        /* Retrieve the system error for the error code CODE.  This returns 0
           if CODE is not a system error code.  
           
           int gpgme_err_code_to_errno(gpgme_err_code_t code); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_err_code_to_errno(
            [In] gpg_err_code_t code);

        /* Return an error value with the error source SOURCE and the system
           error ERR.  
           
           gpgme_error_t gpgme_err_make_from_errno(gpgme_err_source_t source, int err); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_err_make_from_errno(
            [In] gpg_err_source_t source, 
            [In] int err);

        /* Return an error value with the system error ERR.  
        
           gpgme_err_code_t gpgme_error_from_errno(int err); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static gpg_err_code_t gpgme_error_from_errno(
            [In] int err);

        /* Get the string describing protocol PROTO, or NULL if invalid.  
        const char *gpgme_get_protocol_name (gpgme_protocol_t proto); */

        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static IntPtr gpgme_get_protocol_name(
            [In] gpgme_protocol_t proto);


        /* Verify that the engine implementing PROTO is installed and
           available.  
        gpgme_error_t gpgme_engine_check_version(gpgme_protocol_t proto); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_engine_check_version(
            [In] gpgme_protocol_t proto);

        /* Get the information about the configured and installed engines.  A
           pointer to the first engine in the statically allocated linked list
           is returned in *INFO.  If an error occurs, it is returned.  The
           returned data is valid until the next gpgme_set_engine_info.  
        gpgme_error_t gpgme_get_engine_info(gpgme_engine_info_t* engine_info); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_get_engine_info(
            [Out] out IntPtr engine_info);

        /* Return a statically allocated string with the name of the public
           key algorithm ALGO, or NULL if that name is not known.  
        const char *gpgme_pubkey_algo_name (gpgme_pubkey_algo_t algo); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static IntPtr gpgme_pubkey_algo_name(
            [In] gpgme_pubkey_algo_t algo);

        /* Return a statically allocated string with the name of the hash
           algorithm ALGO, or NULL if that name is not known.  
         const char *gpgme_hash_algo_name (gpgme_hash_algo_t algo); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static IntPtr gpgme_hash_algo_name(
            [In] gpgme_hash_algo_t algo);

        /* Create a new context and return it in CTX.  
        gpgme_error_t gpgme_new(gpgme_ctx_t* ctx); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_new(
            [Out] out IntPtr ctx);

        /* Release the context CTX.  
        void gpgme_release(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static void gpgme_release(
            [In] IntPtr ctx);

        /* Set the protocol to be used by CTX to PROTO.  
        gpgme_error_t gpgme_set_protocol(gpgme_ctx_t ctx, gpgme_protocol_t proto); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_set_protocol(
            [In] IntPtr ctx, 
            [In] gpgme_protocol_t proto);

        /* Get the protocol used with CTX 
        gpgme_protocol_t gpgme_get_protocol(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static gpgme_protocol_t gpgme_get_protocol(
            [In] IntPtr ctx);

        /* Get the information about the configured engines.  A pointer to the
           first engine in the statically allocated linked list is returned.
           The returned data is valid until the next gpgme_ctx_set_engine_info.  
        gpgme_engine_info_t gpgme_ctx_get_engine_info(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static IntPtr gpgme_ctx_get_engine_info(
            [In] IntPtr ctx);

        /* Set the engine info for the context CTX, protocol PROTO, to the
           file name FILE_NAME and the home directory HOME_DIR.  
         gpgme_error_t gpgme_ctx_set_engine_info (gpgme_ctx_t ctx,
					 gpgme_protocol_t proto,
					 const char *file_name,
					 const char *home_dir); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_ctx_set_engine_info(
            [In] IntPtr ctx,
            [In] gpgme_protocol_t proto,
		    [In] IntPtr file_name,
			[In] IntPtr home_dir);


        /* If YES is non-zero, enable armor mode in CTX, disable it otherwise.  
        void gpgme_set_armor(gpgme_ctx_t ctx, int yes) */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static void gpgme_set_armor(
            [In] IntPtr ctx,
            [In] int yes);

        /* Return non-zero if armor mode is set in CTX.  
        int gpgme_get_armor(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_get_armor(
            [In] IntPtr ctx);


        /* If YES is non-zero, enable text mode in CTX, disable it otherwise.  
        void gpgme_set_textmode(gpgme_ctx_t ctx, int yes); */
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static void gpgme_set_textmode(
            [In] IntPtr ctx,
            [In] int yes);

        /* Return non-zero if text mode is set in CTX.  
        int gpgme_get_textmode(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_get_textmode(
            [In] IntPtr ctx);

        /* Include up to NR_OF_CERTS certificates in an S/MIME message.  
        void gpgme_set_include_certs(gpgme_ctx_t ctx, int nr_of_certs); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_set_include_certs(
            [In] IntPtr ctx,
            [In] int nr_of_certs);

        /* Return the number of certs to include in an S/MIME message.  
        int gpgme_get_include_certs(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_get_include_certs(
            [In] IntPtr ctx);

        /* Set keylist mode in CTX to MODE.  
        gpgme_error_t gpgme_set_keylist_mode(gpgme_ctx_t ctx,
                              gpgme_keylist_mode_t mode); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_set_keylist_mode(
            [In] IntPtr ctx,
            gpgme_keylist_mode_t mode);

        /* Get keylist mode in CTX.  
        gpgme_keylist_mode_t gpgme_get_keylist_mode(gpgme_ctx_t ctx); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static gpgme_keylist_mode_t gpgme_get_keylist_mode(
            [In] IntPtr ctx);

        /* Set the passphrase callback function in CTX to CB.  HOOK_VALUE is
           passed as first argument to the passphrase callback function.  
        void gpgme_set_passphrase_cb(gpgme_ctx_t ctx,
                                     gpgme_passphrase_cb_t cb, void* hook_value); */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_set_passphrase_cb(
            [In] IntPtr ctx,
            [In] gpgme_passphrase_cb_t cb,
            [In] IntPtr hook_value);

		/* Start a keylist operation within CTX, searching for keys which
           match PATTERN.  If SECRET_ONLY is true, only secret keys are
           returned.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_keylist_start(
		    [In] IntPtr ctx, 
		    [In] IntPtr pattern,
            [In] int secret_only);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_keylist_ext_start(
		    [In] IntPtr ctx,
            [In] IntPtr[] pattern,
            [In] int secret_only, 
		    [In] int reserved);
		
		/* Return the next key from the keylist in R_KEY.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_keylist_next(
		    [In] IntPtr ctx, 
		    [Out] out IntPtr r_key);

		/* Terminate a pending keylist operation within CTX.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_keylist_end(
		    [In] IntPtr ctx);
		
		/* Release a reference to KEY.  If this was the last one the key is
           destroyed.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_key_unref(
		    [In] IntPtr key);
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_key_release(
		    [In] IntPtr key);

        /* Get the key with the fingerprint FPR from the crypto backend.  If
          SECRET is true, get the secret key.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_get_key(
            [In] IntPtr ctx, 
            [In] IntPtr fpr,
			[Out] out IntPtr r_key, 
            [In] int secret);

        /* Generate a new keypair and add it to the keyring.  PUBKEY and
           SECKEY should be null for now.  PARMS specifies what keys should be
           generated.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_genkey_start(
            [In] IntPtr ctx, 
            [In] IntPtr parms,  // const char*
		    [In] IntPtr pubkey, // gpgme_data_t
            [In] IntPtr seckey  //gpgme_data_t
        );

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_genkey(
            [In] IntPtr ctx,
            [In] IntPtr parms,  // const char*
            [In] IntPtr pubkey, // gpgme_data_t
            [In] IntPtr seckey  //gpgme_data_t
        );

        /* Retrieve a pointer to the result of the genkey operation.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_op_genkey_result(
            [In] IntPtr ctx);


        /* Export the keys found by PATTERN into KEYDATA.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_export(
            [In] IntPtr ctx, 
            [In] IntPtr pattern, // const char*
			[In] uint reserved, 
            [In] IntPtr keydata);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_export_ext(
            [In] IntPtr ctx, 
            [In] IntPtr[] pattern, //const char *pattern[]
		    [In] uint reserved,
		    [In] IntPtr keydata);

        /* Create a new data buffer and return it in R_DH.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new(
            [Out] out IntPtr r_dh);

        /* Destroy the data buffer DH.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_data_release(
            [In] IntPtr dh);

        /* Create a new data buffer filled with SIZE bytes starting from
           BUFFER.  If COPY is zero, copying is delayed until necessary, and
           the data is taken from the original location when needed.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_mem(
            [Out] out IntPtr r_dh,
		    [In] IntPtr buffer,
            [In] UIntPtr size, //size_t
			[In] int copy);

        /* Create a new data buffer filled with LENGTH bytes starting from
           OFFSET within the file FNAME or stream FP (exactly one must be
           non-zero).  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_filepart(
            [Out] out IntPtr r_dh,
            [In] IntPtr fname,  // const char*
            [In] IntPtr fp,     //FILE *
		    [In] IntPtr offset,	// off_t
            [In] UIntPtr length);//size_t
		/* Create a new data buffer filled with LENGTH bytes starting from
           OFFSET within the file FNAME or stream FP (exactly one must be
           non-zero).  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_filepart(
            [Out] out IntPtr r_dh,
            [In] IntPtr fname,  // const char*
            [In] IntPtr fp,     //FILE *
		    [In] long offset,	// off_t
            [In] UIntPtr length);//size_t

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_fd(
            [Out] out IntPtr dh, 
            int fd);

        /*
        [DllImport("libgpgme-11.dll", CharSet=CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_cbs(
            [Out] out IntPtr dh,
            [In] _gpgme_data_cbs cbs, //gpgme_data_cbs_t
            [In] IntPtr handle);

        */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_cbs(
            [Out] out IntPtr dh,
            [In][MarshalAs(UnmanagedType.FunctionPtr)] _gpgme_data_cbs cbs, //gpgme_data_cbs_t 
            [In] IntPtr handle);
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
		internal extern static int gpgme_data_new_from_cbs(
            [Out] out IntPtr dh,
            [In][MarshalAs(UnmanagedType.FunctionPtr)] _gpgme_data_cbs_lfs cbs, //gpgme_data_cbs_t_lfs
            [In] IntPtr handle);
		
		[DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_new_from_cbs(
            [Out] out IntPtr dh,
            [In] IntPtr cbs, //gpgme_data_cbs_t
            [In] IntPtr handle);


        /* Read up to SIZE bytes into buffer BUFFER from the data object with
           the handle DH.  Return the number of characters read, 0 on EOF and
           -1 on error.  If an error occurs, errno is set.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_data_read(
            [In] IntPtr dh, 
            [In] byte[] buffer, 
            [In] UIntPtr size); //size_t

        /* Read up to SIZE bytes into buffer BUFFER from the data object with
           the handle DH.  Return the number of characters read, 0 on EOF and
           -1 on error.  If an error occurs, errno is set.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_data_read(
            [In] IntPtr dh,
            [In] IntPtr buffer,
            [In] UIntPtr size); //size_t

        /* Set the current position from where the next read or write starts
           in the data object with the handle DH to OFFSET, relativ to
           WHENCE.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_data_seek(
            [In] IntPtr dh, 
            [In] IntPtr offset, // off_t
            [In] int whence);
	    /* Set the current position from where the next read or write starts
           in the data object with the handle DH to OFFSET, relativ to
           WHENCE.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static long gpgme_data_seek(
            [In] IntPtr dh, 
            [In] long offset, // off_t
            [In] int whence);
	
        /* Write up to SIZE bytes from buffer BUFFER to the data object with
           the handle DH.  Return the number of characters written, or -1 on
           error.  If an error occurs, errno is set.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_data_write(
            [In] IntPtr dh, 
            [In] byte[] buffer, 
            [In] UIntPtr size);

        /* Write up to SIZE bytes from buffer BUFFER to the data object with
           the handle DH.  Return the number of characters written, or -1 on
           error.  If an error occurs, errno is set.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_data_write(
            [In] IntPtr dh,
            [In] IntPtr buffer,
            [In] UIntPtr size);


        /* Get the file name associated with the data object with handle DH, or
           NULL if there is none.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_data_get_file_name(
            [In] IntPtr dh);

        /* Set the file name associated with the data object with handle DH to
           FILE_NAME.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_set_file_name(
            [In] IntPtr dh,
			[In] IntPtr	file_name);

        /* Return the encoding attribute of the data buffer DH */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static gpgme_data_encoding_t gpgme_data_get_encoding(
            [In] IntPtr dh);

        /* Set the encoding attribute of data buffer DH to ENC */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_data_set_encoding(
            [In] IntPtr dh,
            [In] gpgme_data_encoding_t enc);


        /* Retrieve a pointer to the result of the import operation.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_op_import_result(
            [In] IntPtr ctx); // returns gpgme_import_result_t

        /* Import the key in KEYDATA into the keyring.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_import_start(
            [In] IntPtr ctx, 
            [In] IntPtr keydata);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_import(
            [In] IntPtr ctx,
            [In] IntPtr keydata);


        /* Delete KEY from the keyring.  If ALLOW_SECRET is non-zero, secret
           keys are also deleted.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_delete_start(
            [In] IntPtr ctx, 
            [In] IntPtr key,
	        [In] int allow_secret);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_delete(
            [In] IntPtr ctx, 
            [In] IntPtr key,
			[In] int allow_secret);

        /* Start a trustlist operation within CTX, searching for trust items
           which match PATTERN.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_trustlist_start(
            [In] IntPtr ctx,
			[In] IntPtr pattern, // const char * 
            [In] int max_level);

        /* Return the next trust item from the trustlist in R_ITEM.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_trustlist_next(
            [In] IntPtr ctx,
            [Out] out IntPtr r_item //gpgme_trust_item_t *
            ); 

        /* Terminate a pending trustlist operation within CTX.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_trustlist_end(
            [In] IntPtr ctx);

        /* Acquire a reference to ITEM.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_trust_item_ref(
            [In] IntPtr item //gpgme_trust_item_t
            ); 

        /* Release a reference to ITEM.  If this was the last one the trust
           item is destroyed.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_trust_item_unref(
            [In] IntPtr item //gpgme_trust_item_t
        ); 

        /* Encrypt plaintext PLAIN within CTX for the recipients RECP and
           store the resulting ciphertext in CIPHER.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_encrypt_start(
            [In] IntPtr ctx, 
            [In] IntPtr[] recp, // gpgme_key_t []
            [In] gpgme_encrypt_flags_t flags,
            [In] IntPtr plain, // gpgme_data_t
            [In] IntPtr cipher //gpgme_data_t
         );

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_encrypt(
            [In] IntPtr ctx,
            [In] IntPtr[] recp, // gpgme_key_t []
		    [In] gpgme_encrypt_flags_t flags,
            [In] IntPtr plain,  //gpgme_data_t
            [In] IntPtr cipher //gpgme_data_t
         );

        /* Retrieve a pointer to the result of the encrypt operation.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_op_encrypt_result(
            [In] IntPtr ctx); // returns gpgme_encrypt_result_t


        /* Encrypt plaintext PLAIN within CTX for the recipients RECP and
           store the resulting ciphertext in CIPHER.  Also sign the ciphertext
           with the signers in CTX.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_encrypt_sign_start(
            [In] IntPtr ctx,
			[In] IntPtr[] recp, //gpgme_key_t[]
			[In] gpgme_encrypt_flags_t flags,
            [In] IntPtr plain,
			[In] IntPtr cipher);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_encrypt_sign(
            [In] IntPtr ctx, 
            [In] IntPtr[] recp, //gpgme_key_t[]
            [In] gpgme_encrypt_flags_t flags,
			[In] IntPtr plain, 
            [In] IntPtr cipher);

        /* Delete all signers from CTX.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_signers_clear(
            [In] IntPtr ctx);

        /* Add KEY to list of signers in CTX.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_signers_add(
            [In] IntPtr ctx,
            [In] IntPtr key // const gpgme_key_t
        ); 

        /* Return the SEQth signer's key in CTX.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_signers_enum(
            [In] IntPtr ctx, 
            [In] int seq); // returns gpgme_key_t

        
        /* Retrieve a pointer to the result of the signing operation.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_op_sign_result(
            [In] IntPtr ctx); // returns gpgme_sign_result_t 

        /* Sign the plaintext PLAIN and store the signature in SIG.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_sign_start(
            [In] IntPtr ctx,
            [In] IntPtr plain, 
            [In] IntPtr sig,
            [In] gpgme_sig_mode_t mode);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_sign(
            [In] IntPtr ctx,
            [In] IntPtr plain, 
            [In] IntPtr sig,
            [In] gpgme_sig_mode_t mode);

        /* Clear all notation data from the context.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static void gpgme_sig_notation_clear(
            [In] IntPtr ctx);

        /* Add the human-readable notation data with name NAME and value VALUE
           to the context CTX, using the flags FLAGS.  If NAME is NULL, then
           VALUE should be a policy URL.  The flag
           GPGME_SIG_NOTATION_HUMAN_READABLE is forced to be true for notation
           data, and false for policy URLs.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_sig_notation_add(
            [In] IntPtr ctx, 
            [In] IntPtr name, // const char *
            [In] IntPtr value, // const char *
		    [In] gpgme_sig_notation_flags_t flags);

        /* Get the sig notations for this context.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_sig_notation_get(
            [In] IntPtr ctx); // returns gpgme_sig_notation_t


        /* Retrieve a pointer to the result of the decrypt operation.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_op_decrypt_result(
            [In] IntPtr ctx); // returns gpgme_decrypt_result_t

        /* Decrypt ciphertext CIPHER within CTX and store the resulting
           plaintext in PLAIN.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_decrypt_start(
            [In] IntPtr ctx, 
            [In] IntPtr cipher,
            [In] IntPtr plain);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_decrypt(
            [In] IntPtr ctx,
            [In] IntPtr cipher, 
            [In] IntPtr plain);

        /* Decrypt ciphertext CIPHER and make a signature verification within
           CTX and store the resulting plaintext in PLAIN.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_decrypt_verify_start(
            [In] IntPtr ctx,
            [In] IntPtr cipher,
            [In] IntPtr plain);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_decrypt_verify(
            [In] IntPtr ctx, 
            [In] IntPtr cipher,
            [In] IntPtr plain);


        /* Retrieve a pointer to the result of the verify operation.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static IntPtr gpgme_op_verify_result(
            [In] IntPtr ctx); // returns gpgme_verify_result_t

        /* Verify within CTX that SIG is a valid signature for TEXT.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_verify_start(
            [In] IntPtr ctx, 
            [In] IntPtr sig,
            [In] IntPtr signed_text,
            [In] IntPtr plaintext);

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_verify(
            [In] IntPtr ctx, 
            [In] IntPtr sig,
            [In] IntPtr signed_text,
            [In] IntPtr plaintext);

        /* Edit the key KEY.  Send status and command requests to FNC and
           output of edit commands to OUT.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_edit_start(
            [In] IntPtr ctx, 
            [In] IntPtr key, // gpgme_key_t
            [In] gpgme_edit_cb_t fnc, // gpgme_edit_cb_t 
            [In] IntPtr fnc_value, // void *
		    [In] IntPtr outdata // gpgme_data_t
            ); 

        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_edit(
            [In] IntPtr ctx, 
            [In] IntPtr key, // gpgme_key_t
		    [In] gpgme_edit_cb_t fnc, // gpgme_edit_cb_t
            [In] IntPtr fnc_value, // void *
		    [In] IntPtr outdata // gpgme_data_t
            ); 

        /* Edit the card for the key KEY.  Send status and command requests to
           FNC and output of edit commands to OUT.  */
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_card_edit_start(
            [In] IntPtr ctx, 
            [In] IntPtr key, //gpgme_key_t
			[In] gpgme_edit_cb_t fnc, //gpgme_edit_cb_t 
            [In] IntPtr fnc_value, //void *
			[In] IntPtr outdata //gpgme_data_t
            );
        
        [DllImport("libgpgme-11.dll", CharSet = CharSet.Ansi)]
        internal extern static int gpgme_op_card_edit(
            [In] IntPtr ctx,
            [In] IntPtr key, //gpgme_key_t
            [In] gpgme_edit_cb_t fnc, //gpgme_edit_cb_t 
            [In] IntPtr fnc_value, //void *
            [In] IntPtr outdata //gpgme_data_t
            );

        static libgpgme()
        {
            // On Windows systems we have to add the GnuPG directory to DLL search path
            Win32SetLibdir();
			
			// Version check required (could fail on Windows systems)
			try {
				InitLibgpgme();
			} catch {};
        }

        internal static bool Win32SetLibdir()
        {

            if (Environment.OSVersion.Platform.ToString().Contains("Win32") ||
                Environment.OSVersion.Platform.ToString().Contains("Win64"))
            {
                IsWindows = true;
                string gnupgpath = null;
                try
                {
                    gnupgpath = (string)Microsoft.Win32.Registry.GetValue(
                    "HKEY_LOCAL_MACHINE\\SOFTWARE\\GNU\\GnuPG",
                    "Install Directory",
                    null);
                }
                catch { }
                if (gnupgpath != null && !(gnupgpath.Equals(string.Empty)))
                {
                    return libgpgme.SetDllDirectory(gnupgpath);
                }
                else
                {
                    return libgpgme.SetDllDirectory(GNUPG_DIRECTORY);
                }
            }

            return true; // always "true" for UNIX
        }
		
		internal static void InitLibgpgme()
        {
            if (Environment.OSVersion.Platform.ToString().Contains("Win32") ||
                Environment.OSVersion.Platform.ToString().Contains("Win64"))
            {
                IsWindows = true;
            }
            else
			{
                IsWindows = false;
				if (USE_LFS_ON_UNIX)
					// See GPGME manual: 2.3 Largefile Support (LFS)
					use_lfs = true;
			}

#if REQUIRE_GPGME_VERSION
            gpgme_version = new GpgmeVersion(CheckVersion(REQUIRE_GPGME));
#else
            gpgme_version = new GpgmeVersion(CheckVersion(null));
#endif
        }
		
		internal static string CheckVersion(string ReqVersion)
        {
            // we are doing this check only once

            if (gpgme_version_str == null) {
                IntPtr verPtr = IntPtr.Zero;
                IntPtr reqverPtr = IntPtr.Zero;

                if (ReqVersion != null && ReqVersion.Length != 0)
                {
                    // minimun required version
                    reqverPtr = Gpgme.StringToCoTaskMemUTF8(ReqVersion);
                }
                
                // retrieve GPGME's version
                verPtr = libgpgme.gpgme_check_version(reqverPtr);

                if (!reqverPtr.Equals(IntPtr.Zero))
                {
                    Marshal.FreeCoTaskMem(reqverPtr);
                    reqverPtr = IntPtr.Zero;
                }

                if (!verPtr.Equals(IntPtr.Zero))
                {
                    gpgme_version_str = Gpgme.PtrToStringUTF8(verPtr);
                }
                else
                {
                    throw new GeneralErrorException("Could not retrieve a valid GPGME version.\nGot: "
                        + gpgme_version_str
                        + " Minimum required: " + ReqVersion
                    );
                }
            }
            return gpgme_version_str;
        }
    }
}
