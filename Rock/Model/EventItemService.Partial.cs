﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Model
{
    public partial class EventItemService
    {

        /// <summary>
        /// Gets the active calendar items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<EventItem> GetActiveItems()
        {
            return Queryable()
                    .Where( e => e.IsActive == true
                       && e.IsApproved == true )
                    .Where( e => e.EventCalendarItems.Any( c =>
                                   c.EventCalendar.IsActive == true ) );
        }

        /// <summary>
        /// Gets the active items by calendar identifier.
        /// </summary>
        /// <param name="calendarId">The calendar identifier.</param>
        /// <returns></returns>
        public IQueryable<EventItem> GetActiveItemsByCalendarId( int calendarId )
        {
            return this.GetActiveItems()
                        .Where( e => e.EventCalendarItems.Any( c =>
                                        c.EventCalendar.Id == calendarId
                                ) );
        }

        /// <summary>
        /// Gets the indexable active items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<EventItem> GetIndexableActiveItems()
        {
            return this.GetActiveItems()
                        .Where( e => e.EventCalendarItems.Any( c =>
                                        c.EventCalendar.IsActive
                                        && c.EventCalendar.IsIndexEnabled ) );
        }
    }
}
