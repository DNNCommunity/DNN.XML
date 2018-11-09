// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011 by DotNetNuke Corp. 
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//  documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//  to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//  of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//  DEALINGS IN THE SOFTWARE.
// 
using System;
using System.Text;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Xml.Parameters

{
    /// <summary>
    ///   Represents a parameter
    /// </summary>
    public class ParameterInfo
    {
        #region Fields

        private readonly char[] _invalidCharacters = ", ;\"'?".ToCharArray();

        #endregion

        #region Initialization

        /// <summary>
        ///   Instantiates a new instance of the <c>Parameter</c> module.
        /// </summary>
        public ParameterInfo()
        {
            Key = new UniqueKey();
            Type = ParameterType.StaticValue.ToString();
            IsValueRequired = false;
            TypeArgument = "";
        }

        #endregion

        #region Sub-Classes

        /// <summary>
        ///   Represents a unique <see cref = "ParameterInfo" />.
        /// </summary>
        public class UniqueKey
        {
            // fields

            private int _id = -1;

            /// <summary>
            ///   Gets or sets the unique identifier
            /// </summary>
            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

            /// <summary>
            ///   Gets a value indicating whether the <see cref = "ParameterInfo" /> is new.
            /// </summary>
            public bool IsNew
            {
                get { return (_id <= 0); }
            }
        }

        #endregion

        #region Properties

        #region Key

        /// <summary>
        ///   Gets or sets the unique key.
        /// </summary>
        public UniqueKey Key { get; set; }


        /// <summary>
        ///   Gets or sets the unique identifier.
        /// </summary>
        public int Id
        {
            get { return Key.Id; }
            set { Key.Id = value; }
        }


        /// <summary>
        ///   Gets a value indicating whether the object is new.
        /// </summary>
        public bool IsNew
        {
            get { return Key.IsNew; }
        }

        #endregion

        /// <summary>
        ///   Gets or sets the module identifier.
        /// </summary>
        public int ModuleId { get; set; }


        /// <summary>
        ///   Gets or sets the parameter name.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///   Gets or sets the parameter type.
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        ///   Gets or sets the parameter type argument.
        /// </summary>
        /// <remarks>
        ///   The parameter type argument varies in purpose and usage. For instance, the argument may 
        ///   represent a value, querystring parameter name, or custom user profile property.
        ///   It can also be used to store a fallback value that acts as replacement when no value is present.
        /// </remarks>
        public string TypeArgument { get; set; }

        /// <summary>
        ///   Gets or sets whether a value is needed.
        /// </summary>
        /// <remarks>
        ///   The parameter is only valid if it returns a value different from nothing or empty string.
        /// </remarks>
        public bool IsValueRequired { get; set; }


        /// <summary>
        ///   Gets a value indicating whether the parameter definition is valid.
        /// </summary>
        public bool IsValidDefinition
        {
            get
            {
                // check name
                if (string.IsNullOrEmpty(Name) || Name.IndexOfAny(_invalidCharacters) != -1)
                    return false;

                // check argument based on type
                switch (ParseType(Type))
                {
                    case ParameterType.PassThrough:
                    case ParameterType.UserCustomProperty:
                    case ParameterType.StaticValue:
                    case ParameterType.FormPassThrough:
                        return (!string.IsNullOrEmpty(TypeArgument));
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the parameter is valid.
        /// </summary>
        public bool IsValidValue
        {
            get { return !IsValueRequired || !String.IsNullOrEmpty(GetValue()); }
        }


        public bool IsStatic
        {
            get
            {
                switch (ParseType(Type))
                {
                    case ParameterType.StaticValue:
                    case ParameterType.PortalName:
                    case ParameterType.PortalID:
                    case ParameterType.TabID:
                    case ParameterType.HomeDirectory:
                        return (true);
                    default:
                        return false;
                }
            }
        }

        #endregion

        #region Methods [Public]

        /// <summary>
        ///   Converts a string representation of a <see cref = "ParameterType" /> to its object value.
        /// </summary>
        /// <param name = "type">Value to convert.</param>
        private static ParameterType ParseType(string type)
        {
            var objType = default(ParameterType);
            try
            {
                objType = (ParameterType) Enum.Parse(objType.GetType(), type);
            }
            catch
            {
                objType = ParameterType.StaticValue;
            }
            return objType;
        }


        /// <summary>
        ///   Determines parameter value based on applied settings.
        /// </summary>
        public string GetValue()
        {
            // init vars
            var argumentIsEmpty = string.IsNullOrEmpty(TypeArgument);
            var objSecurity = new PortalSecurity();
            // get value based on type
            switch (ParseType(Type))
            {
                case ParameterType.StaticValue:
                    // static

                    return TypeArgument;
                case ParameterType.PassThrough:
                    // pass-thru parameter / Querystring
                    if (argumentIsEmpty)
                        return "";
                    if ((HttpContext.Current != null))
                    {
                        var qString = HttpContext.Current.Request.QueryString[TypeArgument].DefaultIfNullOrEmpty();
                        return objSecurity.InputFilter(qString,
                                                       PortalSecurity.FilterFlag.NoMarkup |
                                                       PortalSecurity.FilterFlag.NoScripting);
                    }

                    break;
                case ParameterType.FormPassThrough:
                    // pass-thru parameter 
                    if (argumentIsEmpty)
                        return "";
                    if ((HttpContext.Current != null))
                    {
                        var fString = HttpContext.Current.Request.Form[TypeArgument].DefaultIfNullOrEmpty();
                        return objSecurity.InputFilter(fString,
                                                       PortalSecurity.FilterFlag.NoMarkup |
                                                       PortalSecurity.FilterFlag.NoScripting);
                    }

                    break;
                case ParameterType.PortalID:
                    // portal id

                    return Convert.ToString(Globals.GetPortalSettings().PortalId);
                case ParameterType.PortalName:
                    // portal name

                    return Convert.ToString(Globals.GetPortalSettings().PortalName);
                case ParameterType.HomeDirectory:
                    // portal name

                    return Convert.ToString(Globals.GetPortalSettings().HomeDirectory);
                case ParameterType.CurrentCulture:
                    // portal name

                    return Convert.ToString(new Localization().CurrentCulture);
                case ParameterType.TabID:
                    // active tab id

                    return Convert.ToString(Globals.GetPortalSettings().ActiveTab.TabID);
                case ParameterType.ModuleID:
                    // module id

                    return Convert.ToString(ModuleId);
                default:
                    // user property
                    // get current user
                    var objUser = UserController.Instance.GetCurrentUserInfo();

                    // handle user property
                    switch (ParseType(Type))
                    {
                        case ParameterType.UserCustomProperty:
                            // custom property
                            if (argumentIsEmpty)
                                return "";
                            return objUser.Profile.GetPropertyValue(TypeArgument);
                        case ParameterType.UserID:

                            return Convert.ToString(objUser.UserID);
                        case ParameterType.UserUsername:

                            return objUser.Username.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserFirstName:

                            return objUser.FirstName.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserLastName:

                            return objUser.LastName.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserFullName:

                            return objUser.DisplayName.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserEmail:

                            return objUser.Email.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserWebsite:

                            return objUser.Profile.Website.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserIM:

                            return objUser.Profile.IM.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserStreet:

                            return objUser.Profile.Street.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserUnit:

                            return objUser.Profile.Unit.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserCity:

                            return objUser.Profile.City.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserCountry:

                            return objUser.Profile.Country.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserRegion:

                            return objUser.Profile.Region.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserPostalCode:

                            return objUser.Profile.PostalCode.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserPhone:

                            return objUser.Profile.Telephone.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserCell:

                            return objUser.Profile.Cell.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserFax:

                            return objUser.Profile.Fax.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserLocale:

                            return objUser.Profile.PreferredLocale.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserTimeZone:

                            return objUser.Profile.PreferredTimeZone.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserIsAuthorized:

                            return objUser.Membership.Approved.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserIsLockedOut:

                            return objUser.Membership.LockedOut.DefaultIfNullOrEmpty(TypeArgument);
                        case ParameterType.UserIsSuperUser:

                            return objUser.IsSuperUser.DefaultIfNullOrEmpty(TypeArgument);
                    }

                    break;
            }
            return string.Empty;
        }


        /// <summary>
        ///   Determines whether the <see cref = "TypeArgument" /> is required based on the <see cref = "ParameterType" />.
        /// </summary>
        public bool IsArgumentRequired()
        {
            switch (ParseType(Type))
            {
                case ParameterType.StaticValue:
                case ParameterType.PassThrough:
                case ParameterType.FormPassThrough:
                case ParameterType.UserCustomProperty:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///   Determines whether the  <see cref = "TypeArgument" /> is allowed as a fallback value.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public bool SupportsFallbackValue()
        {
            return (Type.StartsWith("User") && ParseType(Type) != ParameterType.UserCustomProperty);
        }


        /// <summary>
        ///   Determines parameter value based on applied settings and on a given <see cref = "System.Text.Encoding" />
        /// </summary>
        /// <param name = "encoding"></param>
        /// <returns>paramter value pair (Param=Value)</returns>
        /// <remarks>
        /// </remarks>
        public string ToString(Encoding encoding)
        {
            // if valid, return formatted parameter; otherwise, return empty string
            return IsValidDefinition
                       ? string.Format("{0}={1}", Name, HttpUtility.UrlEncode(GetValue(), encoding))
                       : String.Empty;
        }

        /// <summary>
        ///   Determines parameter value based on applied settings
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }

        #endregion
    }
}