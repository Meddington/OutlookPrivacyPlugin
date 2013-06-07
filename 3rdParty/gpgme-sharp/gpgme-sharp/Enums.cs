/*
 * gpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
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
using System.ComponentModel;
using System.Text;
using Libgpgme.Interop;

namespace Libgpgme
{
    /* The available protocols.  */
    public enum Protocol : int
    {
        OpenPGP = gpgme_protocol_t.GPGME_PROTOCOL_OpenPGP,
        CMS = gpgme_protocol_t.GPGME_PROTOCOL_CMS,
        GPGConf = gpgme_protocol_t.GPGME_PROTOCOL_GPGCONF,
        Unknown = gpgme_protocol_t.GPGME_PROTOCOL_UNKNOWN
    }

    public enum KeyAlgorithm : int
    {
        [DescriptionAttribute("RSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign | AlgorithmCapability.CanEncrypt)]
        RSA   = gpgme_pubkey_algo_t.GPGME_PK_RSA,
        
        [DescriptionAttribute("RSA-E")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt)]
        RSA_E = gpgme_pubkey_algo_t.GPGME_PK_RSA_E,
        
        [DescriptionAttribute("RSA-S")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign)]
        RSA_S = gpgme_pubkey_algo_t.GPGME_PK_RSA_S,
        
        [DescriptionAttribute("ELG-E")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt)]
        ELG_E = gpgme_pubkey_algo_t.GPGME_PK_ELG_E,
        
        [DescriptionAttribute("DSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign)]
        DSA   = gpgme_pubkey_algo_t.GPGME_PK_DSA,
        
        [DescriptionAttribute("ELG")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt | AlgorithmCapability.CanSign)]
        ELG   = gpgme_pubkey_algo_t.GPGME_PK_ELG
    }
    
    public enum HashAlgorithm : int
    {
        None = gpgme_hash_algo_t.GPGME_MD_NONE,
        MD5 = gpgme_hash_algo_t.GPGME_MD_MD5,
        SHA1 = gpgme_hash_algo_t.GPGME_MD_SHA1,
        RMD160 = gpgme_hash_algo_t.GPGME_MD_RMD160,
        MD2 = gpgme_hash_algo_t.GPGME_MD_MD2,
        TIGER = gpgme_hash_algo_t.GPGME_MD_TIGER,   /* TIGER/192. */
        HAVAL = gpgme_hash_algo_t.GPGME_MD_HAVAL,   /* HAVAL, 5 pass, 160 bit. */
        SHA256 = gpgme_hash_algo_t.GPGME_MD_SHA256,
        SHA384 = gpgme_hash_algo_t.GPGME_MD_SHA384,
        SHA512 = gpgme_hash_algo_t.GPGME_MD_SHA512,
        SHA224 = gpgme_hash_algo_t.GPGME_MD_SHA224,
        MD4 = gpgme_hash_algo_t.GPGME_MD_MD4,
        CRC32 = gpgme_hash_algo_t.GPGME_MD_CRC32,
        CRC32_RFC1510 = gpgme_hash_algo_t.GPGME_MD_CRC32_RFC1510,
        CRC24_RFC2440 = gpgme_hash_algo_t.GPGME_MD_CRC24_RFC2440
    }

    public enum CipherAlgorithm : int
    {
        None = gnupg_cipher_algo_t.CIPHER_ALGO_NONE,
        IDEA = gnupg_cipher_algo_t.CIPHER_ALGO_IDEA,
        TRIPLEDES = gnupg_cipher_algo_t.CIPHER_ALGO_3DES,
        CAST5 = gnupg_cipher_algo_t.CIPHER_ALGO_CAST5,
        BLOWFISH = gnupg_cipher_algo_t.CIPHER_ALGO_BLOWFISH,
        AES = gnupg_cipher_algo_t.CIPHER_ALGO_AES,
        AES192 = gnupg_cipher_algo_t.CIPHER_ALGO_AES192,
        AES256 = gnupg_cipher_algo_t.CIPHER_ALGO_AES256,
        TWOFISH = gnupg_cipher_algo_t.CIPHER_ALGO_TWOFISH,
        CAMELLIA128 = gnupg_cipher_algo_t.CIPHER_ALGO_CAMELLIA128,
        CAMELLIA256 = gnupg_cipher_algo_t.CIPHER_ALGO_CAMELLIA256,
        DUMMY = gnupg_cipher_algo_t.CIPHER_ALGO_DUMMY
    }
    
    public enum CompressAlgorithm : int
    {
        None = gnupg_compress_algo_t.COMPRESS_ALGO_NONE,
        ZIP = gnupg_compress_algo_t.COMPRESS_ALGO_ZIP,
        ZLIB = gnupg_compress_algo_t.COMPRESS_ALGO_ZLIB,
        BZIP2 = gnupg_compress_algo_t.COMPRESS_ALGO_BZIP2
    }

    [Flags]
    public enum PgpFeatureFlags
    {
        [DescriptionAttribute("mdc")]
        MDC = 1,                // Modification Detection Code
        [DescriptionAttribute("ks-modify")]
        KeyserverModify = 2     //Keyserver no-modify
    }

    /* The available keylist mode flags.  */
    [Flags]
    public enum KeylistMode : int
    {
        Local = (int)gpgme_keylist_mode_t.GPGME_KEYLIST_MODE_LOCAL,
        Extern = (int)gpgme_keylist_mode_t.GPGME_KEYLIST_MODE_EXTERN,
        Signatures = (int)gpgme_keylist_mode_t.GPGME_KEYLIST_MODE_SIGS,
        SignatureNotations = (int)gpgme_keylist_mode_t.GPGME_KEYLIST_MODE_SIG_NOTATIONS ,
        Validate = (int)gpgme_keylist_mode_t.GPGME_KEYLIST_MODE_VALIDATE
    }

    public enum PassphraseResult : int
    {
        Success = 0,
        Canceled = gpg_err_code_t.GPG_ERR_CANCELED
    }

    /* Signature notations.  */
    /* The available signature notation flags.  */
    [Flags]
    public enum SignatureNotationFlags : int
    {
        HumanReadable = gpgme_sig_notation_flags_t.GPGME_SIG_NOTATION_HUMAN_READABLE,
        Critical = gpgme_sig_notation_flags_t.GPGME_SIG_NOTATION_CRITICAL
    }
	
    /* The available validities for a trust item or key.  */
    public enum Validity : int
    {
        Unknown = gpgme_validity_t.GPGME_VALIDITY_UNKNOWN,
        Undefined = gpgme_validity_t.GPGME_VALIDITY_UNDEFINED,
        Never = gpgme_validity_t.GPGME_VALIDITY_NEVER,
        Marginal = gpgme_validity_t.GPGME_VALIDITY_MARGINAL,
        Full = gpgme_validity_t.GPGME_VALIDITY_FULL,
        Ultimate = gpgme_validity_t.GPGME_VALIDITY_ULTIMATE
    }

    /* The possible encoding mode of gpgme_data_t objects.  */
    public enum DataEncoding : int
    {
        None = gpgme_data_encoding_t.GPGME_DATA_ENCODING_NONE,	/* Not specified.  */
        Binary = gpgme_data_encoding_t.GPGME_DATA_ENCODING_BINARY,
        Base64 = gpgme_data_encoding_t.GPGME_DATA_ENCODING_BASE64,
        Armor = gpgme_data_encoding_t.GPGME_DATA_ENCODING_ARMOR 	/* Either PEM or OpenPGP Armor.  */
    }

    /* Import.  */
    [Flags]
    public enum ImportStatusFlags
    {
        /* The key was new.  */
        New = 1,
        /* The key contained new user IDs.  */
        Uid = 2,
        /* The key contained new signatures.  */
        Signature = 4,
        /* The key contained new sub keys.  */
        Subkey = 8,
        /* The key contained a secret key.  */
        Secret = 16
    }

    public enum TrustItemType
    {
        Key = 1,
        UserId = 2
    }

    public enum EncryptFlags : int
    {
        None = gpgme_encrypt_flags_t.NONE,
        AlwaysTrust = gpgme_encrypt_flags_t.GPGME_ENCRYPT_ALWAYS_TRUST
    }

    /* The available signature modes.  */
    public enum SignatureMode : int
    {
        Normal = gpgme_sig_mode_t.GPGME_SIG_MODE_NORMAL,
        Detach = gpgme_sig_mode_t.GPGME_SIG_MODE_DETACH,
        Clear = gpgme_sig_mode_t.GPGME_SIG_MODE_CLEAR
    }

    public enum PkaStatus : int
    {
        NotAvailable = 0, 
        Bad = 1, 
        Okay = 2, 
        RFU = 3
    }

    [Flags]
    public enum SignatureSummary : int
    {
        Valid = gpgme_sigsum_t.GPGME_SIGSUM_VALID ,        /* The signature is fully valid.  */
        Green = gpgme_sigsum_t.GPGME_SIGSUM_GREEN ,        /* The signature is good.  */
        Red = gpgme_sigsum_t.GPGME_SIGSUM_RED ,          /* The signature is bad.  */
        KeyRevoked = gpgme_sigsum_t.GPGME_SIGSUM_KEY_REVOKED ,  /* One key has been revoked.  */
        KeyExpired = gpgme_sigsum_t.GPGME_SIGSUM_KEY_EXPIRED ,  /* One key has expired.  */
        SignatureExpired = gpgme_sigsum_t.GPGME_SIGSUM_SIG_EXPIRED ,  /* The signature has expired.  */
        KeyMissing = gpgme_sigsum_t.GPGME_SIGSUM_KEY_MISSING ,  /* Can't verify: key missing.  */
        CRLMissing = gpgme_sigsum_t.GPGME_SIGSUM_CRL_MISSING ,  /* CRL not available.  */
        CRLTooOld = gpgme_sigsum_t.GPGME_SIGSUM_CRL_TOO_OLD ,  /* Available CRL is too old.  */
        BadPolicy = gpgme_sigsum_t.GPGME_SIGSUM_BAD_POLICY ,   /* A policy was not met.  */
        SysError = gpgme_sigsum_t.GPGME_SIGSUM_SYS_ERROR      /* A system error occured.  */
    }

    /* Key editing status codes */
    public enum KeyEditStatusCode : int
    {
        Eof = gpgme_status_code_t.GPGME_STATUS_EOF,
        /* mkstatus processing starts here */
        Enter = gpgme_status_code_t.GPGME_STATUS_ENTER,
        Leave = gpgme_status_code_t.GPGME_STATUS_LEAVE,
        Abort = gpgme_status_code_t.GPGME_STATUS_ABORT,

        GoodSignature = gpgme_status_code_t.GPGME_STATUS_GOODSIG,
        BadSignature = gpgme_status_code_t.GPGME_STATUS_BADSIG,
        ErrorSignature = gpgme_status_code_t.GPGME_STATUS_ERRSIG,

        BadArmor = gpgme_status_code_t.GPGME_STATUS_BADARMOR,

        RSAorIDEA = gpgme_status_code_t.GPGME_STATUS_RSA_OR_IDEA,
        KeyExpired = gpgme_status_code_t.GPGME_STATUS_KEYEXPIRED,
        KeyRevoked = gpgme_status_code_t.GPGME_STATUS_KEYREVOKED,

        TrustUndefined = gpgme_status_code_t.GPGME_STATUS_TRUST_UNDEFINED,
        TrustNever = gpgme_status_code_t.GPGME_STATUS_TRUST_NEVER,
        TrustMarginal = gpgme_status_code_t.GPGME_STATUS_TRUST_MARGINAL,
        TrustFully = gpgme_status_code_t.GPGME_STATUS_TRUST_FULLY,
        TrustUltimate = gpgme_status_code_t.GPGME_STATUS_TRUST_ULTIMATE,

        SHMInfo = gpgme_status_code_t.GPGME_STATUS_SHM_INFO,
        SHMGet = gpgme_status_code_t.GPGME_STATUS_SHM_GET,
        SHMGetBool = gpgme_status_code_t.GPGME_STATUS_SHM_GET_BOOL,
        SHMGetHidden = gpgme_status_code_t.GPGME_STATUS_SHM_GET_HIDDEN,

        NeedPassphrase = gpgme_status_code_t.GPGME_STATUS_NEED_PASSPHRASE,
        ValidSignature = gpgme_status_code_t.GPGME_STATUS_VALIDSIG,
        SignatureId = gpgme_status_code_t.GPGME_STATUS_SIG_ID,
        EncryptedTo = gpgme_status_code_t.GPGME_STATUS_ENC_TO,
        NoData = gpgme_status_code_t.GPGME_STATUS_NODATA,
        BadPassphrase = gpgme_status_code_t.GPGME_STATUS_BAD_PASSPHRASE,
        NoPublicKey = gpgme_status_code_t.GPGME_STATUS_NO_PUBKEY,
        NoSecretKey = gpgme_status_code_t.GPGME_STATUS_NO_SECKEY,
        NeedPassphraseSym = gpgme_status_code_t.GPGME_STATUS_NEED_PASSPHRASE_SYM,
        DecryptionFailed = gpgme_status_code_t.GPGME_STATUS_DECRYPTION_FAILED,
        DecryptionOkay = gpgme_status_code_t.GPGME_STATUS_DECRYPTION_OKAY,
        MissingPassphrase = gpgme_status_code_t.GPGME_STATUS_MISSING_PASSPHRASE,
        GoodPassphrase = gpgme_status_code_t.GPGME_STATUS_GOOD_PASSPHRASE,
        GoodMDC = gpgme_status_code_t.GPGME_STATUS_GOODMDC,
        BadMDC = gpgme_status_code_t.GPGME_STATUS_BADMDC,
        ErrorMDC = gpgme_status_code_t.GPGME_STATUS_ERRMDC,
        Imported = gpgme_status_code_t.GPGME_STATUS_IMPORTED,
        ImportOk = gpgme_status_code_t.GPGME_STATUS_IMPORT_OK,
        ImportProblem = gpgme_status_code_t.GPGME_STATUS_IMPORT_PROBLEM,
        ImportRes = gpgme_status_code_t.GPGME_STATUS_IMPORT_RES,
        FileStart = gpgme_status_code_t.GPGME_STATUS_FILE_START,
        FileDone = gpgme_status_code_t.GPGME_STATUS_FILE_DONE,
        FileError = gpgme_status_code_t.GPGME_STATUS_FILE_ERROR,

        BeginDecryption = gpgme_status_code_t.GPGME_STATUS_BEGIN_DECRYPTION,
        EndDecryption = gpgme_status_code_t.GPGME_STATUS_END_DECRYPTION,
        BeginEncryption = gpgme_status_code_t.GPGME_STATUS_BEGIN_ENCRYPTION,
        EndEncryption = gpgme_status_code_t.GPGME_STATUS_END_ENCRYPTION,

        DeleteProblem = gpgme_status_code_t.GPGME_STATUS_DELETE_PROBLEM,
        GetBool = gpgme_status_code_t.GPGME_STATUS_GET_BOOL,
        GetLine = gpgme_status_code_t.GPGME_STATUS_GET_LINE,
        GetHidden = gpgme_status_code_t.GPGME_STATUS_GET_HIDDEN,
        GotIt = gpgme_status_code_t.GPGME_STATUS_GOT_IT,
        Progress = gpgme_status_code_t.GPGME_STATUS_PROGRESS,
        SignatureCreated = gpgme_status_code_t.GPGME_STATUS_SIG_CREATED,
        SessionKey = gpgme_status_code_t.GPGME_STATUS_SESSION_KEY,
        NotationName = gpgme_status_code_t.GPGME_STATUS_NOTATION_NAME,
        NotationData = gpgme_status_code_t.GPGME_STATUS_NOTATION_DATA,
        PolicyURL = gpgme_status_code_t.GPGME_STATUS_POLICY_URL,
        BeginStream = gpgme_status_code_t.GPGME_STATUS_BEGIN_STREAM,
        EndStream = gpgme_status_code_t.GPGME_STATUS_END_STREAM,
        KeyCreated = gpgme_status_code_t.GPGME_STATUS_KEY_CREATED,
        UserIdHint = gpgme_status_code_t.GPGME_STATUS_USERID_HINT,
        Unexpected = gpgme_status_code_t.GPGME_STATUS_UNEXPECTED,
        InvalidRecipient = gpgme_status_code_t.GPGME_STATUS_INV_RECP,
        NoRecipient = gpgme_status_code_t.GPGME_STATUS_NO_RECP,
        AlreadySigned = gpgme_status_code_t.GPGME_STATUS_ALREADY_SIGNED,
        SignatureExpired = gpgme_status_code_t.GPGME_STATUS_SIGEXPIRED,
        ExpiredSignature = gpgme_status_code_t.GPGME_STATUS_EXPSIG,
        ExpiredKeySignature = gpgme_status_code_t.GPGME_STATUS_EXPKEYSIG,
        Truncated = gpgme_status_code_t.GPGME_STATUS_TRUNCATED,
        Error = gpgme_status_code_t.GPGME_STATUS_ERROR,
        NewSignature = gpgme_status_code_t.GPGME_STATUS_NEWSIG,
        RevKeySignature = gpgme_status_code_t.GPGME_STATUS_REVKEYSIG,
        SignedSubPacket = gpgme_status_code_t.GPGME_STATUS_SIG_SUBPACKET,
        NeedPassphrasePin = gpgme_status_code_t.GPGME_STATUS_NEED_PASSPHRASE_PIN,
        SCOperationFailure = gpgme_status_code_t.GPGME_STATUS_SC_OP_FAILURE,
        SCOperationSuccess = gpgme_status_code_t.GPGME_STATUS_SC_OP_SUCCESS,
        Cardctrl = gpgme_status_code_t.GPGME_STATUS_CARDCTRL,
        BackupKeyCreated = gpgme_status_code_t.GPGME_STATUS_BACKUP_KEY_CREATED,
        PKATrustBad = gpgme_status_code_t.GPGME_STATUS_PKA_TRUST_BAD,
        PKATrustGood = gpgme_status_code_t.GPGME_STATUS_PKA_TRUST_GOOD,

        PlainText = gpgme_status_code_t.GPGME_STATUS_PLAINTEXT
    }
    [Flags]
    public enum PgpSignatureType
    {
        Normal = 0,    // sign
        NonExportable = 1,    // l 
        NonRevocable = 2,    // nr
        Trust = 4,    // t
    }
    public enum PgpSignatureClass : int
    {
        Generic = 0,
        Persona = 1,
        Casual = 2,
        Positive = 3
    }
    public enum PgpSignatureTrustLevel : int
    {
        Marginal = 1,
        Full = 2
    }

    public enum PgpRevokeSignatureReasonCode : int
    {
        NoReason = 0,
        UidNoLongerValid = 4
    }

    public enum PgpOwnerTrust : int
    {
        Undefined = gpgme_validity_t.GPGME_VALIDITY_UNDEFINED,  // I don't know or won't say
        Never = gpgme_validity_t.GPGME_VALIDITY_NEVER,          // I do NOT trust
        Marginal = gpgme_validity_t.GPGME_VALIDITY_MARGINAL,    // I trust marginally
        Full = gpgme_validity_t.GPGME_VALIDITY_FULL,            // I trust fully
        Ultimate = gpgme_validity_t.GPGME_VALIDITY_ULTIMATE     // I trust ultimately
    }

    [Flags]
    public enum AlgorithmCapability 
    {
        CanNothing = 0,
        CanSign = 1,
        CanEncrypt = 2,
        CanCert = 4,
        CanAuth = 8,
        Unknown = 128
    }

    public enum PgpSubkeyAlgorithm : int
    {
        [DescriptionAttribute("DSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign)]
        DSASignOnly = 2,

        [DescriptionAttribute("DSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.Unknown)]
        DSAUseCapabilities = 3,

        [DescriptionAttribute("Elgamal")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt)]
        ELGEncryptOnly = 4,

        [DescriptionAttribute("RSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanSign)]
        RSASignOnly = 5,

        [DescriptionAttribute("RSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.CanEncrypt)]
        RSAEncryptOnly = 6,

        [DescriptionAttribute("RSA")]
        [AlgorithmCapabilityAttribute(AlgorithmCapability.Unknown)]
        RSAUseCapabilities = 7

    }
}