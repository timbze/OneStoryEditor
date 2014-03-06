﻿using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ekm.oledb.data;
using Microsoft.Win32;
using OneStoryProjectEditor;

namespace UpdateAccessDbWithOsMetaData
{
    class Program
    {
        private const string CstrCommandLineUpdateDatabase = "/u";
        private const string CstrCommandLineCreateMetaDataFilesFromDatabase = "/c";

        private const string CstrSubfolderToExtractedOsMetaDataFileFormat = "MetaDataFilesFromDatabase";
        private const string CstrOsMetaDataFilename = "OsMetaData.xml";

        static void Main(string[] args)
        {
            try
            {
                string strPathToAccessDatabase;
                if ((args.Length != 2) ||
                    !((args[0] == CstrCommandLineUpdateDatabase) || 
                      (args[0] == CstrCommandLineCreateMetaDataFilesFromDatabase)) ||
                    !File.Exists((strPathToAccessDatabase = args[1])))
                {
                    DisplayUsage();
                    return;
                }

                if (args[0] == CstrCommandLineCreateMetaDataFilesFromDatabase)
                    ProcessOsMetaDataDatabase(strPathToAccessDatabase);
                else
                    UpdateDatabaseFromOsMetaDataFiles(strPathToAccessDatabase);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        private static void UpdateDatabaseFromOsMetaDataFiles(string strPathToAccessDatabase)
        {
            // get all the OSE project folders that have OsMetaData files in them.
            var projectFolders = Directory.GetDirectories(OneStoryProjectFolderRoot).Where(fldr =>
            {
                var strFileName = Path.GetFileName(fldr);
                var strFileSpec = Path.Combine(fldr, strFileName + ".onestory");
                var strOsMetaDataFile = Path.Combine(fldr, CstrOsMetaDataFilename);
                return File.Exists(strFileSpec) && File.Exists(strOsMetaDataFile);
            }).ToList();

            if (projectFolders.Count == 0)
            {
                Console.WriteLine("There are no OSE projects that have a '{0}' file in them!?", CstrOsMetaDataFilename);
                return;
            }

            var db = Db.Open(strPathToAccessDatabase);
            foreach (var projectFolder in projectFolders)
            {
                try
                {
                    ProcessProject(projectFolder, db);
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }
            }
        }

        private static void ProcessProject(string strProjectFolder, DatabaseContext db)
        {
            var strOsMetaDataFile = Path.Combine(strProjectFolder, CstrOsMetaDataFilename);
            var osMetaData = OsMetaDataModel.Load(strOsMetaDataFile);
            var osMetaDataModelRecord = osMetaData.OsProjects.First();
            var record = db.ExecuteSingle("select * from Projects where ose_proj_id = @oseProjectId",
                                           new Param("@oseProjectId", osMetaDataModelRecord.OseProjId, OleDbType.Numeric));
            if (record == null)
            {
                // record doesn't exist, so insert it
                db.ExecuteNonQuery("insert into Projects " +
                                           "(Project_Name, Language_Name, Ethnologue_Code, Continent, Country, Managing_Partner, " +
                                            "Entity, Priorities_Category, Scripture_Status, Scrip_Status_Notes, Project_Facilitators, " +
                                            "PF_Category, PF_Affiliation, Notes, Status, Start_Date, LCA_Workshop, LCA_Coach, " +
                                            "SC_workshop, currently_using_OSE, ose_proj_id, ES_Consultant, ES_Coach, ES_stories_sent, " +
                                            "Process_Check, Multiplication_workshop, Number_SFGs, PS_Consultant, PS_Coach, " +
                                            "PS_stories_prelim_approv, LSR, Number_Final_Stories, Set_Finished_Date, " +
                                            "Uploaded_to_OSMedia, Set_Copyrighted) " +
                                   "values (@ProjectName, @LanguageName, @EthnologueCode, @Continent, @Country, @ManagingPartner, " +
                                           "@Entity, @PrioritiesCategory, @ScriptureStatus, @ScriptureStatusNotes, " +
                                           "@ProjectFacilitators, @PfCategory, @PfAffiliation, @Notes, @Status, @StartDate, " +
                                           "@LcaWorkshop, @LcaCoach, @ScWorkshop, @IsCurrentlyUsingOse, @OseProjId, @EsConsultant, " +
                                           "@EsCoach, @EsStoriesSent, @ProcessCheck, @MultiplicationWorkshop, @NumberSfgs, " +
                                           "@PsConsultant, @PsCoach, @PsStoriesPrelimApprov, @Lsr, @NumInFinalApprov, " +
                                           "@SetFinishedDate, @IsUploadedToOsMedia, @SetCopyrighted)", 
                                   GetParameterArray(osMetaDataModelRecord)
                                );
            }
            else
            {
                db.ExecuteNonQuery("UPDATE [Projects] " +
                                   "SET [Project_Name] = @ProjectName, " +
                                       "[Language_Name] = @LanguageName, " +
                                       "[Ethnologue_Code] = @EthnologueCode, " +
                                       "[Continent] = @Continent, " +
                                       "[Country] = @Country, " +
                                       "[Managing_Partner] = @ManagingPartner, " +
                                       "[Entity] = @Entity, " +
                                       "[Priorities_Category] = @PrioritiesCategory, " + 
                                       "[Scripture_Status] = @ScriptureStatus, " +
                                       "[Scrip_Status_Notes] = @ScriptureStatusNotes, " + 
                                       "[Project_Facilitators] = @ProjectFacilitators, " + 
                                       "[PF_Category] = @PfCategory, " +
                                       "[PF_Affiliation] = @PfAffiliation, " +
                                       "[Notes] = @Notes, " + 
                                       "[Status] = @Status, " +
                                       "[Start_Date] = @StartDate, " +
                                       "[LCA_Workshop] = @LcaWorkshop, " +
                                       "[LCA_Coach] = @LcaCoach, " +
                                       "[SC_workshop] = @ScWorkshop, " +
                                       "[currently_using_OSE] = @IsCurrentlyUsingOse, " +
                                       "[ose_proj_id] = @OseProjId, " +
                                       "[ES_Consultant] = @EsConsultant, " +
                                       "[ES_Coach] = @EsCoach, " +
                                       "[ES_stories_sent] = @EsStoriesSent, " +
                                       "[Process_Check] = @ProcessCheck, " +
                                       "[Multiplication_workshop] = @MultiplicationWorkshop, " +
                                       "[Number_SFGs] = @NumberSfgs, " +
                                       "[PS_Consultant] = @PsConsultant, " +
                                       "[PS_Coach] = @PsCoach, " +
                                       "[PS_stories_prelim_approv] = @PsStoriesPrelimApprov, " +
                                       "[LSR] = @Lsr, " +
                                       "[Number_Final_Stories] = @NumInFinalApprov, " +
                                       "[Set_Finished_Date] = @SetFinishedDate, " +
                                       "[Uploaded_to_OSMedia] = @IsUploadedToOsMedia, " +
                                       "[Set_Copyrighted] = @SetCopyrighted " +
                                    "WHERE [ose_proj_id] = @OseProjId; ",
                                    GetParameterArray(osMetaDataModelRecord)
                                   );
            }
        }

        private static Param[] GetParameterArray(OsMetaDataModelRecord osMetaDataModelRecord)
        {
            return new[]
            {
                new Param("@ProjectName", osMetaDataModelRecord.ProjectName, OleDbType.VarWChar),
                new Param("@LanguageName", osMetaDataModelRecord.LanguageName, OleDbType.VarWChar),
                new Param("@EthnologueCode", osMetaDataModelRecord.EthnologueCode, OleDbType.VarWChar),
                new Param("@Continent", osMetaDataModelRecord.Continent, OleDbType.VarWChar),
                new Param("@Country", osMetaDataModelRecord.Country, OleDbType.VarWChar),
                new Param("@ManagingPartner", osMetaDataModelRecord.ManagingPartner, OleDbType.VarWChar),
                new Param("@Entity", osMetaDataModelRecord.Entity, OleDbType.VarWChar),
                new Param("@PrioritiesCategory", osMetaDataModelRecord.PrioritiesCategory, OleDbType.VarWChar),
                new Param("@ScriptureStatus", osMetaDataModelRecord.ScriptureStatus, OleDbType.VarWChar),
                new Param("@ScriptureStatusNotes", osMetaDataModelRecord.ScriptureStatusNotes, OleDbType.VarWChar),
                new Param("@ProjectFacilitators", osMetaDataModelRecord.ProjectFacilitators, OleDbType.VarWChar),
                new Param("@PfCategory", osMetaDataModelRecord.PfCategory, OleDbType.VarWChar),
                new Param("@PfAffiliation", osMetaDataModelRecord.PfAffiliation, OleDbType.VarWChar),
                new Param("@Notes", osMetaDataModelRecord.Notes, OleDbType.VarWChar),
                new Param("@Status", osMetaDataModelRecord.Status, OleDbType.VarWChar),
                new Param("@StartDate", osMetaDataModelRecord.StartDate, OleDbType.DBDate),
                new Param("@LcaWorkshop", osMetaDataModelRecord.LcaWorkshop, OleDbType.VarWChar),
                new Param("@LcaCoach", osMetaDataModelRecord.LcaCoach, OleDbType.VarWChar),
                new Param("@ScWorkshop", osMetaDataModelRecord.ScWorkshop, OleDbType.VarWChar),
                new Param("@IsCurrentlyUsingOse", osMetaDataModelRecord.IsCurrentlyUsingOse, OleDbType.Boolean),
                new Param("@OseProjId", osMetaDataModelRecord.OseProjId, OleDbType.VarWChar),
                new Param("@EsConsultant", osMetaDataModelRecord.EsConsultant, OleDbType.VarWChar),
                new Param("@EsCoach", osMetaDataModelRecord.EsCoach, OleDbType.VarWChar),
                new Param("@EsStoriesSent", osMetaDataModelRecord.EsStoriesSent, OleDbType.Integer),
                new Param("@ProcessCheck", osMetaDataModelRecord.ProcessCheck, OleDbType.VarWChar),
                new Param("@MultiplicationWorkshop", osMetaDataModelRecord.MultiplicationWorkshop, OleDbType.VarWChar),
                new Param("@NumberSfgs", osMetaDataModelRecord.NumberSfgs, OleDbType.Integer),
                new Param("@PsConsultant", osMetaDataModelRecord.PsConsultant, OleDbType.VarWChar),
                new Param("@PsCoach", osMetaDataModelRecord.PsCoach, OleDbType.VarWChar),
                new Param("@PsStoriesPrelimApprov", osMetaDataModelRecord.PsStoriesPrelimApprov, OleDbType.Integer),
                new Param("@Lsr", osMetaDataModelRecord.Lsr, OleDbType.VarWChar),
                new Param("@NumInFinalApprov", osMetaDataModelRecord.NumInFinalApprov, OleDbType.Integer),
                new Param("@SetFinishedDate", osMetaDataModelRecord.SetFinishedDate, OleDbType.DBDate),
                new Param("@IsUploadedToOsMedia", osMetaDataModelRecord.IsUploadedToOsMedia, OleDbType.Boolean),
                new Param("@SetCopyrighted", osMetaDataModelRecord.SetCopyrighted, OleDbType.VarWChar)
            };
        }

        private static void ProcessOsMetaDataDatabase(string strPathToAccessDatabase)
        {
            var db = Db.Open(strPathToAccessDatabase);

            // this selects all the records for which there is an OSE project id
            var records = db.ExecuteMany("select * from Projects where ose_proj_id is not null");
            foreach (var record in records)
            {
                OsMetaDataModelRecord recordMapped = Mapper.Map<OsMetaDataModelRecord, OsMetaDataModelMapping>(record);
                System.Diagnostics.Debug.Assert(recordMapped.OseProjId != null);
                var strFileSpec = Path.Combine(Path.Combine(Path.Combine(OneStoryProjectFolderRoot, CstrSubfolderToExtractedOsMetaDataFileFormat),
                                                            recordMapped.OseProjId),
                                               CstrOsMetaDataFilename);

                var osMetaDataModel = new OsMetaDataModel
                {
                    OsProjects = new List<OsMetaDataModelRecord> { recordMapped },
                    PathToMetaDataFile = strFileSpec
                };

                osMetaDataModel.Save();
            }
        }

        private static void DisplayUsage()
        {
            Console.WriteLine(
                @"Usage:{0}  UpdateAccessDbWithOsMetaData ""{2}"" | ""{3}"" <path to OS Metadata Access database (e.g. 'D:\temp\OS\OneStory.mdf')>{0}{0}where ""/u"" means update the database from the meta data files (in the various{0}project folders){0}and ""/c"" means create meta data files from the database (in the folder:{0}'<My Documents>\OneStory Editor Projects\{1}' folder)",
                Environment.NewLine,
                CstrSubfolderToExtractedOsMetaDataFileFormat,
                CstrCommandLineUpdateDatabase,
                CstrCommandLineCreateMetaDataFilesFromDatabase);
        }

        public static void ProcessException(Exception ex)
        {
            var strErrorMsg = ex.Message;
            if (ex.InnerException != null)
                strErrorMsg += String.Format("{0}{0}{1}",
                                            Environment.NewLine,
                                            ex.InnerException.Message);
            Console.WriteLine(strErrorMsg);
        }

        protected const string OneStoryHiveRoot = @"Software\SIL\OneStory";
        protected const string CstrRootDirKey = "RootDir";
        protected const string CstrProjectRootFolder = "OneStory Editor Projects";

        public static string OneStoryProjectFolderRoot
        {
            get
            {
                string strDefaultProjectFolderRoot = null;
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    strDefaultProjectFolderRoot = (string)keyOneStoryHiveRoot.GetValue(CstrRootDirKey);

                if (String.IsNullOrEmpty(strDefaultProjectFolderRoot))
                    strDefaultProjectFolderRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                return Path.Combine(strDefaultProjectFolderRoot, CstrProjectRootFolder);
            }
            set
            {
                var keyOneStoryHiveRoot = Registry.CurrentUser.OpenSubKey(OneStoryHiveRoot, true) ??
                                          Registry.CurrentUser.CreateSubKey(OneStoryHiveRoot);
                if (keyOneStoryHiveRoot != null)
                    keyOneStoryHiveRoot.SetValue(CstrRootDirKey, value);
            }
        }
    }
}
