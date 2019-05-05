using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kendo.Mvc.UI
{
    public static class DataSourceRequestExtensions
    {
        /// <summary>
        /// Finds a Filter Member with the "memberName" name and renames it for "newMemberName".
        /// </summary>
        /// <param name="request">The DataSourceRequest instance. <see cref="Kendo.Mvc.UI.DataSourceRequest"/></param>
        /// <param name="memberName">The Name of the Filter to be renamed.</param>
        /// <param name="newMemberName">The New Name of the Filter.</param>
        public static void RenameRequestFilterSortMember(this DataSourceRequest request, string memberName, string newMemberName)
        {
            RenameRequestFilterMember(request.Filters, memberName, newMemberName);
            RenameRequestSortMember(request.Sorts, memberName, newMemberName);
        }

        private static void RenameRequestFilterMember(this IList<Kendo.Mvc.IFilterDescriptor> filterDescriptors, string memberName, string newMemberName)
        {
            foreach (var descriptor in filterDescriptors)
            {
                if (descriptor is CompositeFilterDescriptor)
                {
                    RenameRequestFilterMember(((CompositeFilterDescriptor)descriptor).FilterDescriptors, memberName, newMemberName);
                }
                else
                {
                    var filterDescriptor = descriptor as FilterDescriptor;
                    if (filterDescriptor.Member == memberName)
                    {
                        filterDescriptor.Member = newMemberName;
                    }
                }
            }
        }

        private static void RenameRequestSortMember(this IList<Kendo.Mvc.SortDescriptor> sortDescriptors, string memberName, string newMemberName)
        {
            foreach (var sortDescriptor in sortDescriptors)
            {
                var descriptor = sortDescriptor as Kendo.Mvc.SortDescriptor;
                if (descriptor.Member.Equals(memberName))
                {
                    descriptor.Member = newMemberName;
                }
            }
        }
    }
}