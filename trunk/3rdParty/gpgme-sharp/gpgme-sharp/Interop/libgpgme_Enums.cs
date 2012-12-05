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
    /* The possible encoding mode of gpgme_data_t objects.  */
    internal enum gpgme_data_encoding_t : int
    {
        GPGME_DATA_ENCODING_NONE = 0,	/* Not specified.  */
        GPGME_DATA_ENCODING_BINARY = 1,
        GPGME_DATA_ENCODING_BASE64 = 2,
        GPGME_DATA_ENCODING_ARMOR = 3	/* Either PEM or OpenPGP Armor.  */
    }

    /* Public key algorithms from libgcrypt.  */
    internal enum gpgme_pubkey_algo_t : int
    {
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt | AlgorithmCapability.CanSign)]
        GPGME_PK_RSA = 1,

        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt)]
        GPGME_PK_RSA_E = 2,
        
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign)]
        GPGME_PK_RSA_S = 3,
        
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt)]
        GPGME_PK_ELG_E = 16,
        
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign)]
        GPGME_PK_DSA = 17,
        
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt | AlgorithmCapability.CanSign)]
        GPGME_PK_ELG = 20
    }

    /* Hash algorithms from libgcrypt.  */
    internal enum gpgme_hash_algo_t : int
    {
        GPGME_MD_NONE = 0,
        GPGME_MD_MD5 = 1,
        GPGME_MD_SHA1 = 2,
        GPGME_MD_RMD160 = 3,
        GPGME_MD_MD2 = 5,
        GPGME_MD_TIGER = 6,   /* TIGER/192. */
        GPGME_MD_HAVAL = 7,   /* HAVAL, 5 pass, 160 bit. */
        GPGME_MD_SHA256 = 8,
        GPGME_MD_SHA384 = 9,
        GPGME_MD_SHA512 = 10,
        GPGME_MD_SHA224 = 11,
        GPGME_MD_MD4 = 301,
        GPGME_MD_CRC32 = 302,
        GPGME_MD_CRC32_RFC1510 = 303,
        GPGME_MD_CRC24_RFC2440 = 304
    }

    internal enum gnupg_cipher_algo_t : int
    {
        CIPHER_ALGO_NONE = 0,
        CIPHER_ALGO_IDEA = 1,
        CIPHER_ALGO_3DES = 2,
        CIPHER_ALGO_CAST5 = 3,
        CIPHER_ALGO_BLOWFISH = 4,  /* blowfish 128 bit key */
        /* 5 & 6 are reserved */
        CIPHER_ALGO_AES = 7,
        CIPHER_ALGO_AES192 = 8,
        CIPHER_ALGO_AES256 = 9,
        CIPHER_ALGO_TWOFISH = 10,  /* twofish 256 bit */
        CIPHER_ALGO_CAMELLIA128 = 11,
        CIPHER_ALGO_CAMELLIA256 = 12,
        CIPHER_ALGO_DUMMY = 110  /* no encryption at all */
    }

    internal enum gnupg_compress_algo_t : int
    {
        COMPRESS_ALGO_NONE = 0,
        COMPRESS_ALGO_ZIP = 1,
        COMPRESS_ALGO_ZLIB = 2,
        COMPRESS_ALGO_BZIP2 = 3
    }

    /* The possible signature stati.  Deprecated, use error value in sig
       status.  */
    internal enum _gpgme_sig_stat_t : int
    {
        GPGME_SIG_STAT_NONE = 0,
        GPGME_SIG_STAT_GOOD = 1,
        GPGME_SIG_STAT_BAD = 2,
        GPGME_SIG_STAT_NOKEY = 3,
        GPGME_SIG_STAT_NOSIG = 4,
        GPGME_SIG_STAT_ERROR = 5,
        GPGME_SIG_STAT_DIFF = 6,
        GPGME_SIG_STAT_GOOD_EXP = 7,
        GPGME_SIG_STAT_GOOD_EXPKEY = 8
    }

    /* The available signature modes.  */
    internal enum gpgme_sig_mode_t : int
    {
        GPGME_SIG_MODE_NORMAL = 0,
        GPGME_SIG_MODE_DETACH = 1,
        GPGME_SIG_MODE_CLEAR = 2
    }

    /* The available key and signature attributes.  Deprecated, use the
       individual result structures instead.  */
    internal enum _gpgme_attr_t : int
    {
        GPGME_ATTR_KEYID = 1,
        GPGME_ATTR_FPR = 2,
        GPGME_ATTR_ALGO = 3,
        GPGME_ATTR_LEN = 4,
        GPGME_ATTR_CREATED = 5,
        GPGME_ATTR_EXPIRE = 6,
        GPGME_ATTR_OTRUST = 7,
        GPGME_ATTR_USERID = 8,
        GPGME_ATTR_NAME = 9,
        GPGME_ATTR_EMAIL = 10,
        GPGME_ATTR_COMMENT = 11,
        GPGME_ATTR_VALIDITY = 12,
        GPGME_ATTR_LEVEL = 13,
        GPGME_ATTR_TYPE = 14,
        GPGME_ATTR_IS_SECRET = 15,
        GPGME_ATTR_KEY_REVOKED = 16,
        GPGME_ATTR_KEY_INVALID = 17,
        GPGME_ATTR_UID_REVOKED = 18,
        GPGME_ATTR_UID_INVALID = 19,
        GPGME_ATTR_KEY_CAPS = 20,
        GPGME_ATTR_CAN_ENCRYPT = 21,
        GPGME_ATTR_CAN_SIGN = 22,
        GPGME_ATTR_CAN_CERTIFY = 23,
        GPGME_ATTR_KEY_EXPIRED = 24,
        GPGME_ATTR_KEY_DISABLED = 25,
        GPGME_ATTR_SERIAL = 26,
        GPGME_ATTR_ISSUER = 27,
        GPGME_ATTR_CHAINID = 28,
        GPGME_ATTR_SIG_STATUS = 29,
        GPGME_ATTR_ERRTOK = 30,
        GPGME_ATTR_SIG_SUMMARY = 31,
        GPGME_ATTR_SIG_CLASS = 32
    }

    /* The available validities for a trust item or key.  */
    internal enum gpgme_validity_t : int
    {
        GPGME_VALIDITY_UNKNOWN = 0,
        GPGME_VALIDITY_UNDEFINED = 1,
        GPGME_VALIDITY_NEVER = 2,
        GPGME_VALIDITY_MARGINAL = 3,
        GPGME_VALIDITY_FULL = 4,
        GPGME_VALIDITY_ULTIMATE = 5
    }

    /* The available protocols.  */
    internal enum gpgme_protocol_t : int
    {
        GPGME_PROTOCOL_OpenPGP = 0,  /* The default mode.  */
        GPGME_PROTOCOL_CMS = 1,
        GPGME_PROTOCOL_GPGCONF = 2,  /* Special code for gpgconf.  */
        GPGME_PROTOCOL_UNKNOWN = 255
    }

    /* The available keylist mode flags.  */
    [Flags]
    internal enum gpgme_keylist_mode_t : uint
    {
        GPGME_KEYLIST_MODE_LOCAL = 1,
        GPGME_KEYLIST_MODE_EXTERN = 2,
        GPGME_KEYLIST_MODE_SIGS = 4,
        GPGME_KEYLIST_MODE_SIG_NOTATIONS = 8,
        GPGME_KEYLIST_MODE_VALIDATE = 256,
    }

    /* Flags for the audit log functions.  */
    [Flags]
    internal enum gpgme_auditlog : int
    {
        GPGME_AUDITLOG_HTML = 1,
        GPGME_AUDITLOG_WITH_HELP = 128
    }

    /* Signature notations.  */
    /* The available signature notation flags.  */
    [Flags]
    internal enum gpgme_sig_notation_flags_t : int
    {
        GPGME_SIG_NOTATION_HUMAN_READABLE = 1,
        GPGME_SIG_NOTATION_CRITICAL = 2
    }

    /* The possible stati for the edit operation.  */
    internal enum gpgme_status_code_t : int
    {
        GPGME_STATUS_EOF,
        /* mkstatus processing starts here */
        GPGME_STATUS_ENTER,
        GPGME_STATUS_LEAVE,
        GPGME_STATUS_ABORT,

        GPGME_STATUS_GOODSIG,
        GPGME_STATUS_BADSIG,
        GPGME_STATUS_ERRSIG,

        GPGME_STATUS_BADARMOR,

        GPGME_STATUS_RSA_OR_IDEA,
        GPGME_STATUS_KEYEXPIRED,
        GPGME_STATUS_KEYREVOKED,

        GPGME_STATUS_TRUST_UNDEFINED,
        GPGME_STATUS_TRUST_NEVER,
        GPGME_STATUS_TRUST_MARGINAL,
        GPGME_STATUS_TRUST_FULLY,
        GPGME_STATUS_TRUST_ULTIMATE,

        GPGME_STATUS_SHM_INFO,
        GPGME_STATUS_SHM_GET,
        GPGME_STATUS_SHM_GET_BOOL,
        GPGME_STATUS_SHM_GET_HIDDEN,

        GPGME_STATUS_NEED_PASSPHRASE,
        GPGME_STATUS_VALIDSIG,
        GPGME_STATUS_SIG_ID,
        GPGME_STATUS_ENC_TO,
        GPGME_STATUS_NODATA,
        GPGME_STATUS_BAD_PASSPHRASE,
        GPGME_STATUS_NO_PUBKEY,
        GPGME_STATUS_NO_SECKEY,
        GPGME_STATUS_NEED_PASSPHRASE_SYM,
        GPGME_STATUS_DECRYPTION_FAILED,
        GPGME_STATUS_DECRYPTION_OKAY,
        GPGME_STATUS_MISSING_PASSPHRASE,
        GPGME_STATUS_GOOD_PASSPHRASE,
        GPGME_STATUS_GOODMDC,
        GPGME_STATUS_BADMDC,
        GPGME_STATUS_ERRMDC,
        GPGME_STATUS_IMPORTED,
        GPGME_STATUS_IMPORT_OK,
        GPGME_STATUS_IMPORT_PROBLEM,
        GPGME_STATUS_IMPORT_RES,
        GPGME_STATUS_FILE_START,
        GPGME_STATUS_FILE_DONE,
        GPGME_STATUS_FILE_ERROR,

        GPGME_STATUS_BEGIN_DECRYPTION,
        GPGME_STATUS_END_DECRYPTION,
        GPGME_STATUS_BEGIN_ENCRYPTION,
        GPGME_STATUS_END_ENCRYPTION,

        GPGME_STATUS_DELETE_PROBLEM,
        GPGME_STATUS_GET_BOOL,
        GPGME_STATUS_GET_LINE,
        GPGME_STATUS_GET_HIDDEN,
        GPGME_STATUS_GOT_IT,
        GPGME_STATUS_PROGRESS,
        GPGME_STATUS_SIG_CREATED,
        GPGME_STATUS_SESSION_KEY,
        GPGME_STATUS_NOTATION_NAME,
        GPGME_STATUS_NOTATION_DATA,
        GPGME_STATUS_POLICY_URL,
        GPGME_STATUS_BEGIN_STREAM,
        GPGME_STATUS_END_STREAM,
        GPGME_STATUS_KEY_CREATED,
        GPGME_STATUS_USERID_HINT,
        GPGME_STATUS_UNEXPECTED,
        GPGME_STATUS_INV_RECP,
        GPGME_STATUS_NO_RECP,
        GPGME_STATUS_ALREADY_SIGNED,
        GPGME_STATUS_SIGEXPIRED,
        GPGME_STATUS_EXPSIG,
        GPGME_STATUS_EXPKEYSIG,
        GPGME_STATUS_TRUNCATED,
        GPGME_STATUS_ERROR,
        GPGME_STATUS_NEWSIG,
        GPGME_STATUS_REVKEYSIG,
        GPGME_STATUS_SIG_SUBPACKET,
        GPGME_STATUS_NEED_PASSPHRASE_PIN,
        GPGME_STATUS_SC_OP_FAILURE,
        GPGME_STATUS_SC_OP_SUCCESS,
        GPGME_STATUS_CARDCTRL,
        GPGME_STATUS_BACKUP_KEY_CREATED,
        GPGME_STATUS_PKA_TRUST_BAD,
        GPGME_STATUS_PKA_TRUST_GOOD,

        GPGME_STATUS_PLAINTEXT
    }
         
    /* The valid encryption flags.  */
    internal enum gpgme_encrypt_flags_t : int
    {
        NONE = 0,
        GPGME_ENCRYPT_ALWAYS_TRUST = 1
    }

    /* Verify.  */
    /* Flags used for the SUMMARY field in a gpgme_signature_t.  */
    [Flags]
    internal enum gpgme_sigsum_t : int
    {
        GPGME_SIGSUM_VALID = 0x0001,        /* The signature is fully valid.  */
        GPGME_SIGSUM_GREEN = 0x0002,        /* The signature is good.  */
        GPGME_SIGSUM_RED = 0x0004,          /* The signature is bad.  */
        GPGME_SIGSUM_KEY_REVOKED = 0x0010,  /* One key has been revoked.  */
        GPGME_SIGSUM_KEY_EXPIRED = 0x0020,  /* One key has expired.  */
        GPGME_SIGSUM_SIG_EXPIRED = 0x0040,  /* The signature has expired.  */
        GPGME_SIGSUM_KEY_MISSING = 0x0080,  /* Can't verify: key missing.  */
        GPGME_SIGSUM_CRL_MISSING = 0x0100,  /* CRL not available.  */
        GPGME_SIGSUM_CRL_TOO_OLD = 0x0200,  /* Available CRL is too old.  */
        GPGME_SIGSUM_BAD_POLICY = 0x0400,   /* A policy was not met.  */
        GPGME_SIGSUM_SYS_ERROR = 0x0800     /* A system error occured.  */
    }

    internal partial class libgpgme
    { }
}
