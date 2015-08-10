using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportConverter
{
    public class GenericConverter
    {
        private AppInfo info;
        private Settings settings;
        private Dictionary<string, int[]> f2C;
        private List<List<string>> records;
        private WorkOrder wo;
        private WorkOrder flaggedWO;
        private PartsTable partsTable;
        private AssetTable aTable;
        private WOTable woTable;

        public GenericConverter(AppInfo i, Settings s, PartsTable p, AssetTable a, WOTable woT)
        {
            info = i;
            settings = s;
            woTable = woT;
            partsTable = p;
            aTable = a;
        }

        public void startConversion(Report report, Dictionary<string, int[]> fieldToCell)
        {
            records = report.getRecords("Main");
            f2C = fieldToCell;
            try
            {
                int[] loc = f2C["ID"];
                string id = records[loc[0]][loc[1]];
                wo = new WorkOrder(id, woTable, report.Filepath);
                loc = f2C["Site"];
                wo.Site = info.getSite(records[loc[0]][loc[1]]);
                loc = f2C["Vendor"];
                wo.Vendor = info.getVendor(records[loc[0]][loc[1]]);

                addDates();
                addAssetID();
                addDownTime();
                addWorkOrderInfo();
            }
            catch (Exception e)
            {
                settings.showMessage(e.ToString());
            }
        }

        private void addDates()
        {
            try
            {
                int[] loc = f2C["Start Date"];
                string sDate = records[loc[0]][loc[1]];
                wo.StartDate = Convert.ToDateTime(sDate);
                loc = f2C["End Date"];
                string eDate = records[loc[0]][loc[1]];
                wo.EndDate = Convert.ToDateTime(eDate);
                wo.OpenDate = wo.StartDate;
            }
            catch (Exception e)
            {
                settings.showMessage(e.ToString());
            }
        }

        private void addAssetID()
        {
            int[] loc = f2C["Asset"];
            string asset = records[loc[0]][loc[1] + 2];
            List<string> a = aTable.getAssetID(asset, wo.Site.Name);
            wo.AssetID = a[0];
            wo.AssetDescription = a[1];
        }

        /* Adds information based on work order type from
         * the table present in the AppInfo file */
        private void addWorkOrderInfo()
        {
            int[] loc = f2C["Work Order Type"];
            string workOrderType = records[loc[0]][loc[1]];
            List<string> taskInfo = info.getTypeInfo(workOrderType);
            wo.WorkOrderType = taskInfo[0];
            wo.TaskID = taskInfo[1];
            wo.TaskDescription = taskInfo[2];
            wo.OutageType = taskInfo[3];
            wo.Planning = taskInfo[4];
            wo.UnplannedType = taskInfo[5];
            wo.Priority = taskInfo[6];
        }

        /* Adds the down time to the work order*/
        private void addDownTime()
        {
            int[] loc = f2C["Down Time"];
            string dT = records[loc[0]][loc[1]];
            try
            {
                wo.DownTime = Convert.ToDouble(dT);
            }
            catch (Exception e)
            {
                settings.showMessage(e.ToString());
            }
        }

        private void addParts()
        {
            int start = f2C["Part ID"][0];
            int idLoc = f2C["Part ID"][1];
        }
    }
}
