using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace breakpoint
{
    public class TimeUtil {
	/**
	 * Constants that represent units of time in terms of milliseconds
	 */
	public static long 
		year = 31449600000L,
		month = 2592000000L,
		week = 604800000L,
		day = 86400000L,
		hour = 3600000L,
		minute = 60000L;
        /**Conversion Utility Function for parsing a Date from java.util.Date to a string
		 *using a specific date format. 
		 *@Return Returns null if the date is null.
		 */
		public static String dateToString(DateTime date)
		{
            return date.ToString("YYYY-MM-ddTHH-mm-ssZ");
		}
		/**Conversion Utility Function for parsing a string to a java.util.Date
		 *using a specific date format. 
		 *@Return Returns null if the string is null or 
		 *the string is not formatted properly
		 */
		public static DateTime stringToDate(String str)
		{
			if(str == null || str == "")
				return DateTime.Now;
			return DateTime.Parse(str);
		}
	/**A function for receiving the relative time difference between 
	 *two dates in the format specified in DATE_FORMAT. 
	 *@Return Returns a string
	 *(EXAMPLE: "# year(s) ago" or "due in # year(s))*/
	public static String getDifference(DateTime past, DateTime current){
        TimeSpan span = new TimeSpan(current.Ticks - past.Ticks);
		if(past < current){
			if(past.Year - current.Year > 0)
			{
                if (past.Year - current.Year == 1)
					return "1 year ago";
                return (past.Year - current.Year) + " years ago";
			}
            if (past.Month - current.Month > 0)
			{
                if (past.Month - current.Month == 1)
					return "1 month ago";
                return (past.Month - current.Month) + " months ago";
			}
            if (past.Day - current.Day > 6)
            {
                if (past.Day - current.Day < 14)
                    return "1 week ago";
                return ((past.Day - current.Day) / 7) + " weeks ago";
            }
            if (past.Day - current.Day > 0)
            {
                if (past.Day - current.Day == 1)
                    return "1 day ago";
                return ((past.Day - current.Day)) + " days ago";
            }
            if (past.Hour - current.Hour > 0)
            {
                if (past.Hour - current.Hour == 1)
                    return "1 hour ago";
                return ((past.Hour - current.Hour)) + " hours ago";
            }
            if (past.Minute - current.Minute > 0)
            {
                if (past.Minute - current.Minute == 1)
                    return "1 minute ago";
                return ((past.Minute - current.Minute)) + " minutes ago";
            }
            return "now";
		}
		else{
            if (past.Year - current.Year > 0)
            {
                if (past.Year - current.Year == 1)
                    return "in 1 year";
                return "in " + (past.Year - current.Year) + " years";
            }
            if (past.Month - current.Month > 0)
            {
                if (past.Month - current.Month == 1)
                    return "in 1 month";
                return "in " + (past.Month - current.Month) + " months";
            }
            if (past.Day - current.Day > 6)
            {
                if (past.Day - current.Day < 14)
                    return "in 1 week";
                return "in " + ((past.Day - current.Day) / 7) + " weeks";
            }
            if (past.Day - current.Day > 0)
            {
                if (past.Day - current.Day == 1)
                    return "in 1 day";
                return "in " + ((past.Day - current.Day)) + " days";
            }
            if (past.Hour - current.Hour > 0)
            {
                if (past.Hour - current.Hour == 1)
                    return "in 1 hour";
                return "in " + ((past.Hour - current.Hour)) + " hours";
            }
            if (past.Minute - current.Minute > 0)
            {
                if (past.Minute - current.Minute == 1)
                    return "in 1 minute";
                return "in " + (past.Minute - current.Minute) + " minutes";
            }
            return "now";
        }
	    }
    }
}
