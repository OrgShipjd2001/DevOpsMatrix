﻿using System;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;
using System.Threading;
using DevOpsMatrix.Tfs.Soap.Interface;
using Lumberjack.Interface;

namespace DevOpsMatrix.Tfs.Soap.ApiExecutor
{
    internal class SoapApiCmdUpdateFile : SoapApiCmdBase
    {
        public SoapApiCmdUpdateFile()
        {
        }

        public override CmdResult Execute(SoapCommand command)
        {
            CmdResult result = new CmdResult();
            result.Result = "success";

            // Parse the payload
            SoapPayloadUpdateFile payloadObj = JsonConvert.DeserializeObject<SoapPayloadUpdateFile>(command.CmdHeader);

            if (!string.IsNullOrWhiteSpace(payloadObj.EncodingName))
                Logging48.LogUserMessage("Command: Update (" + payloadObj.EncodingName + ") " + payloadObj.ItemServerPath);
            else
                Logging48.LogUserMessage("Command: Update " + payloadObj.ItemServerPath);

            Workspace area = WorkContext.Instance.GetWorkspace(payloadObj.ItemServerPath);

            if (area == null)
            {
                result.Result = "failed";
                result.Message = "Workspace not found.";
                return result;
            }

            string localpath = area.GetLocalItemForServerItem(payloadObj.ItemServerPath);

            try
            {
                area.Get(new GetRequest(localpath, RecursionType.None, VersionSpec.Latest), GetOptions.Overwrite);
            }
            catch
            {
            }

            bool retry = true;
            string errorMsg = string.Empty;
            int retryCount = 0;
            while ((retry) && (retryCount < 5))
            {
                try
                {
                    area.PendEdit(new string[] { localpath }, RecursionType.None, payloadObj.EncodingName, LockLevel.None);
                    retry = false;
                    errorMsg = string.Empty;
                }
                catch (Exception ex)
                {
                    Logging48.LogWarning("Error: " + ex.Message);
                    Logging48.LogWarning("Retrying...");
                    errorMsg = ex.ToString();
                    retryCount++;
                    Thread.Sleep(1000);
                }
            }

            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                result.Result = "failed";
                result.Message = errorMsg;
                return result;
            }

            // If we got to this point, the file was checked out, so if the readonly flag is still set, we can safely clear it.
            FileAttributes attr = File.GetAttributes(localpath);
            if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(localpath, attr & ~FileAttributes.ReadOnly);
            }

            using (FileStream fileStream = new FileStream(localpath, FileMode.Truncate, FileAccess.Write))
            {
                if ((command.Data == null) || (command.Data.Length == 0))
                {
                    result.Message = localpath + ": No file contents provided for update";
                }
                else
                {
                    fileStream.Write(command.Data, 0, command.Data.Length);
                }
                fileStream.Close();
            }
            
            return result;
        }
    }
}
