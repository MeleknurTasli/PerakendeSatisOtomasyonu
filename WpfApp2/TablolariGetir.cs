using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IniDosyaIslemleri;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media.Animation;
using BespokeFusion;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
using java.util.zip;
using System.Text.RegularExpressions;

namespace WpfApp2
{
    //sor ve sor burada da apiye ihtiyaç var mı?
    static class TablolariGetir
    {
        static SqlConnection sourceConnection, destinationConnection;
        static string localconnection, cpmconnection;

        public static void Baglan()
        {
            Carried.DosyaDecrypt();
            localconnection = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            cpmconnection = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("CPMconnection", "Baglanti.ini");
            Carried.DosyaEncrypt();
            try
            {
                destinationConnection = new SqlConnection(localconnection);
                destinationConnection.Open();
            }
            catch
            {
                Carried.showMessage("Local bağlantı bulunamadı.");
                return;
            }

            try
            {
                sourceConnection = new SqlConnection(cpmconnection);
                sourceConnection.Open();
            }
            catch { Carried.showMessage("Cpm bağlantısı bulunamadı."); return; }
        }

        //sor - sor boşlara ne gelcek? 
        public static void SMRTAPPBAS()
        {
            string cmdstrsayi = "select count(*) from ( select evrbas.ID,[EVRAKNO],[HESAPTIP],evrbas.[HESAPKOD],[UNVAN],[EFATURADURUM],[EARSIV],[PKETIKET],evrbas.[DOVIZCINS], [EVRAKDOVIZCINS],[EVRAKTARIH],evrbas.[ACIKLAMA1],[KARSIHESAPKOD],[_MIKTAR],[_EVRAKDURUM],EVRBAS.[KAYITDURUM] from EVRBAS inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD inner join EFABAS on evrbas.EVRAKSN = EFABAS.EVRAKSN ) as sayi";
            SqlCommand commandSourceDataMiktar = new SqlCommand(cmdstrsayi, sourceConnection);
            int rows = Convert.ToInt32(commandSourceDataMiktar.ExecuteScalar());

            //string cmdstr = "select evrbas.[ID],[EVRAKNO],[HESAPTIP],evrbas.[HESAPKOD],[UNVAN],[EFATURADURUM],[EARSIV],[PKETIKET],evrbas.[DOVIZCINS]," +
            //    "[EVRAKDOVIZCINS],[EVRAKTARIH],evrbas.[ACIKLAMA1],[KARSIHESAPKOD],[_MIKTAR],[_EVRAKDURUM],EVRBAS.[KAYITDURUM] from EVRBAS" +
            //    " inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD inner join EFABAS on evrbas.EVRAKSN = EFABAS.EVRAKSN";

            for (int j = 1; j <= rows; j++)
            {
                SqlCommand c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.ID, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                int s1 = Convert.ToInt32(c.ExecuteScalar());
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.EVRAKNO, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s2 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT CARKRT.HESAPTIP, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                short s3 = (short)c.ExecuteScalar();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.HESAPKOD, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s4 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT CARKRT.UNVAN, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s5 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT CARKRT.EFATURADURUM, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                short s6 = (short)c.ExecuteScalar();
                c = new SqlCommand("WITH CTE AS (SELECT EFABAS.EARSIV, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                short s7 = (short)c.ExecuteScalar();
                c = new SqlCommand("WITH CTE AS (SELECT EFABAS.PKETIKET, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s8 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.DOVIZCINS, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s9 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.EVRAKDOVIZCINS, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s10 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.EVRAKTARIH, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                DateTime s11 = (DateTime)c.ExecuteScalar();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.ACIKLAMA1, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s12 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.KARSIHESAPKOD , ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                string s13 = c.ExecuteScalar().ToString();
                //c = new SqlCommand("WITH CTE AS (SELECT , ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                //string s14 = c.ExecuteScalar().ToString();
                //c = new SqlCommand("WITH CTE AS (SELECT , ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                //string s15 = c.ExecuteScalar().ToString();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS._MIKTAR, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                decimal s16 = (decimal)c.ExecuteScalar();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS._EVRAKDURUM, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                short s17 = (short)c.ExecuteScalar();
                c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.KAYITDURUM, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row=" + j.ToString(), sourceConnection);
                short s18 = (short)c.ExecuteScalar();
                string komut = @"if not exists (select * from SMRTAPPBAS where EVRAKNO = '" + s2 + "') begin insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIVDURUM, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA, KARSIHESAPKOD, MIKTAR, EVRAKDURUM, KAYITDURUM) values(@ID, @EVRAKNO, @HESAPTIP, @HESAPKOD, @UNVAN, @EFATURADURUM, @EARSIVDURUM, @PKETIKET, @DOVIZCINS, @EVRAKDOVIZCINS, @EVRAKTARIH, @ACIKLAMA, @KARSIHESAPKOD, @MIKTAR, @EVRAKDURUM, @KAYITDURUM) end "; /**/   //KARSIUNVAN, REFNO yok
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    //+ " values (" + s1.ToString() + ",'" + s2 + "'," + s3.ToString() + ",'" + s4 + "','" + s5 + "'," + s6.ToString() + "," + s7.ToString() + ",'" + s8 + "','" + s9 + "','" + s10 + "','" + s11.ToString() + "','" + s12 + "','" + s13 + "'," /*+ s14 + "," + s15 + ","*/ + s16.ToString() + "," + s17.ToString() + "," + s18.ToString() + ") end";
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    //SqlCommand commandDestinationData = new SqlCommand(komut, destinationConnection);
                SqlCommand commandDestinationData = new SqlCommand();
                commandDestinationData.CommandText = komut;
                commandDestinationData.Parameters.Add(new SqlParameter("ID", s1));
                commandDestinationData.Parameters.Add(new SqlParameter("EVRAKNO", s2));
                commandDestinationData.Parameters.Add(new SqlParameter("HESAPTIP", s3));
                commandDestinationData.Parameters.Add(new SqlParameter("HESAPKOD", s4));
                commandDestinationData.Parameters.Add(new SqlParameter("UNVAN", s5));
                commandDestinationData.Parameters.Add(new SqlParameter("EFATURADURUM", s6));
                commandDestinationData.Parameters.Add(new SqlParameter("EARSIVDURUM", s7));
                commandDestinationData.Parameters.Add(new SqlParameter("PKETIKET", s8));
                commandDestinationData.Parameters.Add(new SqlParameter("DOVIZCINS", s9));
                commandDestinationData.Parameters.Add(new SqlParameter("EVRAKDOVIZCINS", s10));
                commandDestinationData.Parameters.Add(new SqlParameter("EVRAKTARIH", s11));
                commandDestinationData.Parameters.Add(new SqlParameter("ACIKLAMA", s12));
                commandDestinationData.Parameters.Add(new SqlParameter("KARSIHESAPKOD", s13));
                commandDestinationData.Parameters.Add(new SqlParameter("MIKTAR", s16));
                commandDestinationData.Parameters.Add(new SqlParameter("EVRAKDURUM", s17));
                commandDestinationData.Parameters.Add(new SqlParameter("KAYITDURUM", s18));
                commandDestinationData.Connection = destinationConnection;
                commandDestinationData.ExecuteNonQuery();
            }
        }

        public static void CreateSMRTAPPSKRT()
        {
            SqlCommand commandSourceData = new SqlCommand("DELETE FROM SMRTAPPSKRT", destinationConnection);
            commandSourceData.ExecuteNonQuery();

            commandSourceData = new SqlCommand("SELECT [ID],[SIRKETNO],[KAYITTUR],[KAYITDURUM],[KARTTIP],[MALKOD],[MALTIP],[MALAD],[MALAD2],[BIRIM],[OPERATOR2],[OPERATOR3],[OPERATOR4],[OPERATOR5],[KATSAYI2],[KATSAYI3],[KATSAYI4],[KATSAYI5],[BIRIMBRUTAGIRLIK],[BIRIMNETAGIRLIK],[BIRIMBRUTHACIM],[BIRIMNETHACIM],[BIRIMKAPADET],[DOVIZCINS],[OPSIYON],[ISKONTOORAN],[OTVTIP],[OTVDEGER],[OTVKESINTIORAN],[KDVORAN],[KDVDH],[KDVKESINTIORAN],[GRUPKOD],[SKOD1],[SKOD2],[SKOD3],[SKOD4],[SKOD5],[BKOD1],[BKOD2],[BKOD3],[BKOD4],[BKOD5],[BKOD6],[BKOD7],[BKOD8],[BKOD9],[NKOD1],[NKOD2],[NKOD3],[NKOD4],[NKOD5],[NKOD6],[NKOD7],[NKOD8],[NKOD9],[TARIH1],[TARIH2],[TARIH3],[TARIH4],[TARIH5],[ACIKLAMA1],[ALISFIYAT1],[ALISFIYAT1KDVDH],[ALISFIYAT2],[ALISFIYAT2KDVDH],[ALISFIYAT3],[ALISFIYAT3KDVDH],[ALISFIYAT4],[ALISFIYAT4KDVDH],[ALISFIYAT5],[ALISFIYAT5KDVDH],[ALISFIYAT6],[ALISFIYAT6KDVDH],[SATISFIYAT1],[SATISFIYAT2],[SATISFIYAT3],[SATISFIYAT4],[SATISFIYAT5],[SATISFIYAT6],[SATISFIYAT1KDVDH],[SATISFIYAT2KDVDH],[SATISFIYAT3KDVDH],[SATISFIYAT4KDVDH],[SATISFIYAT5KDVDH],[SATISFIYAT6KDVDH],[ALTSEVIYEKOD],[MRPTIP],[KRITIKSTOKSURE],[KRITIKSTOKMIKTAR],[TEMINTIP],[TEMINSTOKKAPAT],[MAXSTOKSURE],[MAXSTOKMIKTAR],[SABITALIMMIKTAR],[MINALIMMIKTAR],[MAXALIMMIKTAR],[ALIMPARTIBUYUKLUK],[YUVARLAMA],[MONTAJFIREORAN],[TEMINYONTEM],[TEMINOZELYONTEM],[ALIMDAGITIMTIP],[ALIMDAGITIMBASTARIH],[TEMINHAZIRLAMASURE],[TEMINSEVKIYATSURE],[ALIMGUMRUKSURE],[ALIMISLEMSURE],[MINSTOKSURE],[MINSTOKMIKTAR],[SERVISSEVIYEORAN],[GUVENLIKSURETIP],[GUVENLIKSURE],[GUVENLIKSUREORAN],[BILESENFIREORAN],[MINSATISMIKTAR],[MAXSATISMIKTAR],[SATISPARTIBUYUKLUK],[SARFYONTEM],[MEKANIKSARFYONTEM],[MALIYETTIP],[MALIYET1],[MALIYET2],[MALIYET3],[VERGI1ORAN],[SABITVERGI1],[SABITVERGI2],[SABITVERGI3],[KESINTI1ORAN],[KESINTI1FONORAN],[URETICIMALKOD],[GUMRUKVERGIORAN],[GUMRUKFON],[VERSIYONSABLONNO],[SERITAKIP],[KULLANIMSURE],[KALITEMINTESTFREKANS],[STOKKONTROLDURUM],[GRUPNO],[ALTERNATIFTIP],[MHSALIMKOD],[MHSSATISKOD],[MHSALIMIADEKOD],[MHSSATISIADEKOD],[MHSSATISISKONTOKOD],[URUNTAKIP],[KKEGORAN],[GIRENKULLANICI],[GIRENTARIH],[GIRENSAAT],[GIRENKAYNAK],[GIRENSURUM],[DEGISTIRENKULLANICI],[DEGISTIRENTARIH],[DEGISTIRENSAAT],[DEGISTIRENKAYNAK],[DEGISTIRENSURUM],[BARKOD1],[BARKOD2],[BARKOD3],[BARKOD4],[BARKOD5],[MARKAAD] FROM STKKRT", sourceConnection);
            SqlDataReader reader = commandSourceData.ExecuteReader();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
            bulkCopy.DestinationTableName = "SMRTAPPSKRT";
            try
            {
                bulkCopy.WriteToServer(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var v = new DataFormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                    Carried.showMessage(v.toString());
                }
                //throw;
            }
            finally
            {
                reader.Close();
            }
        }

        public static void CreateSMRTAPPCKRT()
        {
            SqlCommand commandSourceData = new SqlCommand("DELETE FROM SMRTAPPCKRT", destinationConnection);
            commandSourceData.ExecuteNonQuery();

            commandSourceData = new SqlCommand("SELECT [ID],[SIRKETNO],[HESAPKOD],[KAYITTUR],[KAYITDURUM] ,[KARTTIP] ,[HESAPTIP] ,[UNVAN] ,[KISITIP] ,[ULKEKOD] ,[VERGIDAIRE] ,[VERGIHESAPNO] ,[KISIUNVAN] ,[KISIAD] ,[KISISOYAD] ,[KISIUYRUK] ,[KISIPASAPORTTARIH] ,[FATURAUNVAN] ,[FATURAADRES1] ,[FATURAADRES2] ,[FATURAADRES4] ,[FATURAADRES5] ,[FATURAADRESBINANO] ,[FATURAADRESBINAAD] ,[FATURAADRESDAIRENO] ,[YETKILI1] ,[YETKILI2] ,[YETKILI3] ,[TELEFON1] ,[TELEFON2] ,[OPSIYONTIP] ,[OPSIYON] ,[ODEMEGUN] ,[ISKONTOORAN] ,[DOVIZBANKA] ,[DOVIZTIP] ,[DOVIZCINS] ,[BKOD1] ,[BKOD2] ,[BKOD3] ,[BKOD4] ,[BKOD5] ,[BKOD6] ,[BKOD7] ,[BKOD8] ,[BKOD9] ,[NKOD1] ,[NKOD2] ,[NKOD3] ,[NKOD4] ,[NKOD5] ,[NKOD6] ,[NKOD7] ,[NKOD8] ,[NKOD9] ,[TARIH1] ,[TARIH2] ,[TARIH3] ,[TARIH4] ,[TARIH5] ,[ACIKLAMA3] ,[ACIKLAMA4] ,[ACIKLAMA5] ,[MUHASEBEKOD1] ,[ACIKHESAPLIMIT] ,[DOVIZACIKHESAPLIMIT] ,[KREDILIMIT] ,[DOVIZKREDILIMIT] ,[BORCLUKREDILIMIT] ,[MUTABAKATTARIH] ,[MUTABAKATBAKIYE] ,[DOVIZMUTABAKATBAKIYE] ,[VADEFARKMUTABAKATTARIH] ,[KONTROLEVRAKTIP] ,[FIRMATIP] ,[TAKVIMOZEL] ,[BLOKE] ,[BABSTIP] ,[EFATURADURUM] ,[EFATURASENARYO] ,[EFATURAPKETIKET] ,[EFATURAYUKLEMETIP] ,[EARSIVKAGITNUSHA] ,[EIRSALIYEDURUM] ,[EIRSALIYEPKETIKET] ,[GIRENKULLANICI] ,[GIRENTARIH] ,[GIRENSAAT] ,[GIRENKAYNAK] ,[GIRENSURUM] ,[DEGISTIRENKULLANICI] ,[DEGISTIRENTARIH] ,[DEGISTIRENSAAT] ,[DEGISTIRENKAYNAK] ,[DEGISTIRENSURUM], EMAIL1, EMAIL2, EMAIL3, EMAIL4, EMAIL5 FROM CARKRT", sourceConnection);
            SqlDataReader reader = commandSourceData.ExecuteReader();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
            bulkCopy.DestinationTableName = "SMRTAPPCKRT";
            try
            {
                bulkCopy.WriteToServer(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var v = new DataFormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                    Carried.showMessage(v.toString());
                }
                //throw;
            }
            finally
            {
                reader.Close();
            }
        }

        public static void CreateSMRTAPPSRES()
        {
            SqlCommand commandSourceData = new SqlCommand("DELETE FROM SMRTAPPSRES", destinationConnection);
            commandSourceData.ExecuteNonQuery();

            commandSourceData = new SqlCommand("SELECT [MALKOD],[RESIM1],[RESIM2],[RESIM3],[RESIM4],[RESIM5] FROM STKRES", sourceConnection);
            SqlDataReader reader = commandSourceData.ExecuteReader();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
            bulkCopy.DestinationTableName = "SMRTAPPSRES";
            try
            {
                bulkCopy.WriteToServer(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var v = new DataFormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                    Carried.showMessage(v.toString());
                }
                //throw;
            }
            finally
            {
                reader.Close();
            }
        }

        public static void CreateSMRTAPPDEPO()
        { 
            SqlCommand commandSourceData = new SqlCommand("DELETE FROM SMRTAPPDEPO", destinationConnection);
            commandSourceData.ExecuteNonQuery();

            commandSourceData = new SqlCommand("select VW_STKDRM.MALKOD, STKKRT.MALAD, STKKRT.BARKOD1, VW_STKDRM.VERSIYONNO, SERINO, VW_STKDRM.DEPOKOD, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, STOKGIRIS-STOKCIKIS AS BAKIYE FROM VW_STKDRM inner join stkkrt on stkkrt.malkod=VW_STKDRM.malkod inner join DEPKRT on VW_STKDRM.DEPOKOD = DEPKRT.DEPOKOD order by MALKOD", sourceConnection);
            SqlDataReader reader = commandSourceData.ExecuteReader();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
            bulkCopy.DestinationTableName = "SMRTAPPDEPO";
            try
            {
                bulkCopy.WriteToServer(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var v = new DataFormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                    Carried.showMessage(v.toString());
                }
                //throw;
            }
            finally
            {
                reader.Close();
            }

            //SqlCommand commandSourceDataMiktar = new SqlCommand("SELECT COUNT(*) FROM (select malkod, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD) as rownum", sourceConnection);
            //int rows = Convert.ToInt32(commandSourceDataMiktar.ExecuteScalar());

            //for (int j = 1; j <= rows; j++)
            //{
            //    SqlCommand c = new SqlCommand("with cte as (select malkod, DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select malkod from CTE where rownum=" + j.ToString(), sourceConnection);
            //    string smalkod = c.ExecuteScalar().ToString(); 
            //    c = new SqlCommand("with cte as (select malkod, DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select depokod from CTE where rownum=" + j.ToString(), sourceConnection);
            //    string sdepokod = c.ExecuteScalar().ToString();
            //    c = new SqlCommand("with cte as (select malkod, DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select DEPOAD from CTE where rownum=" + j.ToString(), sourceConnection);
            //    string sdepoad = c.ExecuteScalar().ToString();
            //    c = new SqlCommand("with cte as (select malkod, DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select VERSIYONNO from CTE where rownum=" + j.ToString(), sourceConnection);
            //    string sversiyonno = c.ExecuteScalar().ToString();
            //    c = new SqlCommand("with cte as (select malkod, DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select SERINO from CTE where rownum=" + j.ToString(), sourceConnection);
            //    string sserino = c.ExecuteScalar().ToString();
            //    c = new SqlCommand("with cte as (select malkod, DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select STOKGIRIS from CTE where rownum=" + j.ToString(), sourceConnection);
            //    int istokgiris = (int)c.ExecuteScalar();
            //    c = new SqlCommand("with cte as (select malkod,  DEPKRT.VERSIYONNO, DEPKRT.SERINO, VW_STKDRM.depokod, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, ROW_NUMBER() over (order by depkrt.DEPOAD) as 'rownum' FROM  VW_STKDRM inner join DEPKRT on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD ) select STOKCIKIS from CTE where rownum=" + j.ToString(), sourceConnection);
            //    int istokcikis = (int)c.ExecuteScalar();
            //    int bakiye = istokgiris - istokcikis;
            //    c = new SqlCommand("select malad from stkkrt where malkod='" + smalkod + "'", sourceConnection);
            //    string smalad = c.ExecuteScalar().ToString();

            ////string komut = @"if not exists (select * from SMRTAPPBAS where EVRAKNO = '" + s2 + "') begin insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIVDURUM, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA, KARSIHESAPKOD, MIKTAR, EVRAKDURUM, KAYITDURUM) values(@ID, @EVRAKNO, @HESAPTIP, @HESAPKOD, @UNVAN, @EFATURADURUM, @EARSIVDURUM, @PKETIKET, @DOVIZCINS, @EVRAKDOVIZCINS, @EVRAKTARIH, @ACIKLAMA, @KARSIHESAPKOD, @MIKTAR, @EVRAKDURUM, @KAYITDURUM) end ";
            ////SqlCommand commandDestinationData = new SqlCommand();
            ////commandDestinationData.CommandText = komut;
            ////commandDestinationData.Parameters.Add(new SqlParameter("ID", s1));
            ////commandDestinationData.Parameters.Add(new SqlParameter("EVRAKNO", s2));
            ////commandDestinationData.Parameters.Add(new SqlParameter("HESAPTIP", s3));
            ////commandDestinationData.Parameters.Add(new SqlParameter("HESAPKOD", s4));
            ////commandDestinationData.Parameters.Add(new SqlParameter("UNVAN", s5));
            ////commandDestinationData.Parameters.Add(new SqlParameter("EFATURADURUM", s6));
            ////commandDestinationData.Parameters.Add(new SqlParameter("EARSIVDURUM", s7));
            ////commandDestinationData.Parameters.Add(new SqlParameter("PKETIKET", s8));
            ////commandDestinationData.Parameters.Add(new SqlParameter("DOVIZCINS", s9));
            ////commandDestinationData.Parameters.Add(new SqlParameter("EVRAKDOVIZCINS", s10));
            ////commandDestinationData.Parameters.Add(new SqlParameter("EVRAKTARIH", s11));
            ////commandDestinationData.Parameters.Add(new SqlParameter("ACIKLAMA", s12));
            ////commandDestinationData.Parameters.Add(new SqlParameter("KARSIHESAPKOD", s13));
            ////commandDestinationData.Parameters.Add(new SqlParameter("MIKTAR", s16));
            ////commandDestinationData.Parameters.Add(new SqlParameter("EVRAKDURUM", s17));
            ////commandDestinationData.Parameters.Add(new SqlParameter("KAYITDURUM", s18));
            ////commandDestinationData.Connection = destinationConnection;
            ////commandDestinationData.ExecuteNonQuery();
        }

        public static void CreateSMRTAPPETIKET()
        {
            SqlCommand commandSourceData = new SqlCommand("DELETE FROM SMRTAPPETIKET", destinationConnection);
            commandSourceData.ExecuteNonQuery();

            commandSourceData = new SqlCommand("select VERGIHESAPNO, ETIKETTIP, ETIKET FROM EFAKUL", sourceConnection);
            SqlDataReader reader = commandSourceData.ExecuteReader();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
            bulkCopy.DestinationTableName = "SMRTAPPETIKET";
            try
            {
                bulkCopy.WriteToServer(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var v = new DataFormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                    Carried.showMessage(v.toString());
                }
                //    throw;
            }
            finally
            {
                reader.Close();
            }
        }

        public static void CreateSRKKRT()
        {
            SqlCommand commandSourceData = new SqlCommand("DELETE FROM SRKKRT", destinationConnection);
            commandSourceData.ExecuteNonQuery();

            commandSourceData = new SqlCommand("select * FROM SRKKRT", sourceConnection);
            SqlDataReader reader = commandSourceData.ExecuteReader();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
            bulkCopy.DestinationTableName = "SRKKRT";
            try
            {
                bulkCopy.WriteToServer(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var v = new DataFormatException(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                    Carried.showMessage(v.toString());
                }
                //    throw;
            }
            finally
            {
                reader.Close();
            }
        }


        //public static void CreateSMRTAPPSHAR()
        //{

        //}

        //public static void CreateSMRTAPPCHAR()
        //{

        //}






        //NOTLAR:
        private static void button2click()
        {
            string sourceConnStr = "Data Source=SMARTSRV\\CPM;Initial Catalog=CPM_UYG;Persist Security Info=True;User ID=Sa;Password=Nova1881";
            string destinationConnStr = "Data Source=DESKTOP-6PSD2C4; Initial Catalog=CPMSMARTAPP; Integrated Security=True";
            ////using (SqlConnection sourceConnection = new SqlConnection(sourceConnStr))
            ////{
            ////sourceConnection.Open();
            //ArrayList arrlist = new ArrayList();
            //// Get data from the source table as a SqlDataReader.
            ////SqlCommand commandSourceData = new SqlCommand("SELECT EVRAKNO FROM EVRBAS", sourceConnection);
            ////SqlDataReader reader = commandSourceData.ExecuteReader();
            //if (reader != null)
            //{
            //    while (reader.Read())
            //    {
            //        arrlist.Add(reader["EVRAKNO"].ToString());
            //    }
            //    reader.Close();
            //}
            ////using (SqlConnection destinationConnection = new SqlConnection(destinationConnStr))
            ////{
            //    destinationConnection.Open();
            //    for (int i=0; i<arrlist.Count; i++)
            //    {
            //        SqlCommand commandDestinationData = new SqlCommand("INSERT INTO SMRTAPPBAS (ID) VALUES (" + i + ")", destinationConnection);
            //        commandDestinationData.ExecuteNonQuery();

            //        commandDestinationData = new SqlCommand("INSERT INTO SMRTAPPBAS (EVRAKNO) VALUES ('"+ arrlist[i].ToString() +"') WHERE ID = "+ i.ToString(), destinationConnection);
            //        commandDestinationData.ExecuteNonQuery();

            //        commandDestinationData = new SqlCommand("INSERT INTO SMRTAPPBAS (EVRAKNO) VALUES ('" + arrlist[i].ToString() + "')", destinationConnection);
            //        commandDestinationData.ExecuteNonQuery();

            //    }
            // Set up the bulk copy object.
            // Note that the column positions in the source
            // data reader match the column positions in
            // the destination table so there is no need to
            // map columns.
            ////using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
            ////{
            ////    bulkCopy.DestinationTableName = "SMRTAPPBAS";
            ////    bulkCopy.ColumnMappings.Add("EVRAKNO", "EVRAKNO");
            ////    try
            ////    {
            ////        bulkCopy.WriteToServer(reader);
            ////    }
            ////    //catch (Exception ex)
            ////    //{
            ////    //    Console.WriteLine(ex.Message);
            ////    //}
            ////    finally
            ////    {
            ////        reader.Close();
            ////    }
            ////}

            // Perform a final count on the destination
            // table to see how many rows were added.
            //long countEnd = System.Convert.ToInt32(
            //    commandRowCount.ExecuteScalar());
            //Console.WriteLine("Ending row count = {0}", countEnd);
            //Console.WriteLine("{0} rows were added.", countEnd - countStart);
            //Console.WriteLine("Press Enter to finish.");
            //Console.ReadLine();
            ////    }
            ////}

            SqlConnection sourceConnection = new SqlConnection(sourceConnStr);
            SqlConnection destinationConnection = new SqlConnection(destinationConnStr);
            sourceConnection.Open();
            destinationConnection.Open();

            //var common = from c in dt.AsEnumerable()
            //             join x in dt2.AsEnumerable() on c.Field<string>("ID") equals x.Field<string>("ID")
            //             select new object[] { c["ID"], c["First Name"], x["Birthday"] };
            //DataTable dt3 = new DataTable();
            //dt3.Columns.Add("ID");
            //dt3.Columns.Add("Name");
            //dt3.Columns.Add("Birthdate");
            //foreach (var item in common)
            //    dt3.LoadDataRow(item.ToArray(), true);

            /*
            ////var dt = new DataTable();
            ////dt.Columns.Add("EVRAKNO");
            ////dt.Columns.Add("KARSIUNVAN");
            ////dt.Columns.Add("REFNO");
            ////dt.Rows.Add("TR000000626", "unvan1", "r1");
            ////dt.Rows.Add("TR000000627", "unvan2", "r2");
            ////dt.Rows.Add("TR000000628", "unvan3", "r3");

            string cmdstrsayi = "select count(*) from ( select evrbas.[ID],[EVRAKNO],[HESAPTIP],evrbas.[HESAPKOD],[UNVAN],[EFATURADURUM],[EARSIV],[PKETIKET],evrbas.[DOVIZCINS]," +
                "[EVRAKDOVIZCINS],[EVRAKTARIH],evrbas.[ACIKLAMA1],[KARSIHESAPKOD],[_MIKTAR],[_EVRAKDURUM],EVRBAS.[KAYITDURUM] from EVRBAS" +
                " inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD inner join EFABAS on evrbas.EVRAKSN = EFABAS.EVRAKSN ) as sayi";
            SqlCommand commandSourceDataMiktar = new SqlCommand(cmdstrsayi, sourceConnection);
            int rows = Convert.ToInt32(commandSourceDataMiktar.ExecuteScalar());

            //string cmdstr = "select evrbas.[ID],[EVRAKNO],[HESAPTIP],evrbas.[HESAPKOD],[UNVAN],[EFATURADURUM],[EARSIV],[PKETIKET],evrbas.[DOVIZCINS]," +
            //    "[EVRAKDOVIZCINS],[EVRAKTARIH],evrbas.[ACIKLAMA1],[KARSIHESAPKOD],[_MIKTAR],[_EVRAKDURUM],EVRBAS.[KAYITDURUM] from EVRBAS" +
            //    " inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD inner join EFABAS on evrbas.EVRAKSN = EFABAS.EVRAKSN";
            
            for (int i = 1; i <= rows; i++)
            {
                SqlCommand c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.ID, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row="+i.ToString(), sourceConnection); 
                int s1 = Convert.ToInt32(c.ExecuteScalar()); MessageBox.Show(s1.ToString());
                c = new SqlCommand("", sourceConnection);
                string s2 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s3 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s4 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s5 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s6 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s7 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s8 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s9 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s10 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s11 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s12 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s13 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s14 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s15 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s16 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s17 = c.ExecuteScalar().ToString();
                c = new SqlCommand("", sourceConnection);
                string s18 = c.ExecuteScalar().ToString();
                string komut = "insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIV, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA,KARSIHESAPKOD,MIKTAR,EVRAKDURUM, KAYITDURUM ) " + //KARSIUNVAN, REFNO yok
                    " values (" + s1 + "," + s2 + "," + s3 + "," + s4 + "," + s5 + "," + s6 + "," + s7 + "," + s8 + "," + s9 + "," + s10 + "," + s11 + "," + s12 + "," + s13 + "," + s14 + "," + s15 + "," + s16 + "," + s17 + "," + s18 + ")";
                SqlCommand commandDestinationData = new SqlCommand(komut, destinationConnection);
                commandDestinationData.ExecuteNonQuery();
            }
            */


            //SqlDataReader reader = commandSourceData.ExecuteReader();
            //while (reader.Read())
            //{
            //    for (int i = 0; i<rows; i++)
            //    {
            //        MessageBox.Show(reader[i].ToString());

            //    }
            //}

            //string komut = "insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIV, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA,KARSIHESAPKOD,KARSIUNVAN,REFNO,MIKTAR,EVRAKDURUM, KAYITDURUM ) " +
            //    " values (" + reader[i].ToString() + ")";
            //SqlCommand commandDestinationData = new SqlCommand(komut, destinationConnection);
            //commandDestinationData.ExecuteNonQuery();


            //    SqlDataReader reader = commandSourceData.ExecuteReader();
            //    var dt1 = new DataTable();
            //    dt1.Columns.Add("ID");
            //    dt1.Columns.Add("EVRAKNO");
            //    dt1.Columns.Add("HESAPTIP");
            //    dt1.Columns.Add("HESAPKOD");
            //    dt1.Columns.Add("UNVAN");
            //    dt1.Columns.Add("EFATURADURUM");
            //    dt1.Columns.Add("EARSIVDURUM");
            //    dt1.Columns.Add("PKETIKET");
            //    dt1.Columns.Add("DOVIZCINS");
            //    dt1.Columns.Add("EVRAKDOVIZCINS");
            //    dt1.Columns.Add("EVRAKTARIH");
            //    dt1.Columns.Add("ACIKLAMA");
            //    dt1.Columns.Add("KARSIHESAPKOD");
            //    dt1.Columns.Add("KARSIUNVAN");
            //    dt1.Columns.Add("REFNO");
            //    dt1.Columns.Add("MIKTAR");
            //    dt1.Columns.Add("EVRAKDURUM");
            //    dt1.Columns.Add("KAYITDURUM");

            //    while (reader.Read())
            //    {
            //        dt1.Rows.Add(reader["EVRAKNO"]);
            //    }
            //    reader.Close();
        }

    }
}