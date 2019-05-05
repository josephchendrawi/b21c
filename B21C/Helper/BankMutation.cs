using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace B21C.Helper
{
    public class BankMutation
    {
        public static List<BankMutationItem> GetBCA(List<string> CSVLines)
        {
            var result = new List<BankMutationItem>();

            foreach(var Rows in CSVLines)
            {
                try
                {
                    var Columns = Rows.Split(',');

                    var column_data = new BankMutationItem();
                    column_data.Date = Columns[0].Trim('\'');
                    column_data.TransactionDescription = Columns[1];

                    var Amount = decimal.Parse(Columns[3]);
                    if (Columns[4] == "CR")
                        column_data.Credit = Amount;
                    else if (Columns[4] == "DB")
                        column_data.Debit = Amount;

                    column_data.Bank = "BCA";
                    result.Add(column_data);
                }
                catch { }
            }

            return result;
        }

        public static List<BankMutationItem> GetMandiri(List<string> CSVLines)
        {
            var result = new List<BankMutationItem>();

            foreach (var Rows in CSVLines)
            {
                try
                {
                    var Columns = Rows.Split('\"');

                    var column_data = new BankMutationItem();
                    column_data.Date = Columns[1];
                    column_data.TransactionDescription = Columns[2].Trim(',');
                    column_data.Debit = decimal.Parse(Columns[3].Replace(".", "").Replace(",", "."));
                    column_data.Credit = decimal.Parse(Columns[5].Replace(".", "").Replace(",", "."));

                    column_data.Bank = "Mandiri";
                    result.Add(column_data);
                }
                catch { }
            }

            return result;
        }

        public static List<BankMutationItem> GetBRI(string HTML)
        {
            var result = new List<BankMutationItem>();

            string TableExpression = "<table id=\"tabel-saldo\"[^>]*>(.*?)</table>";
            string RowExpression = "<tr[^>]*>(.*?)</tr>";
            string ColumnExpression = "<td[^>]*>(.*?)</td>";
            int iCurrentRow = 0;

            // Get a match for all the tables in the HTML    
            MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Loop through each table element    
            foreach (Match Table in Tables)
            {
                // Reset the current row counter and the header flag    
                iCurrentRow = 0;

                // Get a match for all the rows in the table    
                MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each row element    
                foreach (Match Row in Rows)
                {
                    // Only loop through the row if it isn't a header row    
                    if (!(iCurrentRow == 0))
                    {
                        // Get a match for all the columns in the row    
                        MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        try
                        {
                            var column_data = new BankMutationItem();
                            column_data.Date = Columns[0].Groups[1].Value;
                            column_data.TransactionDescription = Columns[1].Groups[1].Value;
                            try { column_data.Debit = decimal.Parse(Columns[2].Groups[1].Value.Replace(".","").Replace(",",".")); } catch { };
                            try { column_data.Credit = decimal.Parse(Columns[3].Groups[1].Value.Replace(".", "").Replace(",", ".")); } catch { };

                            column_data.Bank = "BRI";
                            result.Add(column_data);
                        }
                        catch { }
                    }

                    // Increase the current row counter    
                    iCurrentRow += 1;
                }
            }

            return result;
        }

    }

    public class BankMutationItem
    {
        public string Bank { get; set; }
        public string Date { get; set; }
        public string TransactionDescription { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        //public decimal? Balance { get; set; }
    }
}