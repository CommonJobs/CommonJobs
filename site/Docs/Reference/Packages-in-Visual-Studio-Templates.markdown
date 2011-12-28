﻿# Packages in Visual Studio Templates

NuGet supports adding NuGet packages to Visual Studio project templates and item 
templates. When Visual Studio creates a project using a template that contains these 
NuGet packages, the resulting project will have these packages installed. This is why 
this feature is often referred to as the "Preinstalled NuGet Packages" feature.

This is useful when you want your project/item template to reference a library
(e.g. jQuery or EntityFramework). For example, project templates for ASP.NET MVC 3
include jQuery, jQuery.Validation, Modernizr, and a number of other libraries
as Nuget packages.

This provides a better experience for end-users who can easily update the 
NuGet packages installed by the project/item template long after the template has 
shipped. At the same time it makes it easier to author templates since you only 
need to reference a single library package file instead of tracking all the files 
required by the library

## Authoring Visual Studio templates

This article does not describe how to author a Visual Studio template. Follow 
these links to find more information on how to create templates [directly
using Visual Studio](http://msdn.microsoft.com/en-us/library/s365byhx.aspx)
or [using the Visual Studio SDK](http://msdn.microsoft.com/en-us/library/ff527340.aspx).

## Adding packages to a template

Preinstalled packages work using [template wizards](http://msdn.microsoft.com/en-us/library/ms185301.aspx).
A special wizard gets invoked when the template gets instantiated. The wizard
loads the list of packages that need to be installed and passes that information
to the appropriate NuGet APIs.

The template needs to specify where to find the package nupkg files. Currently
two package repositories are supported:

1. Packages embedded inside of a VSIX package.
2. Packages embedded inside of the project/item template itself.

To add preinstalled packages to your project/item template you need to:

1. Edit your vstemplate file and add a reference to the NuGet template wizard by
adding a [`WizardExtension`](http://msdn.microsoft.com/en-us/library/ms171411.aspx) element:
    <pre><code>&lt;WizardExtension&gt;
        &lt;Assembly&gt;NuGet.VisualStudio.Interop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a&lt;/Assembly&gt;
        &lt;FullClassName&gt;NuGet.VisualStudio.TemplateWizard&lt;/FullClassName&gt;
    &lt;/WizardExtension&gt;</code></pre>

    `NuGet.VisualStudio.Interop.dll` is a new assembly that only contains the
    `TemplateWizard` class. This class is a simple wrapper that calls into the actual
    implementation that lives in `NuGet.VisualStudio.dll`. The assembly version will
    never change so that project/item templates continue to work with new versions of
    NuGet.

2. Add the list of packages to install in the project: 
    <pre><code>&lt;WizardData&gt;
        &lt;packages&gt;
            &lt;package id="jQuery" version="1.6.2" /&gt;
        &lt;/packages&gt;
    &lt;/WizardData&gt;</code></pre>

    The wizard supports multiple `<package>` elements. Both the `id` and
    `version` attributes are required. An important consequence of this is that 
    a _specific_ version of a package will be installed even if a newer version
    is available in the online package feed.
    
    The reason for this behavior is that a future version of a package might 
    introduce a change that is not compatible with the project/item template. The 
    choice to upgrade the package to the latest version using NuGet is left 
    to the developer who is in the best position to assume the risks of upgrading 
    the package to the latest version.

The remaining step is to specify the repository where NuGet can find the package
files. As mentioned earlier, two package repository modes are supported:

### VSIX package repository

The recommended approach for deploying Visual Studio project/item templates is through
a VSIX package ([read more about VSIX deployment here](http://msdn.microsoft.com/en-us/library/ff363239.aspx)).
The VSIX method is
preferable because it allows you to package multiple project/item templates together
and allows developers to easily discover your templates using the VS Extension
Manager or the Visual Studio Gallery. On top of that you can easily push updates
to your users using the [Visual Studio Extension Manager automatic update mechanism](http://msdn.microsoft.com/en-us/library/dd997169.aspx).

1. To specify a VSIX as a package repository you modify the `<package>` element:
    <pre><code>&lt;packages repository="extension"
              repositoryId="MyTemplateContainerExtensionId"&gt;
    ...
    &lt;/packages&gt;</code></pre>
    The `repository` attribute specifies the type of repository (“extension”)
    while `repositoryId` is the unique identifier of your VSIX (i.e. the value of
    the [`ID` attribute](http://msdn.microsoft.com/en-us/library/dd393688.aspx) in
    the extension’s vsixmanifest file).
 
You need to add your nupkg files as [custom extension content](http://msdn.microsoft.com/en-us/library/dd393737.aspx)
and ensure that they are located under a folder called `Packages` within the
VSIX package. You can place the nupkg files in the same VSIX as your project
templates or you can have the packages be located in a separate VSIX if that
makes more sense for your scenario (just note that you should not reference VSIXs
you do not have control over since they could change in the future and your
project/item templates would break).

### Template package repository

If packaging multiple projects is not important to you (e.g. you are only
distributing a single project/item template), a simpler but also more limited approach
is to include the nupgk files in the project/item template zip file itself.

However, if you are bundling a set of project/item templates that relate to each other
and share NuGet packages (e.g. you are shipping a custom MVC project template with
versions for Razor, Web Forms, C#, and VB.NET), we do not recommend adding the NuGet
packages directly to each project/item template zip file. It needlessly increases the
size of the project/item template bundle.

1. To specify the project/item template as a package repository you modify the `<package>` element:
    <pre><code>&lt;packages repository="template"&gt;
        ...
    &lt;/packages&gt;</code></pre>
    The `repository` attribute now has the value "template" and the `repositoryId`
    attribute is not longer required. The nupkg files need to be placed into the
    root directory of the project/item template zip file.

## Best Practices

1. Make your VSIX declare a dependency on the NuGet VSIX.
2. Require project/item templates to be saved on creation by setting
[`<PromptForSaveOnCreation>`](http://msdn.microsoft.com/en-us/library/twfxayz5.aspx) in the
.vstemplate file.

## Packages in VS Templates sample project

A sample project is available to get you started. The source code is available 
[here](https://bitbucket.org/marcind/nugetinvstemplates).