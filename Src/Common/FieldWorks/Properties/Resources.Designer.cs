﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIL.FieldWorks.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SIL.FieldWorks.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap DatabaseNew {
            get {
                object obj = ResourceManager.GetObject("DatabaseNew", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Download {
            get {
                object obj = ResourceManager.GetObject("Download", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Import {
            get {
                object obj = ResourceManager.GetObject("Import", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Info {
            get {
                object obj = ResourceManager.GetObject("Info", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you want to continue with the restore?.
        /// </summary>
        internal static string ksBackupErrorDuringRestore {
            get {
                return ResourceManager.GetString("ksBackupErrorDuringRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FieldWorks is unable to move the following projects to the new location..
        /// </summary>
        internal static string ksCannotMoveProjects {
            get {
                return ResourceManager.GetString("ksCannotMoveProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not change Projects location.
        /// </summary>
        internal static string ksChangeProjectLocationFailed {
            get {
                return ResourceManager.GetString("ksChangeProjectLocationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The attempt to change the location of the projects folder has failed. This is usually because you do not have administrator privileges in this account (or you answered &apos;no&apos; when asked whether it was OK to make changes to your machine). Because the project folder location affects all users of this computer, administrator privilege is required to change it..
        /// </summary>
        internal static string ksChangeProjectLocationFailedDetails {
            get {
                return ResourceManager.GetString("ksChangeProjectLocationFailedDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you want to migrate existing projects from an older version of FieldWorks?.
        /// </summary>
        internal static string ksDoYouWantToMigrate {
            get {
                return ResourceManager.GetString("ksDoYouWantToMigrate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string ksErrorCaption {
            get {
                return ResourceManager.GetString("ksErrorCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred trying to migrate existing projects from an older version of FieldWorks:.
        /// </summary>
        internal static string ksErrorMigratingProjects {
            get {
                return ResourceManager.GetString("ksErrorMigratingProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed.
        /// </summary>
        internal static string ksFailed {
            get {
                return ResourceManager.GetString("ksFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FieldWorks can&apos;t run.
        /// </summary>
        internal static string ksHklmCaption {
            get {
                return ResourceManager.GetString("ksHklmCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FieldWorks is unable to start because some necessary Registry keys are missing or inaccessible. There should be a registry entry {0}.
        ///
        ///We have sometimes seen cases where installing other software leaves the machine in a state where non-administrators cannot access these registry entries. If this is your problem, there is a possible workaround. Try a right click on the icon that you use to start FieldWorks and choose the option &apos;Run as Administrator&apos; from the context menu. See whether that helps. It may be [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ksHklmProblem {
            get {
                return ResourceManager.GetString("ksHklmProblem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to modify project locations..
        /// </summary>
        internal static string ksInsufficientPrivilegesToUpdateProjectLocationCaption {
            get {
                return ResourceManager.GetString("ksInsufficientPrivilegesToUpdateProjectLocationCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Administrator privileges are required to modify project locations. .
        /// </summary>
        internal static string ksInsufficientPrivilegesToUpdateProjectLocationText {
            get {
                return ResourceManager.GetString("ksInsufficientPrivilegesToUpdateProjectLocationText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is not a valid directory for linked files. Please select a valid directory..
        /// </summary>
        internal static string ksInvalidLinkedFilesFolder {
            get {
                return ResourceManager.GetString("ksInvalidLinkedFilesFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linked Files Folder.
        /// </summary>
        internal static string ksLinkedFilesFolder {
            get {
                return ResourceManager.GetString("ksLinkedFilesFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Migrate projects.
        /// </summary>
        internal static string ksMigrateProjects {
            get {
                return ResourceManager.GetString("ksMigrateProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Migrating existing projects failed totally..
        /// </summary>
        internal static string ksMigratingProjectsFailed {
            get {
                return ResourceManager.GetString("ksMigratingProjectsFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Moving project {0}.
        /// </summary>
        internal static string ksMovingProject {
            get {
                return ResourceManager.GetString("ksMovingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FieldWorks: Moving projects from {0} to {1}.
        /// </summary>
        internal static string ksMovingProjectsCaption {
            get {
                return ResourceManager.GetString("ksMovingProjectsCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FieldWorks found one or more invalid writing systems while loading the project. FieldWorks has safely removed the writing system(s) from the list.{0}{0}Writing systems removed:{0}{1}.
        /// </summary>
        internal static string ksNotifyWsRemoved {
            get {
                return ResourceManager.GetString("ksNotifyWsRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open the last project, .
        /// </summary>
        internal static string ksOpenLastEditedProject {
            get {
                return ResourceManager.GetString("ksOpenLastEditedProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open the sample project, .
        /// </summary>
        internal static string ksOpenSampleProject {
            get {
                return ResourceManager.GetString("ksOpenSampleProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Problems moving some Projects.
        /// </summary>
        internal static string ksProblemsMovingProjects {
            get {
                return ResourceManager.GetString("ksProblemsMovingProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The files this project links to are currently still located in the old FieldWorks folder. Would you like to store those files in the new folder for this project?.
        /// </summary>
        internal static string ksProjectLinksStillOld {
            get {
                return ResourceManager.GetString("ksProjectLinksStillOld", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} - {1}.
        /// </summary>
        internal static string ksProjectNameAndServerFmt {
            get {
                return ResourceManager.GetString("ksProjectNameAndServerFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} project(s) failed to migrate properly..
        /// </summary>
        internal static string ksProjectsFailedToMigrate {
            get {
                return ResourceManager.GetString("ksProjectsFailedToMigrate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Restoring from an old FieldWorks backup failed..
        /// </summary>
        internal static string ksRestoringOldFwBackupFailed {
            get {
                return ResourceManager.GetString("ksRestoringOldFwBackupFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Review the location of Linked Files.
        /// </summary>
        internal static string ksReviewLocationOfLinkedFiles {
            get {
                return ResourceManager.GetString("ksReviewLocationOfLinkedFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to initialize the data..
        /// </summary>
        internal static string kstidCacheInitFailure {
            get {
                return ResourceManager.GetString("kstidCacheInitFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to access the project data folder:
        ///{0}
        ///Ensure that this folder is shared and that you have permission to access it..
        /// </summary>
        internal static string kstidCannotAccessProjectPath {
            get {
                return ResourceManager.GetString("kstidCannotAccessProjectPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose a command:.
        /// </summary>
        internal static string kstidChooseCommand {
            get {
                return ResourceManager.GetString("kstidChooseCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User interface language &apos;{0}&apos; is not supported.
        ///Continue using English as the user interface language?.
        /// </summary>
        internal static string kstidFallbackToEnglishUi {
            get {
                return ResourceManager.GetString("kstidFallbackToEnglishUi", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Another instance of FieldWorks appears to be running, but did not respond properly to a request from this one. This is a bug; please report it to FlexErrors@sil.org. We recommend that you shut down all FieldWorks applications and try again. If you still get this message, restart your computer. (Experts may also try to recover by killing any FieldWorks processes using TaskManager.) Do you want to Retry now?.
        /// </summary>
        internal static string kstidFieldWorksDidNotRespond {
            get {
                return ResourceManager.GetString("kstidFieldWorksDidNotRespond", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Another instance of FieldWorks appears to be starting up at the same time as this one, and things have gotten confused. We recommend letting one project open fully before you try to open another one. If you get this message repeatedly or when not trying to open two projects at once, you may have encountered a bug; please report it. It may help to restart your computer..
        /// </summary>
        internal static string kstidFieldWorksRespondedNotSure {
            get {
                return ResourceManager.GetString("kstidFieldWorksRespondedNotSure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File does not exist:
        ///{0}.
        /// </summary>
        internal static string kstidFileNotFound {
            get {
                return ResourceManager.GetString("kstidFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Language Explorer.
        /// </summary>
        internal static string kstidFLEx {
            get {
                return ResourceManager.GetString("kstidFLEx", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid Writing System Found.
        /// </summary>
        internal static string kstidFoundInvalidWs {
            get {
                return ResourceManager.GetString("kstidFoundInvalidWs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Global Writing Systems Changed.
        /// </summary>
        internal static string kstidGlobalWsChangedCaption {
            get {
                return ResourceManager.GetString("kstidGlobalWsChangedCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following writing systems were updated in another project. Would you like to use those changes in this project ({1}) too?
        ///{0}.
        /// </summary>
        internal static string kstidGlobalWsChangedMsg {
            get {
                return ResourceManager.GetString("kstidGlobalWsChangedMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid FieldWorks project type..
        /// </summary>
        internal static string kstidInvalidFwProjType {
            get {
                return ResourceManager.GetString("kstidInvalidFwProjType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading Project {0}....
        /// </summary>
        internal static string kstidLoadingProject {
            get {
                return ResourceManager.GetString("kstidLoadingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FieldWorks project name not specified..
        /// </summary>
        internal static string kstidNoProjectName {
            get {
                return ResourceManager.GetString("kstidNoProjectName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot open project {1} because {2} is busy. If you wait until {2} has finished, {0} will continue..
        /// </summary>
        internal static string kstidOtherApplicationBusy {
            get {
                return ResourceManager.GetString("kstidOtherApplicationBusy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot open project {1} because a dialog box is open in {2}. Close any dialog boxes in {2} to continue..
        /// </summary>
        internal static string kstidOtherApplicationHasDialog {
            get {
                return ResourceManager.GetString("kstidOtherApplicationHasDialog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Renaming project....
        /// </summary>
        internal static string kstidRenamingProject {
            get {
                return ResourceManager.GetString("kstidRenamingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Saving project {0}....
        /// </summary>
        internal static string kstidSaving {
            get {
                return ResourceManager.GetString("kstidSaving", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Startup Problem.
        /// </summary>
        internal static string kstidStartupProblem {
            get {
                return ResourceManager.GetString("kstidStartupProblem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is running but cannot open a window for project {1} because FieldWorks is busy. If you wait until FieldWorks has finished, {0} will continue..
        /// </summary>
        internal static string kstidThisApplicationIsBusy {
            get {
                return ResourceManager.GetString("kstidThisApplicationIsBusy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The last time {0} started, it stopped responding when attempting to open the FieldWorks project:
        ///{1}.
        /// </summary>
        internal static string kstidUnableToOpenLastProject {
            get {
                return ResourceManager.GetString("kstidUnableToOpenLastProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to Open Project.
        /// </summary>
        internal static string kstidUnableToOpenProjectCaption {
            get {
                return ResourceManager.GetString("kstidUnableToOpenProjectCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When I start {0}, always open the last edited project automatically..
        /// </summary>
        internal static string ksWelcomeDialogCheckboxText {
            get {
                return ResourceManager.GetString("ksWelcomeDialogCheckboxText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can try to move the projects to the new location yourself in Windows Explorer..
        /// </summary>
        internal static string ksYouCanTryToMoveProjects {
            get {
                return ResourceManager.GetString("ksYouCanTryToMoveProjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap OpenFile {
            get {
                object obj = ResourceManager.GetObject("OpenFile", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Receive {
            get {
                object obj = ResourceManager.GetObject("Receive", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap SendReceiveGetArrow32x32 {
            get {
                object obj = ResourceManager.GetObject("SendReceiveGetArrow32x32", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
