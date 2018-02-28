using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;
using System.IO;
using System.Text;
using ZXing.QrCode;

namespace ADTelephones.Models
{
    public class Telefon
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Stanowisko { get; set; }

       
        public string Dzial { get; set; }
        public string Email { get; set; }
        public string NumerTelefonu { get; set; }
        public System.Web.UI.WebControls.Image barcode { get; set; }


        //get list from AD
        internal static List<Telefon> GetContacts(string wartosc)
        {
            var dane = new List<Telefon>();

            if (wartosc != "")
            {


                //LDP settings - change here your LDAP
                String adPath = "LDAP://DC=example,DC=com,DC=pl";
                DirectorySearcher ds = new DirectorySearcher(adPath);

                //searching contact by telephone last name
                ds.Filter = "(&(objectClass=user)(SAMAccountName=*" + wartosc + "*))";
                foreach (SearchResult sResultSet in ds.FindAll())
                {

                    if (GetProperty(sResultSet, "mail") != "" && GetProperty(sResultSet, "sn") != "")
                    {

                        var tel = new Telefon();
                        tel.Imie = GetProperty(sResultSet, "givenName");
                        tel.Nazwisko = GetProperty(sResultSet, "sn");
                        tel.Stanowisko = GetProperty(sResultSet, "title");
                        tel.Dzial = GetProperty(sResultSet, "department");
                        tel.NumerTelefonu = GetProperty(sResultSet, "telephoneNumber");
                        tel.Email = GetProperty(sResultSet, "mail");
                        tel.barcode = generate_barcode(tel.Imie + " " + tel.Nazwisko, tel.Stanowisko, tel.NumerTelefonu, tel.Email);
                        dane.Add(tel);
                    }
                }

                //searching contact by telephone number
                ds = new DirectorySearcher(adPath);
                ds.Filter = "(&(objectClass=user)(telephoneNumber=*" + wartosc + "*))";
                foreach (SearchResult sResultSet in ds.FindAll())
                {

                    if (GetProperty(sResultSet, "mail") != "" && GetProperty(sResultSet, "sn") != "")
                    {

                        var tel = new Telefon();
                        tel.Imie = GetProperty(sResultSet, "givenName");
                        tel.Nazwisko = GetProperty(sResultSet, "sn");
                        tel.Stanowisko = GetProperty(sResultSet, "title");
                        tel.Dzial = GetProperty(sResultSet, "department");
                        tel.NumerTelefonu = GetProperty(sResultSet, "telephoneNumber");
                        tel.Email = GetProperty(sResultSet, "mail");
                        tel.barcode = generate_barcode(tel.Imie + " " + tel.Nazwisko, tel.Stanowisko, tel.NumerTelefonu, tel.Email);
                        dane.Add(tel);
                    }
                }

                //searching contact by department
                ds = new DirectorySearcher(adPath);
                ds.Filter = "(&(objectClass=user)(department=*" + wartosc + "*))";
                foreach (SearchResult sResultSet in ds.FindAll())
                {

                    if (GetProperty(sResultSet, "mail") != "" && GetProperty(sResultSet, "sn") != "")
                    {

                        var tel = new Telefon();
                        tel.Imie = GetProperty(sResultSet, "givenName");
                        tel.Nazwisko = GetProperty(sResultSet, "sn");
                        tel.Stanowisko = GetProperty(sResultSet, "title");
                        tel.Dzial = GetProperty(sResultSet, "department");
                        tel.NumerTelefonu = GetProperty(sResultSet, "telephoneNumber");
                        tel.Email = GetProperty(sResultSet, "mail");
                        tel.barcode = generate_barcode(tel.Imie + " " + tel.Nazwisko, tel.Stanowisko, tel.NumerTelefonu, tel.Email);
                        dane.Add(tel);
                    }
                }

                

            }
            return dane.OrderBy(x => x.Nazwisko).Distinct().ToList();

        }


        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }


        public static System.Web.UI.WebControls.Image generate_barcode(string nazwa, string stanowisko, string telefon, string email)

        {

            StringBuilder vcard = new StringBuilder("MECARD:N:" + nazwa + ";");
            vcard.Append("ORG: YourCompany;");
            vcard.Append("TEL: " + telefon + ";");
            vcard.Append("EMAIL: " + email + ";");
            vcard.Append("NOTE:" + stanowisko + "; ;");


            string barCode = vcard.ToString();
            
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 250,
                    Width = 250,
                    Margin = 0
                }
            };
            var pixelData = qrCodeWriter.Write(barCode);


            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }


                System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

                return imgBarCode;
            }
        }

    }

}