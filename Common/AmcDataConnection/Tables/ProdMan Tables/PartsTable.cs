/* ITable Class
 * 
 * This class must inherit from the Database Type (derived from DBWorks) class and the ITable Interface
 * 
 * The Constructor must assign the KeyValue Property and call InitTable() 
 * 
 * InitTable() must assign the KeyIdentifier and TableName Property and call InitializeDatabase()
 * 
 * AddColums() must populate the global _ColumnNames field
 * 
 */

namespace AMCDatabase
{
    public class PartsTable : PMTables, ITable
    {
        public string VERSION = "Version"; 
        public string LABELSCODE = "LabelsCode";
        
        const string KEY_IDENTIFIER = "Partnumber";

        public PartsTable(string sKeyValue, string sVersion, bool bTest = false)
        {
            KeyValue = sKeyValue;
            Version = sVersion;
            TestData = bTest;

            InitTable();            
        }

        public bool InitTable()
        {
            KeyIdentifier = KEY_IDENTIFIER;

            string sTableName = PartsTable.GetPartNumberTable(KeyValue, Version);
            TableName = sTableName;

            if (string.IsNullOrEmpty(sTableName))
                return false;

            // ja - for vfp the connection string has to be set after the table name is known
            ConnectionString = GetPMConnString();

            return InitializeDatabase();            
        }
#if no
        public override void AddColums()
        {
            // TODO: ja - add all columns 
            
            _ColumnNames.Add(KeyIdentifier);
            _ColumnNames.Add(VERSION);
            _ColumnNames.Add("Basemodel");
            _ColumnNames.Add("Revision");
            _ColumnNames.Add(LABELSCODE);
            _ColumnNames.Add("problems");
            _ColumnNames.Add("import_ok");
            _ColumnNames.Add("basemodel");
            _ColumnNames.Add("revision");
            _ColumnNames.Add("extension");
            _ColumnNames.Add("top_assy");
            _ColumnNames.Add("pbcb_other");
            _ColumnNames.Add("amp_type");
            _ColumnNames.Add("eeprom_data");
            _ColumnNames.Add("eeprom_comp");
            _ColumnNames.Add("inspecs");
            _ColumnNames.Add("outspecs");
            _ColumnNames.Add("modelname");
            _ColumnNames.Add("maturity");
            _ColumnNames.Add("origin");
            _ColumnNames.Add("hworvd");
            _ColumnNames.Add("projpath");
            _ColumnNames.Add("projectpt");
            _ColumnNames.Add("project");
            _ColumnNames.Add("vdprojfile");
            _ColumnNames.Add("firmpath");
            _ColumnNames.Add("firmprjpth");
            _ColumnNames.Add("firmprj");
            _ColumnNames.Add("firmprj1");
            _ColumnNames.Add("firmprj2");
            _ColumnNames.Add("firmprj3");
            _ColumnNames.Add("siiprj");
            _ColumnNames.Add("firmfile1");
            _ColumnNames.Add("firmfile1a");
            _ColumnNames.Add("firmfile1b");
            _ColumnNames.Add("firmfile1c");
            _ColumnNames.Add("firmfile2");
            _ColumnNames.Add("firmfile3");
            _ColumnNames.Add("firmfile4");
            _ColumnNames.Add("firmfile5");
            _ColumnNames.Add("firmfile6");
            _ColumnNames.Add("firmfile7");
            _ColumnNames.Add("firmfile8");
            _ColumnNames.Add("firmfile9");
            _ColumnNames.Add("section");
            _ColumnNames.Add("ovenprof1");
            _ColumnNames.Add("ovenprof2");
            _ColumnNames.Add("pcb");
            _ColumnNames.Add("screen");
            _ColumnNames.Add("pld");
            _ColumnNames.Add("pld1");
            _ColumnNames.Add("pld1refdes");
            _ColumnNames.Add("jedfile1");
            _ColumnNames.Add("pld2");
            _ColumnNames.Add("pld2refdes");
            _ColumnNames.Add("jedfile2");
            _ColumnNames.Add("pld3");
            _ColumnNames.Add("pld3refdes");
            _ColumnNames.Add("jedfile3");
            _ColumnNames.Add("smtcode");
            _ColumnNames.Add("controller");
            _ColumnNames.Add("notify_imp");
            _ColumnNames.Add("pcbfile");
            _ColumnNames.Add("asmfile01");
            _ColumnNames.Add("asmfile02");
            _ColumnNames.Add("asmfile02v");
            _ColumnNames.Add("asmfile03");
            _ColumnNames.Add("asmfile03a");
            _ColumnNames.Add("asmfile03b");
            _ColumnNames.Add("cadfile01");
            _ColumnNames.Add("cadfile02");
            _ColumnNames.Add("cadfile03");
            _ColumnNames.Add("visual1");
            _ColumnNames.Add("d_file");
            _ColumnNames.Add("ictreq");
            _ColumnNames.Add("regtest");
            _ColumnNames.Add("regburn");
            _ColumnNames.Add("mantest");
            _ColumnNames.Add("manburn");
            _ColumnNames.Add("testprog");
            _ColumnNames.Add("spburn");
            _ColumnNames.Add("spburnpth");
            _ColumnNames.Add("spburnprg");
            _ColumnNames.Add("spmburn");
            _ColumnNames.Add("spmburnpth");
            _ColumnNames.Add("spmburnprg");
            _ColumnNames.Add("m_regtest");
            _ColumnNames.Add("m_regburn");
            _ColumnNames.Add("m_mantest");
            _ColumnNames.Add("m_manburn");
            _ColumnNames.Add("m_testprog");
            _ColumnNames.Add("elecver");
            _ColumnNames.Add("cc_eldate");
            _ColumnNames.Add("cc_eltime");
            _ColumnNames.Add("cc_mhdate");
            _ColumnNames.Add("cc_mhtime");
            _ColumnNames.Add("cc_perfed");
            _ColumnNames.Add("cc_newimp");
            _ColumnNames.Add("engbom");
            _ColumnNames.Add("acctbom");
            _ColumnNames.Add("imp_elname");
            _ColumnNames.Add("imp_eldate");
            _ColumnNames.Add("imp_eltime");
            _ColumnNames.Add("imp_mhname");
            _ColumnNames.Add("imp_mhdate");
            _ColumnNames.Add("imp_mhtime");
            _ColumnNames.Add("imp_perfed");
            _ColumnNames.Add("created");
            _ColumnNames.Add("updated");
            _ColumnNames.Add("updatetime");
            _ColumnNames.Add("operation");
            _ColumnNames.Add("user");
            _ColumnNames.Add("rec_lock");
            _ColumnNames.Add("sch_chkout");
            _ColumnNames.Add("econumber");
            _ColumnNames.Add("firsttm");
            _ColumnNames.Add("pend_chgs");
            _ColumnNames.Add("schs_nin_p");
            _ColumnNames.Add("ethercat");
            _ColumnNames.Add("emc");
            _ColumnNames.Add("emc_label");
            _ColumnNames.Add("emc_doc");
            _ColumnNames.Add("lvd");
            _ColumnNames.Add("lvd_label");
            _ColumnNames.Add("lvd_doc");
            _ColumnNames.Add("ul");
            _ColumnNames.Add("ul_label");
            _ColumnNames.Add("ul_doc");
            _ColumnNames.Add("amclogo");
            _ColumnNames.Add("wk_note1");
            _ColumnNames.Add("wk_note2");
            _ColumnNames.Add("exp_key");
            _ColumnNames.Add("l_maxwo");
            _ColumnNames.Add("l_maxmake");
            _ColumnNames.Add("l_totqty");
            _ColumnNames.Add("l_currstk");
            _ColumnNames.Add("tagged");
            _ColumnNames.Add("obsolete");
            _ColumnNames.Add("newdir");
            _ColumnNames.Add("phillips");
            _ColumnNames.Add("labelscode");
            _ColumnNames.Add("rohasyorn");
            _ColumnNames.Add("rohaschked");

        }   
   
#endif
    }

}
