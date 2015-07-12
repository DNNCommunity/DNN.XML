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
namespace DotNetNuke.Modules.Xml.Parameters
{
    public enum ParameterType
    {
        StaticValue,
        PassThrough, //means QueryStringPassThrough
        FormPassThrough,
        UserCustomProperty,
        // ReSharper disable InconsistentNaming 
        PortalID, //- it is ...ID for compatibility reason
        PortalName,
        HomeDirectory,
        CurrentCulture,
        TabID,
        ModuleID,
        UserID,
        UserUsername,
        UserFirstName,
        UserLastName,
        UserFullName,
        UserEmail,
        UserWebsite,
        UserIM,
        UserStreet,
        UserUnit,
        UserCity,
        UserCountry,
        UserRegion,
        UserPostalCode,
        UserPhone,
        UserCell,
        UserFax,
        UserLocale,
        UserTimeZone,
        UserIsAuthorized,
        UserIsLockedOut,
        UserIsSuperUser
        // ReSharper restore InconsistentNaming
    }
}