'
' DotNetNukeŽ - http://www.dotnetnuke.com
' Copyright (c) 2002-2013
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System.DirectoryServices
Imports System.Runtime.InteropServices
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Authentication.ActiveDirectory.ADSI

#Region "Enum"

    Public Enum CompareOperator As Integer
        [Is]
        [IsNot]
        [StartsWith]
        [EndsWith]
        [Present]
        [NotPresent]
    End Enum

    Public Enum GroupType
        UNIVERSAL_GROUP = -2147483640
        GLOBAL_GROUP = -2147483646
        DOMAIN_LOCAL_GROUP = -2147483644
    End Enum


    Public Enum UserFlag
        ADS_UF_SCRIPTADS_UF_SCRIPT = 1
        '0x1 The logon script is executed. This flag does not work for the ADSI LDAP provider on either read or write operations. For the  ADSI WinNT provider, this flag is  read-only data, and it cannot be set for user objects. = 1    
        ADS_UF_ACCOUNTDISABLE = 2
        '0x2 user account is disabled.
        ADS_UF_HOMEDIR_REQUIRED = 8
        '0x8 The home directory is required.  
        ADS_UF_LOCKOUT = 16
        '0x10 The account is currently locked out.  
        ADS_UF_PASSWD_NOTREQD = 32
        '0x20 No password is required.
        ADS_UF_PASSWD_CANT_CHANGE = 64
        '0x40 The user cannot change the password. This flag can be read, but not set directly.  For more information and a code example that shows how to prevent a user from changing the password, see User Cannot Change Password. 
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 128
        '0x80 The user can send an encrypted password. 
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = 256
        '0x100 This is an account for users whose primary account is in another domain. This account provides user access to this domain, but not to any domain that trusts this domain. Also known as a  local user account. = 256,    
        ADS_UF_NORMAL_ACCOUNT = 512
        '0x200 This is a default account type that represents a typical user.
        ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 2048
        '0x800 This is a permit to trust account for a system domain that trusts other domains.
        ADS_UF_WORKSTATION_TRUST_ACCOUNT = 4096
        'This is a computer account for a Microsoft Windows NT Workstation/Windows 2000 Professional or Windows NT Server/Windows 2000 Server that is a member of this domain.  0x1000
        ADS_UF_SERVER_TRUST_ACCOUNT = 8192
        'This is a computer account for a system backup domain controller that is a member of this domain. 0x2000
        ADS_UF_DONT_EXPIRE_PASSWD = 65536
        '0x10000 When set, the password will not expire on this account.  
        ADS_UF_MNS_LOGON_ACCOUNT = 131072
        ' 0x20000 This is an MNS logon account. 
        ADS_UF_SMARTCARD_REQUIRED = 262144
        '0x40000 When set, this flag will force the user to log on using a smart card. 
        ADS_UF_TRUSTED_FOR_DELEGATION = 524288
        '0x80000 When set, the service account (user or computer account), under which a service runs, is trusted for Kerberos delegation. Any such service can impersonate a client requesting the service. To enable a service for Kerberos delegation, set this flag on the  userAccountControl property of the service account. 
        ADS_UF_NOT_DELEGATED = 1048576
        '0x100000 When set, the security context of the user will not be delegated to a service even if the service account is set as trusted for Kerberos delegation. 
        ADS_UF_USE_DES_KEY_ONLY = 2097152
        '0x200000 Restrict this principal to use only Data Encryption Standard (DES) encryption types for keys.Active Directory Client Extension:  Not supported.
        ADS_UF_DONT_REQUIRE_PREAUTH = 4194304
        '0x400000 This account does not require Kerberos preauthentication for logon.Active Directory Client Extension:  Not supported.
        ADS_UF_PASSWORD_EXPIRED = 8388608
        '0x800000 The user password has expired. This flag is created by the system using data from the  password last set attribute and the domain policy.  It is read-only and cannot be set. To manually set a user password as expired, use the NetUserSetInfo function with the USER_INFO_3 (usri3_password_expired member) or USER_INFO_4 (usri4_password_expired member) structure.Active Directory Client Extension:  Not supported.
        ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 16777216
        'The account is enabled for delegation. This is a security-sensitive setting; accounts with this option enabled should be strictly controlled. This setting enables a service running under the account to assume a client identity and authenticate as that user to other remote servers on the network.Active Directory Client Extension:  Not supported.
    End Enum

#End Region

    Public Class Configuration
        Implements IConfiguration

        Public Enum Path
            GC
            LDAP
            ADs
            WinNT
        End Enum


        Public Const ADSI_CONFIGURATIONNAMIMGCONTEXT As String = "configurationNamingContext"
        Public Const ADSI_DEFAULTNAMIMGCONTEXT As String = "defaultNamingContext"
        Public Const ADSI_ROOTDOMAINNAMIMGCONTEXT As String = "rootDomainNamingContext"
        Public Const ADSI_USERACCOUNTCONTROL As String = "userAccountControl"
        Public Const ADSI_CLASS As String = "objectClass"
        Public Const ADSI_CATEGORY As String = "objectCategory"
        Public Const ADSI_DC As String = "dc"
        Public Const ADSI_NCNAME As String = "nCName"
        Public Const ADSI_DNSROOT As String = "dnsRoot"
        Public Const ADSI_GROUPTYPE As String = "groupType"
        Public Const ADSI_MEMBER As String = "member"
        Public Const ADSI_CNAME As String = "cn"
        Public Const ADSI_ACCOUNTNAME As String = "sAMAccountName"
        Public Const ADSI_DISTINGUISHEDNAME As String = "distinguishedName"
        Public Const ADSI_CANONICALNAME As String = "canonicalName"
        Public Const ADSI_UPN As String = "userPrincipalName"
        Public Const ADSI_DISPLAYNAME As String = "displayName"
        Public Const ADSI_FIRSTNAME As String = "givenName"
        Public Const ADSI_LASTNAME As String = "sn"
        Public Const ADSI_STREET As String = "streetAddress"
        Public Const ADSI_CITY As String = "l"
        Public Const ADSI_POSTALCODE As String = "postalCode"
        Public Const ADSI_REGION As String = "st"
        Public Const ADSI_COUNTRY As String = "co"
        Public Const ADSI_TELEPHONE As String = "telephoneNumber"
        Public Const ADSI_FAX As String = "facsimileTelephoneNumber"
        Public Const ADSI_CELL As String = "mobile"
        Public Const ADSI_HOMEPHONE As String = "homePhone"
        Public Const ADSI_EMAIL As String = "mail"
        Public Const ADSI_WEBSITE As String = "url"
        Public Const ADSI_DESCRIPTION As String = "description"
        Public Const ADSI_EMPLOYEEID As String = "employeeID"
        Public Const ADSI_COMPANY As String = "company"
        Public Const ADSI_MANAGER As String = "manager"
        Public Const ADSI_DEPARTMENT As String = "department"
        Public Const ADSI_ASSISTANT As String = "assistant"
        Public Const ADSI_PHOTO As String = "thumbnailPhoto"

        Private Const ADSI_CONFIG_CACHE_PREFIX As String = "ADSI.Configuration"


        Private config As ActiveDirectory.ConfigInfo
        Private portalSettings As PortalSettings
        ''' -------------------------------------------------------------------
        ''' <summary>
        '''     Obtain Authentication settings from database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]	08/01/2004	Created
        ''' </history>
        ''' -------------------------------------------------------------------
        Sub New(ByVal configuration As ActiveDirectory.IConfiguration,
                ByVal portalController As IPortalController)

            Me.config = configuration.GetConfig()
            Me.portalSettings = portalController.GetCurrentSettings

        End Sub
        Public Function getConfigInfo() As ConfigInfo Implements IConfiguration.getConfigInfo

            Dim adsiConfig As ConfigInfo = Nothing
            Dim gc As New DirectoryEntry
            Dim ldap As New DirectoryEntry

            If config IsNot Nothing Then
                adsiConfig = New ConfigInfo
                Try
                    'Temporary fix this setting as TRUE for design, to be removed when release
                    adsiConfig.PortalId = config.PortalId
                    adsiConfig.ConfigDomainPath = config.RootDomain
                    adsiConfig.DefaultEmailDomain = config.EmailDomain
                    adsiConfig.UserName = config.UserName
                    adsiConfig.Password = config.Password
                    adsiConfig.AuthenticationType = CType([Enum].Parse(GetType(AuthenticationTypes), config.AuthenticationType), AuthenticationTypes)
                    adsiConfig.RootDomainPath = Utilities.ValidateDomainPath(adsiConfig.ConfigDomainPath)
                    adsiConfig.RootDomainPath = Right(adsiConfig.RootDomainPath, adsiConfig.RootDomainPath.Length - adsiConfig.RootDomainPath.IndexOf("DC="))

                Catch exc As Exception
                    adsiConfig.ProcessLog += exc.Message & "<br>"
                End Try

                Try
                    If DirectoryEntry.Exists("GC://rootDSE") Then
                        Dim rootGC As New DirectoryEntry("GC://rootDSE")
                        adsiConfig.ConfigurationPath = rootGC.Properties(ADSI_CONFIGURATIONNAMIMGCONTEXT).Value.ToString
                        adsiConfig.ADSINetwork = True
                    End If
                Catch exc As COMException
                    adsiConfig.ADSINetwork = False
                    adsiConfig.LDAPAccesible = False
                    adsiConfig.ProcessLog += exc.Message & "<br>"
                    LogException(exc)
                End Try

                ' Also check if LDAP fully accessible
                Try
                    If DirectoryEntry.Exists("LDAP://rootDSE") Then
                        adsiConfig.LDAPAccesible = True
                        adsiConfig.RefCollection = New CrossReferenceCollection(adsiConfig.UserName, adsiConfig.Password, adsiConfig.AuthenticationType)
                    End If
                Catch exc As COMException
                    adsiConfig.LDAPAccesible = False
                    adsiConfig.ProcessLog += exc.Message & "<br>"
                    LogException(exc)
                End Try
            End If

            Return adsiConfig

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtain Authentication Configuration
        ''' </summary>
        ''' <remarks>
        ''' Accessing Active Directory also cost lots of resource, 
        ''' so we only do it once then save into application cache for later use
        ''' </remarks>
        ''' <history>
        '''     [tamttt]	08/01/2004	Created
        ''' </history>
        ''' -------------------------------------------------------------------
        Public Function GetConfig() As ConfigInfo Implements IConfiguration.GetConfig
            Dim strKey As String = $"{ADSI_CONFIG_CACHE_PREFIX}.{CStr(portalSettings.PortalId)}"

            Dim config As ConfigInfo = CType(DataCache.GetCache(strKey), ConfigInfo)
            If config Is Nothing Then
                config = getConfigInfo()
                DataCache.SetCache(strKey, config)
            End If

            Return config

        End Function

        Public Sub ResetConfig() Implements IConfiguration.ResetConfig
            Dim strKey As String = $"{ADSI_CONFIG_CACHE_PREFIX}.{CStr(portalSettings.PortalId)}"
            DataCache.RemoveCache(strKey)
        End Sub

        Public Sub SetSecurity(ByVal Entry As DirectoryEntry, config As ConfigInfo) Implements IConfiguration.SetSecurity
            Try
                Entry.AuthenticationType = config.AuthenticationType
                If (config.UserName.Length > 0) AndAlso (config.Password.Length > 0) Then
                    Entry.Username = config.UserName
                    Entry.Password = config.Password
                End If

            Catch ex As COMException
                LogException(ex)
            End Try
        End Sub


    End Class
End Namespace
